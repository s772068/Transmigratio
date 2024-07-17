using System;
using UnityEngine;

/// <summary>
/// Интерфейс, всплывающие окна и тд
/// </summary>
public class HUD : MonoBehaviour
{
    public static event Action<bool> EventRegionPanelOpen;

    [SerializeField] private Messanger _messanger;

    public bool IsShowMigration = true;

    [Header("Region Details")]
    [SerializeField] private RegionDetails _regionDetails;        //окно с информацией о выбранном регионе

    [SerializeField] private Migration _migration;

    public static Action<string> ShowMessage;

    private void Awake() {
        ShowMessage = OnShowMessage;
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
