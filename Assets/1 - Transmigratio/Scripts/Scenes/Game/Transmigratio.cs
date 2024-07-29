using UnityEngine;
using WorldMapStrategyKit;

//using UnityEditor.Localization.Plugins.XLIFF.V12;
/// <summary>
/// "Главный" синглтон
/// 
/// Потом разнести всякие классы по разным файлам
/// </summary>
public class Transmigratio : PersistentSingleton<Transmigratio> {
    [SerializeField] private TMDB _tmdb;            // база данных ScriptableObjects
    [SerializeField] private HUD _hud;

    private int _activeRegion;
    private bool _isClickableMarker = true;

    public TMDB TMDB => _tmdb;
    
    public bool IsClickableMarker {
        get => _isClickableMarker;
        set {
            if (value && !_isClickableMarker) {
                _isClickableMarker = value;
                TMDB.map.WMSK.OnMarkerMouseDown += OnMarkerMouseDown;
            } else if (!value && _isClickableMarker) {
                _isClickableMarker = value;
                TMDB.map.WMSK.OnMarkerMouseDown -= OnMarkerMouseDown;
            }
        }
    }

    public TM_Region GetRegion(int index) => TMDB.map.AllRegions[index];
    public Civilization GetCiv(string civName) {
        if (civName == "") return null;
        return TMDB.humanity.Civilizations[civName];
    }
    public CivPiece GetCivPice(int regionIndex, string civName) => GetCiv(civName).Pieces[regionIndex];

    public void StartGame() {
        TMDB.StartGame(_activeRegion);
        _hud.ShowRegionDetails(_activeRegion);
        Timeline.Instance.Pause();
    }

    public new void Awake() {
        base.Awake();
        TMDB.TMDBInit();

        TMDB.map.WMSK.OnCountryClick += OnClickFromMain;
        TMDB.map.WMSK.OnCountryLongClick += OnLongClickFromMain;

        TMDB.map.WMSK.OnMarkerMouseDown += OnMarkerMouseDown;
        TMDB.map.WMSK.OnMarkerMouseEnter += OnMarkerEnter;
        TMDB.map.WMSK.OnMarkerMouseExit += OnMarkerExit;
    }

    private void OnClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        _activeRegion = countryIndex;
        _hud.ShowRegionDetails(_activeRegion);
    }
    private void OnLongClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        _activeRegion = countryIndex;
        _hud.ShowRegionDetails(_activeRegion);
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


