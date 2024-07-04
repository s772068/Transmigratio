using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RegionDetails : MonoBehaviour {
    [SerializeField] private RegionElements leftSide;
    [SerializeField] private RegionParams centerSide;
    [SerializeField] private RegionDetailsRightSide rightSide;
    [SerializeField] private ButtonsRadioGroup tabs;
    [SerializeField] private Image civAvatar;

    public int regionID;

    private Dictionary<string, int> dic;
    private string element;

    private TM_Region Region => Transmigratio.Instance.tmdb.map.allRegions[regionID];
    public Sprite Avatar { set => civAvatar.sprite = value; }
    
    private void Awake() {
        tabs.onClick = OnClickTabs;
        leftSide.onClick = OnClickElement;
        centerSide.onClick = OnClickParamiter;
    }

    private void OnEnable() {
        SetRegion(regionID);
        GameEvents.onTickShow += UpdateParams;
    }

    private void OnDisable() {
        GameEvents.onTickShow -= UpdateParams;
    }

    public void ClickStartGame() {
        leftSide.ClickCivTab();
        tabs.gameObject.SetActive(true);
    }

    private void OnClickTabs(int i) {
        centerSide.ClearParams();
        rightSide.gameObject.SetActive(false);
    }

    public void OnClickElement(string key) {
        centerSide.ClearParams();
        rightSide.gameObject.SetActive(false);
        element = key;
        UpdateParams();
    }

    private void UpdateParams() {
        if (element == null) return;
        dic = Transmigratio.Instance.tmdb.GetParam(regionID, element);
        foreach (var pair in dic) {
            if (pair.Value == 0) continue;
            centerSide.SetParamiter(element, pair.Key, pair.Value);
        }
    }

    private void OnClickParamiter(string key) {
        rightSide.gameObject.SetActive(true);
        rightSide.UpdateData(element, key);
    }

    public void NextRegion() => SetRegion((regionID + 1) % Transmigratio.Instance.tmdb.map.allRegions.Count);
    public void PrevRegion() => SetRegion(regionID == 0 ? (Transmigratio.Instance.tmdb.map.allRegions.Count - 1) : (regionID - 1));

    private void SetRegion(int index) {
        regionID = index;
        leftSide.ClearElements();
        centerSide.ClearParams();

        bool isHasCiv = Region.CivMain != null;
        if (isHasCiv) leftSide.ClickCivTab();
        else leftSide.ClickRegionTab();

        tabs.gameObject.SetActive(isHasCiv);
        centerSide.Title = Region.name;
        leftSide.ClickRegionTab();
        leftSide.SelectElement("Climate");
    }
}
