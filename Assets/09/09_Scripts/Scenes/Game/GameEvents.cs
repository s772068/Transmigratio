using System;
using WorldMapStrategyKit;

public static class GameEvents {
    public static Action TickLogic;
    public static Action TickShow;
    /// <summary>
    /// message
    /// </summary>
    public static Action<string> ShowMessage;

    public static Action<CivPiece> ActivateHunger;
    public static Action<CivPiece> DeactivateHunger;
    public static Action<CivPiece> RemoveCivPiece;

    // Marker
    public static Action<MarkerClickHandler, int> MarkerMouseDown;
    public static Action<MarkerClickHandler> MarkerEnter;
    public static Action<MarkerClickHandler> MarkerExit;

}
