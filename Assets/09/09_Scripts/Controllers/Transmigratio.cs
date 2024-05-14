using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.UI;
//using UnityEditor.Localization.Plugins.XLIFF.V12;
/// <summary>
/// "Главный" синглтон
/// 
/// Потом разнести всякие классы по разным файлам
/// </summary>
public class Transmigratio : MonoBehaviour
{
    public static Transmigratio Instance;   // синглтон
    public TMDB tmdb;                       // база данных ScriptableObjects
    public HUD hud;

    public TM_Region activeRegion;
    public bool gameStarted = false;                    // произошёл ли старт игры

    public Text debugText;                              //для отображения сообщений в специальном окошке (для андроида)
    public static Action<string> AddingDebugText;

    private bool isPlayGame;

    private void Awake() {
        GameEvents.onTick += OnTick;
    }

    public void Start()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        AddingDebugText += writeToDebugText;

        tmdb.TMDBInit();
        hud.StartTutorial();

        tmdb.map.wmsk.OnCountryClick += OnClickFromMain;            //короткий и длинный тап по экрану
        tmdb.map.wmsk.OnCountryLongClick += OnLongClickFromMain;
    }
    public void writeToDebugText(string str)
    {
        debugText.text += str;
    }
    private void OnClickFromMain(int countryIndex, int regionIndex, int buttonIndex)
    {
        activeRegion = tmdb.map.allRegions[countryIndex];
        hud.ShowRegionDetails(activeRegion, gameStarted);
    }
    private void OnLongClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        string mouseKey = buttonIndex == 0 ? "LeftMouseButton" : "RightMouseButton";
        Debug.Log($"LongClick: {mouseKey}");
        Debug.Log($"Country: {tmdb.map.allRegions[countryIndex].name}");
        Debug.Log($"RegionIndex: {regionIndex}");
    }

    private void OnTick() {                                         // следующий тик, то есть следующий ход 
        tmdb.NextTick();
        hud.RefreshPanels(tmdb.humanity.totalEarthPop, tmdb.tick);
    }
    public void StartGame()
    {
        gameStarted = true;
        tmdb.humanity.AddCivilization(activeRegion);
    }
    //public void Play()                            // перенесено в Timeline.cs
    //{
    //    StartCoroutine(Time_NormalSpeed());
    //    Debug.Log("Play pressed");
    //}
    //public void Pause()
    //{
    //    StopAllCoroutines();
    //    Debug.Log("Pause pressed");
    //}

    //IEnumerator Time_NormalSpeed()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(1f);
    //        tmdb.NextTick();
    //        hud.RefreshPanels(tmdb.humanity.totalEarthPop, tmdb.tick);
    //    }
    //}
}


