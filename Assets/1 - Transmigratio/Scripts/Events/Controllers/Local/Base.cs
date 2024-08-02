using System.Collections.Generic;
using Unity.VisualScripting;
using WorldMapStrategyKit;

namespace Events.Controllers.Local {
    public abstract class Base : Controllers.Base 
    {
        private protected List<CivPiece> pieces = new();

        private protected override string Territory(CivPiece piece) => Local("Territory 1") + " " +
                              $"<color=#{regionColor.ToHexString()}>" +
                              piece.Region.Name + "</color> " +
                              Local("Territory 2") + " " +
                              $"<color=#{civColor.ToHexString()}>" +
                              Localization.Load("Civilizations", piece.Civilization.Name) + "</color> " +
                              Local("Territory 3");

        private protected void AddEvent(CivPiece piece) {
            if (pieces.Contains(piece)) return;

            pieces.Add(piece);
            if (!AutoChoice) {
                piece.AddEvent(this);
                CreateMarker(piece);
                OpenPanel(piece);
            } 
            else {
                Events.AutoChoice.Events[this][0].ActionClick?.Invoke(piece, Events.AutoChoice.Events[this][0].CostFunc);
            }
        }

        private protected void RemoveEvent(CivPiece piece) {
            pieces.Remove(piece);
            piece.RemoveEvent(this);
            CheckMarker(piece);
        }

        private protected override void CreateMarker(CivPiece piece) {
            if (piece.Region.Marker == null)
                piece.Region.Marker = CreateMarker(WMSK.countries[piece.Region.Id].center, piece);
            piece.Region.Marker.onClick += (piece) => OnClickMarker(piece);
        }

        private void OnClickMarker(CivPiece piece) => OpenPanel(piece);
        
        private protected void CheckMarker(CivPiece piece)
        {
            if (piece.EventsCount == piece.MigrationCount && piece.Region.Marker != null)
            {
                piece.Region.Marker.Destroy();
                piece.Region.Marker = null;
            }    
            else if (piece.Region.Marker != null)
                piece.Region.Marker.onClick -= (piece) => OnClickMarker(piece);
        }
    }
}
