using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

namespace Gameplay.Scenarios.Events.StateMachines
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Scenarios/Events/StateMachines/Flood", fileName = "Flood")]
    public class Flood : Base
    {
        [Header("States")]
        [SerializeField] private SerializedDictionary<State, Data.State> states;
        [Header("Percents")]
        [SerializeField, Range(0, 1)] private float fullPercentFlora;
        [SerializeField, Range(0, 1)] private float fullPercentPopulation;
        [SerializeField, Range(0, 1)] private float partPercentFlora;
        [SerializeField, Range(0, 1)] private float partPercentPopulation;
        [Header("Points")]
        [SerializeField, Min(1)] private float _calmFloodPointsDivision;

        private int CalmFloodPoints => (int)(_eventPiece.Population.Value / _calmFloodPointsDivision);
        private int ReduceLossesPoints => CalmFloodPoints / 2;

        private State _state = State.Start;

        private protected override string Name => "Flood";

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
            AddDesidion(CalmEarthquake, Local("CalmFlood"), (piece) => GetDesidionCost(CalmFloodPoints));
            AddDesidion(ReduceLosses, Local("ReduceLosses"), (piece) => GetDesidionCost(ReduceLossesPoints));
        }

        private bool CalmEarthquake(CivPiece piece, Func<CivPiece, int> interventionPoints){
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "CalmFlood",
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = (int)Math.Abs(piece.PopulationGrow.value) });
            EndEvent(true);
            return true;
        }

        private bool ReduceLosses(CivPiece piece, Func<CivPiece, int> interventionPoints){
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            int dead = (int)(piece.Population.Value * partPercentPopulation);
            piece.Population.Value -= dead;
            piece.Region.Flora["Flora"] *= 1 - partPercentFlora;
            Global.Migration.OnMigration(piece);
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "ReduceLosses",
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = dead });
            EndEvent(true);
            return true;
        }

        public void ActivateFlood(bool nextState = false){
            int dead = (int)(_eventPiece.Population.Value * fullPercentPopulation);
            _eventPiece.Population.Value -= dead;
            _eventPiece.Region.Flora["Flora"] *= 1 - fullPercentFlora;
            Global.Migration.OnMigration(_piece);
            ChroniclesController.Deactivate(Name, _eventPiece.RegionID, panelSprite, "ActivateFlood",
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = dead });
            EndEvent(nextState);
        }

        private bool Nothing(CivPiece piece, Func<CivPiece, int> interventionPoints){
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            ActivateFlood(true);
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
                State.Start => State.ActivateFlood,
                State.ActivateFlood => State.Pause,
                State.Pause => State.ActivateFlood,
            };
            _curState = states[_state];
            _curState.Start();
        }

        private enum State { Start, ActivateFlood, Pause }
    }
}