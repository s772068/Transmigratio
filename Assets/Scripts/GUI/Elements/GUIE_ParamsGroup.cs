using UnityEngine.UI;
using UnityEngine;

public class GUIE_ParamsGroup : MonoBehaviour {
    [SerializeField] private GUI_Paramiter[] paramiters;

    public int CountParamiters => paramiters.Length;

    public void SetParamiterLabel(int index, string label) {
        Show(index, true);
        paramiters[index].Label = label;
    }
    public void SetParamiterMetric(int index, string value) {
        Show(index, true);
        paramiters[index].Metric = value;
    }
    public void SetParamiterMinValue(int index, float value) {
        Show(index, true);
        paramiters[index].MinValue = value;
    }
    public void SetParamiterMaxValue(int index, float value) {
        Show(index, true);
        paramiters[index].MaxValue = value;
    }
    public void SetParamiterColor(int index, Color value) {
        Show(index, true);
        paramiters[index].Color = value;
    }
    public void SetParamiterValue(int index, string value) {
        Show(index, true);
        paramiters[index].SetValue(value);
    }
    public void SetParamiterValue(int index, int value) {
        Show(index, true);
        paramiters[index].SetValue(value);
    }
    public void Show(bool isShow) => gameObject.SetActive(isShow);
    public void Show(int index, bool isShow) => paramiters[index].gameObject.SetActive(isShow);
}
