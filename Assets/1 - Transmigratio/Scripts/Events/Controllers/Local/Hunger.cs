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

        private int AddFoodPoints => (int)(selectedPiece.Population.value / foodPerPerson / 100f * percentPointsForAddFood);
        private int AddSomeFoodPoints => (int)(selectedPiece.Population.value / foodPerPerson / 100f * percentPointsForAddSomeFood);

        private protected override void OpenPanel() {
            PanelFabric.CreateEvent(HUD.Instance.Events, _desidionPrefab, panel, this, panelSprite, Local("Title"),
                                    Territory, Local("Description"), _desidions);
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
            AddDesidion(AddFood, Local("AddFood"), () => AddFoodPoints);
            AddDesidion(AddSomeFood, Local("AddSomeFood"), () => AddSomeFoodPoints);
            AddDesidion(Nothing, Local("Nothing"), () => 0);
        }

        private bool AddFood(Func<int> interventionPoints)
        {
            if (!_useIntervention(interventionPoints()))
                return false;
			
            selectedPiece.ReserveFood += selectedPiece.Population.value / foodPerPerson;
            ChroniclesController.Deactivate(Name, selectedPiece.RegionID, panelSprite, "AddFood");
            RemoveEvent(selectedPiece);
            return true;
        }
		
        private bool AddSomeFood(Func<int> interventionPoints)
        {
            if (!_useIntervention(interventionPoints()))
                return false;

            selectedPiece.ReserveFood += selectedPiece.Population.value / foodPerPerson / 2;
            ChroniclesController.Deactivate(Name, selectedPiece.RegionID, panelSprite, "AddSomeFood");
            RemoveEvent(selectedPiece);
            return true;
        }

        private bool Nothing(Func<int> interventionPoints) {
            if (!_useIntervention(interventionPoints()))
                return false;

            ChroniclesController.AddPassive(Name, selectedPiece.RegionID, panelSprite, "Nothing");
            RemoveEvent(selectedPiece);
            return true;
        }
    }
}
