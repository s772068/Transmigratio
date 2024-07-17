using System.Collections.Generic;
using Unity.VisualScripting;
using WorldMapStrategyKit;

namespace Events.Controllers.Local {
    public abstract class Base : Controllers.Base {
        private protected CivPiece selectedPiece;
        private protected List<CivPiece> pieces = new();

        private protected override string Territory => Local("Territory 1") + " " +
                              $"<color=#{regionColor.ToHexString()}>" +
                              selectedPiece.Region.Name + "</color> " +
                              Local("Territory 2") + " " +
                              $"<color=#{civColor.ToHexString()}>" +
                              Localization.Load("Civilizations", selectedPiece.Civilization.Name) + "</color> " +
                              Local("Territory 3");

        private protected void AddEvent(CivPiece piece) {
            if (pieces.Contains(piece)) return;
            selectedPiece = piece;
            pieces.Add(piece);
            if (isShowAgain) {
                OpenPanel();
                piece.AddEvent(this);
                CreateMarker();
            } else {
                activeDesidion.OnClick?.Invoke();
                pieces.Remove(selectedPiece);
            }
        }

        private protected void RemoveEvent(CivPiece piece) {
            pieces.Remove(piece);
            piece.RemoveEvent(this);
            if (piece.EventsCount == 0 && piece.Region.Marker != null) piece.Region.Marker.Destroy();
        }

        public override void CreateMarker() {
            if (selectedPiece.EventsCount == 1) return;
            selectedPiece.Region.Marker ??= CreateMarker(WMSK.countries[selectedPiece.Region.Id].center);
            selectedPiece.Region.Marker.onClick += () => OnClickMarker(selectedPiece);
        }

        private void OnClickMarker(CivPiece piece) {
            selectedPiece = piece;
            OpenPanel();
            panel.IsShowAgain = isShowAgain;
            selectedPiece.Region.Marker.Destroy();
        }
    }
}
