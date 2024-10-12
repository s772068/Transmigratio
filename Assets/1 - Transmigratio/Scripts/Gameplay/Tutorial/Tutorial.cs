using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

public class Tutorial : MonoBehaviour {
    #region Consts

    private const int WELCOME_SHIFT = 1 << 1;
    private const int GOAL_SHIFT = 1 << 2;
    private const int START_REGION_DETAILS_SHIFT = 1 << 3;
    private const int REGION_DETAILS_SHIFT = 1 << 4;
    private const int HUD_SHIFT = 1 << 5;
    private const int EVENT_SHIFT = 1 << 6;
    private const int MARKERS_SHIFT = 1 << 7;
    private const int LAYERS_SHIFT = 1 << 8;
    private const int AUTOCHOICE_SHIFT = 1 << 9;
    // private const int START_SHIFT = 1 << 6;
    // private const int CHRONICLES_SHIFT = 1 << 10;

    private const TutorialSteps _allSteps = TutorialSteps.Welcome | TutorialSteps.Goal | TutorialSteps.StartRegionDetails |
                  TutorialSteps.RegionDetails | TutorialSteps.HUD/* | TutorialSteps.Start*/ | TutorialSteps.Event | /*TutorialSteps.Markers |*/
                  TutorialSteps.Layers | /*TutorialSteps.Chronicles |*/ TutorialSteps.AutoChoice;
    #endregion

    [SerializeField] private bool _suggestTutorial = true;
    [SerializeField] private TutorialSteps _steps;
    [SerializeField] private GameObject _blurBG;
    private bool _tutorialEnded = false;
    
    public static Action<string> OnShowTutorial;
    
    [System.Flags]
    public enum TutorialSteps
    {
        Welcome = WELCOME_SHIFT,
        Goal = GOAL_SHIFT,
        StartRegionDetails = START_REGION_DETAILS_SHIFT,
        RegionDetails = REGION_DETAILS_SHIFT,
        HUD = HUD_SHIFT,
        //Start = START_SHIFT,
        Event = EVENT_SHIFT,
        Markers = MARKERS_SHIFT,
        Layers = LAYERS_SHIFT,
        //Chronicles = CHRONICLES_SHIFT,
        AutoChoice = AUTOCHOICE_SHIFT,
    }

    [Header("Tutorial Steps")]
    [SerializeField] private GameObject _welcome;
    [SerializeField] private GameObject _gameGoal;
    [SerializeField] private GameObject _HUD;
    [SerializeField] private GameObject _startGame;
    //[SerializeField] private GameObject _gameEvent;
    [SerializeField] private GameObject _markers;
    //[SerializeField] private GameObject _layers;
    //[SerializeField] private GameObject _chrono;
    //[SerializeField] private GameObject _autoChoice;
    

    private void OnEnable() {
        RegionDetails.Controller.onOpenStartRegionPanel += OpenStartRegionDetails;
        RegionDetails.Controller.onOpenRegionPanel += OpenRegionDetails;
        RegionDetails.Defoult.Panel.onClose += TutorialByHUD;
        Gameplay.Scenarios.Events.AutoChoicePanel.onOpen += TutorialByAutoChoice;
        Layers.Panel.onOpen += TutorialByLayers;
        EventPanel.PanelOpen += TutorialByEvent;
        IconMarker.MarkerInst += TutorialByMarker;
        // EventPanel.EventPanelOpen += GameEvent;
    }

    private void OnDisable() {
        RegionDetails.Controller.onOpenStartRegionPanel -= OpenStartRegionDetails;
        RegionDetails.Controller.onOpenRegionPanel -= OpenRegionDetails;
        RegionDetails.Defoult.Panel.onClose -= TutorialByHUD;
        Gameplay.Scenarios.Events.AutoChoicePanel.onOpen -= TutorialByAutoChoice;
        Layers.Panel.onOpen -= TutorialByLayers;
        EventPanel.PanelOpen -= TutorialByEvent;
        IconMarker.MarkerInst -= TutorialByMarker;
        // EventPanel.EventPanelOpen -= GameEvent;
    }

    private void Start() {
        if (_suggestTutorial) {
            _welcome.SetActive(true);
            _steps += (int)TutorialSteps.Welcome;
        }
        else _tutorialEnded = true;
    }

    private void FixedUpdate() {
        if (_tutorialEnded)
            gameObject.SetActive(false);

        if (_steps == _allSteps)
            _tutorialEnded = true;
    }

    public void GameGoal(bool open) {
        if (open && _steps.HasFlag(TutorialSteps.Goal))
            return;
        else if (!open && !_steps.HasFlag(TutorialSteps.Goal)) {
            _steps += (int)TutorialSteps.Goal;
        }
        _gameGoal.SetActive(open);
    }

