using System.Collections.Generic;
using WorldMapStrategyKit;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;

public class VolcanoController : Singleton<VolcanoController> {
    [SerializeField] private EventDesidion _desidionPrefab;
    [SerializeField] private EventPanel _panelPrefab;
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
    [Header("Colors")]
    [SerializeField] private Color regionColor;
    [SerializeField] private Color civColor;

    [Header("OnlyForView")]
    [SerializeField] private int ticker;
    [SerializeField] private int activateIndex;
    [SerializeField] private int ticksToActivateVolcano;

    public bool IsShowAgain = true;
    private CivPiece piece;
    private IconMarker marker;
    private System.Random rand = new();
    private List<Action> autoActions = new();

    private Action onTickLogic;

    private WMSK WMSK => Transmigratio.Instance.TMDB.map.WMSK;

    private string Local(string key) => Localization.Load("Volcano", key);
    private int GetCalmVolcanoPoints(CivPiece piece) => (int) (piece.Population.value / calmVolcanoPointsDivision);

    private void OnEnable() {
        GameEvents.TickLogic += StartCreateEvent;
        GameEvents.RemoveCivPiece += RestartEvent;

        autoActions.Add(CalmVolcano);
        autoActions.Add(ReduceLosses);
        autoActions.Add(ActivateVolcano);
    }

    private void OnDisable() {
        GameEvents.TickLogic -= StartCreateEvent;
        GameEvents.RemoveCivPiece -= RestartEvent;

        autoActions.Clear();
    }

    private void StartCreateEvent() {
        ++ticker;
        if (ticker == startTicksToActivate) {
            ticker = 0;
            GameEvents.TickLogic -= StartCreateEvent;
            CreateEvent();
        }
    }

    private void CreateEvent() {
        System.Random rand = new();
        var civilizations = Transmigratio.Instance.TMDB.humanity.Civilizations;
        if (civilizations.Count == 0) return;
        piece = civilizations.ElementAt(rand.Next(0, civilizations.Count)).Value.Pieces.ElementAt(rand.Next(0, civilizations.Count)).Value;
        ticksToActivateVolcano = rand.Next(minTickToActivate, maxTickToActivate);
        GameEvents.TickLogic += WaitActivateVolcano;
        Debug.Log($"Create event Volcano in region {piece.Region.Id}");
        CreateMarker(WMSK.countries[piece.Region.Id].center);
        if (IsShowAgain)
            OpenPanel();
    }

    private void RestartEvent(CivPiece _piece) {
        if (piece == _piece) RestartEvent();
    }

    private void RestartEvent() {
        Debug.Log("RestartEvent");
        ++ticker;
        if (ticker == ticksToActivateVolcano) {
            GameEvents.TickLogic -= RestartEvent;
            ticker = 0;
            CreateEvent();
        }
    }

    private void CreateMarker(Vector3 position) {
        marker = Instantiate(markerPrefab);
        marker.Sprite = markerSprite;
        marker.onClick += (int i) => { OpenPanel(); _panelPrefab.IsShowAgain = IsShowAgain; };
        position.z = -0.1f;

        MarkerClickHandler handler = WMSK.AddMarker2DSprite(marker.gameObject, position, 0.03f, true, true);
        handler.allowDrag = false;
    }

    private void OpenPanel() {
        string territory = Local("Territory1") + " " +
                          $"<color=#{regionColor.ToHexString()}>" +
                          piece.Region.Name + "</color> " +
                          Local("Territory2") + " " +
                          $"<color=#{civColor.ToHexString()}>" +
                          Localization.Load("Civilizations", piece.Civilization.Name) + "</color> " +
                          Local("Territory3");
        Desidion[] desidions =
        {
            new Desidion(CalmVolcano, Local("CalmVolcano"), GetCalmVolcanoPoints(piece)),
            new Desidion(ReduceLosses, Local("ReduceLosses"), reduceLossesPoints),
            new Desidion(Nothing, Local("Nothing"), 0)
        };

        PanelFabric.CreateEvent(HUD.Instance.Events, _desidionPrefab, _panelPrefab, IsShowAgain, panelSprite, Local("Title"),
                                territory, Local("Description"), desidions);
    }

    private void CalmVolcano() {
        activateIndex = 0;
        Debug.Log("CalmVolcano");

        ticker = 0;
        marker.Destroy();
        GameEvents.TickLogic += RestartEvent;
        GameEvents.TickLogic -= WaitActivateVolcano;
    }

    private void ReduceLosses() {
        activateIndex = 1;
        piece.Population.value -= (int)(piece.Population.value * fullPercentFood * partPercentPopulation);
        piece.ReserveFood -= piece.ReserveFood * fullPercentFood * partPercentFood;

        Debug.Log("ReduceLosses");

        ticker = 0;
        marker.Destroy();
        GameEvents.TickLogic += RestartEvent;
        GameEvents.TickLogic -= WaitActivateVolcano;
        MigrationController.Instance.TryMigration(piece);
    }

    private void Nothing() {
        activateIndex = 2;
    }

    private void ActivateVolcano() {
        piece.Population.value -= (int)(piece.Population.value * fullPercentFood);
        piece.ReserveFood -= piece.ReserveFood * fullPercentFood;

        Debug.Log("ActivateVolcano");

        ticker = 0;
        marker.Destroy();
        GameEvents.TickLogic -= WaitActivateVolcano;
        MigrationController.Instance.TryMigration(piece);
    }

    private void WaitActivateVolcano() {
        Debug.Log("WaitActivateVolcano");
        ++ticker;
        if (ticker == ticksToEruption) {
            ticker = 0;
            marker.Destroy();
            autoActions[activateIndex]?.Invoke();
            GameEvents.TickLogic += RestartEvent;
        }
    }
}
