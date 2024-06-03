using System;
using WorldMapStrategyKit;

public static class GameEvents {
    public static Action onTickLogic;
    public static Action onTickShow;
    /// <summary>
    /// message
    /// </summary>
    public static Action<string> onShowMessage;
    public static Action<CivPiece> onUpdateDeltaPopulation;

    // WMSK
    // Map
    /// <summary>
    /// countryIndex, regionIndex, buttonIndex
    /// </summary>
    public static Action<int, int, int> onCountryClick;
    /// <summary>
    /// countryIndex, regionIndex, buttonIndex
    /// </summary>
    public static Action<int, int, int> onCountryLongClick;

    // Marker
    public static Action<MarkerClickHandler, int> onMarkerMouseDown;
    public static Action<MarkerClickHandler> onMarkerEnter;
    public static Action<MarkerClickHandler> onMarkerExit;
}
