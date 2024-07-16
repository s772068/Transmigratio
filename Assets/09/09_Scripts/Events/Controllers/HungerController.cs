using System.Collections.Generic;
using Unity.VisualScripting;
using WorldMapStrategyKit;
using UnityEngine;
using System;

public class HungerController : Singleton<HungerController> {
    [SerializeField] private EventPanel _panelPrefab;
    [SerializeField] private IconMarker markerPrefab;
    [SerializeField] private Sprite markerSprite;
    [SerializeField] private Sprite panelSprite;
    [Header("Desidion")]
    [SerializeField] private EventDesidion _desidionPrefab;
    [SerializeField, Min(0)] private float addFoodDivision;
    [Header("Points")]
    [SerializeField, Min(0)] private float addFoodDivisionPoints;
    [SerializeField, Range(0, 100)] private int AddSomeFoodPointsPercents;
    [Header("Colors")]
    [SerializeField] private Color regionColor;
    [SerializeField] private Color civColor;

    public bool IsShowAgain = true;
    private int _activateIndex;
    private CivPiece _selectedCivPiece;
    private List<CivPiece> _events = new();
    private List<Action> _autoActions = new();

    private Map Map => Transmigratio.Instance.TMDB.map;
    private WMSK WMSK => Map.WMSK;

    private string Local(string key) => Localization.Load("Hunger", key);
    private int GetAddFoodPoints(CivPiece piece) => (int)(piece.Population.value / addFoodDivision / addFoodDivisionPoints);
    private int GetAddSomeFoodPoints(CivPiece piece) => GetAddFoodPoints(piece) / 100 * AddSomeFoodPointsPercents;

    private void OnEnable() {
        GameEvents.ActivateHunger = AddEvent;
        GameEvents.DeactivateHunger = RemoveEvent;
        GameEvents.RemoveCivPiece += RemoveEvent;

        _autoActions.Add(AddFood);
        _autoActions.Add(AddSomeFood);
        _autoActions.Add(Nothing);
    }

    private void OnDisable() {
        GameEvents.ActivateHunger = default;
        GameEvents.DeactivateHunger = default;
        GameEvents.RemoveCivPiece -= RemoveEvent;

        _autoActions.Clear();
    }

    private void AddEvent(CivPiece piece) {
        if (_events.Contains(piece)) return;
        Debug.Log($"Add event Hunger in region {piece.Region.Id}");
        CreateMarker(piece);
        _events.Add(piece);
        if (IsShowAgain) {
            OpenPanel(piece);
        } 
        else {
            _autoActions[_activateIndex]?.Invoke();
        }
    }

    private void CreateMarker(CivPiece piece) {
        Debug.Log("CreateMarker");
        piece.Region.Marker = Instantiate(markerPrefab);
        piece.Region.Marker.Sprite = markerSprite;
        piece.Region.Marker.Index = _events.Count;
        piece.Region.Marker.onClick += (int i) => OpenPanel(piece);

        Vector3 position = WMSK.countries[piece.Region.Id].center;
        position.z = -0.1f;
        MarkerClickHandler handler = WMSK.AddMarker2DSprite(piece.Region.Marker.gameObject, position, 0.03f, true, true);
        handler.allowDrag = false;
    }

    private void OpenPanel(CivPiece piece) 
    {
        string territory = Local("Territory 1") + " " +
                          $"<color=#{regionColor.ToHexString()}>" +
                          piece.Region.Name + "</color> " +
                          Local("Territory 2") + " " +
                          $"<color=#{civColor.ToHexString()}>" +
                          Localization.Load("Civilizations", piece.Civilization.Name) + "</color> " +
                          Local("Territory 3");

        Desidion[] desidions = 
        { 
            new Desidion(AddFood, Local("AddFood"), GetAddFoodPoints(piece)),
            new Desidion(AddSomeFood, Local("AddSomeFood"), GetAddSomeFoodPoints(piece)),
            new Desidion(Nothing, Local("Nothing"), 0)
        };

        PanelFabric.CreateEvent(HUD.Instance.Events, _desidionPrefab, _panelPrefab, IsShowAgain, panelSprite, Local("Title"),
                                territory, Local("Description"), desidions);

        _selectedCivPiece = piece;
    }

    private void AddFood() {
        Debug.Log("AddFood");
        _selectedCivPiece.ReserveFood += _selectedCivPiece.Population.value / addFoodDivision;
        _activateIndex = 0;
    }

    private void AddSomeFood() {
        Debug.Log("AddSomeFood");
        _selectedCivPiece.ReserveFood += _selectedCivPiece.Population.value / addFoodDivision / 2;
        _activateIndex = 1;
    }

    private void Nothing() {
        Debug.Log("Nothing");
        _activateIndex = 2;
    }

    private void RemoveEvent(CivPiece piece) {
        Debug.Log("RemoveEvent");
        if(piece.Region.Marker != null) piece.Region.Marker.Destroy();
        _events.Remove(piece);
    }
}
