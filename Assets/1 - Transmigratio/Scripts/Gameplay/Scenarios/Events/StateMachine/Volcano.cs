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
        private int CalmVolcanoPoints => (int) (_eventPiece.Population.Value / calmVolcanoPointsDivision);

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
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "CalmVolcano", 
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = (int)Math.Abs(piece.PopulationGrow.value) });
            EndEvent();
            return true;
        }

        private bool ReduceLosses(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            int dead = (int)(piece.Population.Value * fullPercentFood * partPercentPopulation);
            piece.Population.Value -= dead;
            piece.ReserveFood.value -= piece.ReserveFood.value * fullPercentFood * partPercentFood;
            Global.Migration.OnMigration(piece);
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "ReduceLosses", 
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = dead });
            EndEvent();
            return true;
        }

        public void ActivateVolcano() {
            int dead = (int)(_eventPiece.Population.Value * fullPercentFood);
            _eventPiece.Population.Value -= dead;
            _eventPiece.ReserveFood.value -= _eventPiece.ReserveFood.value * fullPercentFood;
            Global.Migration.OnMigration(_piece);
            ChroniclesController.Deactivate(Name, _eventPiece.RegionID, panelSprite, "ActivateVolcano", 
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = dead });
            EndEvent();
        }

        private bool Nothing(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            ActivateVolcano();
            return true;
        }

        private void EndEvent() {
            _eventPiece.RemoveEvent(this);
            CheckMarker();
            NextState();
        }

        private protected override void OpenPanel(CivPiece piece = null) {
            PanelFabric.CreateEvent(HUD.Instance.PanelsParent, _desidionPrefab, panel, this, _eventPiece, panelSprite, Local("Title"),
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
