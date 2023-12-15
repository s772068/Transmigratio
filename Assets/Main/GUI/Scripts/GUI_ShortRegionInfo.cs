using UnityEngine.UI;
using UnityEngine;

public class GUI_ShortRegionInfo : MonoBehaviour, IGameConnecter {
    [SerializeField] private Text countryName;
    [SerializeField] private Text population;
    [SerializeField] private Text eventName;

    private int _regionIndex;

    private SettingsController settings;
    private TimelineController timeline;
    private EventsController events;
    private WmskController wmsk;
    private MapController map;

    public GameController GameController {
        set {
            value.Get(out settings);
            value.Get(out timeline);
            value.Get(out events);
            value.Get(out wmsk);
            value.Get(out map);
        }
    }

    private void SelectRegion(int regionIndex) {
        _regionIndex = regionIndex;
        UpdatePanel();
    }

        private void UpdatePanel() {
        if (_regionIndex == -1) return;
        
        countryName.text = _regionIndex == -1 ?
            settings.Localization.Map.Countries.Name :
            settings.Localization.Map.Countries.Value[_regionIndex];
        
        population.text = _regionIndex == -1 ?
            settings.Localization.Map.Countries.Name :
            settings.Localization.Map.Civilization.Population + "\n" +
            (map.data.GetRegion(_regionIndex).GetAllPopulations() == 0 ?
            settings.Localization.Map.Civilization.EmptyPopulation :
            map.data.GetRegion(_regionIndex).GetAllPopulations());

        //eventName.text = _regionIndex == -1 ?
        //    "" : map.data.Regions[_regionIndex].Events.Count > 0 ?
        //        events.GetEventName(0) : "";
    }

    public void Init() {
        wmsk.OnClick += SelectRegion;
        timeline.OnUpdateData += UpdatePanel;
    }

    private void OnDestroy() {
        wmsk.OnClick -= SelectRegion;
        timeline.OnUpdateData -= UpdatePanel;
    }
}
