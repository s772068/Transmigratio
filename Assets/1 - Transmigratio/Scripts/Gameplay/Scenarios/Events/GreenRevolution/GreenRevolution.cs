namespace Gameplay.Scenarios.Events {
    [UnityEngine.CreateAssetMenu(menuName = "Gameplay/Events/GreenRevolution", fileName = "GreenRevolution")]
    public class GreenRevolution :Base {
        private protected override void OnAddPiece(CivPiece civPiece) {
            civPiece.EcoCulture.GetValue("Farmers").onUpdate += OnUpdate;
        }

        private protected override void OnRemovePiece(CivPiece civPiece) {
            civPiece.EcoCulture.GetValue("Farmers").onUpdate -= OnUpdate;
        }

        private void OnUpdate(float prev, float cur) {
            if (prev == 0 && cur != 0) {
                // Show News GreenRevolution
            }
        }
    }
}
