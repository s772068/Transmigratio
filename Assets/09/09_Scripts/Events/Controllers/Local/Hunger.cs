using UnityEngine;
using System;

namespace Events.Controllers.Local {
    public sealed class Hunger : Base {
        [Header("Desidion")]
        [SerializeField, Min(0)] private float foodPerPerson;
        [Header("Points")]
        [SerializeField, Range(0, 100)] private float percentPointsForAddFood;
        [SerializeField, Range(0, 100)] private int percentPointsForAddSomeFood;

        public static Action<CivPiece> onActivate;
        public static Action<CivPiece> onDeactivate;

        private protected override bool ActiveSlider => false;
        private protected override string Name => "Hunger";

        private int AddFoodPoints => (int)(selectedPiece.Population.value / foodPerPerson / 100 * percentPointsForAddFood);
        private int AddSomeFoodPoints => (int)(selectedPiece.Population.value / foodPerPerson / 100 * percentPointsForAddSomeFood);

        private protected override void ActivateEvents() {
            onActivate = AddEvent;
            onDeactivate = RemoveEvent;
            Civilization.RemoveCivPiece += RemoveEvent;
        }

        private protected override void DeactivateEvents() {
            onActivate = default;
            onDeactivate = default;
            Civilization.RemoveCivPiece -= RemoveEvent;
        }

        private protected override void InitDesidions() {
            AddDesidion(AddFood, Local("AddFood"), () => AddFoodPoints);
            AddDesidion(AddSomeFood, Local("AddSomeFood"), () => AddSomeFoodPoints);
            AddDesidion(default, Local("Nothing"), () => 0);
        }

        private void AddFood() => selectedPiece.ReserveFood += selectedPiece.Population.value / foodPerPerson;
        private void AddSomeFood() => selectedPiece.ReserveFood += selectedPiece.Population.value / foodPerPerson / 2;
    }
}
