using HungerEvent = Events.Controllers.Local.Hunger;
using MigrationEvent = Events.Controllers.Global.Migration;

namespace Gameplay.Scenarios {
    [UnityEngine.CreateAssetMenu(menuName = "Gameplay/Hunger", fileName = "Hunger")]
    public class Hunger : Base {
        private float PrevPopulationGrow => _piece.PrevPopulationGrow;
        private float PopulationGrow => _piece.PopulationGrow;
        private protected override void Play() {
            if (PrevPopulationGrow >= 0 && PopulationGrow < 0) {
                HungerEvent.onActivate?.Invoke(_piece);
                MigrationEvent.OnMigration(_piece);
            } else if (PrevPopulationGrow < 0 && PopulationGrow >= 0) {
                HungerEvent.onDeactivate?.Invoke(_piece);
            }
        }
    }
}
