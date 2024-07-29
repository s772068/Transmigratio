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

        private State state = State.Start;

        private protected override string Name => "Volcano";
        private int CalmVolcanoPoints => (int) (piece.Population.value / calmVolcanoPointsDivision);

        private void Awake() {
            curState = states[State.Start];
            curState.Start();
        }

        private protected override void InitDesidions() {
            AddDesidion(CalmVolcano, Local("CalmVolcano"), () => CalmVolcanoPoints);
            AddDesidion(ReduceLosses, Local("ReduceLosses"), () => reduceLossesPoints);
            AddDesidion(Nothing, Local("Nothing"), () => 0);
        }

        private void CalmVolcano() {
            _activateIndex = 0;
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "CalmVolcano");
            EndEvent();
        }

        private void ReduceLosses() {
            _activateIndex = 1;
            piece.Population.value -= (int) (piece.Population.value * fullPercentFood * partPercentPopulation);
            piece.ReserveFood -= piece.ReserveFood * fullPercentFood * partPercentFood;
            Global.Migration.OnMigration(piece);
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "ReduceLosses");
            EndEvent();
        }

        public void ActivateVolcano() {
            _activateIndex = 2;
            piece.Population.value -= (int) (piece.Population.value * fullPercentFood);
            piece.ReserveFood -= piece.ReserveFood * fullPercentFood;
            Global.Migration.OnMigration(piece);
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "ActivateVolcano");
            EndEvent();
        }

        private void Nothing() {
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "Nothing");
        }

        private void EndEvent() {
            piece.RemoveEvent(this);
            if (piece.EventsCount == 0 && piece.Region.Marker != null) piece.Region.Marker.Destroy();
            NextState();
        }

        private protected override void OpenPanel()
        {
            PanelFabric.CreateEvent(HUD.Instance.Events, _desidionPrefab, panel, this, panelSprite, Local("Title"),
                                    Territory, Local("Description"), _desidions);
        }

        private protected override void NextState() {
            state = state switch {
                State.Start => State.ActivateVolcano,
                State.ActivateVolcano => State.Restart,
                State.Restart => State.ActivateVolcano,
            };
            curState = states[state];
            curState.Start();
        }

        private enum State { Start, ActivateVolcano, Restart }
    }
}
