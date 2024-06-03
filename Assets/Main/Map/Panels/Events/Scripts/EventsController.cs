using UnityEngine;
using System;

public class EventsController : MonoBehaviour, IGameConnecter {
    [SerializeField] private GUIP_Notifications notifications;
    [SerializeField] private GUIP_Event panel;
    [SerializeField, Min(0)] private float markerLiveTime;

    private SettingsController settings;
    private TimelineController timeline;
    private GameController game;
    private WmskController wmsk;

    public Action<I_Event> OnOpenPanel;

    public GameController GameController {
        set {
            game = value;
            value.Get(out settings);
            value.Get(out timeline);
            value.Get(out wmsk);
        }
    }

    public void CheckHungry() {
        HungerEvent e = new HungerEvent();
        e.Game = game;
        if (!e.TryActivate()) return;
        if (!CreateMarker(e)) return;
        notifications.AddEvent(e);
    }

    public void CreateVolcano() {
        VolcanoEvent e = new VolcanoEvent();
        e.Game = game;
        if (!e.TryActivate()) return;
        if (!CreateMarker(e)) return;
        notifications.AddEvent(e);
    }

    private bool CreateMarker(I_Event e) {
        if (!wmsk.GetRegionPosition(e.Region, out Vector2 position)) return false;
        Sprite sprite = settings.Theme.GetEventMarker(e.Index);
        wmsk.CreateMarker(position, markerLiveTime, sprite, (IconMarker owner) => {
            OnOpenPanel?.Invoke(e);
            owner.Destroy();
        },
        () => e.Use(0));
        return true;
    }

    public void Init() {
        timeline.OnCheckHungry += CheckHungry;
        timeline.OnCreateVolcano += CreateVolcano;
    }

    private void OnDestroy() {
        timeline.OnCheckHungry -= CheckHungry;
        timeline.OnCreateVolcano -= CreateVolcano;
    }
}

public interface I_Event {
    int Index { get; }
    GameController Game { set; }
    int CountResults{ get; }
    int Region { get; }
    bool CheckBuild(int i);
    bool TryActivate();
    bool Use(int result);
}
