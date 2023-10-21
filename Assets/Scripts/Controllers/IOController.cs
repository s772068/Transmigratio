using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class IOController : MonoSingleton<IOController> {
    [Inject] private MapHolder map;
    
    private void OnApplicationPause(bool pause) {
        if(pause) {
            //IOHelper.
        } else {
            Load();
        }
    }

    private void Save() {

    }

    private void Load() {

    }
}
