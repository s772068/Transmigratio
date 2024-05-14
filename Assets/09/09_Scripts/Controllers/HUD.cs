using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
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
    public bool tutorCanceled = false;  //флаг, указывающий на то, отменён ли тутор

    [Header("Region Details")]
    public Transform regionDetailsPanel;        //окно с информацией о выбранном регионе
    public bool firstlyOpen = true;             // флаг для обучения
    public TMP_Text title;                      // название региона в окошке RegionDetails
    public Button previous;                     //кнопки перелистывания регионов
    public Button next;                         //

    public GameObject sliderWrapPrefab;         // слайдер, отображающий значение параметра (типа флора, фауна и тд)
    public Transform slidersPanel;              // родитель для слайдеров

    public Button climateBtn;                   // кнопки во вкладке экологии
    public Button terrainBtn;
    public Button floraBtn;
    public Button faunaBtn;

    public Transform tabs;
    public Transform rightSide;
    public Image rightSideImg;
    public TMP_Text rightSideText;

    [Header("TopPanel")]
    public TMP_Text topPop;
    public TMP_Text topYear;

    private void Awake() {
        GameEvents.onShowMessage = OnShowMessage;
    }

    private void OnShowMessage(string message) {
        messanger.gameObject.SetActive(true);
        messanger.Message = message;
    }

    public void StartTutorial()
    {
        welcomePopup.gameObject.SetActive(true);
        welcomePopupText.text = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", "Tutorial0");
        welcomeContinueBtn.onClick.AddListener(() => ShowTutorPopup("Tutorial1"));
        cancelTutor.onClick.AddListener(StopTutorial);
    }
    public void StopTutorial()
    {
        //welcomePopup.gameObject.SetActive(false);
        //surePopup.gameObject.SetActive(false);
        tutorialWhole.gameObject.SetActive(false);
        tutorCanceled = true;
    }
    public void ShowTutorPopup(string tutorKey)
    {
        if (tutorCanceled) return;
        otherPopupText.text = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", tutorKey);
        tutorialWhole.gameObject.SetActive(true);
        otherPopup.gameObject.SetActive(true);
        welcomePopup.gameObject.SetActive(false);
    }
    public void ShowRegionDetails(TM_Region region, bool gameStarted)
    {
        if (firstlyOpen) { ShowTutorPopup("TutorRegionDetails"); firstlyOpen = false; }         //при первом открытии показываем сообщение-обучалку
        if (!gameStarted)
        {
            rightSide.gameObject.SetActive(false);
            tabs.gameObject.SetActive(false);
        }
        else
        {
            rightSide.gameObject.SetActive(true);
            tabs.gameObject.SetActive(true);
        }
        regionDetailsPanel.gameObject.SetActive(true);
        title.text = region.name;
        climateBtn.onClick.AddListener(delegate { ShowParamValues(region.climate, "climate"); });
        terrainBtn.onClick.AddListener(delegate { ShowParamValues(region.terrain, "terrain"); });
        floraBtn.onClick.AddListener(delegate { ShowParamValues(region.flora, "flora"); });
        faunaBtn.onClick.AddListener(delegate { ShowParamValues(region.fauna, "fauna"); });
    }
    public void ShowParamValues(EcologyParam param, string paramname) //для параметров цивилизаций сделать перегрузку. paramname это костыль для отображения флоры и фауны
    {
        
        foreach (Transform child in slidersPanel) { Destroy(child.gameObject); }        //очищаем место для слайдеров

        Vector2 v2 = new Vector2();
        v2 = slidersPanel.transform.position;
        Quaternion q = new Quaternion();

        if (param.isRichnessApplicable)
        {
            GameObject go = Instantiate(sliderWrapPrefab, v2, q, slidersPanel);
            //go.GetComponentInChildren<TMP_Text>().text = param.name;
            go.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", paramname); // текст над слайдером
            go.GetComponentInChildren<Slider>().value = param.richness;
            go.GetComponentInChildren<Text>().text = param.richness.ToString();
        }
        else
        {
            foreach (KeyValuePair<string, float> pair in param.quantities) {
                GameObject go = Instantiate(sliderWrapPrefab, v2, q, slidersPanel);
                //GameObject go = Instantiate(sliderWrapPrefab, slidersPanel);
                //go.GetComponentInChildren<TMP_Text>().text = entry.Key;             
                go.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", pair.Key); // текст над слайдером
                go.GetComponentInChildren<Slider>().value = pair.Value;            // значение слайдера
                go.GetComponentInChildren<Text>().text = pair.Value.ToString();    // значение текстом
                go.GetComponentInChildren<Button>().onClick.AddListener(delegate () { ShowRightSideOfRD(pair.Key); }); //  чтобы при нажатии на шкалу подпараметра, в правом окошке открывался текст и картинка с описанием
                v2.y -= 45;
            }
        }
    }
    public void ShowRightSideOfRD(string ecoParamName) // принимает подпараметр (типа равнины, горы)
    {
        rightSideText.text = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", ecoParamName + "RightSide");
    }
    public void RefreshPanels(Population pop, int tick)
    {
        topPop.text = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", "population") + pop.value.ToString("### ### ###");
        int year = 40000 - tick * 10;
        topYear.text = year.ToString("### ###") + LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", "BCE");
    }
}
