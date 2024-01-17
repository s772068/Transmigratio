using UnityEngine.UI;
using UnityEngine;
using System;

public class GUIE_RegionInfo : MonoBehaviour {
    [SerializeField] private Image infoImg;
    [SerializeField] private Text infoTxt;
    [SerializeField] private Button button;

    [HideInInspector] public int groupIndex;
    [HideInInspector] public int paramiterIndex;
    [HideInInspector] public int detailIndex;

    public Action OnClickButton;

    public SettingsController Settings { private get; set; }

    public void UpdateInfo() {
        //if (!infoImg.gameObject.activeSelf) infoImg.gameObject.SetActive(true);
        //if (!infoTxt.gameObject.activeSelf) infoTxt.gameObject.SetActive(true);
        //switch (groupIndex) {
        //    case 0:
        //        // infoImg.sprite = Settings.Theme.GetEcologySprite(paramiterIndex, detailIndex);
        //        infoTxt.text = detailIndex < 0 ?
        //            Settings.Localization.Map.Ecology.Value[paramiterIndex].Description :
        //            Settings.Localization.Map.Ecology.Value[paramiterIndex].Value[detailIndex].Description;
        //        break;
        //    case 1:
        //        // infoImg.sprite = Settings.Theme.GetCivilizationSprite(paramiterIndex, detailIndex);
        //        infoTxt.text = detailIndex < 0 ?
        //            Settings.Localization.Map.Civilization.Paramiters[paramiterIndex].Description :
        //            infoTxt.text = Settings.Localization.Map.Civilization.Paramiters[paramiterIndex].Value[detailIndex].Description;
        //        break;
        //    default:
        //        button.gameObject.SetActive(false);
        //        break;
        //}
        //if(groupIndex >= 0) button?.gameObject.SetActive(paramiterIndex == 3);
    }

    //public void ClickButton() => OnClickButton?.Invoke();

    public void Clear() {
        //infoImg.gameObject.SetActive(false);
        //infoTxt.gameObject.SetActive(false);
        //button?.gameObject.SetActive(false);
    }
}
