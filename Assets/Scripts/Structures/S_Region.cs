// Ecology
// 0: Terrain
// 1: Climat
// 2: Flora
// 3: Fauna

using System.Collections.Generic;

[System.Serializable]
public struct S_Region {
    public string Name;
    public int EventChanceIndex;
    public int FreePeople;
    public UnityEngine.Color Color;
    public List<int> Events;
    public S_Paramiter[] Ecology;
    public List<S_Civilization> Civilizations;
    public int[] Neighbours;

    public int MaxCivilizationStage {
        get {
            int stage = -1;
            int max = -1;
            for (int i = 0; i < Civilizations.Count; ++i) {
                if (i >= Civilizations.Count) return MaxCivilizationStage;
                if (Civilizations[i].Stage > max) {
                    max = Civilizations[i].Stage;
                    stage = i;
                }
            }
            return stage;
        }
    }

    public int MaxPopulationsIndex {
        get {
            int index = -1;
            int max = -1;
            for(int i = 0; i < Civilizations.Count; ++i) {
                if (i >= Civilizations.Count) return MaxPopulationsIndex;
                if (Civilizations[i].Population > max) {
                    max = Civilizations[i].Population;
                    index = i;
                }
            }
            return index;
        }
    }

    public int MaxPopulationsValue {
        get {
            int max = 0;
            for (int i = 0; i < Civilizations.Count; ++i) {
                if (i >= Civilizations.Count) return MaxPopulationsValue;
                if (Civilizations[i].Population > max) {
                    max = Civilizations[i].Population;
                }
            }
            return max;
        }
    }

    public int AllPopulations {
        get {
            int all = 0;
            for(int i = 0; i < Civilizations.Count; ++i) {
                if (i >= Civilizations.Count) return AllPopulations;
                all += Civilizations[i].Population;
            }
            return all;
        }
    }

    public int[] ArrayPopulation {
        get {
            int[] arr = new int[Civilizations.Count];
            for(int i = 0; i < arr.Length; ++i) {
                if (i >= arr.Length) return ArrayPopulation;
                arr[i] = Civilizations[i].Population;
            }
            return arr;
        }
    }

    public int[] ArrayCivilizationParamiters(int paramiterIndex) {
        if(Civilizations.Count < 0) return new int[0];
        int[] arr = new int[Civilizations[0].Paramiters[paramiterIndex].Values.Length];
        for (int i = 0; i < Civilizations.Count; ++i) {
            if (i >= Civilizations.Count) ArrayCivilizationParamiters(paramiterIndex);
            for (int j = 0; j < Civilizations[i].Paramiters[paramiterIndex].Values.Length; ++j) {
                arr[j] += Civilizations[i].Paramiters[paramiterIndex].Values[j];
            }
        }
        return arr;
    }

    public int MaxCivilizationIndex(int paramiterIndex) {
        int max = -1;
        int index = -1;
        int value;
        for(int i = 0; i < Civilizations.Count; ++i) {
            if (i >= Civilizations.Count) MaxCivilizationIndex(paramiterIndex);
            value = Civilizations[i].Paramiters[paramiterIndex].MaxValue;
            if(value > max) {
                max = value;
                index = i;
            }
        }
        return index;
    }

    public int MaxCivilizationValue(int paramiterIndex) {
        int max = -1;
        int value;
        for (int i = 0; i < Civilizations.Count; ++i) {
            if (i >= Civilizations.Count) MaxCivilizationValue(paramiterIndex);
            value = Civilizations[i].Paramiters[paramiterIndex].MaxValue;
            if (value > max) {
                max = value;
            }
        }
        return max;
    }

    public int AllCivilizationVlaues(int paramiterIndex) {
        int all = 0;
        for (int i = 0; i < Civilizations.Count; ++i) {
            if (i >= Civilizations.Count) AllCivilizationVlaues(paramiterIndex);
            all += Civilizations[i].Paramiters[paramiterIndex].AllVlaues;
        }
        return all;
    }
}
