namespace Gameplay.Scenarios.Events {
    [UnityEngine.CreateAssetMenu(menuName = "Gameplay/GreenRevolution", fileName = "GreenRevolution")]
    public class Base : Scenarios.Base {
        private protected virtual void OnAddPiece(CivPiece civPiece) { }
        private protected virtual void OnRemovePiece(CivPiece civPiece) { }

        public override void Init() {
            Civilization.onAddPiece += OnAddPiece;
            Civilization.onRemovePiece += OnRemovePiece;
        }
    }
}
