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

    public Text debugText;
    public static Action<string> AddingDebugText;

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

        tmdb.map.wmsk.OnCountryClick += OnClickFromMain;

        
         
        //AddingDebugText.Invoke(s);
    }
    public void writeToDebugText(string str)
    {
        debugText.text += str;
    }
    private void OnClickFromMain(int countryIndex, int regionIndex, int buttonIndex)
    {
        hud.ShowRegionDetails(tmdb.map.allRegions[countryIndex]);
    }
    public void Play()
    {
        StartCoroutine(Time_NormalSpeed());
        Debug.Log("Play pressed");
    }
    public void Pause()
    {
        StopAllCoroutines();
        Debug.Log("Pause pressed");
    }

    IEnumerator Time_NormalSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            tmdb.NextTick();
            hud.RefreshPanels(tmdb.humanity.totalEarthPop, tmdb.tick);
        }
    }
}


