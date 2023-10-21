using UnityEngine;

[CreateAssetMenu(menuName = "Holders/Prefabs", fileName = "PrefabsHolder")]
public class PrefabsHolder : SO_Singleton<PrefabsHolder> {
    [Header("Elements")]
    public GUI_ProgressBar ProgressBar;
    public GUI_Param Param;
    [Header("Containers")]
    public GUI_ParamsGroup ParamsGroup;
    [Header("Panels")]
    public GUI_PathPanel PathPanel;
    public GUI_RegionPanel RegionPanel;
}
