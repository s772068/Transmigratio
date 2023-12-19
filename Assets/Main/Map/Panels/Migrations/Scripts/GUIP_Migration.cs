using UnityEngine.UI;
using UnityEngine;
using System;

public class GUIP_Migration : GUI_BasePanel, IGameConnecter {
    [Header("Text")]
    [SerializeField] private Text title;
    [SerializeField] private Text description;
    [SerializeField] private Text valueTxt;
    [SerializeField] private Text breakTxt;
    [SerializeField] private Text advanceTxt;
    [SerializeField] private Text closeTxt;
    //[Header("Button")]
    //[SerializeField] private Button breakBtn;
    //[SerializeField] private Button amplifyBtn;
    //[SerializeField] private Button closeBtn;
    [Header("Other")]
    //[SerializeField] private Image icon;
    [SerializeField] private GUI_ProgressBar progress;

    private MigrationController migration;
    private SettingsController settings;

    //private float maxPopulation;
    private string pathFormat;
    private int fromIndex;

    [HideInInspector] public int index;

    public Action OnAmplify;
    public Action OnBreak;
    public Action OnClose;

    public string Title { set => title.text = value; }
    public string Description { set => description.text = value; }
//    public Sprite Icon { set => icon.sprite = value; }

    public int Percent {
        set {
            valueTxt.text = value.ToString();
            progress.Value = value;
        }
    }

    public string BreakString { set => breakTxt.text = value; }
    public string AdvanceString { set => advanceTxt.text = value; }
    public string CloseString { set => closeTxt.text = value; }
    public GameController GameController {
        set {
            value.Get(out migration);
            value.Get(out settings);
        }
    }

    private void Open(int from, int to, int percent) {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
            fromIndex = from;
            Localization(from, to);
            Percent = percent;
        }
    }

    public void Close(int from) {
        if(from == fromIndex) Close();
    }

    public void Close() {
        gameObject.SetActive(false);
    }

    public void Localization(int from, int to) { // , SL_Migration migration, SL_System system) {
        // Label = migration.Label;
        // Description = migration.Description;
        Title = string.Format(settings.Localization.Migration.Title, settings.Localization.Map.Countries.Value[from], settings.Localization.Map.Countries.Value[to]);
        Description = settings.Localization.Migration.Description;
        AdvanceString = settings.Localization.Migration.Advance;
        BreakString = settings.Localization.System.Break;
        CloseString = settings.Localization.System.Close;
    }

    public void UpdatePanel(int from, int percent) {
        if (from != fromIndex) return;
            Percent = percent;
    }

    public void Amplify() => migration.AmplifyMigration(fromIndex);
    public void Break() => migration.DestroyMigration(fromIndex);

    public void Init() {
        migration.OnOpenPanel += Open;
        migration.OnClosePanel += Close;
        migration.OnUpdatePanel += UpdatePanel;
    }
}
