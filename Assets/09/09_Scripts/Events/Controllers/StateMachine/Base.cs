using Unity.VisualScripting;
using WorldMapStrategyKit;
using System.Linq;
using System;

namespace Events.Controllers.StateMachines {
    public abstract class Base : Controllers.Base {
        private protected CivPiece piece;

        private protected Data.State curState;
        private Random rand = new();

        private protected override string Territory => Local("Territory1") + " " +
                              $"<color=#{regionColor.ToHexString()}>" +
                              piece.Region.Name + "</color> " +
                              Local("Territory2") + " " +
                              $"<color=#{civColor.ToHexString()}>" +
                              Localization.Load("Civilizations", piece.Civilization.Name) + "</color> " +
                              Local("Territory3");

        private protected abstract void NextState();

        public override void CreateMarker() {
            if (piece.EventsCount == 1) return;
            piece.Region.Marker = CreateMarker(WMSK.countries[piece.Region.Id].center);
            piece.Region.Marker.onClick += () => OnClickMarker();
        }

        private protected override void ActivateEvents() {
            Timeline.TickLogic += OnTickLogic;
        }

        private protected override void DeactivateEvents() {
            Timeline.TickLogic -= OnTickLogic;
        }

        private void OnTickLogic() {
            bool isActivate = curState.Update();
            if (isActivate) NextState();
        }

        public void CreateEvent() {
            Random rand = new();
            var civilizations = Transmigratio.Instance.TMDB.humanity.Civilizations;
            if (civilizations.Count == 0) return;
            piece = civilizations.ElementAt(rand.Next(0, civilizations.Count)).Value.Pieces.ElementAt(rand.Next(0, civilizations.Count)).Value;
            if (isShowAgain) {
                OpenPanel();
                CreateMarker();
                piece.AddEvent(this);
            } else {
                activeDesidion.OnClick?.Invoke();
            }
        }

        private protected void RemoveEvent(CivPiece piece) {
            if(piece.EventsCount == 0 && piece.Region.Marker != null) piece.Region.Marker.Destroy();
            piece.RemoveEvent(this);
        }

        private void OnClickMarker() {
            OpenPanel();
            panel.IsShowAgain = isShowAgain;
            piece.Region.Marker.Destroy();
        }
    }
}
