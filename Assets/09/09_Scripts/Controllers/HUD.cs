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
/// ���������, ����������� ���� � ��
/// </summary>
public class HUD : MonoBehaviour
{
    [SerializeField] private Messanger messanger;

    [Header("Tutorial")]
    public Transform tutorialWhole;     //������-�������� ���� ��������� ��������
    public Transform welcomePopup;      //���������� � ��������� ����
    public Transform surePopup;         //���� ������������� ������ ��������
    public TMP_Text welcomePopupText;   //����� � ������������� ����
    public Button welcomeContinueBtn;   //������ ����������� ��������
    public Button cancelTutor;          //������ "��" � �������������� ������, ��� ������� ���������� ��������
    public Transform otherPopup;        //������ ��� ��������� ��������� ���������
    public TMP_Text otherPopupText;     //����� � otherPopup
    public Button okBtn;                //������ "��" � otherPopup
    public bool tutorCanceled = false;  //����, ����������� �� ��, ������ �� �����

    [Header("Region Details")]
    public Transform regionDetailsPanel;        //���� � ����������� � ��������� �������
    public bool firstlyOpen = true;             // ���� ��� ��������
    public TMP_Text title;                      // �������� ������� � ������ RegionDetails
    public Button previous;                     //������ �������������� ��������
    public Button next;                         //

    public GameObject sliderWrapPrefab;         // �������, ������������ �������� ��������� (���� �����, ����� � ��)
    public Transform slidersPanel;              // �������� ��� ���������

    public Button climateBtn;                   // ������ �� ������� ��������
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
        if (firstlyOpen) { ShowTutorPopup("TutorRegionDetails"); firstlyOpen = false; }         //��� ������ �������� ���������� ���������-��������
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
    public void ShowParamValues(EcologyParam param, string paramname) //��� ���������� ����������� ������� ����������. paramname ��� ������� ��� ����������� ����� � �����
    {
        
        foreach (Transform child in slidersPanel) { Destroy(child.gameObject); }        //������� ����� ��� ���������

        Vector2 v2 = new Vector2();
        v2 = slidersPanel.transform.position;
        Quaternion q = new Quaternion();

        if (param.isRichnessApplicable)
        {
            GameObject go = Instantiate(sliderWrapPrefab, v2, q, slidersPanel);
            //go.GetComponentInChildren<TMP_Text>().text = param.name;
            go.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", paramname); // ����� ��� ���������
            go.GetComponentInChildren<Slider>().value = param.richness;
            go.GetComponentInChildren<Text>().text = param.richness.ToString();
        }
        else
        {
            foreach (KeyValuePair<string, float> pair in param.quantities) {
                GameObject go = Instantiate(sliderWrapPrefab, v2, q, slidersPanel);
                //GameObject go = Instantiate(sliderWrapPrefab, slidersPanel);
                //go.GetComponentInChildren<TMP_Text>().text = entry.Key;             
                go.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", pair.Key); // ����� ��� ���������
                go.GetComponentInChildren<Slider>().value = pair.Value;            // �������� ��������
                go.GetComponentInChildren<Text>().text = pair.Value.ToString();    // �������� �������
                go.GetComponentInChildren<Button>().onClick.AddListener(delegate () { ShowRightSideOfRD(pair.Key); }); //  ����� ��� ������� �� ����� ������������, � ������ ������ ���������� ����� � �������� � ���������
                v2.y -= 45;
            }
        }
    }
    public void ShowRightSideOfRD(string ecoParamName) // ��������� ����������� (���� �������, ����)
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
