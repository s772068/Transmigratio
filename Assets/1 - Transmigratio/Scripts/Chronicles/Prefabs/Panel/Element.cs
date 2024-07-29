using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace Chronicles.Prefabs.Panel {
    public class Element : MonoBehaviour {
        [SerializeField] private TMP_Text textEventName;
        [SerializeField] private TMP_Text textRegion;
        [SerializeField] private TMP_Text textStartYear;
        [SerializeField] private Image background;
        [SerializeField] private Sprite activeBack;
        [SerializeField] private Sprite passiveBack;

        private Data.Panel.Element _data;
        
        public Action<Data.Panel.Element> onClick;

        public Data.Panel.Element Data { set {
                _data = value;
                textEventName.text = value.eventName;
                textRegion.text = Transmigratio.Instance.TMDB.map.AllRegions[value.regionID].Name;
                textStartYear.text = $"{value.startYear.ToString("### ###")} {Localization.Load("Base", "BCE")}";
                background.sprite = value.isActive ? activeBack : passiveBack;
            }
        }

        public void Click() {
            if (_data.isActive) {
                _data.onClick?.Invoke();
            } else {
                onClick?.Invoke(_data);
            }
        }
        public void Destroy() => Destroy(gameObject);
    }
}
