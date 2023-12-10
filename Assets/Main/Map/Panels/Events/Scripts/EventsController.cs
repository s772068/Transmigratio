using System.Collections.Generic;
using UnityEngine;

public class EventsController : MonoBehaviour {// , IGameConnecter {
    // [SerializeField] private GUIP_Event panel;
    // [SerializeField] private float markerLiveTime;
    // [SerializeField] private float[] chances;
    // 
    // private List<BaseEvent> events = new();
    // private Dictionary<int, List<int>> eventLog = new();
    // 
    // private SettingsController settings;
    // private TimelineController timeline;
    // private GameController game;
    // private WmskController wmsk;
    // private MapController map;
    // 
    // public float MarkerLiveTime => markerLiveTime;
    // 
    // public GameController GameController {
    //     set {
    //         game = value;
    //         settings = value.Get<SettingsController>();
    //         timeline = value.Get<TimelineController>();
    //         wmsk = value.Get<WmskController>();
    //         map = value.Get<MapController>();
    //     }
    // }
    // 
    // public void Call() {
    //     for (int eventIndex = 0; eventIndex < events.Count; ++eventIndex) {
    //         for (int regionIndex = 0; regionIndex < map.data.Regions.Length; ++regionIndex) {
    //             if (events[eventIndex].CheckActivate(ref map.data.Regions[regionIndex])) {
    //                 wmsk.CreateEventMarker(events[eventIndex].Data(settings.Language), eventIndex, regionIndex);
    //             }
    //         }
    //     }
    //     // foreach (KeyValuePair<int, List<int>> pair in eventLog) {
    //     //     if (Random.Range(0, 1f) <= chances[map.data.Regions[pair.Key].EventChanceIndex]) {
    //     //         map.data.Regions[pair.Key].EventChanceIndex = 0;
    //     //         int numEvent = Random.Range(0, pair.Value.Count - 1);
    //     //         int eventIndex = pair.Value[numEvent];
    //     //         wmsk.CreateEventMarker(events[eventIndex].Data(settings.Language), eventIndex, pair.Key);
    //     //         pair.Value.RemoveAt(numEvent);
    //     //         map.data.Regions[pair.Key].Events.Add(eventIndex);
    //     //         if (pair.Value.Count == 0) eventLog.Remove(pair.Key);
    //     //         return;
    //     //     } else {
    //     //         ++map.data.Regions[pair.Key].EventChanceIndex;
    //     //     }
    //     // }
    // }
    // 
    // public bool OpenPanel(S_Event data, int eventIndex, int countryIndex) {
    //     if (panel.gameObject.activeSelf) return false;
    //     panel.index = eventIndex;
    //     panel.gameObject.SetActive(true);
    //     panel.Init(data);
    // 
    //     for (int i = 0; i < data.Results.Length; ++i) {
    //         if (events[eventIndex].CheckBuild(countryIndex, i))
    //             panel.Build(data.Results[i], i);
    //     }
    // 
    //     panel.OnClickResult = (int index) => ClickResult(eventIndex, countryIndex, index);
    //     panel.OnClose = () => panel.gameObject.SetActive(false);
    //     return true;
    // }
    // 
    // public void ClickResult(int eventIndex, int countryIndex, int resultIndex) {
    //     events[eventIndex].Use(countryIndex, resultIndex);
    // }
    // 
    // public void AddEventsForRegion(int regionIndex) {
    //     eventLog[regionIndex] = new();
    //     for (int i = 0; i < events.Count; ++i) {
    //         eventLog[regionIndex].Add(i);
    //     }
    // }
    // 
    // // private void InitEventLog() {
    // //     for (int i = 0; i < map.data.Regions.Length; ++i) {
    // //         if (map.data.Regions[i].Civilizations.Length > 0) {
    // //             AddEventsForRegion(i);
    // //         }
    // //     }
    // // }
    // 
    // public void Init() {
    //     events.Add(new VolcanoEvent(game, 100, 3));
    //     events.Add(new HungerEvent(game, 100, 5));
    //     // InitEventLog();
    //     timeline.OnTick += Call;
    // }
    // 
    // private void OnDestroy() {
    //     timeline.OnTick -= Call;
    // }
}

