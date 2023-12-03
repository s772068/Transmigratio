using UnityEngine.UI;
using UnityEngine;

public class GUI_Paramiter : MonoBehaviour {
    [SerializeField] private GUI_ProgressBar progressBar;
    [SerializeField] private Image pictogram;
    [SerializeField] private Text val;
    
    private string metric;

    public Sprite Pictogram { set => pictogram.sprite = value; }
    public string Metric { set => metric = value; }
    public float MinValue { set => progressBar.MinValue = value;}
    public float MaxValue { set => progressBar.MaxValue = value; }
    public Color Color { set => progressBar.Color = value; }

    public void SetValue(float value) {
        val.text = value + metric;
        progressBar.Value = value;
    }
    public void SetValue(string value) {
        val.text = value;
        progressBar.Value = progressBar.MaxValue;
    }

    public void DestroyGO() {
        Destroy(gameObject);
    }
}
