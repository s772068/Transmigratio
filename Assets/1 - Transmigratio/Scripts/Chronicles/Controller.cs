using System.Collections.Generic;
using Chronicles.Data.Panel;
using UnityEngine;
using System;

namespace Chronicles {
    public class Controller : Singleton<Controller> {
        [SerializeField] private Prefabs.Panel.Panel panel;
        [SerializeField] private Prefabs.Consequence consequence;
        
        private int numList;
        private List<Element> _activeEvents = new();
        private List<Element> _passiveEvents = new();

        public void AddActive(string eventName, int regionID, Action onClick) {
            _activeEvents.Add(new() {
                isActive = true,
                eventName = eventName,
                regionID = regionID,
                startYear = Transmigratio.Instance.TMDB.Year,
                onClick = onClick,
            });
            UpdatePanel();
        }

        public void AddPassive(string eventName, int regionID, Sprite sprite, string description) {
            _passiveEvents.Add(new() {
                isActive = false,
                eventName = eventName,
                description = description,
                regionID = regionID,
                startYear = Transmigratio.Instance.TMDB.Year,
                sprite = sprite,
                onClick = default
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
                if (list[i].eventName == eventName &&
                    list[i].regionID == regionID) {
                    list.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void OpenChronicle() {
            panel.gameObject.SetActive(true);
            panel.onClickElement = OpenConsequence;
            numList = 0;
            UpdatePanel();
        }

        public void OpenConsequence(Element element) {
            consequence.gameObject.SetActive(true);
            consequence.Init(element);
        }

        public void PrevPage() {
            if (numList != 0) {
                --numList;
                UpdatePanel();
            }
        }

        public void NextPage() {
            int listCount = _activeEvents.Count + _passiveEvents.Count;
            int count = panel.CountElements;
            int index = numList * count;

            if (listCount > (numList + 1) * count) {
                ++numList;
                UpdatePanel();
            }
        }

        private void UpdatePanel() {
            if (!panel.gameObject.activeSelf) return;

            Debug.Log($"Active count: {_activeEvents.Count}");
            Debug.Log($"Passive count: {_passiveEvents.Count}");

            List<Element> list = new();
            list.AddRange(_activeEvents);
            list.AddRange(_passiveEvents);


            int count = panel.CountElements;
            int index = numList * count;
            count = list.Count - index < count ? list.Count - index : count;

            List<Element> list2 = list.GetRange(index, count);

            panel.Elements = list.GetRange(index, count);
        }

        //private void SaveToJSON() {
        //    // Save passive events
        //}

        //private void LoadFromJSON() {
        //    // Load passive events
        //}
    }
}
