using UnityEngine.UI;
using UnityEngine;
using System;

//using UnityEditor.Localization.Plugins.XLIFF.V12;
/// <summary>
/// "�������" ��������
/// 
/// ����� �������� ������ ������ �� ������ ������
/// </summary>
public class Transmigratio : MonoBehaviour {
    public static Transmigratio Instance;   // ��������
    public TMDB tmdb;                       // ���� ������ ScriptableObjects
    public HUD hud;

    public int activeRegionIndex;
    public bool gameStarted = false;                    // ��������� �� ����� ����

    public Text debugText;                              //��� ����������� ��������� � ����������� ������ (��� ��������)
    public static Action<string> AddingDebugText;

    private bool isPlayGame;

    private void Awake() {
        GameEvents.onTick += OnTick;
    }

    public void Start() {
        if (Instance == null) {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this) {
            Destroy(gameObject);
        }
        AddingDebugText += WriteToDebugText;

        tmdb.TMDBInit();
        hud.StartTutorial();

        tmdb.map.wmsk.OnCountryClick += OnClickFromMain;            //�������� � ������� ��� �� ������
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
        hud.ShowRegionDetails(GetRegion(activeRegionIndex), gameStarted);
    }
    private void OnLongClickFromMain(int countryIndex, int regionIndex, int buttonIndex) {
        activeRegionIndex = countryIndex;
        hud.ShowRegionDetails(GetRegion(activeRegionIndex), gameStarted);
    }

    /// <summary>
    /// ��������� ���
    /// </summary>
    private void OnTick() {
        tmdb.NextTick();
        hud.RefreshPanels(tmdb.humanity.totalEarthPop, tmdb.tick);
    }

    public void StartGame() {
        gameStarted = true;
        tmdb.StartGame(activeRegionIndex);
        hud.ShowRegionDetails(GetRegion(activeRegionIndex), gameStarted);
    }

    //public void Play()                            // ���������� � Timeline.cs
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


