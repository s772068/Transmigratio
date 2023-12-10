using UnityEngine.UI;
using UnityEngine;

public class GUI_ShortRegionInfo : MonoBehaviour { //, IGameConnecter {
    // [SerializeField] private Text countryName;
    // [SerializeField] private Text population;
    // [SerializeField] private Text eventName;
    // 
    // private string _region;
    // 
    // private SettingsController settings;
    // private TimelineController timeline;
    // private EventsController events;
    // private WmskController wmsk;
    // private MapController map;
    // 
    // public GameController GameController {
    //     set {
    //         value.Get(out settings);
    //         value.Get(out timeline);
    //         value.Get(out events);
    //         value.Get(out wmsk);
    //         value.Get(out map);
    //     }
    // }
    // 
    // private void SelectRegion(string region) {
    //     _region = region;
    //     UpdatePanel();
    // }
    // 
    //     private void UpdatePanel() {
    //     if(_region.Equals("")) return;
    //     
    //     countryName.text = settings.Localization.Map.Countries.Names[_region];
    //     
    //     population.text = settings.Localization.Map.Civilization.Population + "\n" +
    //         (map.data.Regions[_region].AllPopulations == 0 ?
    //         settings.Localization.Map.Civilization.EmptyPopulation :
    //         map.data.Regions[_region].AllPopulations);
    // 
    //     //eventName.text = _regionIndex == -1 ?
    //     //    "" : map.data.Regions[_regionIndex].Events.Count > 0 ?
    //     //        events.GetEventName(0) : "";
    // }
    // 
    // public void Init() {
    //     wmsk.OnClick += SelectRegion;
    //     timeline.OnTick += UpdatePanel;
    // }
    // 
    // private void OnDestroy() {
    //     wmsk.OnClick -= SelectRegion;
    //     timeline.OnTick -= UpdatePanel;
    // }
}
