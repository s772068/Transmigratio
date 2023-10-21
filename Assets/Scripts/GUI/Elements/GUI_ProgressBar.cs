using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class GUI_ProgressBar : MonoBehaviour {
    [SerializeField] private Image fill;

    private Image background;
    private Vector2 range;
    private float val;
    private Vector3 scale = new Vector3(1, 1, 1);

    public int Index { private get; set; }

    public float Value {
        set {
            val = Mathf.Clamp(value, range.x, range.y);
            UpdateValue();
        }
    }
    public Vector2 Range {
        set {
            range = value;
            UpdateValue();
        }
    }
    public Vector4 FillColor { set => fill.color = new Color().Parse(value); }
    public Vector4 BackgroundColor { set => background.color = new Color().Parse(value); }

    public void Init(Data data) {
        Value = data.Value;
        Range = data.Range;
        FillColor = data.FillColor;
        BackgroundColor = data.BackgroundColor;
    }

    private void UpdateValue() {
        scale.x = range.x == range.y ? 0 : ((val - range.x) / (range.y - range.x));
        fill.rectTransform.localScale = scale;
    }
    
    public void DestroyGO() {
        Destroy(gameObject);
    }

    private void Awake() {
        background = GetComponent<Image>();
    }

    private void OnDestroy() {
        background = null;
    }

    [System.Serializable]
    public struct Data {
        public int Value;
        public Vector2 Range;
        public Vector4 FillColor;
        public Vector4 BackgroundColor;
    }
}
