using System.Collections.Generic;

[System.Serializable]
public struct S_Country {
    public int population;
    public int eventChanceIndex;
    public S_Goverment goverment;
    public List<string> names;
    public List<int> neighbours;
    public List<int> events;
}
