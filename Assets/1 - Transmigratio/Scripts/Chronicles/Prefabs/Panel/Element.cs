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
        [SerializeField] private GameObject activate;

        private Data.Panel.Element _data;
        
        public Action<Data.Panel.Element> onClick;

        public Data.Panel.Element Data { set {
                _data = value;
                textEventName.text = value.EventName;
                textRegion.text = Transmigratio.Instance.TMDB.map.AllRegions[value.RegionID].Name;
                textStartYear.text = $"{value.StartYear.ToString("### ###")} {Localization.Load("Base", "BCE")}";
                background.sprite = value.IsActive ? activeBack : passiveBack;
            }
        }

        public void Click() {
            activate.SetActive(true);

            if (_data.IsActive) {
                _data.Click?.Invoke(_data.Piece);
            } else {
                onClick?.Invoke(_data);
            }
        }
        public void Destroy() => Destroy(gameObject);
    }
}
