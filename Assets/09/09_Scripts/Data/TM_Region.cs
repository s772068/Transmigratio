using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TM_Region {
    public int Id; //айдишник региона, соотвествует wmsk id
    public string Name;
    public float Population {
        get {
            float res = 0;
            for(int i = 0; i < CivsList.Count; ++i) {
                res += Transmigratio.Instance.GetCivPice(Id, CivsList[i]).Population.value;
            }
            return res;
        }
    }

    private int _mainCivIndex;

    public Paramiter Flora;
    public Paramiter Fauna;
    public Paramiter Climate;
    public Paramiter Terrain;

    public List<string> CivsList = new();

    [HideInInspector] public IconMarker Marker;
    
    public Civilization CivMain => Transmigratio.Instance.GetCiv(GetMainCiv());
    private CivPiece GetPiece(string civName) => Transmigratio.Instance.GetCivPice(Id, civName);

    private float TakenFood { get {
            float res = 0;
            for(int i = 0; i < CivsList.Count; ++i) {
                res += GetPiece(CivsList[i]).TakenFood;
            }
            return res;
        }
    }

    public void AddCivilization(string civName) {
        CivsList.Add(civName);
    }

    public Dictionary<string, int> GetCivParamiter() {
        Dictionary<string, int> res = new();
        
        for (int i = 0; i < CivsList.Count; ++i) {
            res[CivsList[i]] = GetPiece(CivsList[i]).Population.value;
        }

        return res;
    }

    public string GetMainCiv() {
        if (CivsList.Count == 0) return "";

        Dictionary<string, int> dic = new();

        for (int i = 0; i < CivsList.Count; ++i) {
            dic[CivsList[i]] = GetPiece(CivsList[i]).Population.value;
        }

        return dic.FirstOrDefault(x => x.Value == dic.Values.Max()).Key;
    }

    public void Init() {
        GameEvents.TickLogic += UpdateFauna;
    }

    private void UpdateFauna() {
        Fauna["Fauna"].Value = Mathf.Min(
            (int) (Fauna["Fauna"].Value - TakenFood / 10f + (Fauna["Fauna"].Value == 0 ? 1 : (50 / Fauna["Fauna"].Value))),
            Fauna["Fauna"].Max
        );
    }
}