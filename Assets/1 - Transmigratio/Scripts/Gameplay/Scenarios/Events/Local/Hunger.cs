using UnityEngine;
using System;

using MigrationEvent = Gameplay.Scenarios.Events.Global.Migration;

namespace Gameplay.Scenarios.Events.Local {
    [CreateAssetMenu(menuName = "ScriptableObjects/Scenarios/Events/Local/Hunger", fileName = "Hunger")]
    public sealed class Hunger : Base {
        [Header("Desidion")]
        [SerializeField, Min(0)] private int foodPerPerson;
        [Header("Points")]
        [SerializeField, Range(0, 100)] private int percentPointsForAddFood;
        [SerializeField, Range(0, 100)] private int percentPointsForAddSomeFood;

        private protected override string Name => "Hunger";

        private int AddFoodPoints(CivPiece piece) => (int)(piece.Population.Value / foodPerPerson / 100f * percentPointsForAddFood);
        private int AddSomeFoodPoints(CivPiece piece) => (int)(piece.Population.Value / foodPerPerson / 100f * percentPointsForAddSomeFood);

        private protected override void OnAddPiece(CivPiece civPiece) {
            civPiece.PopulationGrow.onUpdate += OnUpdate;
        }

        private protected override void OnRemovePiece(CivPiece civPiece) {
            civPiece.PopulationGrow.onUpdate -= OnUpdate;
        }

        private void OnUpdate(float prev, float cur, CivPiece piece) {
            if (prev >= 0 && cur < 0) {
                AddEvent(piece);
                MigrationEvent.OnMigration(piece);
            } else if (prev < 0 && cur >= 0) {
                RemoveEvent(piece);
            }
        }

        private protected override void OpenPanel(CivPiece piece) {
            PanelFabric.CreateEvent(HUD.Instance.PanelsParent, _desidionPrefab, panel, this, piece, panelSprite, Local("Title"),
                                    Territory(piece), Local("Description"), _desidions);
        }

        private protected override void ActivateEvents() {
            Civilization.onRemovePiece += RemoveEvent;
            Events.AutoChoice.NewEvent(this, _desidions);
        }

        private protected override void InitDesidions() {
            AddDesidion(Nothing, Local("Nothing"), (piece) => 0);
            AddDesidion(AddFood, Local("AddFood"), (piece) => GetDesidionCost(AddFoodPoints(piece)));
            AddDesidion(AddSomeFood, Local("AddSomeFood"), (piece) => GetDesidionCost(AddSomeFoodPoints(piece)));
        }

        private bool AddFood(CivPiece piece, Func<CivPiece, int> interventionPoints)
        {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            piece.ReserveFood.value += piece.Population.Value / foodPerPerson;
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "AddFood", 
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = (int)Math.Abs(piece.PopulationGrow.value) });
            RemoveEvent(piece);
            return true;
        }
		
        private bool AddSomeFood(CivPiece piece, Func<CivPiece, int> interventionPoints)
        {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            piece.ReserveFood.value += piece.Population.Value / foodPerPerson / 2;
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "AddSomeFood", 
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = (int)Math.Abs(piece.PopulationGrow.value) });
            RemoveEvent(piece);
            return true;
        }

        private bool Nothing(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            ChroniclesController.AddPassive(Name, piece.RegionID, panelSprite, "Nothing", 
                new Chronicles.Data.Panel.LocalVariablesChronicles { Count = (int)Math.Abs(piece.PopulationGrow.value) });
            RemoveEvent(piece);
            return true;
        }
    }
}
