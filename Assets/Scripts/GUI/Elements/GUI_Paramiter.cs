using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class GUI_Paramiter : MonoBehaviour {
    [SerializeField] private GUI_ProgressBar progressBar;
    [SerializeField] private Text label;
    [SerializeField] private Text val;
    [SerializeField] private bool isHideble;

    public bool IsHideble => isHideble;

    public string Label { set => label.text = value; }
    public string Value { set => val.text = value; }    
    public float Fill { set => progressBar.Fill = value; }

    public void DestroyGO() {
        Destroy(gameObject);
    }
}
