using UnityEngine.UI;
using UnityEngine;

public class ResourcesController : BaseController, ISave {
    [SerializeField] private GUI_ProgressBar progressBar;
    [SerializeField] private Text progressTxt;
    [Range(0, 100)]
    public int intervention;

    private SettingsController settings;
    
    public override GameController GameController {
        set {
            settings = value.Get<SettingsController>();
        }
    }

    public void Save() {
        IOHelper.SaveToJson(new S_Resources() {
            intervention = intervention
        });
    }

    public void Load() {
        IOHelper.LoadFromJson(out S_Resources data);
        intervention = data.intervention;
    }

    public void UpdateResources() {
        progressBar.Fill = intervention / 100f;
        progressTxt.text = settings.Localization.Resources.Intervention + " " + intervention + "%";
    }
}
