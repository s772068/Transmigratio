using UnityEngine;
using System;

namespace Events.Controllers.Local {
    public sealed class Hunger : Base {
        [Header("Desidion")]
        [SerializeField, Min(0)] private int foodPerPerson;
        [Header("Points")]
        [SerializeField, Range(0, 100)] private int percentPointsForAddFood;
        [SerializeField, Range(0, 100)] private int percentPointsForAddSomeFood;

        public static Action<CivPiece> onActivate;
        public static Action<CivPiece> onDeactivate;

        private protected override string Name => "Hunger";

        private int AddFoodPoints(CivPiece piece) => (int)(piece.Population.value / foodPerPerson / 100f * percentPointsForAddFood);
        private int AddSomeFoodPoints(CivPiece piece) => (int)(piece.Population.value / foodPerPerson / 100f * percentPointsForAddSomeFood);

        private protected override void OpenPanel(CivPiece piece) {
            PanelFabric.CreateEvent(HUD.Instance.Events, _desidionPrefab, panel, this, piece, panelSprite, Local("Title"),
                                    Territory(piece), Local("Description"), _desidions);
        }

        private protected override void ActivateEvents() {
            onActivate = AddEvent;
            onDeactivate = RemoveEvent;
            Civilization.RemoveCivPiece += RemoveEvent;
            Events.AutoChoice.NewEvent(this, _desidions);
        }

        private protected override void DeactivateEvents() {
            onActivate = default;
            onDeactivate = default;
            Civilization.RemoveCivPiece -= RemoveEvent;
            Events.AutoChoice.RemoveEvent(this);
        }

        private protected override void InitDesidions() {
            AddDesidion(AddFood, Local("AddFood"), (piece) => AddFoodPoints(piece));
            AddDesidion(AddSomeFood, Local("AddSomeFood"), (piece) => AddSomeFoodPoints(piece));
            AddDesidion(Nothing, Local("Nothing"), (piece) => 0);
        }

        private bool AddFood(CivPiece piece, Func<CivPiece, int> interventionPoints)
        {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            piece.ReserveFood += piece.Population.value / foodPerPerson;
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "AddFood");
            RemoveEvent(piece);
            return true;
        }
		
        private bool AddSomeFood(CivPiece piece, Func<CivPiece, int> interventionPoints)
        {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            piece.ReserveFood += piece.Population.value / foodPerPerson / 2;
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "AddSomeFood");
            RemoveEvent(piece);
            return true;
        }

        private bool Nothing(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            ChroniclesController.AddPassive(Name, piece.RegionID, panelSprite, "Nothing");
            RemoveEvent(piece);
            return true;
        }
    }
}
