using UnityEngine;

public class GUIP_Civilization : MonoBehaviour {
    [SerializeField] private GUIE_RegionParamiters paramiters;
    [SerializeField] private GUIE_RegionDetails details;
    [SerializeField] private GUIE_RegionInfo info;

    [SerializeField] private SettingsController Settings;
    [SerializeField] private MapController Map;

    public void Open(int regionIndex, int civilizationIndex) {
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);

        Map.OnUpdate += paramiters.UpdateCivilization;
        paramiters.OnClickParamiter += InitDetails;
        paramiters.OnClickParamiter += UpdateParamiterInfo;
        details.OnClickDetail += UpdateDetailInfo;

        paramiters.regionIndex = regionIndex;
        paramiters.civilizationIndex = civilizationIndex;
        details.civilizationIndex = civilizationIndex;
        Localization();
        paramiters.UpdateCivilization();
    }

    public void Close() {
        Map.OnUpdate -= paramiters.UpdateCivilization;
        Map.OnUpdate -= details.UpdateDetails;
        paramiters.OnClickParamiter -= InitDetails;
        paramiters.OnClickParamiter -= UpdateParamiterInfo;
        details.OnClickDetail -= UpdateDetailInfo;
        
        details.Clear();
        info.Clear();
        gameObject.SetActive(false);
    }

    private void Localization() {
        paramiters.LocalizationName();
        paramiters.LocalizationPartret();
        paramiters.LocalizationCivilization();
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

    private void Awake() {
        paramiters.Map = Map;
        details.Map = Map;

        paramiters.Settings = Settings;
        details.Settings = Settings;
        info.Settings = Settings;
    }
}
