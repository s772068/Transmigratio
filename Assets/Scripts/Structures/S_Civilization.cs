// Paramiters
// 0: ProdMode
// 1: Economics
// 2: Governments

using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public struct S_Civilization : IEnumerator<S_Paramiter> {
    public int Stage;
    public int Population;
    public int TakenFood;
    public S_Paramiter[] Paramiters;
    public SerializedDictionary<string, int> ParamitersNameIndexes;

    private int _index;

    public int this[params string[] val] {
        get => val[0] switch {
            "Stage" => Stage,
            "Population" => Population,
            "TakenFood" => TakenFood,
            "Paramiters" => Paramiters[ParamitersNameIndexes[val[1]]][val[2]],
            _ => 0
        };
        set {
            switch (val[0]) {
                case "Stage": Stage = value; break;
                case "Population": Population = value; break;
                case "TakenFood": TakenFood = value; break;
                case "Paramiters": Paramiters[ParamitersNameIndexes[val[1]]] [val[2]] = value; break;
                default: return;
            }
        }  
    }
    
    public int this[params int[] val] {
        get => val[0] switch {
            0 => Stage,
            1 => Population,
            2 => TakenFood,
            3 => Paramiters[val[1]][val[2]],
            _ => 0
        };
        set {
            switch (val[0]) {
                case 0: Stage = value; break;
                case 1: Population = value; break;
                case 2: TakenFood = value; break;
                case 3: Paramiters[val[1]][val[2]] = value; break;
                default: return;
            }
        }
    }

    public void Add(S_Paramiter value) {
        ParamitersNameIndexes[Paramiters.Length.ToString()] = Paramiters.Length;
        Paramiters.Add(value);
    }

    public void Add(string name, S_Paramiter value) {
        ParamitersNameIndexes[name] = Paramiters.Length;
        Paramiters.Add(value);
    }

    public void Remove(int index) {
        ParamitersNameIndexes.Remove(index.ToString());
        Paramiters.Remove(index);
    }

    public void Remove(string name) {
        Paramiters.Remove(ParamitersNameIndexes[name]);
        ParamitersNameIndexes[name] = Paramiters.Length;
    }

    public void Clear() {
        ParamitersNameIndexes.Clear();
        Paramiters.Clear();
    }

    public S_Paramiter Current => Paramiters[_index];

    object IEnumerator.Current => Current;

    public bool MoveNext() {
        ++_index;
        return _index < Paramiters.Length;
    }

    public void Reset() => _index = -1;

    public void Dispose() { }
}
