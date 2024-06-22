using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RegionDetails : MonoBehaviour {
    [SerializeField] private RegionElements leftSide;
    [SerializeField] private RegionParams centerSide;
    [SerializeField] private RegionDetailsRightSide rightSide;
    [SerializeField] private ButtonsGroup tabs;
    [SerializeField] private Image civAvatar;

    public TM_Region region;

    private Dictionary<string, int> dic;
    private string element;

    public Sprite Avatar { set => civAvatar.sprite = value; }
    
    private void Awake() {
        tabs.onClick = OnClickTabs;
        leftSide.onClick = OnClickElement;
        centerSide.onClick = OnClickParamiter;
    }

    private void OnEnable() {
        bool isHasCiv = region.CivMain != null;
        if (isHasCiv) leftSide.ClickCivTab();
        else          leftSide.ClickRegionTab();

        tabs.gameObject.SetActive(isHasCiv);
        centerSide.Title = region.name;
        GameEvents.onTickShow += UpdateParams;
    }

    private void OnDisable() {
        leftSide.ClearElements();
        centerSide.ClearParams();
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
        dic = Transmigratio.Instance.tmdb.GetParam(region.id, element);
        foreach (var pair in dic) {
            if (pair.Value == 0) continue;
            centerSide.SetParamiter(element, pair.Key, pair.Value);
        }
    }

    private void OnClickParamiter(string key) {
        rightSide.gameObject.SetActive(true);
        rightSide.UpdateData(element, key);
    }
}
