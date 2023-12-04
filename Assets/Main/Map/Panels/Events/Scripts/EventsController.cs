using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class EventsController : MonoBehaviour, IGameConnecter {
    [Header("GUI")]
    [SerializeField] private Image icon;
    [SerializeField] private Text eventName;
    [SerializeField] private Text description;
    [SerializeField] private Transform content;
    [SerializeField] private GUI_EventResult resultPref;
    [Header("Values")]
    [SerializeField] private float markerLiveTime;
    [SerializeField] private List<Sprite> iconSprites;
    [SerializeField] private float[] chances;

    private GameController game;
    private WmskController wmsk;
    private TimelineController timeline;
    private MapController map;
    private SettingsController settings;

    private int index;

    private List<BaseEvent> events = new();
    private Dictionary<int, List<int>> eventLog = new();
    private List<GUI_EventResult> results = new();

    public float MarkerLiveTime => markerLiveTime;

    public GameController GameController {
        set {
            game = value;
            value.Get(out timeline);
            value.Get(out settings);
            value.Get(out wmsk);
            value.Get(out map);
        }
    }

    public Action<int> OnClickResult;
    public Action OnClose;

    public string GetEventName(int eventIndex) => events[eventIndex].Data(settings.Language).Name;

    public void Call() {
        foreach(KeyValuePair<int, List<int>> pair in eventLog) {
            if (UnityEngine.Random.Range(0, 1f) <= chances[map.data.Regions[pair.Key].EventChanceIndex]) {
                map.data.Regions[pair.Key].EventChanceIndex = 0;
                int numEvent = UnityEngine.Random.Range(0, pair.Value.Count - 1);
                int eventIndex = pair.Value[numEvent];
                wmsk.CreateEventMarker(events[eventIndex].Data(settings.Language), eventIndex, pair.Key);
                pair.Value.RemoveAt(numEvent);
                map.data.Regions[pair.Key].Events.Add(eventIndex);
                if(pair.Value.Count == 0) eventLog.Remove(pair.Key);
                return;
            } else {
                ++map.data.Regions[pair.Key].EventChanceIndex;
            }
        }
    }

    public bool Open(S_Event data, int regionIndex, int eventIndex) {
        if (gameObject.activeSelf) return false;
        index = eventIndex;
        gameObject.SetActive(true);
        
        eventName.text = data.Name;
        description.text = data.Description;
        icon.sprite = iconSprites[data.IconIndex];

        for (int i = 0; i < data.Results.Length; ++i) {
            if (events[eventIndex].CheckBuild(regionIndex, i))
            Build(data.Results[i], i);
        }

        return true;
    }

    public GUI_EventResult Build(S_EventResult data, int resultIndex) {
        GUI_EventResult result = Instantiate(resultPref, content);
        result.Index = resultIndex;
        result.Init(data);
        // result.OnClick = ClickResult;
        return result;
    }

    public void ClickResult(int regionIndex, int eventIndex, int resultIndex) {

        OnClickResult?.Invoke(resultIndex);
        Clear();
        gameObject.SetActive(false);
        events[eventIndex].Use(regionIndex, resultIndex);
    }

    public void AddEventsForRegion(int regionIndex) {
        eventLog[regionIndex] = new();
        for (int i = 0; i < events.Count; ++i) {
            eventLog[regionIndex].Add(i);
        }
    }

    private void Clear() {
        while (results.Count > 0) {
            results[0].DestroyGO();
            results.RemoveAt(0);
        }
    }

    public void Close() {
        Clear();
        gameObject.SetActive(false);
    }

    private void InitEventLog() {
        for (int i = 0; i < map.data.Regions.Length; ++i) {
            // if (map.data.Regions[i].Population > 0) {
            AddEventsForRegion(i);
            //}
        }
    }

    public void Init() {
        events.Add(new VolcanoEvent(game, 100, 3));
        events.Add(new HungerEvent(game, 100, 5));
        InitEventLog();
        timeline.OnTick += Call;
    }
}
