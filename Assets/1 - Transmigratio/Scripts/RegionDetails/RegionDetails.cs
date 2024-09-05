using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UI;

namespace RegionDetails.Old {
    public class RegionDetails : MonoBehaviour {
        [SerializeField] private RegionElements _leftSide;
        [SerializeField] private RegionParams _centerSide;
        [SerializeField] private RegionDetailsRightSide _rightSide;
        [SerializeField] private ButtonsRadioGroup _tabs;
        [SerializeField] private Image _civAvatar;

        public int RegionID;

        private Dictionary<string, float> _dic;
        private string _element;

        private TM_Region Region => Transmigratio.Instance.TMDB.map.AllRegions[RegionID];
        public Sprite Avatar { set => _civAvatar.sprite = value; }

        private void Awake() {
            _tabs.onClick = OnClickTabs;
            _leftSide.onClick = OnClickElement;
            _centerSide.onClick = OnClickParamiter;
        }

        private void OnEnable() {
            SetRegion(RegionID);
            Timeline.TickShow += UpdateParams;
        }

        private void OnDisable() {
            Timeline.TickShow -= UpdateParams;
        }

        public void ClickStartGame() {
            _leftSide.ClickCivTab();
            _tabs.gameObject.SetActive(true);
        }

        private void OnClickTabs(int i) {
            _centerSide.ClearParams();
            _rightSide.gameObject.SetActive(false);
        }

        public void OnClickElement(string key) {
            _centerSide.ClearParams();
            _rightSide.gameObject.SetActive(false);
            _element = key;
            UpdateParams();
        }

        private void UpdateParams() {
            if (_element != null)
                _centerSide.UpdateParamiters(RegionID, _element);
        }

        private void OnClickParamiter(string key) {
            _rightSide.gameObject.SetActive(true);
            _rightSide.UpdateData(_element, key);
        }

        public void NextRegion() => SetRegion((RegionID + 1) % Transmigratio.Instance.TMDB.map.AllRegions.Count);
        public void PrevRegion() => SetRegion(RegionID == 0 ? (Transmigratio.Instance.TMDB.map.AllRegions.Count - 1) : (RegionID - 1));

        private void SetRegion(int index) {
            RegionID = index;
            _leftSide.ClearElements();
            _centerSide.ClearParams();

            bool isHasCiv = Region.CivMain != null;
            if (isHasCiv) _leftSide.ClickCivTab();
            else _leftSide.ClickRegionTab();

            _tabs.gameObject.SetActive(isHasCiv);
            _centerSide.Title = Region.Name;
            _leftSide.ClickRegionTab();
            _leftSide.SelectElement("Climate");
        }
    }
}
