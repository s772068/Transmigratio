using UnityEngine;

public class SettingsController : MonoBehaviour, ISave, IGameConnecter {
    [SerializeField] private E_Language language;
    [SerializeField] private E_Theme theme;
    [SerializeField] private SO_Localization[] localizations;
    [SerializeField] private SO_Theme[] themes;

    public SO_Localization Localization => localizations[(int) language];
    public SO_Theme Theme => themes[(int) theme];

    public int ThemeIndex {
        get => (int) theme;
        set => theme = (E_Theme) value;
    }

    public int Language {
        get => (int) language;
        set => language = (E_Language) value;
    }
    public GameController GameController { set { } }

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

    public void Init() { }
}
