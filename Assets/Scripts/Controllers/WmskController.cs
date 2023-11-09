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

    private SaveController save;
    private EventsController events;
    private HUD hud;
    
    private WMSK wmsk;
    private int selectedIndex = -1;
    private List<S_LineMigration> lineMigrations;
    
    public Action<int> OnClick;

    public override GameController GameController {
        set {
            save = value.Get<SaveController>();
            events = value.Get<EventsController>();
            hud  = value.Get<HUD>();
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
        SelectCountry(wmsk.GetCountryIndex(new Vector2(x, y)));
        OnClick?.Invoke(selectedIndex);
    }

    private void ClickMarker(MarkerClickHandler marker, int buttonIndex) => marker.GetComponent<IconMarker>()?.Click();

    private void Print(IconMarker owner) {
        print("Click: " + owner.name);
    }

    private void SelectCountry(int index) {
        wmsk.ToggleCountrySurface(selectedIndex, false, Color.clear);
        selectedIndex = index;
        wmsk.ToggleCountrySurface(selectedIndex, true, selectColor);
    }

    public void CreateEventMarker(S_Event e, int eventIndex, int countryIndex) {
        CreateIconMarker(wmsk.GetCountry(countryIndex).center, e.MarkerIndex, eventMarkerLiveTime, (IconMarker owner) => {
            hud.OpenEventPanel(e, countryIndex, eventIndex);
            owner.DestroyGO();
        });
    }

    private IconMarker CreateIconMarker(Vector3 position, int markerIndex, int liveTime, Action<IconMarker> OnClick) {
        IconMarker marker = Instantiate(iconMarker);
        marker.Sprite = markerSprites[markerIndex];
        marker.LiveTime = liveTime;
        marker.OnClick += OnClick;
        wmsk.AddMarker2DSprite(marker.gameObject, position, new Vector2(0.025f, 0.05f), true);
        return marker;
    }

    public void StartMigration(S_Migration migration) {
        Vector2 start = wmsk.GetCountry(migration.From).center;
        Vector2 end = wmsk.GetCountry(migration.To).center;

        IconMarker marker = CreateIconMarker(start, migration.MarkerIndex, -1, (IconMarker marker) => { });

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
    }

    private void Start() {
        // AddTrajectories(0, 3);
    }
}
