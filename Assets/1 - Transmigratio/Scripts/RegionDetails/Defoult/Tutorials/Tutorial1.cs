using UnityEngine;
using TMPro;

namespace RegionDetails.Defoult.Tutorials {
    public class Tutorial1 : MonoBehaviour {
        [SerializeField] private TMP_Text text;
        private void OnEnable() {
            text.text = Localization.Load("Tutorial", "DefoultRegionDetails1") +
                        Transmigratio.Instance.TMDB.map.AllRegions[MapData.RegionID].Name +
                        Localization.Load("Tutorial", "DefoultRegionDetails2");
        }
    }
}
