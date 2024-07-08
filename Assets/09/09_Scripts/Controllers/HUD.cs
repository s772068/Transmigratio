using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

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
    public bool tutorCanceled;  //����, ����������� �� ��, ������ �� �����
    
    public bool isShowMigration = true;

    [Header("Region Details")]
    public RegionDetails regionDetails;        //���� � ����������� � ��������� �������

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
