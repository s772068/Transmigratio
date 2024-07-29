using AYellowpaper.SerializedCollections;
using UnityEngine;
using System;

namespace Events.Controllers.StateMachines {
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
        private int CalmVolcanoPoints => (int) (_piece.Population.value / calmVolcanoPointsDivision);

        private void Awake() {
            _curState = states[State.Start];
            _curState.Start();
        }

        private protected override void ActivateEvents() {
            Events.AutoChoice.NewEvent(this, _desidions);
            base.ActivateEvents();
        }

        private protected override void DeactivateEvents() {
            Events.AutoChoice.RemoveEvent(this);
            base.DeactivateEvents();
        }

        private protected override void InitDesidions() {
            AddDesidion(CalmVolcano, Local("CalmVolcano"), () => CalmVolcanoPoints);
            AddDesidion(ReduceLosses, Local("ReduceLosses"), () => reduceLossesPoints);
            AddDesidion(Nothing, Local("Nothing"), () => 0);
        }

        private void CalmVolcano() {
            ChroniclesController.Deactivate(Name, _piece.RegionID, panelSprite, "CalmVolcano");
            EndEvent();
        }

        private void ReduceLosses() {
            _piece.Population.value -= (int) (_piece.Population.value * fullPercentFood * partPercentPopulation);
            _piece.ReserveFood -= _piece.ReserveFood * fullPercentFood * partPercentFood;
            Global.Migration.OnMigration(_piece);
            ChroniclesController.Deactivate(Name, _piece.RegionID, panelSprite, "ReduceLosses");
            EndEvent();
        }

        public void ActivateVolcano() {
            _piece.Population.value -= (int) (_piece.Population.value * fullPercentFood);
            _piece.ReserveFood -= _piece.ReserveFood * fullPercentFood;
            Global.Migration.OnMigration(_piece);
            ChroniclesController.Deactivate(Name, _piece.RegionID, panelSprite, "ActivateVolcano");
            EndEvent();
        }

        private void Nothing() {
            ChroniclesController.AddPassive(Name, _piece.RegionID, panelSprite, "Nothing");
        }

        private void EndEvent() {
            _piece.RemoveEvent(this);
            CheckMarker();
            NextState();
        }

        private protected override void OpenPanel() {
            PanelFabric.CreateEvent(HUD.Instance.Events, _desidionPrefab, panel, this, panelSprite, Local("Title"),
                                    Territory, Local("Description"), _desidions);
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
