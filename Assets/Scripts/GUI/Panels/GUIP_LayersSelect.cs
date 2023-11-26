using UnityEngine;

public class GUIP_LayersSelect : MonoBehaviour {
    [SerializeField] private SettingsController settings;
    [SerializeField] private LayersController layers;
    [SerializeField] private MapController map;
    
    private int[] values;
    
    public void Open() {
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);
        values = new int[map.data.Regions.Length];
        Localization();
    }

    public void Close() {
        gameObject.SetActive(false);
    }

    public void Click(int paramiterIndex) {
        int max = 0;
        if (paramiterIndex >= 0 && paramiterIndex <= 3) {
            max = map.data.MaxEcologyValue(paramiterIndex);
            for(int i = 0; i < values.Length; ++i) {
                values[i] = map.data.Regions[i].Ecology[paramiterIndex].MaxValue;
            }
        }
        if(paramiterIndex == 4) {
            max = map.data.MaxPopulationValue;
            for (int i = 0; i < values.Length; ++i) {
                values[i] = map.data.Regions[i].MaxPopulationsValue;
            }
        }
        if(paramiterIndex >= 5 && paramiterIndex <= 7) {
            paramiterIndex -= 5;
            max = map.data.MaxCivilizationValue(paramiterIndex);
            for (int i = 0; i < values.Length; ++i) {
                values[i] = map.data.Regions[i].MaxCivilizationValue(paramiterIndex);
            }
        }
        if(paramiterIndex == 8) {
            max = map.data.MaxCivilizationStage;
            for (int i = 0; i < values.Length; ++i) {
                values[i] = map.data.Regions[i].MaxCivilizationStage;
            }
        }
        layers.Painting(max, values);
    }

    private void Localization() {
    }
}
