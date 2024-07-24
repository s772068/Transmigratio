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
            if (IsShowAgain) {
                piece.AddEvent(this);
                CreateMarker(piece);
                OpenPanel();
            } 
            else {
                _activeDesidion.ActionClick?.Invoke();
                pieces.Remove(selectedPiece);
            }
        }

        private protected void RemoveEvent(CivPiece piece) {
            pieces.Remove(piece);
            piece.RemoveEvent(this);
            CheckMarker(piece);
        }

        private protected override void CreateMarker(CivPiece piece) {
            if (selectedPiece.Region.Marker == null)
                selectedPiece.Region.Marker = CreateMarker(WMSK.countries[selectedPiece.Region.Id].center);
            selectedPiece.Region.Marker.onClick += () => OnClickMarker(piece);
        }

        private void OnClickMarker(CivPiece piece) {
            selectedPiece = piece;
            OpenPanel();
        }

        private protected void CheckMarker(CivPiece piece)
        {
            if (piece.EventsCount == piece.MigrationCount && piece.Region.Marker != null)
            {
                piece.Region.Marker.Destroy();
                piece.Region.Marker = null;
            }    
            else if (piece.Region.Marker != null)
                piece.Region.Marker.onClick -= () => OnClickMarker(piece);
        }
    }
}
