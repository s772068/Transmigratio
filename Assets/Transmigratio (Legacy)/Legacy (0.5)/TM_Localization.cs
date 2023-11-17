using UnityEngine;
using UnityEngine.Localization.Settings;

/// <summary>
/// для использования создаем объект:
/// TM_Localization locale;
/// 
/// получение строки:
/// locale.SD.GetLocalizedString("key001");
/// 
/// получение ассета (картинки, звуки):
/// locale.AD.GetLocalizedAsset<GameObject>("key001");
/// 
/// Чтобы получить значение из таблицы непосредственно в объект на сцене, нужно добавить компонент Localize String Event
/// 
/// </summary>
[System.Serializable]
public class TM_Localization
{
    public LocalizedStringDatabase SD;
    public LocalizedAssetDatabase AD;

    public void Init(int index)
    {
        LoadLocale(index);
    }
    public void LoadLocale(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        SD = LocalizationSettings.StringDatabase;
        AD = LocalizationSettings.AssetDatabase;
    }
    
}