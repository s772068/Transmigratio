using UnityEngine.UI;
using UnityEngine;

public class GUIP_LayersSelect : MonoBehaviour, IGameConnecter {
    [SerializeField] private GameController game;
    [SerializeField] private Text Label;
    [SerializeField] private Text[] LayersTxts;

    private SettingsController settings;
    private WmskController wmsk;
    private MapController map;

    private int _layerIndex = -1;

    private ILayer[] layers = {
        new TerrainLayer(),
        new ClimateLayer(),
        new FloraLayer(),
        new FaunaLayer(),
        new PopulationLayer(),
        new ProductionLayer(),
        new EconomicsLayer(),
        new GovermentLayer()
        // new CivilizationLayer()
    };

    public GameController GameController {
        set {
            value.Get(out settings);
            value.Get(out wmsk);
            value.Get(out map);
        }
    }

    public void Open() {
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);
        Localization();
        map.OnUpdate += UpdateLayer;
    }

    public void Close() {
        gameObject.SetActive(false);
    }

    public void Click(int layerIndex) {
        Clear();
        if(layerIndex < 0 || layers.Length - 1 < layerIndex) return;
        _layerIndex = layerIndex;
        layers[_layerIndex].Show(settings, wmsk, map, _layerIndex);
    }

    private void Clear() {
        for (int i = 0; i < map.data.CountRegions; ++i) {
            map.data.GetRegion(i).Color = Color.clear;
            wmsk.RegionPainting(i, Color.clear);
        }
    }

    private void UpdateLayer() {
        if(_layerIndex < 0 || layers.Length - 1 < _layerIndex) return;
        layers[_layerIndex].Show(settings, wmsk, map, _layerIndex);
    }

    private void Localization() {
        Label.text = settings.Localization.Layers.Name;
        for(int i = 0; i < settings.Localization.Layers.Value.Length; ++i) {
            LayersTxts[i].text = settings.Localization.Layers.Value[i];
        }
    }

    private void Painting(int maxValue, int[] paramiters) {
        if (maxValue == 0) return;

        Color color;

        for (int i = 0; i < paramiters.Length; ++i) {
            color = Color.HSVToRGB(paramiters[i] / maxValue * 7f / 9f, 1f, 1f);
            map.data.GetRegion(i).Color = color;
            wmsk.RegionPainting(i, color);
        }
    }

    public void Init() {
        game.Get(out settings);
        game.Get(out wmsk);
        game.Get(out map);
    }
    
    public void OnDestroy() {
        map.OnUpdate -= UpdateLayer;
        Clear();
    }
}
