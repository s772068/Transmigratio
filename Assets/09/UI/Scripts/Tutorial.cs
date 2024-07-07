using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Localization.Components;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private bool _suggestTutorial = true;
    [SerializeField] private GameObject _zones;
    [SerializeField] private SerializedDictionary<string, GameObject> _uiZones;
    private bool _tutorialEnded = false;

    [Header("Tutorial Steps")]
    [SerializeField] private GameObject _welcome;
    [SerializeField] private GameObject _gameGoal;
    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private GameObject _startGame;
    [SerializeField] private GameObject _gameEvent;
    [SerializeField] private GameObject _markers;
    [SerializeField] private GameObject _layers;
    LocalizeStringEvent test;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

    }

    private void Start()
    {
        if (_suggestTutorial)
        {
            _zones.SetActive(true);
            _welcome.SetActive(true);
        }
        else
            _tutorialEnded = true;
    }

    private void FixedUpdate()
    {
        if (_tutorialEnded)
            gameObject.SetActive(false);
    }

    public void GameGoal(bool open) => _gameGoal.SetActive(open);

    public void InfoPanel(bool open)
    {
        if (open)
        {
            _infoPanel.SetActive(true);
            _uiZones["InfoPanel"].SetActive(true);
        }
        else
        {
            _infoPanel.SetActive(false);
            _uiZones["InfoPanel"].SetActive(false);
        }
    }

    public void GameStarted(bool open)
    {
        if (open)
        {
            _startGame.SetActive(true);
            _uiZones["Timer"].SetActive(true);
        }
        else
        {
            _startGame.SetActive(false);
            _uiZones["Timer"].SetActive(false);
        }
    }

    public void GameEvent(bool open)
    {
        if (open)
        {
            _gameEvent.SetActive(true);
            _uiZones["Event"].SetActive(true);
        }
        else
        {
            _gameEvent.SetActive(false);
            _uiZones["Event"].SetActive(false);
        }
    }

    public void Skip() => _tutorialEnded = true;
}