using System;
using UnityEngine;
using UnityEngine.UI;

// States
// 1: Region
// 2: Civilization
public class GUIP_Region : MonoBehaviour, IGameConnecter {
    [SerializeField] private GameObject groupButtons;
    [SerializeField] private Text EcologyBtnTxt;
    [SerializeField] private Text CivilizationBtnTxt;
    [SerializeField] private GUIE_RegionParamiters paramiters;
    [SerializeField] private GUIE_RegionDetails details;
    [SerializeField] private GUIE_RegionInfo info;
    
    private SettingsController settings;
    private WmskController wmsk;
    private MapController map;

    private int state;
    private int viewGroupIndex = -1;

    public Action<int, int> OnClickCivilizationBtn;

    public GameController GameController {
        set {
            value.Get(out map);
            paramiters.Map = map;
            details.Map = map;

            value.Get(out settings);
            paramiters.Settings = settings;
            details.Settings = settings;
            info.Settings = settings;

            value.Get(out wmsk);
        }
    }

    public void Open() {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
            state = 0;
        }
        else state = 1;
        Initialization();
    }

    public void Close() {
        details.Clear();
        info.Clear();

        map.OnUpdate -= paramiters.UpdateEcology;
        map.OnUpdate -= paramiters.UpdateCivilization;
        map.OnUpdate -= details.UpdateDetails;
        paramiters.OnClickParamiter -= InitDetails;
        paramiters.OnClickParamiter -= UpdateParamiterInfo;
        details.OnClickDetail -= UpdateDetailInfo;
        info.OnClickButton -= Open;

        --state;
        if (state >= 0) Initialization();
        else {
            viewGroupIndex = -1;
            gameObject.SetActive(false);
        }
    }

    public void ShowEcology() {
        if (viewGroupIndex != 0) {
            viewGroupIndex = 0;
            UpdateEcology();
        }
    }

    public void ShowCivilization() {
        if (viewGroupIndex != 1) {
            viewGroupIndex = 1;
            UpdateCivilization();
        }
    }

    private void Clear() {
        details.Clear();
        info.Clear();

        map.OnUpdate -= paramiters.UpdateEcology;
        map.OnUpdate -= paramiters.UpdateCivilization;
        map.OnUpdate -= details.UpdateDetails;
        paramiters.OnClickParamiter -= InitDetails;
        paramiters.OnClickParamiter -= UpdateParamiterInfo;
        details.OnClickDetail -= UpdateDetailInfo;
        info.OnClickButton -= Open;
    }

    private void Initialization() {
        Clear();

        paramiters.regionIndex = wmsk.SelectedIndex;
        paramiters.State = state;

        paramiters.OnClickParamiter += InitDetails;
        paramiters.OnClickParamiter += UpdateParamiterInfo;
        details.OnClickDetail += UpdateDetailInfo;

        Localization();

        switch (state) {
            case 0: InitializationRegionState(); break;
            case 1: InitializationCivilizationState(); break;
        }
    }

    private void InitializationRegionState() {
        paramiters.civilizationIndex = -1;
        details.civilizationIndex = -1;

        groupButtons.SetActiveFlexalon(true);
        paramiters.ShowPortrait(false);

        info.OnClickButton += Open;

        ShowEcology();
    }

    private void InitializationCivilizationState() {
        int civilizationIndex = info.detailIndex > -1 ?
            info.detailIndex : map.data.Regions[paramiters.regionIndex].MaxPopulationsIndex;
        paramiters.civilizationIndex = civilizationIndex;
        details.civilizationIndex = civilizationIndex;

        groupButtons.SetActiveFlexalon(false);
        paramiters.ShowPortrait(true);

        UpdateCivilization();
    }

    private void Localization() {
        paramiters.LocalizationName();
        EcologyBtnTxt.text = settings.Localization.Map.Ecology.Name;
        CivilizationBtnTxt.text = settings.Localization.Map.Civilization.Name;
    }

    private void InitDetails() {
        map.OnUpdate -= details.UpdateDetails;
        map.OnUpdate += details.UpdateDetails;
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

    private void UpdateCivilization() {
        map.OnUpdate -= paramiters.UpdateEcology;
        map.OnUpdate += paramiters.UpdateCivilization;
        paramiters.ShowCivilization();
    }

    private void UpdateEcology() {
        map.OnUpdate -= paramiters.UpdateCivilization;
        map.OnUpdate += paramiters.UpdateEcology;
        paramiters.ShowEcology();
    }

    public void Init() { }
}
