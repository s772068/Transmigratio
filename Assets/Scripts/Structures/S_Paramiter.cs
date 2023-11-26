[System.Serializable]
public struct S_Paramiter {
    public string Name;
    public int[] Values;

    public int MaxIndex {
        get {
            int index = -1;
            int max = -1;
            for (int i = 0; i < Values.Length; ++i) {
                if (Values[i] > max) {
                    index = i;
                    max = Values[i];
                }
            }
            return index;
        }
    }
    public int MaxValue {
        get {
            int max = -1;
            for (int i = 0; i < Values.Length; ++i) {
                if (Values[i] > max) {
                    max = Values[i];
                }
            }
            return max;
        }
    }
    public int AllVlaues {
        get {
            int all = 0;
            for (int i = 0; i < Values.Length; ++i) {
                all += Values[i];
            }
            return all;
        }
    }
}
