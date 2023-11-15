using UnityEngine.UI;
using UnityEngine;

public class GUI_Legend : MonoBehaviour {
    [SerializeField] private Image img;
    [SerializeField] private Text label;
    [SerializeField] private Text val;

    private int _value;

    public Color ImageColor { set => img.color = value; }
    public string Label { set => label.text = value; }
    public int Value {
        get => _value;
        set {
            _value = value;
            val.text = value.ToString();
        }
    }

    public void DestroyGO() {
        Destroy(gameObject);
    }
}
