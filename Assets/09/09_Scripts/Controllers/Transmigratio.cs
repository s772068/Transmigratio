using UnityEngine.UI;
using UnityEngine;
using System;
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
    [HideInInspector] public bool isClickableRegion = true;

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
    }

    public void Start() {
        tmdb.TMDBInit();
        hud.StartTutorial();

        tmdb.map.wmsk.OnCountryClick += OnClickFromMain;
        tmdb.map.wmsk.OnCountryLongClick += OnLongClickFromMain;
    }

    private void OnClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        if (!isClickableRegion) return;
        activeRegion = tmdb.map.allRegions[countryIndex];
        hud.ShowRegionDetails(activeRegion);
    }
    private void OnLongClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        if (!isClickableRegion) return;
        activeRegion = tmdb.map.allRegions[countryIndex];
        hud.ShowRegionDetails(activeRegion);
    }
}


