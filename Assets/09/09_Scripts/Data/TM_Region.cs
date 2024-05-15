using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TM_Region {
    public int id; //айдишник региона, соотвествует wmsk id
    public string name;
    public Population population;

    public EcologyParam flora;
    public EcologyParam fauna;
    public EcologyParam climate;
    public EcologyParam terrain;
    List<EcologyParam> ecologyParams = new List<EcologyParam>();

    public List<int> civsList = new();
    
    private int civMainIndex;
    private Civilization GetCiv(int index) => Transmigratio.Instance.GetCiv(index);
    public Civilization CivMain => Transmigratio.Instance.GetCiv(civMainIndex);

    public void Init() {
        ecologyParams.Clear();
        ecologyParams.Add(flora);
        ecologyParams.Add(fauna);
        ecologyParams.Add(climate);
        ecologyParams.Add(terrain);

        foreach (EcologyParam param in ecologyParams) { param.Init(); }
    }

    /// <summary>
    /// Обновление и пересчет всех значений региона
    /// </summary>
    public void RefreshRegion() {
        foreach (EcologyParam param in ecologyParams) { param.RefreshParam(); }
    }

    public void RefreshCiv() {
        Dictionary<int, int> dic = new();

        Civilization civ;
        CivPiece piece;

        for (int i = 0; i < civsList.Count; ++i) {
            civ = GetCiv(civsList[i]);
            for (int j = 0; j < civ.civPiecesList.Count; ++i) {
                piece = civ.civPiecesList[j];
                if (piece.regionResidenceIndex == id) {
                    dic[civ.civIndex] = piece.population.Value;
                }
            }
        }

        civMainIndex = dic.FirstOrDefault(x => x.Value == dic.Values.Max()).Key;
    }
}