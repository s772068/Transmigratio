using System;

public abstract class BaseEvent {
    public abstract void Use(ref S_Country country, int index);
    public abstract S_Event Data { get; }
    public abstract int CountRes { get; }
}
