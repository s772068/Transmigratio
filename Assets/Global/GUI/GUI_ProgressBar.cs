using UnityEngine.UI;
using UnityEngine;

public class GUI_ProgressBar : MonoBehaviour {
    const float MAX_PERCENT = 100f;

    [SerializeField] private Image fill;

    public float Value { set => fill.fillAmount = value / MAX_PERCENT; }
}
