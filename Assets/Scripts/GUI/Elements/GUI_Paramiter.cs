using UnityEngine.UI;
using UnityEngine;

public class GUI_Paramiter : MonoBehaviour {
    [SerializeField] private Image fill;
    [SerializeField] private Text label;
    [SerializeField] private Text val;
    [SerializeField] private bool isHideble;

    private Vector3 scale = new Vector3(1, 1, 1);

    public bool IsHideble => isHideble;

    public string Label { set => label.text = value; }
    public string Value { set => val.text = value; }
    
    public float Fill {
        set {
            scale.x = value;
            fill.rectTransform.localScale = scale;
        }
    }

    public void DestroyGO() {
        Destroy(gameObject);
    }
}
