using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

[RequireComponent(typeof(Button))]
public class GUI_Param : MonoBehaviour {
    [SerializeField] private Text label;
    [SerializeField] private Text valTxt;
    [SerializeField] private GUI_ProgressBar progress;

    private Button button;
    private List<string> values = new();
    private E_Param type;
    private int val;
    public Action<int> OnClick;

    public int Index { private get; set; }
    /// <summary>
    /// ToDo
    /// </summary>
    public E_Param Type {
        private get => type;
        set {
            label.text = ""; // Init label
            Metric = ""; // Init Metric
            values.Clear();
            for(int i = 0; i < 0 /* Count Values */; ++i) {
                values.Add(""); // Add
                // Init Values
            }
            Value = val;
            progress.Range = new Vector2(0, values.Count);
        }
    }
    public string Label { set => label.text = value; }
    public string Metric { private get; set; }
    public bool IsClickable {
        set {
            if (value) button.onClick.AddListener(Click);
            else button.onClick.RemoveListener(Click);
        }
    }

    public string[] Values {
        set {
            for (int i = 0; i < value.Length; ++i) {
                values.Add(value[i]);
            }
        }
    }

    public int Value {
        set {
            if(value == 62) print("62");
            val = value;
            progress.Value = value;
            valTxt.text = (values.Count == 0 ?
                          val.ToString() :
                          values[val]) +
                          Metric;
        }
    }

    public void Init(Data data) {
        progress.Init(data.Progress);
        Label = data.Label;
        Metric = data.Metric;
        Value = data.Value;
        IsClickable = data.IsClickable;
    }

    public void Click() => OnClick?.Invoke(Index);

    public void DestroyGO() {
        Destroy(gameObject);
    }

    private void Awake() {
        button = GetComponent<Button>();
    }

    private void OnDestroy() {
        button = null;
        progress.DestroyGO();
    }

    [Serializable]
    public struct Data {
        public bool IsClickable;
        public int Value;
        public string Label;
        public string Metric;
        public GUI_ProgressBar.Data Progress;
    }
}
