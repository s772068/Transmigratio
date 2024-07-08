using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

/// <summary>
/// Интерфейс, всплывающие окна и тд
/// </summary>
public class HUD : MonoBehaviour
{
    [SerializeField] private Messanger messanger;

    [Header("Tutorial")]
    public Transform tutorialWhole;     //объект-родитель всех обучающих объектов
    public Transform welcomePopup;      //привествие с портретом бога
    public Transform surePopup;         //окно подтверждения отмены обучения
    public TMP_Text welcomePopupText;   //текст в привественном окне
    public Button welcomeContinueBtn;   //кнопка продолжения обучения
    public Button cancelTutor;          //кнопка "Да" в подтверждающем окошке, при нажатии отменяется обучение
    public Transform otherPopup;        //окошко для остальных обучающих сообщений
    public TMP_Text otherPopupText;     //текст в otherPopup
    public Button okBtn;                //кнопка "ок" в otherPopup
    public bool tutorCanceled;  //флаг, указывающий на то, отменён ли тутор
    
    public bool isShowMigration = true;

    [Header("Region Details")]
    public RegionDetails regionDetails;        //окно с информацией о выбранном регионе

    public Migration migration;

    private void Awake() {
        GameEvents.onShowMessage = OnShowMessage;
    }

    private void OnShowMessage(string message) {
        messanger.gameObject.SetActive(true);
        messanger.Message = message;
    }

    public void StartTutorial() {
        tutorialWhole.gameObject.SetActive(true);
        welcomePopup.gameObject.SetActive(true);
        welcomePopupText.text = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", "Tutorial0");
        welcomeContinueBtn.onClick.AddListener(() => ShowTutorPopup("Tutorial1"));
        cancelTutor.onClick.AddListener(StopTutorial);
    }

    public void StopTutorial() {
        //welcomePopup.gameObject.SetActive(false);
        //surePopup.gameObject.SetActive(false);
        tutorialWhole.gameObject.SetActive(false);
        tutorCanceled = true;
    }

    public void ShowTutorPopup(string tutorKey) {
        if (tutorCanceled) return;
        otherPopupText.text = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", tutorKey);
        tutorialWhole.gameObject.SetActive(true);
        otherPopup.gameObject.SetActive(true);
        welcomePopup.gameObject.SetActive(false);
    }

    public void ShowMigration() {
        if (!isShowMigration) return;
        //migration
    }

    public void ShowRegionDetails(int region) {
        regionDetails.regionID = region;
        regionDetails.gameObject.SetActive(true);
    }
}
