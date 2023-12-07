using System.Collections.Generic;
using WorldMapStrategyKit;
using UnityEngine;
using System;

public class WmskController : MonoBehaviour, IGameConnecter {
    [SerializeField] private GameObject startArrow;
    [SerializeField] private GameObject endArrow;
    [SerializeField] private Material lineArrow;
    [SerializeField] private float arrowInter;
    [SerializeField] private IconMarker iconMarker;
    [SerializeField] private Color selectColor;
    [SerializeField] private int eventMarkerLiveTime;
    [SerializeField] private Sprite[] markerSprites;

    private MigrationController migration;
    private EventsController events;
    private SettingsController settings;
    private TimelineController timeline;
    private MapController map;
    
    private WMSK wmsk;

    private int selectedIndex = -1;
    private List<S_LineMigration> lineMigrations = new();

    public Action<int> OnClick;

    public int SelectedIndex => selectedIndex;

    public GameController GameController {
        set {
            value.Get(out migration);
            value.Get(out events);
            value.Get(out settings);
            value.Get(out timeline);
            value.Get(out map);
        }
    }

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

    public void CreateEventMarker(S_Event data, int eventIndex, int regionIndex) {
        CreateIconMarker(wmsk.GetCountry(regionIndex).center, data.MarkerIndex, events.MarkerLiveTime, (IconMarker owner) => {
            events.OpenPanel(data, eventIndex, regionIndex);
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

        // IconMarker marker = CreateIconMarker(start, data.MarkerIndex, -1, (IconMarker marker) => migration.OpenPanel(data, index));

        Color color = Color.red;
        float lineWidth = 0.5f;
        float elevation = 0f;

        LineMarkerAnimator lma = wmsk.AddLine(start, end, color, elevation, lineWidth);

        float drawingDuration = UnityEngine.Random.Range(2f, 4f);
        float autoFadeAfter = drawingDuration + UnityEngine.Random.Range(1.5f, 2f);
        lma.autoFadeAfter = autoFadeAfter;
        lma.drawingDuration = drawingDuration + 1;
        lma.dashInterval = 0.01f;
        lma.dashAnimationDuration = drawingDuration;

        lma.startCap = startArrow;
        lma.startCapOffset = 0.5f;
        lma.startCapScale = new Vector3(1f, 1f, 1f);

        lma.lineMaterial = lineArrow;
        lma.lineWidth = 2;

        lma.endCap = endArrow;
        lma.endCapOffset = 0.5f;
        lma.endCapScale = new Vector3(1f, 1f, 1f);
    }

    private void CreateArrow() {
        Vector2 start = wmsk.GetCountry(UnityEngine.Random.Range(0, 54)).center;
        Vector2 end = wmsk.GetCountry(UnityEngine.Random.Range(0, 54)).center;
        Color color = Color.red;
        float lineWidth = 0.5f;
        float elevation = 0f;

        LineMarkerAnimator lma = wmsk.AddLine(start, end, color, elevation, lineWidth);

        float drawingDuration = UnityEngine.Random.Range(2f, 4f);
        float autoFadeAfter = drawingDuration + UnityEngine.Random.Range(1.5f, 2f);
        lma.autoFadeAfter = autoFadeAfter;
        lma.drawingDuration = drawingDuration + 1;
        lma.dashInterval = 0.01f;
        lma.dashAnimationDuration = drawingDuration;

        lma.startCap = startArrow;
        lma.startCapOffset = 0.5f;
        lma.startCapScale = new Vector3(1f, 1f, 1f);

        lma.lineMaterial = lineArrow;
        lma.lineWidth = 2;

        lma.endCap = endArrow;
        lma.endCapOffset = 0.5f;
        lma.endCapScale = new Vector3(1f, 1f, 1f);
    }

    public void EndMigration(int index) {
        lineMigrations[index].Marker.DestroyGO();
        lineMigrations[index].Lma.FadeOut(0);
        lineMigrations.RemoveAt(index);
    }

    public void Init() {
        wmsk = WMSK.instance;
        wmsk.OnClick += Click;
        wmsk.OnMarkerMouseDown += ClickMarker;
        
        // For havn't friezes at first change colors
        for (int i = 0; i < wmsk.countries.Length; ++i) {
            wmsk.ToggleCountrySurface(i, true, Color.clear);
        }
        timeline.OnArrows += CreateArrow;
    }

    public void OnDestroy() {
        timeline.OnTick -= CreateArrow;
    }
}
