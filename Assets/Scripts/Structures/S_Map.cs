using System.Linq;

[System.Serializable]
public struct S_Map {
    public S_Value<int[]>[] Civilizations;
    public S_Region[] Regions;

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

    public int AllCivilizationVlaues(int paramiterIndex) {
        int all = 0;
        for (int i = 0; i < Regions.Length; ++i) {
            all += Regions[i].AllCivilizationVlaues(paramiterIndex);
        }
        return all;
    }

    public int AllEcologyVlaues(int paramiterIndex) {
        int all = 0;
        for (int i = 0; i < Regions.Length; ++i) {
            all += Regions[i].Ecology[paramiterIndex].AllVlaues;
        }
        return all;
    }
}
