using UnityEngine.UI;
using UnityEngine;
using System;

public class GUI_Legend : MonoBehaviour {
    [SerializeField] private Image img;
    [SerializeField] private Text label;
    [SerializeField] private Text val;

    private int _index;
    private int _value;

    public Action<int> OnClick;

    public int Index { set => _index = value; }
    public Color ImageColor { set => img.color = value; }
    public string Label { set => label.text = value; }
    public int Value {
        get => _value;
        set {
            _value = value;
            val.text = value.ToString();
        }
    }

    public void Click() {
        OnClick?.Invoke(_index);
    }

    public void DestroyGO() {
        Destroy(gameObject);
    }
}
