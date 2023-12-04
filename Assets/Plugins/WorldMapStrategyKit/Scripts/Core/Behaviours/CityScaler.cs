using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {
    /// <summary>
    /// City scaler. Checks the city icons' size is always appropiate
    /// </summary>
    public class CityScaler : MonoBehaviour {

        const float CITY_SIZE_ON_SCREEN = 10.0f;
        Vector3 lastCamPos, lastPos;
        float lastIconSize;
        float lastCustomSize;
        float lastOrtographicSize;
        [NonSerialized]
        public WMSK
            map;

        void Start() {
            ScaleCities();
        }

        // Update is called once per frame
        void Update() {
            if (lastPos != transform.position || lastCamPos != map.currentCamera.transform.position || lastIconSize != map.cityIconSize ||
                         map.currentCamera.orthographic && map.currentCamera.orthographicSize != lastOrtographicSize) {
                ScaleCities();
            }
        }


        public void ScaleCities() {
            if (map == null || map.currentCamera == null || map.currentCamera.pixelWidth == 0 || gameObject == null)
                return; // Camera pending setup

            try {
                // annotate current values 
                lastPos = transform.position;
                lastCamPos = map.currentCamera.transform.position;
                lastIconSize = map.cityIconSize;
                lastOrtographicSize = map.currentCamera.orthographicSize;

                Plane plane = new Plane(transform.forward, transform.position);
                float dist = plane.GetDistanceToPoint(lastCamPos);
                Vector3 centerPos = lastCamPos - transform.forward * dist;
                Vector3 a = map.currentCamera.WorldToScreenPoint(centerPos);
                Vector3 b = new Vector3(a.x, a.y + CITY_SIZE_ON_SCREEN, a.z);
                if (map.currentCamera.pixelWidth == 0) return; // Camera pending setup
                Vector3 aa = map.currentCamera.ScreenToWorldPoint(a);
                Vector3 bb = map.currentCamera.ScreenToWorldPoint(b);
                float scale = (aa - bb).magnitude * map.cityIconSize;
                if (map.currentCamera.orthographic) {
                    scale /= 1 + (map.currentCamera.orthographicSize * map.currentCamera.orthographicSize) * (0.1f / map.transform.localScale.x);
                } else {
                    scale /= 1 + dist * dist * (0.1f / map.transform.localScale.x);
                }
                Vector3 newScale = new Vector3(scale / WMSK.mapWidth, scale / WMSK.mapHeight, 1.0f);
                map.currentCityScale = newScale;

                // check if scale has changed
                Transform tNormalCities = transform.Find("Normal Cities");
                bool needRescale = false;
                Transform tChild;
                if (tNormalCities != null && tNormalCities.childCount > 0) {
                    tChild = tNormalCities.GetChild(0);
                    if (tChild != null) {
                        if (tChild.localScale != newScale)
                            needRescale = true;
                    }
                }
                Transform tRegionCapitals = transform.Find("Region Capitals");
                if (!needRescale && tRegionCapitals != null && tRegionCapitals.childCount > 0) {
                    tChild = tRegionCapitals.GetChild(0);
                    if (tChild != null) {
                        if (tChild.localScale != newScale)
                            needRescale = true;
                    }
                }
                Transform tCountryCapitals = transform.Find("Country Capitals");
                if (!needRescale && tCountryCapitals != null && tCountryCapitals.childCount > 0) {
                    tChild = tCountryCapitals.GetChild(0);
                    if (tChild != null) {
                        if (tChild.localScale != newScale)
                            needRescale = true;
                    }
                }

                if (!needRescale)
                    return;

                // apply scale to all cities children
                for (int k = 0, max = tNormalCities.childCount; k < max; k++) {
                    tNormalCities.GetChild(k).localScale = newScale;
                }
                Vector3 regionScale = newScale * 1.75f;
                for (int k = 0, max = tRegionCapitals.childCount; k < max; k++) {
                    tRegionCapitals.GetChild(k).localScale = regionScale;
                }
                Vector3 capitalScale = newScale * 2f;
                for (int k = 0, max = tCountryCapitals.childCount; k < max; k++) {
                    tCountryCapitals.GetChild(k).localScale = capitalScale;
                }
            } catch { }
        }

        public void ScaleCities(float customSize) {
            if (customSize == lastCustomSize)
                return;
            lastCustomSize = customSize;
            Vector3 newScale = new Vector3(customSize / WMSK.mapWidth, customSize / WMSK.mapHeight, 1);
            foreach (Transform t in transform.Find("Normal Cities"))
                t.localScale = newScale;
            foreach (Transform t in transform.Find("Region Capitals"))
                t.localScale = newScale * 1.75f;
            foreach (Transform t in transform.Find("Country Capitals"))
                t.localScale = newScale * 2.0f;
        }
    }

}