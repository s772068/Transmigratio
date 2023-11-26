using UnityEditor.Localization.Editor;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Localization", fileName = "Localization")]
public class SO_Localization : ScriptableObject {
    public SL_Migration Migration;
    public SL_Resources Resources;
    public SL_System System;
    public SL_Map Map;
    public SL_LayerSelect Layers;
}
