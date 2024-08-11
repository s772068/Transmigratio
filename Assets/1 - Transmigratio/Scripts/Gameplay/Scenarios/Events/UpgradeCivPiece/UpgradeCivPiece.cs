namespace Gameplay.Scenarios.Events {
    [UnityEngine.CreateAssetMenu(menuName = "Gameplay/Events/UpgradeCivPiece", fileName = "UpgradeCivPiece")]
    public class UpgradeCivPiece : Base {
        private protected override void OnAddPiece(CivPiece civPiece) {
            civPiece.Government.GetValue("Monarchy").onUpdate += OnUpdate;
        }

        private protected override void OnRemovePiece(CivPiece civPiece) {
            civPiece.Government.GetValue("Monarchy").onUpdate -= OnUpdate;
        }

        private void OnUpdate(float prev, float cur) {
            if(prev == 0 && cur != 0) {
                Humanity humanity = Transmigratio.Instance.TMDB.humanity;
                if (!humanity.Civilizations.ContainsKey("Aztec")) {
                    Civilization civ = new("Aztec");
                    humanity.Civilizations["Aztec"] = civ;
                }
                humanity.Civilizations["Aztec"].AddPiece(_piece);
                humanity.RemovePiece(_piece.CivName, _piece.RegionID);
            }
        }
    }
}
