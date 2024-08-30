using WorldMapStrategyKit;
using UnityEngine;
using Gameplay;
using System;
using UI;

//using UnityEditor.Localization.Plugins.XLIFF.V12;
/// <summary>
/// "Главный" синглтон
/// 
/// Потом разнести всякие классы по разным файлам
/// </summary>
public class Transmigratio : PersistentSingleton<Transmigratio> {
    [SerializeField] private TMDB _tmdb;            // база данных ScriptableObjects
    [SerializeField] private HUD _hud;
    [SerializeField] private int _intervetionPoints = 100;
    [SerializeField] private NewsPanel _newsPrefab;

    private Intervention _intervention;
    private News _news;
    private int _activeRegion;
    private bool _isClickableMarker = true;

    public TMDB TMDB => _tmdb;
    public Intervention Intervention => _intervention;
    public static event Action GameStarted;

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
        GameStarted?.Invoke();
    }

    public new void Awake() {
        base.Awake();
        TMDB.TMDBInit();

        TMDB.map.WMSK.OnCountryClick += OnClickFromMain;
        TMDB.map.WMSK.OnCountryLongClick += OnLongClickFromMain;
        TMDB.map.WMSK.OnCountryHighlight += OnCountryHighlight;
        //TMDB.map.WMSK.OnCountryExit += OnCountryExit;

        TMDB.map.WMSK.OnMarkerMouseDown += OnMarkerMouseDown;
        TMDB.map.WMSK.OnMarkerMouseEnter += OnMarkerEnter;
        TMDB.map.WMSK.OnMarkerMouseExit += OnMarkerExit;

        Gameplay.Controller.Init();

        _intervention = new Intervention(_intervetionPoints);
        _news = new News(HUD.Instance.PanelsParent, _newsPrefab, TMDB.News);
    }

    private void OnClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        _activeRegion = countryIndex;
        _hud.ShowRegionDetails(_activeRegion);
        _tmdb.map.SelectRegion(countryIndex);
    }

    private void OnLongClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        _activeRegion = countryIndex;
        _hud.ShowRegionDetails(_activeRegion);
        _tmdb.map.SelectRegion(countryIndex);
    }

    public void UnselectRegion() {
        _tmdb.map.UnselectRegion();
    }

    private void OnCountryHighlight(int countryIndex, int regionIndex, ref bool allowHighlight) {
        //_tmdb.map.WMSK.ToggleCountrySurface(countryIndex, true, _tmdb.map.WMSK.fillColor);
        _tmdb.map.WMSK.ToggleCountryOutline(countryIndex, true, borderWidth: 0.2f, tintColor: _tmdb.map.WMSK.outlineColor);
    }

    //private void OnCountryExit(int countryIndex, int regionIndex) {
    //    _tmdb.map.WMSK.ToggleCountrySurface(countryIndex, true, Color.clear);
    //}

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


