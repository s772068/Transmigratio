using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldMapStrategyKit;

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
    public List<CivPiece> civPiecesList = new();
    
    public Civilization CivMain => Transmigratio.Instance.GetCiv(GetMainCiv());

    private float TakenFood { get {
            float res = 0;
            for(int i = 0; i < civPiecesList.Count; ++i) {
                res += civPiecesList[i].takenFood;
            }
            return res;
        }
    }

    public void AddCivilization(Civilization civilization) {
        civsList.Add(civilization);
        civPiecesList.Add(civilization.pieces[id]);
    }

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

    public void Init() {
        GameEvents.onTickLogic += UpdateFlauna;
    }

    private void UpdateFlauna() {
        fauna["fauna"].value = Mathf.Min(
            (int) (fauna["fauna"].value - TakenFood / 10f + (fauna["fauna"].value == 0 ? 1 : (50 / fauna["fauna"].value))),
            fauna["fauna"].max
        );
    }
}