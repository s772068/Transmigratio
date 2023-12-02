using UnityEngine.UI;
using UnityEngine;

public class GUI_ProgressBar : MonoBehaviour {
    [SerializeField] private Image background;
    [SerializeField] private Image fill;
    [SerializeField] private Color color;
    [SerializeField] private float maxValue = 1f;
    [SerializeField] private bool isSlider;

    public float MinValue { private get; set; }
    public float MaxValue {
        get => maxValue;
        set => maxValue = value;
    }

    public float Value {
        set {
            Color = color;
            if (isSlider) fill.fillAmount = (value - MinValue) / (maxValue - MinValue);
            else fill.fillAmount = 1;
        }
    }

    public Color Color {
        set {
            color = value;
            fill.color = value;
        }
    }
}
