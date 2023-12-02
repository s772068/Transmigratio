using AYellowpaper.SerializedCollections;

// Example: GetRegion => IndexRegion => GetCivilization => IndexCivilization => GetParamiter => IndexParamiter => IndexDetail
[System.Serializable]
public struct S_Map {
    public int[][] Civilizations;
    public S_Region[] Regions;
    public SerializedDictionary<string, int> CivilizationsIndexes;
    public SerializedDictionary<string, int> CivilizationsRegionsIndexes;
    public SerializedDictionary<string, int> RegionsNameIndexes;

    public int this[int regionIndex, params string[] val] {
        get {
            switch (val[0]) {
                case "Civilizations": return Civilizations[CivilizationsIndexes[val[1]]][CivilizationsIndexes[val[2]]];
                case "Regions": return Regions[regionIndex][val[1], val[2], val[3], val[4], val[5]];
                default: return 0;
            }
        }
        set {
            switch (val[0]) {
                case "Civilizations": Civilizations[CivilizationsIndexes[val[1]]][CivilizationsIndexes[val[2]]] = value; break;
                case "Regions": Regions[regionIndex][val[1], val[2], val[3], val[4], val[5]] = value; break;
                default: return;
            }
        }
    }

    public int this[params int[] val] {
        get {
            switch (val[0]) {
                case 0: return Civilizations[val[1]][val[2]];
                case 1: return Regions[val[1]][val[2], val[3], val[4], val[5], val[6]];
                default: return 0;
            }
        }
        set {
            switch (val[0]) {
                case 0: Civilizations[val[1]][val[2]] = value; break;
                case 1: Regions[val[1]][val[2], val[3], val[4], val[5], val[6]] = value; break;
                default: return;
            }
        }
    }

    public int MaxCivilizationStage {
        get {
            int stage = -1;
            int max = -1;
            for (int i = 0; i < Regions.Length; ++i) {
                if (Regions[i].MaxCivilizationStage > max) {
                    max = Regions[i].MaxCivilizationStage;
                    stage = i;
                }
            }
            return stage;
        }
    }

    public int MaxPopulationValue {
        get {
            int max = 0;
            for (int i = 0; i < Regions.Length; ++i) {
                if(max < Regions[i].AllPopulations) {
                    max = Regions[i].MaxPopulationsValue;
                }
            }
            return max;
        }
    }

    public int AllPopulations {
        get {
            int all = 0;
            for (int i = 0; i < Regions.Length; ++i) {
                all += Regions[i].AllPopulations;
            }
            return all;
        }
    }

    public int MaxEcologyIndex(int paramiterIndex) {
        int max = -1;
        int index = -1;
        int value;
        for (int i = 0; i < Regions.Length; ++i) {
            value = Regions[i].Ecology[paramiterIndex].MaxValue;
            if (value > max) {
                max = value;
                index = i;
            }
        }
        return index;
    }

    public int MaxCivilizationIndex(int paramiterIndex) {
        int max = -1;
        int index = -1;
        int value;
        for (int i = 0; i < Regions.Length; ++i) {
            value = Regions[i].MaxCivilizationValue(paramiterIndex);
            if (value > max) {
                max = value;
                index = i;
            }
        }
        return index;
    }

    public int MaxEcologyValue(int paramiterIndex) {
        int max = -1;
        int value;
        for (int i = 0; i < Regions.Length; ++i) {
            value = Regions[i].Ecology[paramiterIndex].MaxValue;
            if (value > max) {
                max = value;
            }
        }
        return max;
    }

    public int MaxCivilizationValue(int paramiterIndex) {
        int max = -1;
        int value;
        for (int i = 0; i < Regions.Length; ++i) {
            value = Regions[i].MaxCivilizationValue(paramiterIndex);
            if (value > max) {
                max = value;
            }
        }
        return max;
    }

    public int AllEcologyVlaues(int paramiterIndex, int detailIndex) {
        int all = 0;
        for (int i = 0; i < Regions.Length; ++i) {
            all += Regions[i].Ecology[paramiterIndex].Details[detailIndex];
        }
        return all;
    }

    public int AllCivilizationVlaues(int paramiterIndex, int detailIndex) {
        int all = 0;
        for (int i = 0; i < Regions.Length; ++i) {
            all += Regions[i].AllCivilizationVlaues(paramiterIndex, detailIndex);
        }
        return all;
    }
}
