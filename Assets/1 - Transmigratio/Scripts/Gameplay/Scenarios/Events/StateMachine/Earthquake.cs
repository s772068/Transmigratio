using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

namespace Gameplay.Scenarios.Events.StateMachines
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Scenarios/Events/StateMachines/Earthquake", fileName = "Earthquake")]
    public class Earthquake : Base
    {
        [Header("States")]
        [SerializeField] private SerializedDictionary<State, Data.State> states;
        [Header("Percents")]
        [SerializeField, Range(0, 1)] private float fullPercentFauna;
        [SerializeField, Range(0, 1)] private float fullPercentPopulation;
        [SerializeField, Range(0, 1)] private float partPercentFauna;
        [SerializeField, Range(0, 1)] private float partPercentPopulation;
        [Header("Points")]
        [SerializeField, Min(1)] private float _calmEarthquakePointsDivision;

        private int CalmEarthquackePoints => (int)(_eventPiece.Population.Value / _calmEarthquakePointsDivision);
        private int ReduceLossesPoints => CalmEarthquackePoints / 2;

        private State _state = State.Start;

        private protected override string Name => "EarthQuake";

        public override void Init()
        {
            base.Init();
            _curState = states[State.Start];
            _curState.Start();
        }

        private protected override void ActivateEvents()
        {
            Events.AutoChoice.NewEvent(this, _desidions);
            base.ActivateEvents();
        }

        private protected override void InitDesidions(){
            AddDesidion(Nothing, Local("Nothing"), (piece) => 0);
            AddDesidion(CalmEarthquake, Local("CalmEarthquake"), (piece) => GetDesidionCost(CalmEarthquackePoints));
            AddDesidion(ReduceLosses, Local("ReduceLosses"), (piece) => GetDesidionCost(ReduceLossesPoints));
        }

        private bool CalmEarthquake(CivPiece piece, Func<CivPiece, int> interventionPoints){
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "CalmEarthquake",
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = (int)Math.Abs(piece.PopulationGrow.value) });
            EndEvent(true);
            return true;
        }

        private bool ReduceLosses(CivPiece piece, Func<CivPiece, int> interventionPoints){
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            int dead = (int)(piece.Population.Value * partPercentPopulation);
            piece.Population.Value -= dead;
            piece.Region.Fauna["Fauna"] *= 1 - partPercentFauna;
            Global.Migration.OnMigration(piece);
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "ReduceLosses",
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = dead });
            EndEvent(true);
            return true;
        }

        public void ActivateEarthquake(bool nextState = false){
            int dead = (int)(_eventPiece.Population.Value * fullPercentPopulation);
            _eventPiece.Population.Value -= dead;
            _eventPiece.Region.Fauna["Fauna"] *= 1 - fullPercentFauna;
            Global.Migration.OnMigration(_piece);
            ChroniclesController.Deactivate(Name, _eventPiece.RegionID, panelSprite, "ActivateEarthquake",
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = dead });
            EndEvent(nextState);
        }

        private bool Nothing(CivPiece piece, Func<CivPiece, int> interventionPoints){
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            ActivateEarthquake(true);
            return true;
        }

        private protected override void OpenPanel(CivPiece piece){
            PanelFabric.CreateEvent(HUD.Instance.PanelsParent, _desidionPrefab, panel, this, panelSprite, Local("Title"),
                                    Territory(), Local("Description"), _desidions, _eventPiece);
        }

        private void EndEvent(bool nextState = false){
            _eventPiece.RemoveEvent(this);
            CheckMarker();
            if (nextState)
                NextState();
        }

        private protected override void NextState(){
            _state = _state switch
            {
                State.Start => State.ActivateQuake,
                State.ActivateQuake => State.Pause,
                State.Pause => State.ActivateQuake,
            };
            _curState = states[_state];
            _curState.Start();
        }

        private enum State { Start, ActivateQuake, Pause }
    }
}