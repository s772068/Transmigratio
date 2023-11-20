using UnityEngine.UI;
using UnityEngine;

public class MenuController : BaseController, ISave {
    [SerializeField] private GameObject info;

    [SerializeField] private Text labelWelcomeTxt;
    [SerializeField] private Text salutationTxt;
    [SerializeField] private Text startGameTxt;
    [SerializeField] private Text creditsBtnTxt;
    [SerializeField] private Text creditDirectionTxt;
    [SerializeField] private Text creditsDeveloperTxt;
    [SerializeField] private Text thirdPartySupportTxt;
    [SerializeField] private Text creditsCloseTxt;

    [SerializeField] private E_Language language;

    [SerializeField] private string[] labelWelcome;
    [SerializeField] private string[] salutation;
    [SerializeField] private string[] startGame;
    [SerializeField] private string[] creditsBtn;
    [SerializeField] private string[] creditDirection;
    [SerializeField] private string[] creditsDeveloper;
    [Multiline(3)]
    [SerializeField] private string[] thirdPartySupport;
    [SerializeField] private string[] creditsClose;

    private E_Theme theme;

    public void ActivateCredits(bool isActivate) => info.SetActive(isActivate);
    public void UrlOpen(string url) {
        Application.OpenURL(url);
    }

    public void Save() {
        IOHelper.SaveToJson(new S_Settings() {
            Language = (int) language,
            Theme = (int) theme
        });
    }

    public void Load() {
        IOHelper.LoadFromJson(out S_Settings data);
        language = (E_Language) data.Language;
        theme = (E_Theme) data.Theme;
    }

    public void Localization() {
        labelWelcomeTxt.text = labelWelcome[(int) language];
        salutationTxt.text = salutation[(int) language];
        startGameTxt.text = startGame[(int) language];
        creditsBtnTxt.text = creditsBtn[(int) language];
        creditDirectionTxt.text = creditDirection[(int) language];
        creditsDeveloperTxt.text = creditsDeveloper[(int) language];
        thirdPartySupportTxt.text = thirdPartySupport[(int) language];
        creditsCloseTxt.text = creditsClose[(int) language];
    }

    private void Start() {
        Localization();
    }
}
