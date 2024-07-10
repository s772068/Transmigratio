using System;
using UnityEngine;

/// <summary>
/// Интерфейс, всплывающие окна и тд
/// </summary>
public class HUD : MonoBehaviour
{
    public static event Action<bool> EventRegionPanelOpen;

    [SerializeField] private Messanger messanger;

    public bool isShowMigration = true;

    [Header("Region Details")]
    public RegionDetails regionDetails;        //окно с информацией о выбранном регионе

    public Migration migration;

    private void Awake() {
        GameEvents.ShowMessage = OnShowMessage;
    }

    private void OnShowMessage(string message) {
        messanger.gameObject.SetActive(true);
        messanger.Message = message;
    }

    public void ShowMigration() {
        if (!isShowMigration) return;
        //migration
    }

    public void ShowRegionDetails(int region) {
        regionDetails.regionID = region;
        regionDetails.gameObject.SetActive(true);
        EventRegionPanelOpen?.Invoke(true);
    }
}
