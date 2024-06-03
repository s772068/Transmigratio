using WorldMapStrategyKit;

[System.Serializable]
public class MigrationData {
    public LineMarkerAnimator line;
    public IconMarker marker;
    public Civilization civilization;
    public TM_Region from;
    public TM_Region to;
    public int stepPopulations;
    public int curPopulations;
    public int fullPopulations;
    public int timerToStart;
    public int timerInterval;
}
