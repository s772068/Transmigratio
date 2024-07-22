using WorldMapStrategyKit;
using Database.Data;
using UnityEngine;

namespace Scenes.Game {
    //using UnityEditor.Localization.Plugins.XLIFF.V12;
    /// <summary>
    /// "Главный" синглтон
    /// 
    /// Потом разнести всякие классы по разным файлам
    /// </summary>
    public class Transmigratio : Utilits.PersistentSingleton<Transmigratio> {

        [SerializeField] private Database.Base _database;            // база данных ScriptableObjects
        [SerializeField] private HUD _hud;

        public Database.Base Database => _database;

        private int activeRegion;

        public bool IsClickableMarker {
            set {
                if (value) {
                    Database.map.WMSK.OnMarkerMouseDown += OnMarkerMouseDown;
                } else {
                    Database.map.WMSK.OnMarkerMouseDown -= OnMarkerMouseDown;
                }
            }
        }

        public Database.Data.Region GetRegion(int index) => Database.map.AllRegions[index];
        public Civilization GetCiv(string civName) {
            if (civName == "") return null;
            return Database.humanity.Civilizations[civName];
        }
        public CivPiece GetCivPice(int regionIndex, string civName) => GetCiv(civName).Pieces[regionIndex];

        public void StartGame() {
            Database.StartGame(activeRegion);
            _hud.ShowRegionDetails(activeRegion);
            Timeline.Instance.Pause();
        }

        public new void Awake() {
            base.Awake();
            Database.TMDBInit();

            Database.map.WMSK.OnCountryClick += OnClickFromMain;
            Database.map.WMSK.OnCountryLongClick += OnLongClickFromMain;

            Database.map.WMSK.OnMarkerMouseDown += OnMarkerMouseDown;
            Database.map.WMSK.OnMarkerMouseEnter += OnMarkerEnter;
            Database.map.WMSK.OnMarkerMouseExit += OnMarkerExit;
        }

        private void OnClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
            activeRegion = countryIndex;
            _hud.ShowRegionDetails(activeRegion);
        }
        private void OnLongClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
            activeRegion = countryIndex;
            _hud.ShowRegionDetails(activeRegion);
        }

        private void OnMarkerEnter(MarkerClickHandler marker) {
            IsClickableMarker = true;
        }

        private void OnMarkerExit(MarkerClickHandler marker) {
            IsClickableMarker = false;
        }

        private void OnMarkerMouseDown(MarkerClickHandler marker, int buttonIndex) {
            marker.GetComponent<IconMarker>().Click();
        }
    }
}
