using System.Collections.Generic;
using Unity.VisualScripting;
using WorldMapStrategyKit;
using UnityEngine;
using System;

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
    private int activateIndex;
    private CivPiece selectedCivPiece;
    private List<CivPiece> events = new();
    private List<Action> autoActions = new();

    private Map Map => Transmigratio.Instance.tmdb.map;
    private WMSK WMSK => Map.wmsk;

    private string Local(string key) => Localization.Load("Hunger", key);
    private int GetAddFoodPoints(CivPiece piece) => (int)(piece.population.value / addFoodDivision / addFoodDivisionPoints);
    private int GetAddSomeFoodPoints(CivPiece piece) => GetAddFoodPoints(piece) / 100 * AddSomeFoodPointsPercents;

    private void OnEnable() {
        GameEvents.onActivateHunger = AddEvent;
        GameEvents.onDeactivateHunger = RemoveEvent;
        GameEvents.onRemoveCivPiece += RemoveEvent;

        autoActions.Add(AddFood);
        autoActions.Add(AddSomeFood);
        autoActions.Add(Nothing);
    }

    private void OnDisable() {
        GameEvents.onActivateHunger = default;
        GameEvents.onDeactivateHunger = default;
        GameEvents.onRemoveCivPiece -= RemoveEvent;

        autoActions.Clear();
    }

    private void AddEvent(CivPiece piece) {
        if (events.Contains(piece)) return;
        Debug.Log($"Add event Hunger in region {piece.Region.id}");
        CreateMarker(piece);
        events.Add(piece);
        if (isShowAgain) {
            OpenPanel(piece);
            Timeline.Instance.Pause();
        } else {
            autoActions[activateIndex]?.Invoke();
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
        panel.Title = Local("Title");
        panel.Description = Local("Description");
        panel.Territory = Local("Territory 1") + " " +
                          $"<color=#{regionColor.ToHexString()}>" +
                          piece.Region.name + "</color> " +
                          Local("Territory 2") + " " +
                          $"<color=#{civColor.ToHexString()}>" +
                          Localization.Load("Civilizations", piece.Civilization.name) + "</color> " +
                          Local("Territory 3");
        panel.AddDesidion(AddFood, Local("AddFood"), GetAddFoodPoints(piece));
        panel.AddDesidion(AddSomeFood, Local("AddSomeFood"), GetAddSomeFoodPoints(piece));
        panel.AddDesidion(Nothing, Local("Nothing"), 0);

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
        activateIndex = 0;
    }

    private void AddSomeFood() {
        Debug.Log("AddSomeFood");
        ClosePanel();
        selectedCivPiece.reserveFood += selectedCivPiece.population.value / addFoodDivision / 2;
        activateIndex = 1;
    }

    private void Nothing() {
        Debug.Log("Nothing");
        ClosePanel();
        activateIndex = 2;
    }

    private void RemoveEvent(CivPiece piece) {
        Debug.Log("RemoveEvent");
        if(piece.Region.marker != null) piece.Region.marker.Destroy();
        events.Remove(piece);
    }
}
