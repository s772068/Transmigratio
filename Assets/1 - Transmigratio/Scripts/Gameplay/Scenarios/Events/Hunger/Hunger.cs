using HungerEvent = Events.Controllers.Local.Hunger;
using MigrationEvent = Events.Controllers.Global.Migration;

namespace Gameplay.Scenarios.Events {
    [UnityEngine.CreateAssetMenu(menuName = "Gameplay/Events/Hunger", fileName = "Hunger")]
    public class Hunger : Base {
        private protected override void OnAddPiece(CivPiece civPiece) {
            civPiece.PopulationGrow.onUpdate += OnUpdate;
        }

        private protected override void OnRemovePiece(CivPiece civPiece) {
            civPiece.PopulationGrow.onUpdate -= OnUpdate;
        }

        private void OnUpdate(float prev, float cur) {
            if (prev >= 0 && cur < 0) {
                HungerEvent.onActivate?.Invoke(_piece);
                MigrationEvent.OnMigration(_piece);
            } else if (prev < 0 && cur >= 0) {
                HungerEvent.onDeactivate?.Invoke(_piece);
            }
        }
    }
}
