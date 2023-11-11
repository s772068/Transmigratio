using UnityEngine;

[System.Serializable]
public struct S_LocalizationMigration {
    public string Label;
    public string Path;
    [Multiline(6)]
    public string Description;
    public string BreakString;
    public string CloseString;
    public S_LocalizationParamiter Paramiter;
}
