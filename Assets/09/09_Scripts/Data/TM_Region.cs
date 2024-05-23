using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TM_Region {
    public int id; //айдишник региона, соотвествует wmsk id
    public string name;
    public Population population = new();

    private int mainCivIndex;

    public Paramiter flora;
    public Paramiter fauna;
    public Paramiter climate;
    public Paramiter terrain;

    public List<int> civsList = new();
    private Civilization GetCiv(int index) => Transmigratio.Instance.GetCiv(index);
    public Civilization CivMain { get => Transmigratio.Instance.GetCiv(GetMainCiv()); }

    public Dictionary<string, int> GetCivParamiter() {
        Dictionary<string, int> res = new();
        Civilization civ;

        for (int i = 0; i < civsList.Count; ++i) {
            civ = GetCiv(civsList[i]);
            res[civ.name] = civ.Population;
        }

        return res;
    }

    public int GetMainCiv() {
        if (civsList.Count == 0) return -1;

        Dictionary<int, int> dic = new();

        Civilization civ;
        CivPiece piece;

        for (int i = 0; i < civsList.Count; ++i) {
            civ = GetCiv(civsList[i]);
            for (int j = 0; j < civ.civPiecesList.Count; ++j) {
                piece = civ.civPiecesList[j];
                if (piece.regionResidenceIndex == id) {
                    dic[civ.civIndex] = piece.population.Value;
                }
            }
        }

        return dic.FirstOrDefault(x => x.Value == dic.Values.Max()).Key;
    }
}