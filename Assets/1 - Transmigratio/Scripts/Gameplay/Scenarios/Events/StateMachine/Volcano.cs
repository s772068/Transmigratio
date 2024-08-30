using AYellowpaper.SerializedCollections;
using UnityEngine;
using System;

namespace Gameplay.Scenarios.Events.StateMachines {
    [CreateAssetMenu(menuName = "ScriptableObjects/Scenarios/Events/StateMachines/Volcano", fileName = "Volcano")]
    public sealed class Volcano : Base {
        [Header("States")]
        [SerializeField] private SerializedDictionary<State, Data.State> states;
        [Header("Percents")]
        [SerializeField, Range(0, 1)] private float fullPercentFood;
        [SerializeField, Range(0, 1)] private float fullPercentPopulation;
        [SerializeField, Range(0, 1)] private float partPercentFood;
        [SerializeField, Range(0, 1)] private float partPercentPopulation;
        [Header("Points")]
        [SerializeField, Min(1)] private float calmVolcanoPointsDivision;
        [SerializeField, Min(0)] private int reduceLossesPoints;

        private State _state = State.Start;

        private protected override string Name => "Volcano";
        private int CalmVolcanoPoints => (int) (_piece.Population.Value / calmVolcanoPointsDivision);

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
            AddDesidion(CalmVolcano, Local("CalmVolcano"), (piece) => CalmVolcanoPoints);
            AddDesidion(ReduceLosses, Local("ReduceLosses"), (piece) => reduceLossesPoints);
            AddDesidion(Nothing, Local("Nothing"), (piece) => 0);
        }

        private bool CalmVolcano(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (!_useIntervention(interventionPoints(_piece)))
                return false;

            ChroniclesController.Deactivate(Name, _piece.RegionID, panelSprite, "CalmVolcano");
            EndEvent();
            return true;
        }

        private bool ReduceLosses(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (!_useIntervention(interventionPoints(_piece)))
                return false;

            _piece.Population.Value -= (int) (_piece.Population.Value * fullPercentFood * partPercentPopulation);
            _piece.ReserveFood.value -= _piece.ReserveFood.value * fullPercentFood * partPercentFood;
            Global.Migration.OnMigration(_piece);
            ChroniclesController.Deactivate(Name, _piece.RegionID, panelSprite, "ReduceLosses");
            EndEvent();
            return true;
        }

        public void ActivateVolcano() {
            _piece.Population.Value -= (int) (_piece.Population.Value * fullPercentFood);
            _piece.ReserveFood.value -= _piece.ReserveFood.value * fullPercentFood;
            Global.Migration.OnMigration(_piece);
            ChroniclesController.Deactivate(Name, _piece.RegionID, panelSprite, "ActivateVolcano");
            EndEvent();
        }

        private bool Nothing(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (!_useIntervention(interventionPoints(_piece)))
                return false;

            ChroniclesController.AddPassive(Name, _piece.RegionID, panelSprite, "Nothing");
            return true;
        }

        private void EndEvent() {
            _piece.RemoveEvent(this);
            CheckMarker();
            NextState();
        }

        private protected override void OpenPanel(CivPiece piece = null) {
            PanelFabric.CreateEvent(HUD.Instance.PanelsParent, _desidionPrefab, panel, this, _piece, panelSprite, Local("Title"),
                                    Territory(), Local("Description"), _desidions);
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
