using Unity.VisualScripting;
using WorldMapStrategyKit;
using System.Linq;
using System;

namespace Gameplay.Scenarios.Events.StateMachines {
    public abstract class Base : Events.Base {
        private protected Data.State _curState;
        private Random _rand = new();

        private protected override string Territory(CivPiece piece = null) => Local("Territory1") + " " +
                              $"<color=#{regionColor.ToHexString()}>" +
                              _piece.Region.Name + "</color> " +
                              Local("Territory2") + " " +
                              $"<color=#{civColor.ToHexString()}>" +
                              Localization.Load("Civilizations", _piece.Civilization.Name) + "</color> " +
                              Local("Territory3");

        private protected abstract void NextState();

        private protected override void CreateMarker(CivPiece piece = null) {
            if (_piece.Region.Marker == null)
                _piece.Region.Marker = CreateMarker(WMSK.countries[_piece.Region.Id].center, piece);
            _piece.Region.Marker.onClick += (_piece) => OnClickMarker();
        }

        private protected override void ActivateEvents() {
            Timeline.TickLogic += OnTickLogic;
        }

        private void OnTickLogic() {
            bool isActivate = _curState.Update();
            if (isActivate) NextState();
        }

        public void CreateEvent() {
            Random rand = new();
            var civilizations = Transmigratio.Instance.TMDB.humanity.Civilizations;
            if (civilizations.Count == 0) return;
            _piece = civilizations.ElementAt(rand.Next(0, civilizations.Count)).Value.Pieces.ElementAt(rand.Next(0, civilizations.Count)).Value;

            ChroniclesController.AddActive(Name, _piece.RegionID, OnClickMarker);

            if (!AutoChoice) {
                CreateMarker();
                _piece.AddEvent(this);
                OpenPanel(_piece);
            } else {
                Events.AutoChoice.Events[this][0].ActionClick?.Invoke(_piece, Events.AutoChoice.Events[this][0].CostFunc);
            }
        }

        private void OnClickMarker(CivPiece piece = null) {
            OpenPanel(_piece);
        }

        private protected void CheckMarker()
        {
            if (_piece.EventsCount == _piece.MigrationCount && _piece.Region.Marker != null)
            {
                _piece.Region.Marker.Destroy();
                _piece.Region.Marker = null;
            }
            else if (_piece.Region.Marker != null)
                _piece.Region.Marker.onClick -= (_piece) => OnClickMarker();
        }
    }
}
