using System;
using UnityEngine;

/// <summary>
/// Интерфейс, всплывающие окна и тд
/// </summary>
public class HUD : StaticInstance<HUD>
{
    public static event Action<bool> EventRegionPanelOpen;
    

    [SerializeField] private Messanger _messanger;
    [SerializeField] private Transform _panelsParent;

    [Header("Region Details")]
    [SerializeField] private RegionDetails _regionDetails;        //окно с информацией о выбранном регионе
    [SerializeField] private Migration _migration;

    public bool IsShowMigration = true;
    public Transform PanelsParent => _panelsParent;

    protected override void Awake() {
        base.Awake();
    }

    private void OnShowMessage(string message) {
        _messanger.gameObject.SetActive(true);
        _messanger.Message = message;
    }

    public void ShowMigration() {
        if (!IsShowMigration) return;
        //migration
    }

    public void ShowRegionDetails(int region) {
        _regionDetails.RegionID = region;
        _regionDetails.gameObject.SetActive(true);
        EventRegionPanelOpen?.Invoke(true);
    }
}