    public void OpenStartRegionDetails(bool open) {
        if (!_steps.HasFlag(TutorialSteps.StartRegionDetails)) {
            if(open) OnShowTutorial?.Invoke("StartRegionDetails");
            _gameGoal.SetActive(false);
            _steps += (int) TutorialSteps.StartRegionDetails;
        }
    }

    public void OpenRegionDetails(bool open) {
        if (!_steps.HasFlag(TutorialSteps.RegionDetails)) {
            if(open) OnShowTutorial?.Invoke("RegionDetails");
            _steps += (int) TutorialSteps.RegionDetails;
        }
    }

    public void TutorialByHUD(bool open) {
        if (!_steps.HasFlag(TutorialSteps.HUD)) {
            if(open) _HUD.SetActive(true);
            _steps += (int) TutorialSteps.HUD;
        }
    }

    public void TutorialByAutoChoice(bool open) {
        if (!_steps.HasFlag(TutorialSteps.AutoChoice)) {
            if(open) OnShowTutorial?.Invoke("AutoChoice");
            Debug.Log("AutoChoice");
            _steps += (int) TutorialSteps.AutoChoice;
        }
    }

    public void TutorialByLayers(bool open) {
        if (!_steps.HasFlag(TutorialSteps.Layers)) {
            if(open) OnShowTutorial?.Invoke("Layers");
            _steps += (int) TutorialSteps.Layers;
        }
    }

    public void TutorialByEvent(bool open)
    {
        if (!_steps.HasFlag(TutorialSteps.Event))
        {
            if (open) OnShowTutorial?.Invoke("Event");
            Debug.Log("Event");
            _steps += (int)TutorialSteps.AutoChoice;
        }
    }

    //public void GameStarted(bool open) {
    //    if (!_steps.HasFlag(TutorialSteps.Start)) {
    //        if (open) {
    //            _startGame.SetActive(true);
    //            _uiZones["Timer"].SetActive(true);
    //            ActivateZone(true);
    //        }
    //        _steps += (int)TutorialSteps.Start;
    //    }
    //}

    //public void GameEvent(bool open) {
    //    if (open && !_steps.HasFlag(TutorialSteps.Event)) {
    //        _gameEvent.SetActive(true);
    //        _uiZones["Event"].SetActive(true);
    //        ActivateZone(true);
    //    } else {
    //        if (!_steps.HasFlag(TutorialSteps.Event))
    //            _steps += (int)TutorialSteps.Event;

    //        _gameEvent.SetActive(false);
    //        _uiZones["Event"].SetActive(false);
    //        ActivateZone(false);
    //    }
    //}

    public void TutorialByMarker() {
        if (!_steps.HasFlag(TutorialSteps.Markers)) {
            Timeline.Instance.Pause();
            _blurBG.SetActive(true);
            _markers.SetActive(true);
            _steps += (int)TutorialSteps.Markers;
        }
    }

    //public void GameLayers(bool open)
    //{
    //    if (open && !_steps.HasFlag(TutorialSteps.Layers)) {
    //        _layers.SetActive(true);
    //        _uiZones["Layers"].SetActive(true);
    //        ActivateZone(true);
    //    } else {
    //        if (!_steps.HasFlag(TutorialSteps.Layers))
    //            _steps += (int)TutorialSteps.Layers;

    //        _layers.SetActive(false);
    //        _uiZones["Layers"].SetActive(false);
    //        ActivateZone(false);
    //    }
    //}

    //public void Chronicles(bool open) {
    //    if (open && !_steps.HasFlag(TutorialSteps.Chronicles)) {
    //        _chrono.SetActive(true);
    //        _uiZones["Chronicles"].SetActive(true);
    //        ActivateZone(true);
    //    } else {
    //        if (!_steps.HasFlag(TutorialSteps.Chronicles))
    //            _steps += (int)TutorialSteps.Chronicles;

    //        _chrono.SetActive(false);
    //        _uiZones["Chronicles"].SetActive(false);
    //        ActivateZone(false);
    //    }
    //}

    //public void AutoChoice(bool open) {
    //    if (open && !_steps.HasFlag(TutorialSteps.AutoChoice))
    //    {
    //        _autoChoice.SetActive(true);
    //        _uiZones["AutoChoice"].SetActive(true);
    //        ActivateZone(true);
    //    } else {
    //        if (!_steps.HasFlag(TutorialSteps.AutoChoice))
    //            _steps += (int)TutorialSteps.AutoChoice;

    //        _autoChoice.SetActive(false);
    //        _uiZones["AutoChoice"].SetActive(false);
    //        ActivateZone(false);
    //    }
    //}

    public void Skip() => _tutorialEnded = true;
}