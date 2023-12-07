using System;
using UnityEngine;

public class ResourcesController : MonoBehaviour, ISave, IGameConnecter {
    [Range(0, 100)]
    public int intervention;
    public int Intervention {
        get => intervention;
        set {
            intervention = Math.Clamp(value, 0, 100);
            OnIntervention.Invoke(intervention);
        }
    }

    public Action<int> OnIntervention;

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
