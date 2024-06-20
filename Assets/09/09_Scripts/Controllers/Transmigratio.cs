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

    [HideInInspector] public TM_Region activeRegion;

    public bool IsClickableMarker {
        set {
            if (value) {
                tmdb.map.wmsk.OnMarkerMouseDown += OnMarkerMouseDown;
            } else {
                tmdb.map.wmsk.OnMarkerMouseDown -= OnMarkerMouseDown;
            }
        }
    }

    public TM_Region GetRegion(int index) {
        if (index < 0) return null;
        if (index > tmdb.map.allRegions.Count - 1) return null;
        return tmdb.map.allRegions[index];
    }

    public Civilization GetCiv(string civName) {
        if(!tmdb.humanity.civilizations.ContainsKey(civName)) return null;
        return tmdb.humanity.civilizations[civName];
    }

    public CivPiece GetCivPice(int regionIndex, string civName) {
        if (GetRegion(regionIndex) == null) return null;
        if (GetCiv(civName) == null) return null;
        if (!GetCiv(civName).pieces.ContainsKey(regionIndex)) return null;
        return GetCiv(civName).pieces[regionIndex];
    }

    public void StartGame() {
        tmdb.StartGame(activeRegion);
        hud.ShowRegionDetails(activeRegion);
        Timeline.Instance.Pause();
    }

    public new void Awake() {
        base.Awake();
        tmdb.TMDBInit();
        hud.StartTutorial();

        tmdb.map.wmsk.OnCountryClick += OnClickFromMain;
        tmdb.map.wmsk.OnCountryLongClick += OnLongClickFromMain;

        tmdb.map.wmsk.OnMarkerMouseDown += OnMarkerMouseDown;
        tmdb.map.wmsk.OnMarkerMouseEnter += OnMarkerEnter;
        tmdb.map.wmsk.OnMarkerMouseExit += OnMarkerExit;
    }

    private void OnClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        activeRegion = tmdb.map.allRegions[countryIndex];
        hud.ShowRegionDetails(activeRegion);
    }
    private void OnLongClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        activeRegion = tmdb.map.allRegions[countryIndex];
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


