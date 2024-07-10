using UnityEngine;
using WorldMapStrategyKit;

//using UnityEditor.Localization.Plugins.XLIFF.V12;
/// <summary>
/// "Главный" синглтон
/// 
/// Потом разнести всякие классы по разным файлам
/// </summary>
public class Transmigratio : PersistentSingleton<Transmigratio> {
    public TMDB tmdb;                       // база данных ScriptableObjects
    public HUD hud;

    [HideInInspector] public int activeRegion;

    public bool IsClickableMarker {
        set {
            if (value) {
                tmdb.map.WMSK.OnMarkerMouseDown += OnMarkerMouseDown;
            } else {
                tmdb.map.WMSK.OnMarkerMouseDown -= OnMarkerMouseDown;
            }
        }
    }

    public TM_Region GetRegion(int index) => tmdb.map.AllRegions[index];
    public Civilization GetCiv(string civName) {
        if (civName == "") return null;
        return tmdb.humanity.Civilizations[civName];
    }
    public CivPiece GetCivPice(int regionIndex, string civName) => GetCiv(civName).Pieces[regionIndex];

    public void StartGame() {
        tmdb.StartGame(activeRegion);
        hud.ShowRegionDetails(activeRegion);
        Timeline.Instance.Pause();
    }

    public new void Awake() {
        base.Awake();
        tmdb.TMDBInit();

        tmdb.map.WMSK.OnCountryClick += OnClickFromMain;
        tmdb.map.WMSK.OnCountryLongClick += OnLongClickFromMain;

        tmdb.map.WMSK.OnMarkerMouseDown += OnMarkerMouseDown;
        tmdb.map.WMSK.OnMarkerMouseEnter += OnMarkerEnter;
        tmdb.map.WMSK.OnMarkerMouseExit += OnMarkerExit;
    }

    private void OnClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        activeRegion = countryIndex;
        hud.ShowRegionDetails(activeRegion);
    }
    private void OnLongClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        activeRegion = countryIndex;
        hud.ShowRegionDetails(activeRegion);
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


