using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Localization", fileName = "Localization")]
public class SO_Localization : ScriptableObject {
    public SL_System System;
    public SL_Migration Migration;
    public SL_Map Map;
}
