using UnityEngine.UI;
using UnityEngine;

public class GUI_ProgressBar : MonoBehaviour {
    [SerializeField] private Image fill;

    private Vector3 scale = Vector3.one;

    public float Fill { set {
            scale.x = value;
            fill.rectTransform.localScale = scale;
        }
    } 
}
