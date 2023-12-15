public abstract class BaseEvent {
    public abstract bool CheckActivate(ref S_Region region);
    public abstract bool CheckBuild(int countryIndex, int resultIndex);
    public abstract void Use(int countryIndex, int index);
    // public abstract S_Event Data(int localization);
}
