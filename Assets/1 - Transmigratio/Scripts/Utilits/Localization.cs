using UnityEngine.Localization.Settings;

public static class Localization {
    public static string Load(string tableName, string key) {
        return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);
    }
}
