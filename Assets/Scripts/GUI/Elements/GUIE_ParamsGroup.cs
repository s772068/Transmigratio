using UnityEngine.UI;
using UnityEngine;

public class GUIE_ParamsGroup : MonoBehaviour {
    [SerializeField] private Text label;
    [SerializeField] private GUI_Paramiter[] paramiters;

    public int CountParamiters => paramiters.Length;

    public string Label { set { label.text = value; } }
    public void SetParamiterLabel(int index, string label) => paramiters[index].Label = label;
    public void SetParamiterValue(int index, string value) => paramiters[index].Value = value;
    public void Show(bool isShow, int index) => paramiters[index].gameObject.SetActive(isShow);
    public void Show(bool isShow) => gameObject.SetActive(isShow);
}
