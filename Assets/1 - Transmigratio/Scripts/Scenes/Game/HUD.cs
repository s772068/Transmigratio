using UnityEngine;
using System;

namespace Scenes.Game {
    /// <summary>
    /// Интерфейс, всплывающие окна и тд
    /// </summary>
    public class HUD : Utilits.StaticInstance<HUD> {
        public static event Action<bool> EventRegionPanelOpen;

        [SerializeField] private Transform _events;

        [Header("Region Details")]
        [SerializeField] private RegionDetails.Panel _regionDetails;        //окно с информацией о выбранном регионе
        [SerializeField] private Migration _migration;

        public bool IsShowMigration = true;
        public Transform Events => _events;

        protected override void Awake() {
            base.Awake();
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
}
