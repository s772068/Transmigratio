using UnityEngine.UI;
using UnityEngine;

public class GUI_ShortRegionInfo : MonoBehaviour {
    [SerializeField] private Text countryName;
    [SerializeField] private Text population;
    [SerializeField] private Text eventName;
    [Space]
    [SerializeField] private GUIP_Region regionPanel;
    [Space]
    [SerializeField] private SettingsController settings;
    [SerializeField] private EventsController events;
    [SerializeField] private WmskController wmsk;
    [SerializeField] private MapController map;

    private int regionIndex;

    public void Click() => regionPanel.Open(regionIndex);
    
    private void UpdatePanel(int regionIndex) {
        this.regionIndex = regionIndex;
        countryName.text = settings.Localization.Map.Countries[regionIndex];
        population.text = settings.Localization.Map.Civilization.Population + "\n" +
                          map.data.Regions[regionIndex].AllPopulations;
        eventName.text = map.data.Regions[regionIndex].Events.Count > 0 ?
                         events.GetEventName(0) :
                         "";
    }

    private void Awake() {
        wmsk.OnClick += UpdatePanel;
    }
}
