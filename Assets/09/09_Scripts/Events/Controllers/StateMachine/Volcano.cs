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

        private protected override bool ActiveSlider => false;
        private protected override string Name => "Volcano";
        private int CalmVolcanoPoints => (int) (piece.Population.value / calmVolcanoPointsDivision);

        private void Awake()
        {
            curState = states[State.Start];
            curState.Start();
        }

        private protected override void InitDesidions() {
            AddDesidion(CalmVolcano, Local("CalmVolcano"), () => CalmVolcanoPoints);
            AddDesidion(ReduceLosses, Local("ReduceLosses"), () => reduceLossesPoints);
            AddDesidion(default, Local("Nothing"), () => 0);
        }

        private void CalmVolcano() {
            activateIndex = 0;
            EndEvent();
        }

        private void ReduceLosses() {
            activateIndex = 1;
            piece.Population.value -= (int) (piece.Population.value * fullPercentFood * partPercentPopulation);
            piece.ReserveFood -= piece.ReserveFood * fullPercentFood * partPercentFood;
            Global.Migration.onMigration(piece);
            EndEvent();
        }

        public void ActivateVolcano() {
            activateIndex = 2;
            piece.Population.value -= (int) (piece.Population.value * fullPercentFood);
            piece.ReserveFood -= piece.ReserveFood * fullPercentFood;
            Global.Migration.onMigration(piece);
            EndEvent();
        }

        private void EndEvent() {
            ClosePanel();
            piece.Region.Marker.Destroy();
            NextState();
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
