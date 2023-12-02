using UnityEngine;

public class ResourcesController : MonoBehaviour, ISave, IGameConnecter {
    [Range(0, 100)]
    public int intervention;

    public GameController GameController { set { } }

    public void Save() {
        IOHelper.SaveToJson(new S_Resources() {
            intervention = intervention
        });
    }

    public void Load() {
        IOHelper.LoadFromJson(out S_Resources data);
        intervention = data.intervention;
    }

    public void Init() { }
}
