using UnityEngine;
using UnityEngine.Localization.Settings;

/// <summary>
/// ��� ������������� ������� ������:
/// TM_Localization locale;
/// 
/// ��������� ������:
/// locale.SD.GetLocalizedString("key001");
/// 
/// ��������� ������ (��������, �����):
/// locale.AD.GetLocalizedAsset<GameObject>("key001");
/// 
/// ����� �������� �������� �� ������� ��������������� � ������ �� �����, ����� �������� ��������� Localize String Event
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