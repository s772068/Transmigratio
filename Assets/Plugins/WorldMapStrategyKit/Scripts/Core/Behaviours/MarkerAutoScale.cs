using UnityEngine;
using System;


namespace WorldMapStrategyKit {

    /// <summary>
    /// Changes scale automatically based on zoom distance
    /// </summary>
    public class MarkerAutoScale : MonoBehaviour {

        public float scaleMultiplier = 1f;
        public float maxScale = 100f;
        public float minScale = 0.01f;

        WMSK map;
        Vector3 originalScale;

        void Start() {
            // Get a reference to the World Map API:
            if (map == null) {
                map = WMSK.instance;
            }
            originalScale = transform.localScale;
        }


        void LateUpdate() {
            if (map == null) {
                Destroy(this);
                return;
            }
            float desiredScale = scaleMultiplier / map.renderViewportScaleFactor;
            if (desiredScale < minScale) {
                desiredScale = minScale;
            } else if (desiredScale > maxScale) {
                desiredScale = maxScale;
            }
            transform.localScale = new Vector3(originalScale.x * desiredScale, originalScale.y * desiredScale, 1f);
        }
       
      
    }

}

