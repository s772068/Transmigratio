using System.Collections.Generic;
using UnityEngine;
using System;

public class EventsController : MonoBehaviour, IGameConnecter {
    [SerializeField] private GUIP_Notifications notifications;
    [SerializeField] private GUIP_Event panel;
    [SerializeField, Min(0)] private float markerLiveTime;
    // [SerializeField] private float[] chances;
    // [SerializeField, Range(0, 100)] private float migratePercent;

    private List<I_Event> events;
    private Dictionary<int, List<int>> logs = new();

    private SettingsController settings;
    private TimelineController timeline;
    private GameController game;
    private WmskController wmsk;
    private MapController map;

    public Action<I_Event> OnOpenPanel;

    public GameController GameController {
        set {
            game = value;
            value.Get(out settings);
            value.Get(out timeline);
            value.Get(out wmsk);
            value.Get(out map);
        }
    }

    public void CreateRandomEvent() {
        CreateEvent(Randomizer.Random(events.Count));
        //++countTryCreateEvent;
        //if (! &&
        //    countTryCreateEvent < 100)
        //    CreateRandomEvent();
    }

    public bool CreateEvent(int eventIndex) {
        I_Event e = events[eventIndex];
        e.Index = eventIndex;
        e.Game = game;
        if (!e.Init()) return false;
        if (!CreateMarker(e)) return false;
        if (!logs.ContainsKey(e.Region)) logs.Add(e.Region, new());
        notifications.AddEvent(e);
        logs[e.Region].Add(eventIndex);
        return true;
    }

    //private bool CreateHungryEvent() {
    //    print(migratePopulation);
    //    HungerEvent e = new HungerEvent() {
    //        Game = game,
    //        Region = region,
    //        MigrateRegion = migrateRegion,
    //        MigratePopulation = 100,
    //        MarkerLiveTime = 5
    //    };

    //    events.Add(e);
    //    return CreateMarker(e, 1, region);
    //}

    //private void CreateVolcanoEvent() {
    //    events.Add(new VolcanoEvent() { Game = game });
    //    return CreateMarker(e, 1, region);
    //}

    private bool CreateMarker(I_Event e) {
        if (!wmsk.GetRegionPosition(e.Region, out Vector2 position)) return false;
        Sprite sprite = settings.Theme.GetEventMarker(e.Index);
        wmsk.CreateMarker(position, markerLiveTime, sprite, (IconMarker owner) => {
            OnOpenPanel?.Invoke(e);
            owner.DestroyGO();
        });
        return true;
    }

    public void UpdateEvent() {
        //for(int i = 0; i < eventLog.Count; ++i) {

        //}
        //print("Update Event");
    }

    public void Call() {
        //for (int eventIndex = 0; eventIndex < events.Count; ++eventIndex) {
        //    for (int regionIndex = 0; regionIndex < map.data.CountRegions; ++regionIndex) {
        //        if (events[eventIndex].CheckActivate(ref map.data.GetRegion(regionIndex))) {
        //            wmsk.CreateEventMarker(events[eventIndex].Data(settings.Language), eventIndex, regionIndex);
        //        }
        //    }
        //}


        // foreach (KeyValuePair<int, List<int>> pair in eventLog) {
        //     if (Random.Range(0, 1f) <= chances[map.data.Regions[pair.Key].EventChanceIndex]) {
        //         map.data.Regions[pair.Key].EventChanceIndex = 0;
        //         int numEvent = Random.Range(0, pair.Value.Count - 1);
        //         int eventIndex = pair.Value[numEvent];
        //         wmsk.CreateEventMarker(events[eventIndex].Data(settings.Language), eventIndex, pair.Key);
        //         pair.Value.RemoveAt(numEvent);
        //         map.data.Regions[pair.Key].Events.Add(eventIndex);
        //         if (pair.Value.Count == 0) eventLog.Remove(pair.Key);
        //         return;
        //     } else {
        //         ++map.data.Regions[pair.Key].EventChanceIndex;
        //     }
        // }
    }

    public bool OpenPanel(I_Event e, int eventIndex, int region) {
        return true;
        //    if (panel.gameObject.activeSelf) return false;
        //    panel.index = eventIndex;
        //    panel.gameObject.SetActive(true);
        //    panel.Init(data);

        //    for (int i = 0; i < data.Results.Length; ++i) {
        //        if (events[eventIndex].CheckBuild(region, i))
        //            panel.Build(data.Results[i], i);
        //    }

        //    panel.OnClickResult = (int result) => ClickResult(eventIndex, region, result);
        //    panel.OnClose = () => panel.gameObject.SetActive(false);
        //    return true;
    }

    //public void ClickResult(int eventIndex, int countryIndex, int resultIndex) {
    //    events[eventIndex].Use(countryIndex, resultIndex);
    //}

    //public void AddEventsForRegion(int regionIndex) {
    //    eventLog[regionIndex] = new();
    //    for (int i = 0; i < events.Count; ++i) {
    //        eventLog[regionIndex].Add(i);
    //    }
    //}

    // private void InitEventLog() {
    //     for (int i = 0; i < map.data.Regions.Length; ++i) {
    //         if (map.data.Regions[i].Civilizations.Length > 0) {
    //             AddEventsForRegion(i);
    //         }
    //     }
    // }

    public void Init() {
        // InitEventLog();
        timeline.OnCreateEvent += CreateRandomEvent;
        timeline.OnUpdateData += UpdateEvent;

        events = new() {
            new VolcanoEvent(),
            new HungerEvent()
        };
    }

    private void OnDestroy() {
        timeline.OnCreateEvent += CreateRandomEvent;
        timeline.OnUpdateData -= UpdateEvent;
    }
}

public interface I_Event {
    int Index { get; set; }
    GameController Game { set; }
    int CountResults{ get; }
    int Region { get; }
    bool Init();
    bool CheckBuild(int i);
    bool TryActivate();
    bool Use(int result);
}
