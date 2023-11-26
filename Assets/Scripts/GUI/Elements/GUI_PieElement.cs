using UnityEngine.UI;
using UnityEngine;

public class GUI_PieElement : MonoBehaviour {
    [SerializeField] private Image img;

    public Color Color { set => img.color = value; }
    public float FillAmount { set => img.fillAmount = value; }

    public void DestroyGO() {
        Destroy(gameObject);
    }
}
