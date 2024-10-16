using AYellowpaper.SerializedCollections;
using UnityEngine;
using System;

namespace Gameplay.Scenarios.Events.StateMachines {
    [CreateAssetMenu(menuName = "ScriptableObjects/Scenarios/Events/StateMachines/Volcano", fileName = "Volcano")]
    public sealed class Volcano : Base {
        [Header("States")]
        [SerializeField] private SerializedDictionary<State, Data.State> states;
        [Header("Percents")]
        [SerializeField, Range(0, 1)] private float fullPercentPopulation;
        [SerializeField, Range(0, 1)] private float partPercentPopulation;
        [Header("Points")]
        [SerializeField, Min(1)] private float calmVolcanoPointsDivision;
        [SerializeField, Min(0)] private int reduceLossesPoints;

        private State _state = State.Start;

        private protected override string Name => "Volcano";
        private int CalmVolcanoPoints => (int) (_eventPiece.Population.Value / calmVolcanoPointsDivision) > reduceLossesPoints ? (int)(_eventPiece.Population.Value / calmVolcanoPointsDivision) : reduceLossesPoints + 1;

        public override void Init() {
            base.Init();
            _curState = states[State.Start];
            _curState.Start();
        }

        private protected override void ActivateEvents() {
            Events.AutoChoice.NewEvent(this, _desidions);
            base.ActivateEvents();
        }

        private protected override void InitDesidions() {
            AddDesidion(Nothing, Local("Nothing"), (piece) => 0);
            AddDesidion(CalmVolcano, Local("CalmVolcano"), (piece) => GetDesidionCost(CalmVolcanoPoints));
            AddDesidion(ReduceLosses, Local("ReduceLosses"), (piece) => GetDesidionCost(reduceLossesPoints));
        }

        private bool CalmVolcano(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "CalmVolcano", 
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = (int)Math.Abs(piece.PopulationGrow.value) });
            EndEvent(true);
            return true;
        }

        private bool ReduceLosses(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            int dead = (int)(piece.Population.Value * partPercentPopulation);
            piece.Population.Value -= dead;
            Global.Migration.OnMigration(piece);
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "ReduceLosses", 
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = dead });
            EndEvent(true);
            return true;
        }

        public void ActivateVolcano(bool nextState = false) {
            int dead = (int)(_eventPiece.Population.Value * fullPercentPopulation);
            _eventPiece.Population.Value -= dead;
            Global.Migration.OnMigration(_piece);
            ChroniclesController.Deactivate(Name, _eventPiece.RegionID, panelSprite, "ActivateVolcano", 
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = dead });
            EndEvent(nextState);
        }

        private bool Nothing(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            ActivateVolcano(true);
            return true;
        }

        private void EndEvent(bool nextState = false) {
            _eventPiece.RemoveEvent(this);
            CheckMarker();
            if (nextState)
                NextState();
        }

        private protected override void OpenPanel(CivPiece piece = null) {
            PanelFabric.CreateEvent(HUD.Instance.PanelsParent, _desidionPrefab, panel, this, panelSprite, Local("Title"),
                                    Territory(), Local("Description"), _desidions, _eventPiece);
        }

        private protected override void NextState() {
            _state = _state switch {
                State.Start => State.ActivateVolcano,
                State.ActivateVolcano => State.Restart,
                State.Restart => State.ActivateVolcano,
            };
            _curState = states[_state];
            _curState.Start();
        }

        private enum State { Start, ActivateVolcano, Restart }
    }
}
