using UnityEngine;

namespace WorldMapStrategyKit {

    [ExecuteAlways]
    public class CameraPivot : MonoBehaviour {

        private void OnDestroy() {
            Camera cam = transform.root.GetComponentInChildren<Camera>();
            if (cam != null) cam.enabled = true;
        }

    }
}
