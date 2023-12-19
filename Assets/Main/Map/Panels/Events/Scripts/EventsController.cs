using System.Collections.Generic;
using UnityEngine;
using System;

public class EventsController : MonoBehaviour, IGameConnecter {
    [SerializeField] private GUIP_Notifications notifications;
    [SerializeField] private GUIP_Event panel;
    [SerializeField, Min(0)] private float markerLiveTime;

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

    public void CreateHungryEvent() {
        HungerEvent e = new HungerEvent();
        e.Game = game;
        if (!e.TryActivate()) return;
        if (!e.Init()) return;
        if (!CreateMarker(e)) return;
        if (!logs.ContainsKey(e.Region)) logs.Add(e.Region, new());
        notifications.AddEvent(e);
        logs[e.Region].Add(e.Index);
    }

    public void CreateVolcanoEvent() {
        VolcanoEvent e = new VolcanoEvent();
        e.Game = game;
        if (!e.TryActivate()) return;
        if (!e.Init()) return;
        if (!CreateMarker(e)) return;
        if (!logs.ContainsKey(e.Region)) logs.Add(e.Region, new());
        notifications.AddEvent(e);
        logs[e.Region].Add(e.Index);
    }

    private bool CreateMarker(I_Event e) {
        if (!wmsk.GetRegionPosition(e.Region, out Vector2 position)) return false;
        Sprite sprite = settings.Theme.GetEventMarker(e.Index);
        wmsk.CreateMarker(position, markerLiveTime, sprite, (IconMarker owner) => {
            OnOpenPanel?.Invoke(e);
            owner.DestroyGO();
        },
        () => e.Use(0));
        return true;
    }

    public void Init() {
        //timeline.OnUpdateData += CreateHungryEvent;
        //timeline.OnCreateVolcano += CreateVolcanoEvent;
    }

    private void OnDestroy() {
        //timeline.OnUpdateData -= CreateHungryEvent;
        //timeline.OnCreateVolcano -= CreateVolcanoEvent;
    }
}

public interface I_Event {
    int Index { get; }
    GameController Game { set; }
    int CountResults{ get; }
    int Region { get; }
    bool Init();
    bool CheckBuild(int i);
    bool TryActivate();
    bool Use(int result);
}
