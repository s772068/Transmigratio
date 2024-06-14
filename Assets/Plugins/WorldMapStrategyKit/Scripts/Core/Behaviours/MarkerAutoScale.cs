using UnityEngine;
using System;

namespace WorldMapStrategyKit {
    /// <summary>
    /// Changes scale automatically based on zoom distance
    /// </summary>
    public class MarkerAutoScale : MonoBehaviour {
        private const float SCALE_MULTIPLIER = 1f;
        private float minScale;
        private float maxScale;

        private WMSK map;
        private Vector3 originalScale;

        void Start() {
            // Get a reference to the World Map API:
            if (map == null) {
                map = WMSK.instance;
            }
            originalScale = transform.localScale;
            minScale = map.zoomMinDistance;
            maxScale = map.zoomMaxDistance;
        }

        void LateUpdate() {
            if (map == null) {
                Destroy(this);
                return;
            }
            float desiredScale = SCALE_MULTIPLIER / map.GetZoomLevel();
            Debug.Log($"Zoom: {map.GetZoomLevel()}");
            if (desiredScale < minScale) {
                desiredScale = minScale;
            } else if (desiredScale > maxScale) {
                desiredScale = maxScale;
            }
            transform.localScale = new Vector3(originalScale.x * desiredScale, originalScale.y * desiredScale, 1f);
        }
    }
}

