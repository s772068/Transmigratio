using UnityEngine.Localization.Settings;

public static class StringLoader {
    public static string Load(string key) {
        return LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", key);
    }
}
