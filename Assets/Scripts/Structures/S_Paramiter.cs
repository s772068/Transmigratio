using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public struct S_Paramiter : IEnumerator<int> {
    public string Name;
    public S_Value<int>[] details;
    public SerializedDictionary<string, int> detailsNamesIndexes;

    private int _index;

    public int this[int detailIndex] {
        get => details[detailIndex].Value;
        set => details[detailIndex].Value = value;
    }

    public int this[string DetailName] {
        get => details[detailsNamesIndexes[DetailName]].Value;
        set => details[detailsNamesIndexes[DetailName]].Value = value;
    }

    public int[] Details {
        get {
            int[] arr = new int[details.Length];
            for (int i = 0; i < arr.Length; ++i) {
                arr[i] = details[i].Value;
            }
            return arr;
        }
        //set {
        //    details = new int[value.Length];
        //    for (int i = 0; i < value.Length; ++i) {
        //        details[i] = value[i];
        //    }
        //}
    }

    public int MaxIndex {
        get {
            int index = -1;
            int max = -1;
            for (int i = 0; i < Details.Length; ++i) {
                if (details[i].Value > max) {
                    index = i;
                    max = details[i].Value;
                }
            }
            return index;
        }
    }
    public int MaxValue {
        get {
            int max = -1;
            for (int i = 0; i < Details.Length; ++i) {
                if (details[i].Value > max) {
                    max = details[i].Value;
                }
            }
            return max;
        }
    }

    public void Add(string name, int value) {
        detailsNamesIndexes[name] = details.Length;
        details.Add(new() { Name = name, Value = value });
    }

    public void Remove(string name) {
        details.Remove(detailsNamesIndexes[name]);
        detailsNamesIndexes[name] = details.Length;
    }

    public void Clear() {
        detailsNamesIndexes.Clear();
        details.Clear();
    }

    public int Current => details[_index].Value;

    object IEnumerator.Current => Current;

    public bool MoveNext() {
        ++_index;
        return _index < details.Length;
    }

    public void Reset() => _index = -1;

    public void Dispose() { }
}
