using System.Collections.Generic;
using WorldMapStrategyKit;
using UnityEngine;

public class HungerController : Singleton<HungerController> {
    [SerializeField] private EventPanel panel;
    [SerializeField] private IconMarker markerPrefab;
    [SerializeField] private Sprite markerSprite;
    [SerializeField] private Sprite panelSprite;
    [Header("Desidion")]
    [SerializeField, Min(0)] private float addFoodDivision;
    [Header("Points")]
    [SerializeField, Min(0)] private float addFoodDivisionPoints;
    [SerializeField, Range(0, 1)] private float AddSomeFoodPointsPercent;

    private bool isShowAgain;
    private IconMarker marker;
    private CivPiece selectedCivPiece;
    private List<CivPiece> events = new();

    private Map Map => Transmigratio.Instance.tmdb.map;
    private WMSK WMSK => Map.wmsk;

    private void Start() {
        GameEvents.onActivateHunger = AddEvent;
        GameEvents.onDeactivateHunger = RemoveEvent;
    }

    public void AddEvent(CivPiece piece) {
        if (events.Contains(piece)) return;
        CreateMarker(piece);
        events.Add(piece);
        if (panel.IsShowAgain) OpenPanel(piece);
    }

    public void CreateMarker(CivPiece piece) {
        Debug.Log("CreateMarker");
        marker = Instantiate(markerPrefab);
        marker.Sprite = markerSprite;
        marker.Index = events.Count;
        marker.onClick += (int i) => { OpenPanel(events[i]); panel.IsShowAgain = isShowAgain; };

        Vector2 position = WMSK.countries[piece.region.id].center;
        MarkerClickHandler handler = WMSK.AddMarker2DSprite(marker.gameObject, position, 0.03f, true, true);
        handler.allowDrag = false;
    }

    public void OpenPanel(CivPiece piece) {
        panel.Open();
        panel.onClick = ActivateDesidion;
        panel.Image = panelSprite;
        panel.Title = /*StringLoader.Load(*/"HungerTitle"/*)*/;
        panel.Description = /*StringLoader.Load(*/"HungerDescription"/*)*/;
        panel.Territory = /*string.Format(StringLoader.Load(*/"HungerFrom"/*),*/;
                                        //StringLoader.Load($"Region {events[index].piece.region.id}"),
                                        //StringLoader.Load(events[index].piece.civilization.name));
        panel.AddDesidion(/*StringLoader.Load(*/"AddFood"/*)*/, (int)(piece.population.value / addFoodDivision / addFoodDivisionPoints));
        panel.AddDesidion(/*StringLoader.Load(*/"AddSomeFood"/*)*/, 5);
        panel.AddDesidion(/*StringLoader.Load(*/"Nothing"/*)*/, 0);

        selectedCivPiece = piece;
    }

    public void ClosePanel(bool isPlay) => panel.Close(isPlay);

    public void ActivateDesidion(int index) {
        Debug.Log("ActivateDesidion");
        isShowAgain = panel.IsShowAgain;
        if (index == 0) AddFood();
        if (index == 1) AddSomeFood();
        if (index == 2) Nothing();
    }

    private void AddFood() {
        Debug.Log("AddFood");
        ClosePanel(true);
        selectedCivPiece.reserveFood += selectedCivPiece.population.value / addFoodDivision;
    }

    private void AddSomeFood() {
        Debug.Log("AddSomeFood");
        ClosePanel(false);
        selectedCivPiece.reserveFood += selectedCivPiece.population.value / addFoodDivision / 2;
        MigrationController.Instance.TryMigration(selectedCivPiece);
    }

    private void Nothing() {
        Debug.Log("Nothing");
        ClosePanel(true);
    }

    public void RemoveEvent(CivPiece piece) {
        events.Remove(piece);
    }
}
