public class ResourcesController : BaseController, ISave {
    public int intervention;

    public void Save() {
        IOHelper.SaveToJson(new S_Resources() {
            intervention = intervention
        });
    }

    public void Load() {
        IOHelper.LoadFromJson(out S_Resources data);
        intervention = data.intervention;
    }
}
