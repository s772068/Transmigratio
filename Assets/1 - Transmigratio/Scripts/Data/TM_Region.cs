using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TM_Region {
    public int Id; //айдишник региона, соотвествует wmsk id
    public string Name;
    public float Population {
        get {
            float res = 0;
            for(int i = 0; i < CivsList.Count; ++i) {
                res += Transmigratio.Instance.GetCivPice(Id, CivsList[i]).Population.Value;
            }
            return res;
        }
    }

    public float PopulationGrow {
        get {
            float res = 0;
            for (int i = 0; i < CivsList.Count; ++i)
            {
                res += Transmigratio.Instance.GetCivPice(Id, CivsList[i]).PopulationGrow.value;
            }
            return res;
        }
    }

    public List<CivPiece> GetPieces {
        get {
            List<CivPiece> pieces = new();
            foreach (var civ in  CivsList) {
                pieces.Add(Transmigratio.Instance.GetCivPice(Id, civ));
            }

            return pieces;
        }
    }
    private int _mainCivIndex;

    public Paramiter Flora;
    public Paramiter Fauna;
    public Paramiter Climate;
    public Paramiter Terrain;

    public List<string> CivsList = new();

    public IconMarker Marker;
    
    public Civilization CivMain => Transmigratio.Instance.GetCiv(GetMainCiv());
    private CivPiece GetPiece(string civName) => Transmigratio.Instance.GetCivPice(Id, civName);

    private float TakenFood { get {
            float res = 0;
            for(int i = 0; i < CivsList.Count; ++i) {
                res += GetPiece(CivsList[i]).TakenFood.value;
            }
            return res;
        }
    }

    public void AddCivilization(string civName) {
        if (!CivsList.Contains(civName))
            CivsList.Add(civName);
    }

    public Dictionary<string, float> GetCivParamiter() {
        Dictionary<string, float> res = new();
        
        for (int i = 0; i < CivsList.Count; ++i) {
            res[CivsList[i]] = GetPiece(CivsList[i]).Population.Value;
        }

        return res;
    }

    public string GetMainCiv() {
        if (CivsList.Count == 0) return "";

        Dictionary<string, int> dic = new();

        for (int i = 0; i < CivsList.Count; ++i) {
            dic[CivsList[i]] = GetPiece(CivsList[i]).Population.Value;
        }

        return dic.FirstOrDefault(x => x.Value == dic.Values.Max()).Key;
    }

    //public void Init() {
    //    Timeline.TickLogic += UpdateFauna;
    //}

    //private void UpdateFauna() {
    //    Fauna["Fauna"].Value = Mathf.Min(
    //        (int) (Fauna["Fauna"].Value - TakenFood / 10f + (Fauna["Fauna"].Value == 0 ? 1 : (50 / Fauna["Fauna"].Value))),
    //        Fauna["Fauna"].StartValue
    //    );
    //}
}