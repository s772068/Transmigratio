using AYellowpaper.SerializedCollections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    #region Consts

    private const int WELCOME_SHIFT = 1 << 1;
    private const int GOAL_SHIFT = 1 << 2;
    private const int INFO_SHIFT = 1 << 3;
    private const int START_SHIFT = 1 << 4;
    private const int EVENT_SHIFT = 1 << 5;
    private const int MARKERS_SHIFT = 1 << 6;
    private const int LAYERS_SHIFT = 1 << 7;

    private const TutorialSteps _allSteps = TutorialSteps.Welcome | TutorialSteps.Goal | TutorialSteps.Info | 
                    TutorialSteps.Start | TutorialSteps.Event | TutorialSteps.Markers | TutorialSteps.Layers;
    #endregion

    [SerializeField] private bool _suggestTutorial = true;
    [SerializeField] private GameObject _zones;
    [SerializeField] private SerializedDictionary<string, GameObject> _uiZones;
    [SerializeField] private TutorialSteps _steps;
    private bool _tutorialEnded = false;
    
    [System.Flags]
    public enum TutorialSteps
    {
        Welcome = WELCOME_SHIFT,
        Goal = GOAL_SHIFT,
        Info = INFO_SHIFT,
        Start = START_SHIFT,
        Event = EVENT_SHIFT,
        Markers = MARKERS_SHIFT,
        Layers = LAYERS_SHIFT
    }

    [Header("Tutorial Steps")]
    [SerializeField] private GameObject _welcome;
    [SerializeField] private GameObject _gameGoal;
    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private GameObject _startGame;
    [SerializeField] private GameObject _gameEvent;
    [SerializeField] private GameObject _markers;
    [SerializeField] private GameObject _layers;
    

    private void OnEnable()
    {
        HUD.EventRegionPanelOpen += InfoPanel;
        EventPanel.EventPanelOpen += GameEvent;
        EventPanel.EventPanelClose += GameMarker;
    }

    private void OnDisable()
    {
        HUD.EventRegionPanelOpen -= InfoPanel;
        EventPanel.EventPanelOpen -= GameEvent;
        EventPanel.EventPanelClose -= GameMarker;
    }

    private void Start()
    {
        if (_suggestTutorial)
        {
            ActivateZone(true);
            _welcome.SetActive(true);
            _steps += (int)TutorialSteps.Welcome;
        }
        else
            _tutorialEnded = true;
    }

    private void FixedUpdate()
    {
        if (_tutorialEnded)
            gameObject.SetActive(false);

        if (_steps == _allSteps)
            _tutorialEnded = true;
    }

    public void GameGoal(bool open)
    {
        if (open && _steps.HasFlag(TutorialSteps.Goal))
            return;
        else if (!open && !_steps.HasFlag(TutorialSteps.Goal))
            _steps += (int)TutorialSteps.Goal;
        
        _gameGoal.SetActive(open);
        ActivateZone(open);
    }

    public void InfoPanel(bool open)
    {
        if (open && !_steps.HasFlag(TutorialSteps.Info))
        {
            _infoPanel.SetActive(true);
            _uiZones["InfoPanel"].SetActive(true);
            ActivateZone(true);
        }
        else
        {
            if (!_steps.HasFlag(TutorialSteps.Info))
                _steps += (int)TutorialSteps.Info;

            _infoPanel.SetActive(false);
            _uiZones["InfoPanel"].SetActive(false);
            ActivateZone(false);
        }
    }

    public void GameStarted(bool open)
    {
        if (open && !_steps.HasFlag(TutorialSteps.Start))
        {
            _startGame.SetActive(true);
            _uiZones["Timer"].SetActive(true);
            ActivateZone(true);
        }
        else
        {
            if (!_steps.HasFlag(TutorialSteps.Start))
                _steps += (int)TutorialSteps.Start;

            _startGame.SetActive(false);
            _uiZones["Timer"].SetActive(false);
            ActivateZone(false);
        }
    }

    public void GameEvent(bool open)
    {
        if (open && !_steps.HasFlag(TutorialSteps.Event))
        {
            _gameEvent.SetActive(true);
            _uiZones["Event"].SetActive(true);
            ActivateZone(true);
        }
        else
        {
            if (!_steps.HasFlag(TutorialSteps.Event))
                _steps += (int)TutorialSteps.Event;

            _gameEvent.SetActive(false);
            _uiZones["Event"].SetActive(false);
            ActivateZone(false);
        }
    }

    public void GameMarker(bool open)
    {
        if (open && _steps.HasFlag(TutorialSteps.Markers))
            return;
        else if (!open && !_steps.HasFlag(TutorialSteps.Markers))
            _steps += (int)TutorialSteps.Markers;
        
        _markers.SetActive(open);
        ActivateZone(open);
    }

    public void GameLayers(bool open)
    {
        if (open && !_steps.HasFlag(TutorialSteps.Layers))
        {
            _layers.SetActive(true);
            _uiZones["Layers"].SetActive(true);
            ActivateZone(true);
        }
        else
        {
            if (!_steps.HasFlag(TutorialSteps.Layers))
                _steps += (int)TutorialSteps.Layers;

            _layers.SetActive(false);
            _uiZones["Layers"].SetActive(false);
            ActivateZone(false);
        }
    }

    public void Skip() => _tutorialEnded = true;

    private void ActivateZone(bool status) => _zones.SetActive(status);
}