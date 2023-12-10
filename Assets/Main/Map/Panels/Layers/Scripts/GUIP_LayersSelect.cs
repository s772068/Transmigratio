using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GUIP_LayersSelect : MonoBehaviour { //, IGameConnecter {
    // [SerializeField] private GameController game;
    // [SerializeField] private Text Label;
    // [SerializeField] private Text[] LayersTxts;
    // 
    // private SettingsController settings;
    // private WmskController wmsk;
    // private MapController map;
    // 
    // private string _layer;
    // 
    // private Dictionary<string, ILayer> layers = new() {
    //     { "Terrain", new TerrainLayer() },
    //     { "Climate", new ClimateLayer() },
    //     { "Flora", new FloraLayer() },
    //     { "Fauna", new FaunaLayer() },
    //     { "Population", new PopulationLayer() },
    //     { "Production", new ProductionLayer() },
    //     { "Economics", new EconomicsLayer() },
    //     { "Goverment", new GovermentLayer() }
    //     // new CivilizationLayer()
    // };
    // 
    // public GameController GameController {
    //     set {
    //         value.Get(out settings);
    //         value.Get(out wmsk);
    //         value.Get(out map);
    //     }
    // }
    // 
    // public void Open() {
    //     if (gameObject.activeSelf) return;
    //     gameObject.SetActive(true);
    //     Localization();
    //     map.OnUpdate += UpdateLayer;
    // }
    // 
    // public void Close() {
    //     gameObject.SetActive(false);
    // }
    // 
    // public void Click(string layer) {
    //     if(layer.Equals("")) return;
    //     if(!layers.ContainsKey(layer)) return;
    //     Clear();
    //     _layer = layer;
    //     layers[_layer].Show(settings, wmsk, map, _layer);
    // }
    // 
    // private void Clear() {
    //     for (int i = 0; i < map.data.Regions.Length; ++i) {
    //         map.data.Regions[i].Color = Color.clear;
    //         wmsk.RegionPainting(i, Color.clear);
    //     }
    // }
    // 
    // private void UpdateLayer() {
    //     if (_layer.Equals("")) return;
    //     if (!layers.ContainsKey(_layer)) return;
    //     layers[_layer].Show(settings, wmsk, map, _layer);
    // }
    // 
    // private void Localization() {
    //     Label.text = settings.Localization.Layers.Name;
    //     for(int i = 0; i < settings.Localization.Layers.Value.Length; ++i) {
    //         LayersTxts[i].text = settings.Localization.Layers.Value[i];
    //     }
    // }
    // 
    // //private void Painting(int maxValue, int[] paramiters) {
    // //    if (maxValue == 0) return;
    // 
    // //    Color color;
    // 
    // //    for (int i = 0; i < paramiters.Length; ++i) {
    // //        color = Color.HSVToRGB(paramiters[i] / maxValue * 7f / 9f, 1f, 1f);
    // //        map.data.Regions[i].Color = color;
    // //        wmsk.RegionPainting(i, color);
    // //    }
    // //}
    // 
    // public void Init() {
    //     game.Get(out settings);
    //     game.Get(out wmsk);
    //     game.Get(out map);
    // }
    // 
    // public void OnDestroy() {
    //     map.OnUpdate -= UpdateLayer;
    //     Clear();
    // }
}
