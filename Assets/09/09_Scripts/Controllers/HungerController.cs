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
    [SerializeField, Range(0, 100)] private int AddSomeFoodPointsPercents;

    private bool isShowAgain;
    private CivPiece selectedCivPiece;
    private List<CivPiece> events = new();

    private Map Map => Transmigratio.Instance.tmdb.map;
    private WMSK WMSK => Map.wmsk;

    private void Start() {
        GameEvents.onActivateHunger = AddEvent;
        GameEvents.onDeactivateHunger = RemoveEvent;
    }

    private void AddEvent(CivPiece piece) {
        if (events.Contains(piece)) return;
        Debug.Log($"Add event Hunger in region {piece.region.id}");
        CreateMarker(piece);
        events.Add(piece);
        if (panel.IsShowAgain) {
            OpenPanel(piece);
            Timeline.Instance.Pause();
        }
    }

    private void CreateMarker(CivPiece piece) {
        Debug.Log("CreateMarker");
        piece.region.marker = Instantiate(markerPrefab);
        piece.region.marker.Sprite = markerSprite;
        piece.region.marker.Index = events.Count;
        piece.region.marker.onClick += (int i) => { OpenPanel(piece); panel.IsShowAgain = isShowAgain; };

        Vector2 position = WMSK.countries[piece.region.id].center;
        MarkerClickHandler handler = WMSK.AddMarker2DSprite(piece.region.marker.gameObject, position, 0.03f, true, true);
        handler.allowDrag = false;
    }

    private void OpenPanel(CivPiece piece) {
        panel.Open();
        panel.onClick = ActivateDesidion;
        panel.Image = panelSprite;
        panel.Title = StringLoader.Load("Hunger", "Title");
        panel.Description = StringLoader.Load("Hunger", "Description");
        panel.Territory = StringLoader.Load("Hunger", "Territory 1") + " " +
                          piece.region.name + " " +
                          StringLoader.Load("Hunger", "Territory 2") + " " +
                          piece.civilization.name + " " +
                          StringLoader.Load("Hunger", "Territory 3");
        panel.AddDesidion(StringLoader.Load("Hunger", "AddFood"), (int)(piece.population.value / addFoodDivision / addFoodDivisionPoints));
        panel.AddDesidion(StringLoader.Load("Hunger", "AddSomeFood"), AddSomeFoodPointsPercents);
        panel.AddDesidion(StringLoader.Load("Hunger", "Nothing"), 0);

        selectedCivPiece = piece;
    }

    private void ClosePanel() => panel.Close();

    private void ActivateDesidion(int index) {
        Debug.Log("ActivateDesidion");
        isShowAgain = panel.IsShowAgain;
        if (index == 0) AddFood();
        if (index == 1) AddSomeFood();
        if (index == 2) Nothing();
    }

    private void AddFood() {
        Debug.Log("AddFood");
        ClosePanel();
        selectedCivPiece.reserveFood += selectedCivPiece.population.value / addFoodDivision;
    }

    private void AddSomeFood() {
        Debug.Log("AddSomeFood");
        ClosePanel();
        selectedCivPiece.reserveFood += selectedCivPiece.population.value / addFoodDivision / 2;
    }

    private void Nothing() {
        Debug.Log("Nothing");
        ClosePanel();
    }

    private void RemoveEvent(CivPiece piece) {
        Debug.Log("RemoveEvent");
        if(piece.region.marker != null) piece.region.marker.Destroy();
        events.Remove(piece);
    }
}
