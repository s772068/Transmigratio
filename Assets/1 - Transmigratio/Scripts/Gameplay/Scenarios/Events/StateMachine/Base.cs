using Unity.VisualScripting;
using WorldMapStrategyKit;
using System.Linq;
using System;
using System.Collections.Generic;
using Gameplay.Scenarios.Events.Data;

namespace Gameplay.Scenarios.Events.StateMachines {
    public abstract class Base : Events.Base {
        private protected Data.State _curState;
        private Random _rand = new();
        protected CivPiece _eventPiece;
        protected TM_Region _eventRegion;

        private protected override string Territory(CivPiece piece = null) => Local("Territory1") + " " +
                              $"<color=#{regionColor.ToHexString()}>" +
                              _eventPiece.Region.Name + "</color> " +
                              Local("Territory2") + " " +
                              $"<color=#{civColor.ToHexString()}>" +
                              Localization.Load("Civilizations", _eventPiece.Civilization.Name) + "</color> " +
                              Local("Territory3");

        private protected abstract void NextState();

        private protected override void CreateMarker(CivPiece piece = null) {
            if (_eventPiece.Region.Marker == null)
                _eventPiece.Region.Marker = CreateMarker(WMSK.countries[_eventPiece.Region.Id].center, piece);
            else
                _eventPiece.Region.Marker.SetCount += 1;
            _eventPiece.Region.Marker.onClick += (_piece) => OnClickMarker();
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
            List<Civilization> civs = new();

            foreach(var emptyCiv in civilizations)
            {
                if (emptyCiv.Value.Pieces.Count > 0)
                    civs.Add(emptyCiv.Value);
            }

            if (civs.Count <= 0) return;

            Civilization civ = civs.ElementAt(rand.Next(0, civs.Count - 1));
            _eventPiece = civ.Pieces.ElementAt(rand.Next(0, civ.Pieces.Count - 1)).Value;
            _eventRegion = _eventPiece.Region;

            ChroniclesController.AddActive(Name, _eventPiece.RegionID, OnClickMarker, new Chronicles.Data.Panel.LocalVariablesChronicles { Count = 0 });

            CreateMarker();
            _eventPiece.AddEvent(this);

            if (!AutoChoice)
                OpenPanel(_eventPiece);
            else
            {
                foreach (var autochoice in Events.AutoChoice.Events[this])
                {
                    if (autochoice is DesidionPiece desP)
                    {
                        if (desP.CostFunc(_eventPiece) <= MaxAutoInterventionPoints)
                        {
                            if (desP.ActionClick.Invoke(_eventPiece, desP.CostFunc))
                                break;
                        }
                    }
                    else if (autochoice is DesidionRegion desR)
                    {
                        if (desR.CostFunc(_eventRegion) <= MaxAutoInterventionPoints)
                        {
                            if (desR.ActionClick.Invoke(_eventRegion, desR.CostFunc))
                                break;
                        }
                    }
                }
            }
        }

        private void OnClickMarker(CivPiece piece = null) {
            OpenPanel(_eventPiece);
        }

        private protected void CheckMarker()
        {
            if (_eventPiece.EventsCount == _eventPiece.MigrationCount && _eventPiece.Region.Marker != null)
            {
                _eventPiece.Region.Marker.Destroy();
                _eventPiece.Region.Marker = null;
            }
            else if (_eventPiece.Region.Marker != null)
            {
                _eventPiece.Region.Marker.SetCount -= 1;
                _eventPiece.Region.Marker.onClick -= (_piece) => OnClickMarker();
            }    
        }
    }
}
