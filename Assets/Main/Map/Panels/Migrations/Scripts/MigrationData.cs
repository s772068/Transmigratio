using WorldMapStrategyKit;

[System.Serializable]
public class MigrationData {
    public S_Civilization Step;
    public int Percent;
    public int CivID;
    public int From;
    public int To;
    public LineMarkerAnimator Line;
    public IconMarker Marker;
}
