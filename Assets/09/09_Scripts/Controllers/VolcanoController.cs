using System.Collections.Generic;
using WorldMapStrategyKit;
using UnityEngine;
using System.Linq;
using System;

public class VolcanoController : Singleton<VolcanoController> {
    [SerializeField] private EventPanel panel;
    [SerializeField] private IconMarker markerPrefab;
    [SerializeField] private Sprite markerSprite;
    [SerializeField] private Sprite panelSprite;
    [Header("Tickers")]
    [SerializeField, Min(0)] private int startTicksToActivate;
    [SerializeField, Min(0)] private int minTickToActivate;
    [SerializeField, Min(0)] private int maxTickToActivate;
    [SerializeField, Min(0)] private int ticksToEruption;
    [Header("Percents")]
    [SerializeField, Range(0, 1)] private float fullPercentFood;
    [SerializeField, Range(0, 1)] private float fullPercentPopulation;
    [SerializeField, Range(0, 1)] private float partPercentFood;
    [SerializeField, Range(0, 1)] private float partPercentPopulation;
    [Header("Points")]
    [SerializeField, Min(1)] private float calmVolcanoPointsDivision;
    [SerializeField, Min(0)] private int reduceLossesPoints;

    [Header("OnlyForView")]
    [SerializeField] private int ticker;
    [SerializeField] private int activateIndex;
    [SerializeField] private int ticksToActivateVolcano;
    private bool isShowAgain;
    private CivPiece piece;
    private IconMarker marker;
    private System.Random rand = new();
    private List<Action> autoActions = new();

    private WMSK WMSK => Transmigratio.Instance.tmdb.map.wmsk;

    private void Start() {
        autoActions.Add(CalmVolcano);
        autoActions.Add(ReduceLosses);
        autoActions.Add(ActivateVolcano);
        GameEvents.onTickLogic += StartCreateEvent;
    }

    private void StartCreateEvent() {
        ++ticker;
        if (ticker == startTicksToActivate) {
            ticker = 0;
            GameEvents.onTickLogic -= StartCreateEvent;
            CreateEvent();
        }
    }

    public void CreateEvent() {
        System.Random rand = new();
        var civilizations = Transmigratio.Instance.tmdb.humanity.civilizations;
        if (civilizations.Count == 0) return;
        piece = civilizations.ElementAt(rand.Next(0, civilizations.Count)).Value.pieces.ElementAt(rand.Next(0, civilizations.Count)).Value;
        ticksToActivateVolcano = rand.Next(minTickToActivate, maxTickToActivate);
        GameEvents.onTickLogic += WaitActivateVolcano;
        Debug.Log($"Create event Volcano in region {piece.region.id}");
        CreateMarker(WMSK.countries[piece.region.id].center);
        if (panel.IsShowAgain) OpenPanel();
    }

    public void RestartEvent() {
        Debug.Log("RestartEvent");
        ++ticker;
        if (ticker == ticksToActivateVolcano) {
            GameEvents.onTickLogic -= RestartEvent;
            ticker = 0;
            CreateEvent();
        }
    }

    public void CreateMarker(Vector2 position) {
        marker = Instantiate(markerPrefab);
        marker.Sprite = markerSprite;
        marker.onClick += (int i) => { OpenPanel(); panel.IsShowAgain = isShowAgain; };

        MarkerClickHandler handler = WMSK.AddMarker2DSprite(marker.gameObject, position, 0.03f, true, true);
        handler.allowDrag = false;
    }

    public void OpenPanel() {
        panel.Open();
        panel.onClick = ActivateDesidion;
        panel.Image = panelSprite;
        panel.Title = /*StringLoader.Load(*/"VolcanoTitle"/*)*/;
        panel.Description = /*StringLoader.Load(*/"VolcanoDescription"/*)*/;
        panel.Territory = /*string.Format(StringLoader.Load(*/ "VolcanoTerritory"/*) */;
        //                                 StringLoader.Load($"{piece.region.name}"),
        //                                 StringLoader.Load($"{piece.civilization.name}"));
        panel.AddDesidion(/*StringLoader.Load(*/"CalmVolcano"/*)*/, (int)(piece.population.value / calmVolcanoPointsDivision));
        panel.AddDesidion(/*StringLoader.Load(*/"ReduceLosses"/*)*/, reduceLossesPoints);
        panel.AddDesidion(/*StringLoader.Load(*/"Nothing"/*)*/, 0);
    }

    public void ClosePanel() => panel.Close();

    public void ActivateDesidion(int index) {
        isShowAgain = panel.IsShowAgain;
        if (index == 0) CalmVolcano();
        if (index == 1) ReduceLosses();
        if (index == 2) Nothing();
    }

    private void CalmVolcano() {
        activateIndex = 0;
        Debug.Log("CalmVolcano");

        ticker = 0;
        ClosePanel();
        marker.Destroy();
        GameEvents.onTickLogic -= WaitActivateVolcano;
        GameEvents.onTickLogic += RestartEvent;
    }

    private void ReduceLosses() {
        activateIndex = 1;
        piece.population.value -= (int)(piece.population.value * fullPercentFood * partPercentPopulation);
        piece.reserveFood -= piece.reserveFood * fullPercentFood * partPercentFood;

        Debug.Log("ReduceLosses");

        ticker = 0;
        ClosePanel();
        marker.Destroy();
        GameEvents.onTickLogic -= WaitActivateVolcano;
        GameEvents.onTickLogic += RestartEvent;
        MigrationController.Instance.TryMigration(piece);
    }

    private void Nothing() {
        activateIndex = 2;
        ClosePanel();
    }

    private void ActivateVolcano() {
        piece.population.value -= (int)(piece.population.value * fullPercentFood);
        piece.reserveFood -= piece.reserveFood * fullPercentFood;

        Debug.Log("ActivateVolcano");

        ticker = 0;
        ClosePanel();
        marker.Destroy();
        MigrationController.Instance.TryMigration(piece);
    }

    private void WaitActivateVolcano() {
        Debug.Log("WaitActivateVolcano");
        ++ticker;
        if (ticker == ticksToEruption) {
            ticker = 0;
            marker.Destroy();
            autoActions[activateIndex]?.Invoke();
            GameEvents.onTickLogic -= WaitActivateVolcano;
            GameEvents.onTickLogic += RestartEvent;
        }
    }
    // 1 раз показ панельки голода
}
