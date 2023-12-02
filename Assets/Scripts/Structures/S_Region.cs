// Ecology
// 0: Terrain
// 1: Climat
// 2: Flora
// 3: Fauna

using AYellowpaper.SerializedCollections;
using UnityEngine;

[System.Serializable]
public struct S_Region {
    public string Name;
    public Color Color;
    public int EventChanceIndex;
    public int TakenFood;
    public int[] Events;
    public int[] Neighbours;
    public S_Paramiter[] Ecology;
    public S_Civilization[] Civilizations;
    public SerializedDictionary<string, int> EventsNameIndexes;
    public SerializedDictionary<string, int> NeighboursNameIndexes;
    public SerializedDictionary<string, int> EcologyNameIndexes;
    public SerializedDictionary<string, int> CivilizationsNameIndexes;

    public int this[params string[] val] {
        get {
            switch (val[0]) {
                case "EventChanceIndex": return EventChanceIndex;
                case "TakenFood": return TakenFood;
                case "Events": return Events[EventsNameIndexes[val[1]]];
                case "Neighbours": return Neighbours[NeighboursNameIndexes[val[1]]];
                case "Ecology": return Ecology[EcologyNameIndexes[val[1]]] [val[2]];
                case "Civilizations": return Civilizations[CivilizationsNameIndexes[val[1]]] [val[2], val[3], val[4]];
                default: return 0;
            }
        }
        set {
            switch (val[0]) {
                case "EventChanceIndex": EventChanceIndex = value; break;
                case "TakenFood": TakenFood = value; break;
                case "Events": Events[EventsNameIndexes[val[1]]] = value; break;
                case "Neighbours": Neighbours[NeighboursNameIndexes[val[1]]] = value; break;
                case "Ecology": Ecology[EcologyNameIndexes[val[1]]] [val[2]] = value; break;
                case "Civilizations": Civilizations[CivilizationsNameIndexes[val[1]]] [val[2], val[3], val[4]] = value; break;
                default: return;
            }
        }
    }

    public int this[params int[] val] {
        get { // 5
            switch (val[0]) {
                case 0: return EventChanceIndex;
                case 1: return TakenFood;
                case 2: return Events[val[1]];
                case 3: return Neighbours[val[1]];
                case 4: return Ecology[val[1]][val[2]];
                case 5: return Civilizations[val[1]][val[2], val[3], val[4]];
                default: return 0;
            }
        }
        set {
            switch (val[0]) {
                case 0: EventChanceIndex = value; break;
                case 1: TakenFood = value; break;
                case 2: Events[val[1]] = value; break;
                case 3: Neighbours[val[1]] = value; break;
                case 4: Ecology[val[1]][val[2]] = value; break;
                case 5: Civilizations[val[1]][val[2], val[3], val[4]] = value; break;
                default: return;
            }
        }
    }

    public int MaxCivilizationStage {
        get {
            int stage = -1;
            int max = -1;
            for (int i = 0; i < Civilizations.Length; ++i) {
                if (i >= Civilizations.Length) return MaxCivilizationStage;
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
            for(int i = 0; i < Civilizations.Length; ++i) {
                if (i >= Civilizations.Length) return MaxPopulationsIndex;
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
            for (int i = 0; i < Civilizations.Length; ++i) {
                if (i >= Civilizations.Length) return MaxPopulationsValue;
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
            for(int i = 0; i < Civilizations.Length; ++i) {
                if (i >= Civilizations.Length) return AllPopulations;
                all += Civilizations[i].Population;
            }
            return all;
        }
    }

    public int[] ArrayPopulation {
        get {
            int[] arr = new int[Civilizations.Length];
            for(int i = 0; i < arr.Length; ++i) {
                if (i >= arr.Length) return ArrayPopulation;
                arr[i] = Civilizations[i].Population;
            }
            return arr;
        }
    }

    public int StageToIndex(int stage) {
        int index = -1;
        for (int i = 0; i < Civilizations.Length; ++i) {
            if (Civilizations[i].Stage == stage) {
                index = i; break;
            }
        }
        return index;
    }

    public int[] ArrayCivilizationParamiters(int paramiterIndex) {
        if(Civilizations.Length < 0) return new int[0];
        int[] arr = new int[Civilizations[0].Paramiters[paramiterIndex].Details.Length];
        for (int i = 0; i < Civilizations.Length; ++i) {
            if (i >= Civilizations.Length) ArrayCivilizationParamiters(paramiterIndex);
            for (int j = 0; j < Civilizations[i].Paramiters[paramiterIndex].Details.Length; ++j) {
                arr[j] += Civilizations[i].Paramiters[paramiterIndex][j];
            }
        }
        return arr;
    }

    public int MaxCivilizationIndex(int paramiterIndex) {
        int max = -1;
        int index = -1;
        int value;
        for(int i = 0; i < Civilizations.Length; ++i) {
            if (i >= Civilizations.Length) MaxCivilizationIndex(paramiterIndex);
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
        for (int i = 0; i < Civilizations.Length; ++i) {
            if (i >= Civilizations.Length) MaxCivilizationValue(paramiterIndex);
            value = Civilizations[i].Paramiters[paramiterIndex].MaxValue;
            if (value > max) {
                max = value;
            }
        }
        return max;
    }

    public int AllCivilizationVlaues(int paramiterIndex, int detailIndex) {
        int all = 0;
        for (int i = 0; i < Civilizations.Length; ++i) {
            if (i >= Civilizations.Length) AllCivilizationVlaues(paramiterIndex, detailIndex);
            all += Civilizations[i].Paramiters[paramiterIndex][detailIndex];
        }
        return all;
    }

    public void AddEvent(int value) {
        EventsNameIndexes[Events.Length.ToString()] = Events.Length;
        Events.Add(value);
    }

    public void AddEvent(string name, int value) {
        EventsNameIndexes[name] = Events.Length;
        Events.Add(value);
    }

    public void RemoveEvent(int index) {
        EventsNameIndexes.Remove(index.ToString());
        Events.Remove(index);
    }

    public void RemoveEvent(string name) {
        Events.Remove(EventsNameIndexes[name]);
        EventsNameIndexes[name] = Events.Length;
    }

    public void ClearEvent() {
        EventsNameIndexes.Clear();
        Events.Clear();
    }

    public void AddCivilization(S_Civilization value) {
        CivilizationsNameIndexes[Civilizations.Length.ToString()] = Civilizations.Length;
        Civilizations.Add(value);
    }

    public void AddCivilization(string name, S_Civilization value) {
        CivilizationsNameIndexes[name] = Civilizations.Length;
        Civilizations.Add(value);
    }

    public void RemoveCivilization(int index) {
        CivilizationsNameIndexes.Remove(index.ToString());
        Civilizations.Remove(index);
    }

    public void RemoveCivilization(string name) {
        Civilizations.Remove(CivilizationsNameIndexes[name]);
        CivilizationsNameIndexes[name] = Civilizations.Length;
    }

    public void ClearCivilization() {
        CivilizationsNameIndexes.Clear();
        Civilizations.Clear();
    }

    public void ClearAll() {
        CivilizationsNameIndexes.Clear();
        Civilizations.Clear();
    }
}
