using System.Collections.Generic;
using UnityEngine.UI;
using Database.Data;
using UnityEngine;
using Scenes.Game;

namespace RegionDetails {
    public class Panel : MonoBehaviour {
        [SerializeField] private LeftSide _leftSide;
        [SerializeField] private CenterSide _centerSide;
        [SerializeField] private RightSide _rightSide;
        [SerializeField] private UI.Buttons.Radio.Group _tabs;
        [SerializeField] private Image _civAvatar;

        public int RegionID;

        private Dictionary<string, int> _dic;
        private string _element;

        private Region Region => Transmigratio.Instance.Database.map.AllRegions[RegionID];
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
            if (_element == null) return;
            _dic = Transmigratio.Instance.Database.GetParam(RegionID, _element);
            foreach (var pair in _dic) {
                if (pair.Value == 0) continue;
                _centerSide.SetParamiter(_element, pair.Key, pair.Value);
            }
        }

        private void OnClickParamiter(string key) {
            _rightSide.gameObject.SetActive(true);
            _rightSide.UpdateData(_element, key);
        }

        public void NextRegion() => SetRegion((RegionID + 1) % Transmigratio.Instance.Database.map.AllRegions.Count);
        public void PrevRegion() => SetRegion(RegionID == 0 ? (Transmigratio.Instance.Database.map.AllRegions.Count - 1) : (RegionID - 1));

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
