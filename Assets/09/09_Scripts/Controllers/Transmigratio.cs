using UnityEngine.UI;
using UnityEngine;
using System;

//using UnityEditor.Localization.Plugins.XLIFF.V12;
/// <summary>
/// "Главный" синглтон
/// 
/// Потом разнести всякие классы по разным файлам
/// </summary>
public class Transmigratio : PersistentSingleton<Transmigratio> {
    public TMDB tmdb;                       // база данных ScriptableObjects
    public HUD hud;

    public int activeRegionIndex;
    public bool gameStarted = false;                    // произошёл ли старт игры

    public Text debugText;                              //для отображения сообщений в специальном окошке (для андроида)
    public static Action<string> AddingDebugText;

    public void Start() {
        AddingDebugText += WriteToDebugText;

        tmdb.TMDBInit();
        hud.StartTutorial();

        tmdb.map.wmsk.OnCountryClick += OnClickFromMain;            //короткий и длинный тап по экрану
        tmdb.map.wmsk.OnCountryLongClick += OnLongClickFromMain;
    }

    public TM_Region GetRegion(int index) => tmdb.map.allRegions[index];
    public Civilization GetCiv(int index) => index >= 0 ? tmdb.humanity.civsList[index] : null;
    public (string, float) GetEcoCulture(int regionIndex) {
        TM_Region region = GetRegion(regionIndex);
        float res = 0;
        for(int i = 0; i < region.civsList.Count; ++i) {
            Civilization civ = GetCiv(region.civsList[i]);
        }
        return default;
    }

    public void WriteToDebugText(string str) {
        debugText.text += str;
    }
    private void OnClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        activeRegionIndex = countryIndex;
        hud.ShowRegionDetails(activeRegionIndex, gameStarted);
    }
    private void OnLongClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        activeRegionIndex = countryIndex;
        hud.ShowRegionDetails(activeRegionIndex, gameStarted);
    }

    public void StartGame() {
        gameStarted = true;
        tmdb.StartGame(activeRegionIndex);
        hud.ShowRegionDetails(activeRegionIndex, gameStarted);
    }
}


