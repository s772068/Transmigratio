using UnityEngine.UI;
using UnityEngine;

public class GUIP_Top : MonoBehaviour, IGameConnecter {
    [SerializeField] private Text populationTxt;
    [SerializeField] private Text interventionTxt;
    [SerializeField] private GUI_ProgressBar interventionsPB;
    [SerializeField] private Button selectRegionBtn;

    private ResourcesController resources;
    private TimelineController timeline;
    private SettingsController settings;
    private WmskController wmsk;
    private MapController map;

    private string populationStr;
    private string interventionStr;

    public GameController GameController {
        set {
            value.Get(out resources);
            value.Get(out timeline);
            value.Get(out settings);
            value.Get(out wmsk);
            value.Get(out map);
        }
    }

    public void SelectRegion() {
        if (wmsk.SelectedIndex < 0) return;
        selectRegionBtn.gameObject.SetActive(false);
        timeline.OnSelectRegion?.Invoke(wmsk.SelectedIndex);
    }

    private void Localization() {
        populationStr = settings.Localization.Map.Civilization.Population;
        interventionStr = settings.Localization.Resources.Intervention;
    }

    private void UpdatePlatform() {
        populationTxt.text = populationStr + "\n" +
            (map.data.GetPopulations() == 0 ?
            settings.Localization.Map.Civilization.EmptyPopulation :
            map.data.GetPopulations());
        interventionTxt.text = interventionStr + " " + resources.intervention + "%";
        interventionsPB.Value = resources.intervention;
    }

    public void Init() {
        map.OnUpdate += UpdatePlatform;
        timeline.OnSelectRegion += (int index) => UpdatePlatform();
        Localization();
        UpdatePlatform();
    }
}
