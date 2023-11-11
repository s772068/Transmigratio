using System.Collections.Generic;
using UnityEngine;

public class EventsController : BaseController {
    [SerializeField] private GUI_EventPanel panel;
    [SerializeField] private float markerLiveTime;
    [SerializeField] private float[] chances;

    private List<BaseEvent> events = new();
    private Dictionary<int, List<int>> eventLog = new();
    
    private GameController game;
    private WmskController wmsk;
    private SaveController save;

    public float MarkerLiveTime => markerLiveTime;

    public override GameController GameController {
        set {
            game = value;
            wmsk = value.Get<WmskController>();
            save = value.Get<SaveController>();
        }
    }

    public void Call() {
        foreach(KeyValuePair<int, List<int>> pair in eventLog) {
            if (Random.Range(0, 1f) <= chances[save.countries[pair.Key].eventChanceIndex]) {
                save.countries[pair.Key].eventChanceIndex = 0;
                int numEvent = Random.Range(0, pair.Value.Count - 1);
                int eventIndex = pair.Value[numEvent];
                wmsk.CreateEventMarker(events[eventIndex].Data((int) save.localization), eventIndex, pair.Key);
                pair.Value.RemoveAt(numEvent);
                save.countries[pair.Key].events.Add(eventIndex);
                if(pair.Value.Count == 0) eventLog.Remove(pair.Key);
                return;
            } else {
                ++save.countries[pair.Key].eventChanceIndex;
            }
        }
    }

    public bool OpenPanel(S_Event data, int countryIndex, int eventIndex) {
        if (panel.gameObject.activeSelf) return false;
        panel.index = eventIndex;
        panel.gameObject.SetActive(true);
        panel.Init(data);

        for(int i = 0; i < data.Results.Length; ++i) {
            if (events[eventIndex].CheckBuild(countryIndex, i))
            panel.Build(data.Results[i], i);
        }

        panel.OnClickResult = (int index) => ClickResult(countryIndex, eventIndex, index);
        panel.OnClose = () => panel.gameObject.SetActive(false);
        return true;
    }

    public void ClickResult(int countryIndex, int eventIndex, int resultIndex) {
        events[eventIndex].Use(countryIndex, resultIndex);
    }

    public void AddEventsForCountry(int countryIndex) {
        eventLog[countryIndex] = new();
        for (int i = 0; i < events.Count; ++i) {
            eventLog[countryIndex].Add(i);
        }
    }

    private void InitEventLog() {
        for (int i = 0; i < save.countries.Length; ++i) {
            if (save.countries[i].population > 0) {
                AddEventsForCountry(i);
            }
        }
    }

    public override void Init() {
        events.Add(new VolcanoEvent(game, 100, 3));
        events.Add(new HungerEvent(game, 100, 5));
        InitEventLog();
    }
}
