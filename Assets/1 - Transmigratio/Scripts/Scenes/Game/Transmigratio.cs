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
    [SerializeField] private Texture2D borderTexture;

    private Intervention _intervention;
    private News _news;
    private bool _isClickableMarker = true;

    public TMDB TMDB => _tmdb;
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

    public new void Awake() {
        base.Awake();
        TMDB.TMDBInit();
        MapData.Init(borderTexture);
        
        Gameplay.Controller.Init();

        _intervention = new Intervention(_intervetionPoints);
        _news = new News(HUD.Instance.PanelsParent, _newsPrefab, TMDB.News);

        RegionDetails.StartGame.Panel.onStartGame += StartGame;
    }

    public void UnselectRegion() {
        MapData.UnselectRegion();
    }
}


