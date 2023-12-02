using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public struct S_Paramiter : IEnumerator<int> {
    public int[] details;
    public SerializedDictionary<string, int> detailsNamesIndexes;

    private int _index;

    public int this[int detailIndex] {
        get => details[detailIndex];
        set => details[detailIndex] = value;
    }

    public int this[string DetailName] {
        get => details[detailsNamesIndexes[DetailName]];
        set => details[detailsNamesIndexes[DetailName]] = value;
    }

    public int[] Details {
        get {
            int[] arr = new int[details.Length];
            for (int i = 0; i < arr.Length; ++i) {
                arr[i] = details[i];
            }
            return arr;
        }
        set {
            details = new int[value.Length];
            for (int i = 0; i < value.Length; ++i) {
                details[i] = value[i];
            }
        }
    }

    public int MaxIndex {
        get {
            int index = -1;
            int max = -1;
            for (int i = 0; i < Details.Length; ++i) {
                if (details[i] > max) {
                    index = i;
                    max = details[i];
                }
            }
            return index;
        }
    }
    public int MaxValue {
        get {
            int max = -1;
            for (int i = 0; i < Details.Length; ++i) {
                if (details[i] > max) {
                    max = details[i];
                }
            }
            return max;
        }
    }

    public void Add(int value) {
        detailsNamesIndexes[details.Length.ToString()] = details.Length;
        details.Add(value);
    }

    public void Add(string name, int value) {
        detailsNamesIndexes[name] = details.Length;
        details.Add(value);
    }

    public void Remove(int index) {
        detailsNamesIndexes.Remove(index.ToString());
        details.Remove(index);
    }

    public void Remove(string name) {
        details.Remove(detailsNamesIndexes[name]);
        detailsNamesIndexes[name] = details.Length;
    }

    public void Clear() {
        detailsNamesIndexes.Clear();
        details.Clear();
    }

    public int Current => details[_index];

    object IEnumerator.Current => Current;

    public bool MoveNext() {
        ++_index;
        return _index < Details.Length;
    }

    public void Reset() => _index = -1;

    public void Dispose() { }
}
