using System.Collections.Generic;
using WorldMapStrategyKit;
using UnityEngine;
using System;

public class WmskController : MonoBehaviour, IGameConnecter {
    [SerializeField] private GameObject arrow;
    [SerializeField] private IconMarker iconMarker;
    [SerializeField] private Color selectColor;
    [SerializeField] private int eventMarkerLiveTime;
    [SerializeField] private Sprite[] markerSprites;
    [SerializeField] private Material migrationMat;

    private bool canOpenPanel;
    private MapController map;
    
    private WMSK wmsk;

    private int selectedIndex = -1;
    private bool isDraging;
    //private List<S_LineMigration> lineMigrations = new();

    public Action<int> OnClick;

    public int SelectedIndex => selectedIndex;

    public GameController GameController {
        set {
            value.Get(out map);
        }
    }

    public bool GetNeighbours(int region, out int[] neighbours) {
        neighbours = null;
        if (region < 0 || wmsk.countries.Length >= region) return false;
        if (wmsk.countries[region].neighbours == null ||
            wmsk.countries[region].neighbours.Length == 0) return false;
        neighbours = wmsk.countries[region].neighbours;
        return true;
    }

    public bool GetRegionPosition(int region, out Vector2 position) {
        position = Vector2.zero;
        if(region < 0 || wmsk.countries.Length <= region) return false;
        position = wmsk.countries[region].center;
        return true;
    }

    public IconMarker CreateMarker(Vector2 position, float liveTime, Sprite sprite, Action<IconMarker> OnClick, Action OnTimeDestroy) {
        IconMarker marker = Instantiate(iconMarker);
        marker.Sprite = sprite;
        //marker.LiveTime = liveTime;
        //marker.OnClick += OnClick;
        marker.OnTimeDestroy += OnTimeDestroy;
        //wmsk.AddMarker2DSprite(marker.gameObject, position, new Vector2(0.025f, 0.05f), true);
        wmsk.AddMarker2DSprite(marker.gameObject, position, 0.025f, true);
        return marker;
    }

    public void RegionPainting(int region, Color color) {
        wmsk.ToggleCountrySurface(region, true, color);
    }

    public LineMarkerAnimator CreateLine(Vector2 start, Vector2 end) {
        LineMarkerAnimator lma = wmsk.AddLine(start, end, Color.red, 0f, 4f);
        lma.lineMaterial = migrationMat;
        lma.lineWidth = 2f;
        lma.drawingDuration = 1.5f;
        lma.dashInterval = 0.005f;
        lma.dashAnimationDuration = 0.8f;
        return lma;
    }
    /*
    public LineMarkerAnimator CreateLine(Vector2 start, Vector2 end, Color color,
        Material lineMaterial, GameObject startCap, GameObject endCap,
        Vector3 startCapScale, Vector3 endCapScale,
        float startCapOffset = 0.5f, float endCapOffset = 0.5f,
        float elevation = 0f, float lineWidth = 0.5f, float drawingDuration = 4f, float dashInterval = 0.01f, float dashAnimationDuration = 0.25f) {

        LineMarkerAnimator lma = wmsk.AddLine(start, end, color, elevation, lineWidth);
        lma.drawingDuration = drawingDuration;
        // lma.autoFadeAfter = 2.0f;

        //lma.drawingDuration = 2.0f;
        lma.dashInterval = dashInterval;
        lma.dashAnimationDuration = dashAnimationDuration;

        lma.startCap = startCap;
        lma.startCapOffset = startCapOffset;
        lma.startCapScale = startCapScale;

        lma.lineMaterial = lineMaterial;

        lma.endCap = endCap;
        lma.endCapOffset = endCapOffset;
        lma.endCapScale = endCapScale;
        return lma;
    }
    */

    private void ClickMap(float x, float y, int buttonIndex) {
        if (isDraging) return;
        int index = wmsk.GetCountryIndex(new Vector2(x, y));
        if (index < 0 || index > wmsk.countries.Length - 1) return;

        wmsk.ToggleCountrySurface(selectedIndex, true,
            selectedIndex < map.data.CountRegions &&
            selectedIndex >= 0 ?
            map.data.GetColor(selectedIndex) : Color.clear);
        selectedIndex = index;
        wmsk.ToggleCountrySurface(selectedIndex, true, selectColor);

        OnClick?.Invoke(selectedIndex);
    }

    //private void ClickMarker(MarkerClickHandler marker, int buttonIndex) => marker.GetComponent<IconMarker>()?.Click();
    
    #region PutInMigration
    //public void StartMigration(MigrationData data, int index) {
    //    Vector2 start = wmsk.GetCountry(data.From).center;
    //    Vector2 end = wmsk.GetCountry(data.To).center;

    //    // IconMarker marker = CreateMarker(start, 0, null, (IconMarker marker) => { }/*migration.OpenPanel(data, index)*/);

    //    Color color = Color.red;
    //    float lineWidth = 0.5f;
    //    float elevation = 0f;

    //    LineMarkerAnimator lma = wmsk.AddLine(start, end, color, elevation, lineWidth);
    //    lma.drawingDuration = 4.0f;
    //    // lma.autoFadeAfter = 2.0f;
        
    //    //lma.drawingDuration = 2.0f;
    //    lma.dashInterval = 0.01f;
    //    lma.dashAnimationDuration = 0.25f;

    //    lma.endCap = arrow;
    //    lma.endCapOffset = 0.5f;
    //    lma.endCapScale = new Vector3(3f, 3f, 1f);

    //    lineMigrations.Add(new S_LineMigration { Lma = lma });
    //}

    //public void EndMigration(int index) {
    //    lineMigrations[index].Marker.DestroyGO();
    //    lineMigrations[index].Lma.FadeOut(0);
    //    lineMigrations.RemoveAt(index);
    //}
    #endregion

    public void Init() {
        wmsk = WMSK.instance;
        wmsk.OnMouseRelease += ClickMap;
        //wmsk.OnMarkerMouseDown += ClickMarker;
        wmsk.OnDragStart += () => isDraging = true;
        wmsk.OnDragEnd += () => isDraging = false;
        
        // For havn't friezes at first change colors
        for (int i = 0; i < wmsk.countries.Length; ++i) {
            wmsk.ToggleCountrySurface(i, true, Color.clear);
        }
    }
}
