using AYellowpaper;
using UnityEngine;

public class SaveController : MonoBehaviour {
    [SerializeField] private InterfaceReference<ISave, MonoBehaviour>[] saves;

    public void Save() {
        for (int i = 0; i < saves.Length; ++i) {
            saves[i].Value.Save();
        }
    }

    public void Load() {
        for (int i = 0; i < saves.Length; ++i) {
            saves[i].Value.Load();
        }
    }
}
