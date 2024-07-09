using System.Collections.Generic;
using Unity.VisualScripting;
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
    [Header("Colors")]
    [SerializeField] private Color regionColor;
    [SerializeField] private Color civColor;

    private bool isShowAgain = true;
    private CivPiece selectedCivPiece;
    private List<CivPiece> events = new();

    private Map Map => Transmigratio.Instance.tmdb.map;
    private WMSK WMSK => Map.wmsk;

    private void Start() {
        GameEvents.onActivateHunger = AddEvent;
        GameEvents.onDeactivateHunger = RemoveEvent;
        GameEvents.onRemoveCivPiece += RemoveEvent;
    }

    private void AddEvent(CivPiece piece) {
        if (events.Contains(piece)) return;
        Debug.Log($"Add event Hunger in region {piece.Region.id}");
        CreateMarker(piece);
        events.Add(piece);
        if (isShowAgain) {
            OpenPanel(piece);
            Timeline.Instance.Pause();
        }
    }

    private void CreateMarker(CivPiece piece) {
        Debug.Log("CreateMarker");
        piece.Region.marker = Instantiate(markerPrefab);
        piece.Region.marker.Sprite = markerSprite;
        piece.Region.marker.Index = events.Count;
        piece.Region.marker.onClick += (int i) => { OpenPanel(piece); panel.IsShowAgain = isShowAgain; };

        Vector3 position = WMSK.countries[piece.Region.id].center;
        position.z = -0.1f;
        MarkerClickHandler handler = WMSK.AddMarker2DSprite(piece.Region.marker.gameObject, position, 0.03f, true, true);
        handler.allowDrag = false;
    }

    private void OpenPanel(CivPiece piece) {
        panel.Open();
        panel.IsShowAgain = isShowAgain;
        panel.Image = panelSprite;
        panel.Title = Localization.Load("Hunger", "Title");
        panel.Description = Localization.Load("Hunger", "Description");
        panel.Territory = Localization.Load("Hunger", "Territory 1") + " " +
                          $"<color=#{regionColor.ToHexString()}>" +
                          piece.Region.name + "</color> " +
                          Localization.Load("Hunger", "Territory 2") + " " +
                          $"<color=#{civColor.ToHexString()}>" +
                          Localization.Load("Civilizations", piece.Civilization.name) + "</color> " +
                          Localization.Load("Hunger", "Territory 3");
        panel.AddDesidion(AddFood, Localization.Load("Hunger", "AddFood"), (int)(piece.population.value / addFoodDivision / addFoodDivisionPoints));
        panel.AddDesidion(AddSomeFood, Localization.Load("Hunger", "AddSomeFood"), AddSomeFoodPointsPercents);
        panel.AddDesidion(Nothing, Localization.Load("Hunger", "Nothing"), 0);

        selectedCivPiece = piece;
    }

    private void ClosePanel() {
        isShowAgain = panel.IsShowAgain;
        panel.Close();
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
        if(piece.Region.marker != null) piece.Region.marker.Destroy();
        events.Remove(piece);
    }
}
