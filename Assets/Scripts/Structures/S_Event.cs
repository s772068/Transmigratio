using System.Collections.Generic;

[System.Serializable]
public struct S_Event {
    public string Name;
    public string Description;
    public int MarkerIndex;
    public int IconIndex;
    public S_EventResult[] Results;
}
