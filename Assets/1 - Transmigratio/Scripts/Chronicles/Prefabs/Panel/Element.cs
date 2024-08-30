using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

namespace Chronicles.Prefabs.Panel {
    public class Element : MonoBehaviour {
        [SerializeField] private TMP_Text textEventName;
        [SerializeField] private TMP_Text textRegion;
        [SerializeField] private TMP_Text textStartYear;
        [SerializeField] private GameObject activate;
        [SerializeField] private Toggle _toggle;

        public Toggle Toggle => _toggle;

        private Data.Panel.Element _data;
        
        public Action<Data.Panel.Element> onClick;

        public Data.Panel.Element Data { set {
                _data = value;
                textEventName.text = value.EventName;
                textRegion.text = Transmigratio.Instance.TMDB.map.AllRegions[value.RegionID].Name;
                textStartYear.text = $"{value.StartYear.ToString("### ###")} {Localization.Load("Base", "BCE")}";
            }
        }

        public void Click() {
            //activate.SetActive(true);
            onClick?.Invoke(_data);
        }
        public void Destroy() => Destroy(gameObject);
    }
}
