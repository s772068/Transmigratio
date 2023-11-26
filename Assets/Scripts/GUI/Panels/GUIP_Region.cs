using System;
using UnityEngine;

public class GUIP_Region : MonoBehaviour {
    [SerializeField] private GUIE_RegionParamiters paramiters;
    [SerializeField] private GUIE_RegionDetails details;
    [SerializeField] private GUIE_RegionInfo info;
    [Space]
    [SerializeField] private GUIP_Civilization civilization;
    [Space]
    [SerializeField] private SettingsController Settings;
    [SerializeField] private MapController Map;

    public Action<int, int> OnClickCivilizationBtn;

    public void Open(int regionIndex) {
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);

        Map.OnUpdate += UpdatePanel;
        paramiters.OnClickParamiter += InitDetails;
        paramiters.OnClickParamiter += UpdateParamiterInfo;
        details.OnClickDetail += UpdateDetailInfo;
        info.OnClickButton += OpenCivilization;

        paramiters.regionIndex = regionIndex;
        Localization();
        UpdatePanel();
    }

    public void Close() {
        Map.OnUpdate -= UpdatePanel;
        Map.OnUpdate -= details.UpdateDetails;
        paramiters.OnClickParamiter -= InitDetails;
        paramiters.OnClickParamiter -= UpdateParamiterInfo;
        details.OnClickDetail -= UpdateDetailInfo;
        info.OnClickButton -= OpenCivilization;

        details.Clear();
        info.Clear();
        gameObject.SetActive(false);
    }

    private void Localization() {
        paramiters.LocalizationName();
        paramiters.LocalizationEcology();
        paramiters.LocalizationCivilization();
    }

    private void UpdatePanel() {
        paramiters.UpdateEcology();
        paramiters.UpdateCivilization();
    }

    private void InitDetails() {
        Map.OnUpdate -= details.UpdateDetails;
        Map.OnUpdate += details.UpdateDetails;
        details.regionIndex = paramiters.regionIndex;
        details.groupIndex = paramiters.groupIndex;
        details.paramiterIndex = paramiters.paramiterIndex;
        details.Initialization();
    }

    private void UpdateParamiterInfo() {
        info.groupIndex = paramiters.groupIndex;
        info.paramiterIndex = paramiters.paramiterIndex;
        info.detailIndex = -1;
        info.UpdateInfo();
    }

    private void UpdateDetailInfo() {
        info.detailIndex = details.detailIndex;
        info.UpdateInfo();
    }

    public void OpenCivilization() {
        civilization.Open(paramiters.regionIndex,
            Map.data.Regions[paramiters.regionIndex].MaxPopulationsIndex);
    }

    private void Awake() {
        paramiters.Map = Map;
        details.Map = Map;

        paramiters.Settings = Settings;
        details.Settings = Settings;
        info.Settings = Settings;

        paramiters.civilizationIndex = -1;
        details.civilizationIndex = -1;
    }
}
