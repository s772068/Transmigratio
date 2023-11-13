using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    [SerializeField] private BaseController[] controllers;

    public T Get<T>() where T : BaseController {
        for(int i = 0; i < controllers.Length; ++i) {
            if (controllers[i] is T) {
                return controllers[i] as T;
            }
        }
        return null;
    }

    public void Save() {
        for (int i = 0; i < controllers.Length; ++i) {
            if (controllers[i] is ISave) {
                (controllers[i] as ISave).Save();
            }
        }
    }

    public void Load() {
        for (int i = 0; i < controllers.Length; ++i) {
            if (controllers[i] is ISave) {
                (controllers[i] as ISave).Load();
            }
        }
    }

    private void Awake() {
        for(int i = 0; i < controllers.Length; ++i) {
            controllers[i].GameController = this;
            controllers[i].Init();
        }
    }
}
