using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TM_Region {
    public int id; //айдишник региона, соотвествует wmsk id
    public string name;
    public float Population {
        get {
            float res = 0;
            for(int i = 0; i < civsList.Count; ++i) {
                res += Transmigratio.Instance.GetCivPice(id, civsList[i]).population.value;
            }
            return res;
        }
    }

    private int mainCivIndex;

    public Paramiter flora;
    public Paramiter fauna;
    public Paramiter climate;
    public Paramiter terrain;

    public List<string> civsList = new();

    [HideInInspector] public IconMarker marker;
    
    public Civilization CivMain => Transmigratio.Instance.GetCiv(GetMainCiv());
    private CivPiece GetPiece(string civName) => Transmigratio.Instance.GetCivPice(id, civName);

    private float TakenFood { get {
            float res = 0;
            for(int i = 0; i < civsList.Count; ++i) {
                res += GetPiece(civsList[i]).takenFood;
            }
            return res;
        }
    }

    public void AddCivilization(string civName) {
        civsList.Add(civName);
    }

    public Dictionary<string, int> GetCivParamiter() {
        Dictionary<string, int> res = new();
        
        for (int i = 0; i < civsList.Count; ++i) {
            res[civsList[i]] = GetPiece(civsList[i]).population.value;
        }

        return res;
    }

    public string GetMainCiv() {
        if (civsList.Count == 0) return "";

        Dictionary<string, int> dic = new();

        for (int i = 0; i < civsList.Count; ++i) {
            dic[civsList[i]] = GetPiece(civsList[i]).population.value;
        }

        return dic.FirstOrDefault(x => x.Value == dic.Values.Max()).Key;
    }

    public void Init() {
        GameEvents.onTickLogic += UpdateFlauna;
    }

    private void UpdateFlauna() {
        fauna["Fauna"].value = Mathf.Min(
            (int) (fauna["Fauna"].value - TakenFood / 10f + (fauna["Fauna"].value == 0 ? 1 : (50 / fauna["Fauna"].value))),
            fauna["Fauna"].max
        );
    }
}