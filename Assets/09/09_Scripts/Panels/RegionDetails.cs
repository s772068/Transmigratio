using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class RegionDetails : MonoBehaviour {
    [SerializeField] private RegionElements elements;
    [SerializeField] private RegionParams paramiters;
    [SerializeField] private RegionDetailsRightSide rightSide;
    [SerializeField] private ButtonsGroup tabs;
    [SerializeField] private Image civAvatar;

    public Sprite Avatar { set => civAvatar.sprite = value; }

    public TM_Region Region { private get; set; }

    private void Awake() {
        tabs.onClick = OnClickTabs;
        elements.onClick = OnClickElement;
        paramiters.onClick = OnClickParamiter;
    }

    private void OnEnable() {
        bool isHasCiv = Region.CivMain != null;
        if (isHasCiv) elements.ClickCivTab();
        else          elements.ClickRegionTab();

        tabs.gameObject.SetActive(isHasCiv);
    }

    private void OnDisable() {
        elements.ClearElements();
        paramiters.ClearParams();
    }

    public void ClickStartGame() {
        elements.ClickCivTab();
        tabs.gameObject.SetActive(true);
    }

    private void OnClickTabs(int i) {
        paramiters.ClearParams();
        rightSide.gameObject.SetActive(false);
    }

    public void OnClickElement(string key) {
        paramiters.ClearParams();
        rightSide.gameObject.SetActive(false);
        Dictionary<string, int> dic = Transmigratio.Instance.tmdb.GetParam(Region.id, key);
        foreach(var pair in dic) {
            paramiters.CreateParam(pair.Key, pair.Value);
        }
    }

    private void OnClickParamiter(string key) {
        rightSide.gameObject.SetActive(true);
        rightSide.UpdateData(key);
    }
}
