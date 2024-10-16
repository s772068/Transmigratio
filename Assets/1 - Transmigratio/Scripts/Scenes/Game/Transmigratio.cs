using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine;
using Gameplay;
using System;
using UI;

public class Transmigratio : MonoBehaviour {
    [SerializeField] private TMDB _tmdb;
    [SerializeField] private HUD _hud;
    [SerializeField] private int _intervetionPoints = 100;
    [SerializeField] private NewsPanel _newsPrefab;
    [SerializeField] private Texture2D borderTexture;
    [SerializeField] private Locale _local;

    private Intervention _intervention;
    private News _news;
    private bool _isClickableMarker = true;
    public static Transmigratio Instance { get; private set; }


    public TMDB TMDB => _tmdb;
    public HUD HUD => _hud;
    public Intervention Intervention => _intervention;
    public int MaxInterventionPoints => _intervetionPoints;

    public static event Action GameStarted;

    public TM_Region GetRegion(int index) => TMDB.map.AllRegions[index];
    public Civilization GetCiv(string civName) {
        if (civName == "") return null;
        return TMDB.humanity.Civilizations[civName];
    }
    public CivPiece GetCivPice(int regionIndex, string civName) => GetCiv(civName).Pieces[regionIndex];

    public void StartGame() {
        TMDB.StartGame(MapData.RegionID);
        Timeline.Instance.Pause();
        GameStarted?.Invoke();
    }

    public void Awake() {
        if (Instance == null) 
            Instance = this;

        TMDB.TMDBInit();
        MapData.Init(borderTexture);
        
        Gameplay.Controller.Init();

        _intervention = new Intervention(_intervetionPoints);
        _news = new News(HUD.Instance.PanelsParent, _newsPrefab, TMDB.News);

        RegionDetails.StartGame.Panel.onStartGame += StartGame;
    }

    private void Start()
    {
        LocalizationSettings.SelectedLocale = _local;
    }

    private void OnDestroy()
    {
        Instance = null;
        MapData.Clear();
    }

    public void UnselectRegion() {
        MapData.UnselectRegion();
    }
}


