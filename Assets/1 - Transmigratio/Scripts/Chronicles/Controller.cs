using System.Collections.Generic;
using Chronicles.Data.Panel;
using UnityEngine;
using System;

namespace Chronicles {
    public class Controller : Singleton<Controller> {
        [SerializeField] private Prefabs.Panel.Panel panel;
        [SerializeField] private GameObject _descriptionBlock;

        private List<Element> _activeEvents = new();
        private List<Element> _passiveEvents = new();

        public void AddActive(string eventName, int regionID, Action<CivPiece> onClick) {
            _activeEvents.Add(new() {
                IsActive = true,
                EventName = eventName,
                RegionID = regionID,
                StartYear = Transmigratio.Instance.TMDB.Year,
                Click = onClick,
            });
            UpdatePanel();
        }

        public void AddPassive(string eventName, int regionID, Sprite sprite, string description) {
            _passiveEvents.Add(new() {
                IsActive = false,
                EventName = eventName,
                DescriptionName = description,
                RegionID = regionID,
                StartYear = Transmigratio.Instance.TMDB.Year,
                Sprite = sprite,
                Click = default
            });
            UpdatePanel();
        }

        public void Deactivate(string eventName, int regionID, Sprite sprite, string description) {
            Debug.Log("Deactivate");
            AddPassive(eventName, regionID, sprite, description);
            RemoveElement(_activeEvents, eventName, regionID);
            UpdatePanel();
        }

        private bool RemoveElement(List<Element> list, string eventName, int regionID) {
            for (int i = 0; i < list.Count; ++i) {
                if (list[i].EventName == eventName &&
                    list[i].RegionID == regionID) {
                    list.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void OpenChronicle() {
            panel.gameObject.SetActive(true);
            panel.onClickElement = UpdateDescription;
            UpdatePanel();
        }

        public void UpdateDescription(Element element) {
            _descriptionBlock.gameObject.SetActive(true);
            panel.InitDescription(element);
        }

        private void UpdatePanel() {
            if (!panel.gameObject.activeSelf) return;

            List<Element> list = new();
            list.AddRange(_activeEvents);
            list.AddRange(_passiveEvents);

            panel.Elements = list;
        }

        //private void SaveToJSON() {
        //    // Save passive events
        //}

        //private void LoadFromJSON() {
        //    // Load passive events
        //}
    }
}
