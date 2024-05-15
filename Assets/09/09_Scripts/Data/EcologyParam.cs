using AYellowpaper.SerializedCollections;
using System.Linq;

[System.Serializable]
public class EcologyParam {

    public string currentMax;                              //������� ������������ ��������
    public float richness;                              //��������� ��������� (��� �����, �����)
    public bool isRichnessApplicable = true;
    // public S_Dictionary<string, float> quantities;        //�������� ��� ������ ������������. ���� forest:15, steppe:40. ����� current="steppe" (��������������� ������ ��� ����� SetCurrent())
    public SerializedDictionary<string, float> quantities;
    public void Init() {
        if (richness == -1) { isRichnessApplicable = false; }

        RefreshParam();
    }
    /// <summary>
    /// ���������� �������� � quantities � ����� current=max
    /// </summary>
    public void SetCurrent() {
        var pair = quantities.FirstOrDefault(x => x.Value == quantities.Values.Max());
        if (pair.Value >= 0)
            currentMax = pair.Key;
        else currentMax = "none";
    }
    /*
    public void QuantitiesToProcents()
    {
        float sum = quantities.GetValues().Sum();
        for(int i = 0; i < quantities.sources.Count; ++i) {
            var pair = quantities.sources[i];
            quantities[pair.Key] = (float) Math.Round(pair.Value / sum * 100, 1);
        }
    }
    */
    public void RefreshParam() {
        //QuantitiesToProcents();
        if (quantities != null) {
            SetCurrent();
        }
    }
}
