using WorldMapStrategyKit;
using UnityEngine;
using System;
using System.Collections.Generic;

public class WmskController : BaseController {
    [SerializeField] private GameObject arrow;
    [SerializeField] private IconMarker iconMarker;
    [SerializeField] private Color selectColor;
    [SerializeField] private int eventMarkerLiveTime;
    [SerializeField] private Sprite[] markerSprites;

    private MigrationController migration;
    private EventsController events;
    private MapController map;

    private WMSK wmsk;
    private int selectedIndex = -1;
    private List<S_LineMigration> lineMigrations = new();

    public Action<int> OnClick;

    public override GameController GameController {
        set {
            migration = value.Get<MigrationController>();
            events = value.Get<EventsController>();
            map = value.Get<MapController>();
        }
    }

    public int SelectedIndex => selectedIndex;

    public void GetNeighbours(out int[] res, int index) {
        res = wmsk.countries[index].neighbours;
    }

    public void GetNeighbours(out Country[] res, int index) {
        res = new Country[wmsk.countries[index].neighbours.Length];
        for(int i = 0; i < res.Length; ++i) {
            res[i] = wmsk.GetCountry(wmsk.countries[index].neighbours[i]);
        }
    }

    private void Click(float x, float y, int buttonIndex) {
        int index = wmsk.GetCountryIndex(new Vector2(x, y));
        if (index < 0 || index > wmsk.countries.Length - 1) return;
        UpdateSelectIndex(index);
        OnClick?.Invoke(selectedIndex);
    }

    private void ClickMarker(MarkerClickHandler marker, int buttonIndex) => marker.GetComponent<IconMarker>()?.Click();

    private void UpdateSelectIndex(int index) {
        wmsk.ToggleCountrySurface(selectedIndex, true,
            selectedIndex < map.data.Regions.Length &&
            selectedIndex >= 0 ?
            map.data.Regions[selectedIndex].Color : Color.clear);
        selectedIndex = index;
        wmsk.ToggleCountrySurface(selectedIndex, true, selectColor);
    }

    public void RegionPainting(int regionIndex, Color color) {
        wmsk.ToggleCountrySurface(regionIndex, true, color);
    }

    public void CreateEventMarker(S_Event e, int eventIndex, int regionIndex) {
        CreateIconMarker(wmsk.GetCountry(regionIndex).center, e.MarkerIndex, events.MarkerLiveTime, (IconMarker owner) => {
            events.OpenPanel(e, regionIndex, eventIndex);
            owner.DestroyGO();
        });
    }

    private IconMarker CreateIconMarker(Vector3 position, int markerIndex, float liveTime, Action<IconMarker> OnClick) {
        IconMarker marker = Instantiate(iconMarker);
        marker.Sprite = markerSprites[markerIndex];
        marker.LiveTime = liveTime;
        marker.OnClick += OnClick;
        wmsk.AddMarker2DSprite(marker.gameObject, position, new Vector2(0.025f, 0.05f), true);
        return marker;
    }

    public void StartMigration(S_Migration data, int index) {
        Vector2 start = wmsk.GetCountry(data.From).center;
        Vector2 end = wmsk.GetCountry(data.To).center;

        IconMarker marker = CreateIconMarker(start, data.MarkerIndex, -1, (IconMarker marker) => migration.OpenPanel(data, index));

        Color color = Color.red;
        float lineWidth = 0.5f;
        float elevation = 0f;

        LineMarkerAnimator lma = wmsk.AddLine(start, end, color, elevation, lineWidth);
        lma.drawingDuration = 4.0f;
        // lma.autoFadeAfter = 2.0f;
        
        //lma.drawingDuration = 2.0f;
        lma.dashInterval = 0.01f;
        lma.dashAnimationDuration = 0.25f;

        lma.endCap = arrow;
        lma.endCapOffset = 0.5f;
        lma.endCapScale = new Vector3(3f, 3f, 1f);

        lineMigrations.Add(new S_LineMigration { Lma = lma, Marker = marker });
    }

    public void EndMigration(int index) {
        lineMigrations[index].Marker.DestroyGO();
        lineMigrations[index].Lma.FadeOut(0);
        lineMigrations.RemoveAt(index);
    }

    public override void Init() {
        wmsk = WMSK.instance;
        wmsk.OnClick += Click;
        wmsk.OnMarkerMouseDown += ClickMarker;
        // For havn't friezes at first change colors
        for (int i = 0; i < wmsk.countries.Length; ++i) {
            wmsk.ToggleCountrySurface(i, true, Color.clear);
        }
    }

    private void Start() {
        // AddTrajectories(0, 3);
    }
}
