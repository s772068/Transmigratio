using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TM_Region {
    public int id; //айдишник региона, соотвествует wmsk id
    public string name;
    public float Population => civsList.Sum(x => x.pieces[id].population.value);

    private int mainCivIndex;

    public Paramiter flora;
    public Paramiter fauna;
    public Paramiter climate;
    public Paramiter terrain;

    public List<Civilization> civsList = new();
    public Civilization CivMain { get => Transmigratio.Instance.GetCiv(GetMainCiv()); }

    public Dictionary<string, int> GetCivParamiter() {
        Dictionary<string, int> res = new();
        
        for (int i = 0; i < civsList.Count; ++i) {
            res[civsList[i].name] = civsList[i].Population;
        }

        return res;
    }

    public string GetMainCiv() {
        if (civsList.Count == 0) return "";

        Dictionary<string, int> dic = new();

        for (int i = 0; i < civsList.Count; ++i) {
            dic[civsList[i].name] = civsList[i].pieces[id].population.value;
        }

        return dic.FirstOrDefault(x => x.Value == dic.Values.Max()).Key;
    }
}