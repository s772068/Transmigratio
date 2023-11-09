// World Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

    public enum VIEWPORT_LIGHTING_MODE {
        Lit,
        Unlit
    }

    public partial class WMSK : MonoBehaviour {

        #region Public properties

        [SerializeField]
        float _earthElevation = 1.0f;

        /// <summary>
        /// Ground elevation when viewport is used.
        /// </summary>
        /// <value>The earth elevation.</value>
        public float earthElevation {
            get {
                return _earthElevation;
            }
            set {
                if (value != _earthElevation) {
                    _earthElevation = value;
                    isDirty = true;
                    EarthBuildMesh();
                }
            }
        }


        [SerializeField]
        bool _earthCloudLayer;

        /// <summary>
        /// Enables/disables the cloud layer when viewport is used.
        /// </summary>
        public bool earthCloudLayer {
            get {
                return _earthCloudLayer;
            }
            set {
                if (value != _earthCloudLayer) {
                    _earthCloudLayer = value;
                    isDirty = true;
                    UpdateCloudLayer();
                }
            }
        }

        [SerializeField]
        float _earthCloudLayerSpeed = 1.2f;

        /// <summary>
        /// Speed of the cloud animation of cloud layer when viewport is used.
        /// </summary>
        public float earthCloudLayerSpeed {
            get {
                return _earthCloudLayerSpeed;
            }
            set {
                if (value != _earthCloudLayerSpeed) {
                    _earthCloudLayerSpeed = value;
                    isDirty = true;
                    UpdateCloudLayer();
                }
            }
        }


        [SerializeField]
        float _earthCloudLayerElevation = -5.0f;

        /// <summary>
        /// Elevation of cloud layer when viewport is used.
        /// </summary>
        public float earthCloudLayerElevation {
            get {
                return _earthCloudLayerElevation;
            }
            set {
                if (value != _earthCloudLayerElevation) {
                    _earthCloudLayerElevation = value;
                    isDirty = true;
                    UpdateCloudLayer();
                }
            }
        }



        [SerializeField]
        float _earthCloudLayerAlpha = 1.0f;

        /// <summary>
        /// Global alpha for the optional cloud layer when viewport is used.
        /// </summary>
        public float earthCloudLayerAlpha {
            get {
                return _earthCloudLayerAlpha;
            }
            set {
                if (value != _earthCloudLayerAlpha) {
                    _earthCloudLayerAlpha = value;
                    isDirty = true;
                    UpdateCloudLayer();
                }
            }
        }

        [SerializeField]
        float _earthCloudLayerShadowStrength = 0.35f;

        /// <summary>
        /// Global alpha for the optional cloud layer when viewport is used.
        /// </summary>
        public float earthCloudLayerShadowStrength {
            get {
                return _earthCloudLayerShadowStrength;
            }
            set {
                if (value != _earthCloudLayerShadowStrength) {
                    _earthCloudLayerShadowStrength = value;
                    isDirty = true;
                    UpdateCloudLayer();
                }
            }
        }


        [SerializeField]
        GameObject
            _renderViewport;

        /// <summary>
        /// Target gameobject to display de map (optional)
        /// </summary>
        public GameObject renderViewport {
            get {
                return _renderViewport;
            }
            set {
                if (value == null) {
                    value = gameObject;
                }
                if (value != _renderViewport) {
                    if (_renderViewport != null) {
                        GetCurrentMapLocation(out lastKnownMapCoordinates);
                        if (_currentCamera != null) {
                            GetZoomLevel(); // updates lastKnownZoomLevel
                        }
                    }
                    AssignRenderViewport(value);
                    isDirty = true;
                    SetupViewport();
                    RepositionViewportObjects();
                    RepositionCamera();
                }
            }
        }


        [SerializeField]
        RectTransform _renderViewportUIPanel;

        /// <summary>
        /// Panel placeholder where viewport is positioned
        /// </summary>
        public RectTransform renderViewportUIPanel {
            get {
                return _renderViewportUIPanel;
            }
            set {
                if (value != _renderViewportUIPanel) {
                    _renderViewportUIPanel = value;
                    panelUIOldSize = Vector2.zero;
                    isDirty = true;
                    SetupViewport();
                }
            }
        }


        Rect _renderViewportRect;

        /// <summary>
        /// Returns the visible rectangle of the map represented by current GameView viewport location and zoom
        /// </summary>
        public Rect renderViewportRect {
            get {
                ComputeViewportRect();
                return _renderViewportRect;
            }
        }

        /// <summary>
        /// Returns the visible rectangle of the map represented by current SceneView viewport location and zoom
        /// </summary>
        public Rect renderViewportRectFromSceneView {
            get {
                Rect rect;
                ComputeViewportRect(true);
                rect = _renderViewportRect;
                lastRenderViewportGood = false;
                ComputeViewportRect();
                return rect;
            }
        }


        [SerializeField]
        float _renderViewportResolution = 2;

        /// <summary>
        /// Quality of render viewport. This is a factor of the screen width. x2 is good for antialiasis. x1 equals to screen width.
        /// </summary>
        public float renderViewportResolution {
            get {
                return _renderViewportResolution;
            }
            set {
                if (value != _renderViewportResolution) {
                    _renderViewportResolution = value;
                    isDirty = true;
                    SetupViewport();
                }
            }
        }


        [SerializeField]
        int _renderViewportResolutionMaxRTWidth = 2048;

        /// <summary>
        /// Maximum width for the render texture. A value of 2048 is the recommended for most cases.
        /// </summary>
        public int renderViewportResolutionMaxRTWidth {
            get {
                return _renderViewportResolutionMaxRTWidth;
            }
            set {
                if (value != _renderViewportResolutionMaxRTWidth) {
                    _renderViewportResolutionMaxRTWidth = value;
                    isDirty = true;
                    SetupViewport();
                }
            }
        }

        /// <summary>
        /// Returns true if render viewport is a terrain
        /// </summary>
        public bool renderViewportIsTerrain => viewportMode == ViewportMode.Terrain;

        /// <summary>
        /// Returns true if render viewport is a map panel UI element
        /// </summary>
        public bool renderViewportIsMapPanel => viewportMode == ViewportMode.MapPanel;

        /// <summary>
        /// Returns true if render viewport is a 3D viewport
        /// </summary>
        public bool renderViewportIs3DViewport => viewportMode == ViewportMode.Viewport3D;


        [SerializeField]
        FilterMode _renderViewportFilterMode = FilterMode.Trilinear;

        public FilterMode renderViewportFilterMode {
            get { return _renderViewportFilterMode; }
            set {
                if (_renderViewportFilterMode != value) {
                    _renderViewportFilterMode = value;
                    isDirty = true;
                    SetupViewport();
                }
            }
        }

        [SerializeField] VIEWPORT_LIGHTING_MODE _renderViewportLightingMode = VIEWPORT_LIGHTING_MODE.Lit;

        public VIEWPORT_LIGHTING_MODE renderViewportLightingMode {
            get { return _renderViewportLightingMode; }
            set {
                if (_renderViewportLightingMode != value) {
                    _renderViewportLightingMode = value;
                    SetupViewport();
                }
            }
        }



        [SerializeField]
        float _renderViewportCurvature = 0;

        /// <summary>
        /// Curvature of render viewport
        /// </summary>
        public float renderViewportCurvature {
            get {
                return _renderViewportCurvature;
            }
            set {
                if (value != _renderViewportCurvature) {
                    _renderViewportCurvature = value;
                    isDirty = true;
                    SetupViewport();
                }
            }
        }


        [SerializeField]
        float _renderViewportCurvatureMinZoom;

        /// <summary>
        /// Curvature of render viewport when zoom is at minimum
        /// </summary>
        public float renderViewportCurvatureMinZoom {
            get {
                return _renderViewportCurvatureMinZoom;
            }
            set {
                if (value != _renderViewportCurvatureMinZoom) {
                    _renderViewportCurvatureMinZoom = value;
                    isDirty = true;
                    SetupViewport();
                }
            }
        }



        [SerializeField]
        RenderingPath _renderViewportRenderingPath = RenderingPath.Forward;

        public RenderingPath renderViewportRenderingPath {
            get { return _renderViewportRenderingPath; }
            set {
                if (_renderViewportRenderingPath != value) {
                    _renderViewportRenderingPath = value;
                    isDirty = true;
                    SetupViewport();
                }
            }
        }


        [SerializeField]
        float _renderViewportTerrainAlpha = 1.0f;

        /// <summary>
        /// Global alpha for the WMSK texture projectino on Unity terrain
        /// </summary>
        public float renderViewportTerrainAlpha {
            get {
                return _renderViewportTerrainAlpha;
            }
            set {
                if (value != _renderViewportTerrainAlpha) {
                    _renderViewportTerrainAlpha = value;
                    isDirty = true;
                    lastMainCameraPos = Misc.Vector3zero; // forces terrain viewport refresh
                }
            }
        }

        [SerializeField]
        float _renderViewportGOAutoScaleMultiplier = 1f;

        /// <summary>
        /// Global scale multiplier for game objects put on top of the viewport.
        /// </summary>
        public float renderViewportGOAutoScaleMultiplier {
            get {
                return _renderViewportGOAutoScaleMultiplier;
            }
            set {
                if (value != _renderViewportGOAutoScaleMultiplier) {
                    _renderViewportGOAutoScaleMultiplier = value;
                    isDirty = true;
                    UpdateViewportObjectsTransformAndVisibility();
                }
            }
        }


        [SerializeField]
        float _renderViewportGOAutoScaleMin = 1f;

        /// <summary>
        /// Minimum scale applied to game objects on the viewport.
        /// </summary>
        public float renderViewportGOAutoScaleMin {
            get {
                return _renderViewportGOAutoScaleMin;
            }
            set {
                if (value != _renderViewportGOAutoScaleMin) {
                    _renderViewportGOAutoScaleMin = value;
                    isDirty = true;
                    UpdateViewportObjectsTransformAndVisibility();
                }
            }
        }

        [SerializeField]
        float _renderViewportGOAutoScaleMax = 10f;

        /// <summary>
        /// Maximum scale applied to game objects on the viewport.
        /// </summary>
        public float renderViewportGOAutoScaleMax {
            get {
                return _renderViewportGOAutoScaleMax;
            }
            set {
                if (value != _renderViewportGOAutoScaleMax) {
                    _renderViewportGOAutoScaleMax = value;
                    isDirty = true;
                    UpdateViewportObjectsTransformAndVisibility();
                }
            }
        }

        [SerializeField]
        GameObject _sun;

        public GameObject sun {
            get { return _sun; }
            set {
                if (value != _sun) {
                    _sun = value;
                    UpdateSun();
                }
            }
        }

        [SerializeField]
        bool _sunUseTimeOfDay = false;

        /// <summary>
        /// Whether the rotation of the Sun can be controlled using the timeOfDay property (0-24h)
        /// </summary>
        public bool sunUseTimeOfDay {
            get {
                return _sunUseTimeOfDay;
            }
            set {
                if (value != _sunUseTimeOfDay) {
                    _sunUseTimeOfDay = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _timeOfDay;

        /// <summary>
        /// Simulated time of day (0-24). This would move the light gameobject orientation referenced by sun property around the map.
        /// </summary>
        public float timeOfDay {
            get {
                return _timeOfDay;
            }
            set {
                if (value != _timeOfDay) {
                    _timeOfDay = value;
                    isDirty = true;
                    UpdateSun();
                }
            }
        }




        #endregion

        #region Viewport APIs

        public bool renderViewportIsEnabled {
            get { return viewportMode != ViewportMode.None; }
        }

        /// <summary>
        /// Computes the interpolated, perspective adjusted or not, height on given position.
        /// </summary>
        public float ComputeEarthHeight(Vector2 position, bool perspectiveAjusted) {

            if (position.x < -0.5f || position.x > 0.5f || position.y < -0.5f || position.y > 0.5f)
                return 0;

            position.x += 0.5f;
            position.y += 0.5f;

            int x0 = (int)(position.x * heightmapTextureWidth);
            int y0 = (int)(position.y * heightmapTextureHeight);
            int x1 = x0 + 1;
            if (x1 >= heightmapTextureWidth - 1)
                x1 = heightmapTextureWidth - 1;
            int y1 = y0 + 1;
            if (y1 >= heightmapTextureHeight - 1)
                y1 = heightmapTextureHeight - 1;

            int pos00 = (int)(y0 * heightmapTextureWidth + x0);
            int pos10 = (int)(y0 * heightmapTextureWidth + x1);
            int pos01 = (int)(y1 * heightmapTextureWidth + x0);
            int pos11 = (int)(y1 * heightmapTextureWidth + x1);
            float elev00 = viewportElevationPoints[pos00];
            float elev10 = viewportElevationPoints[pos10];
            float elev01 = viewportElevationPoints[pos01];
            float elev11 = viewportElevationPoints[pos11];
            if (perspectiveAjusted) {
                elev00 *= _renderViewportElevationFactor;
                elev10 *= _renderViewportElevationFactor;
                elev01 *= _renderViewportElevationFactor;
                elev11 *= _renderViewportElevationFactor;
            }

            float cellWidth = 1.0f / heightmapTextureWidth;
            float cellHeight = 1.0f / heightmapTextureHeight;
            float cellx = (position.x - (float)x0 * cellWidth) / cellWidth;
            float celly = (position.y - (float)y0 * cellHeight) / cellHeight;

            float elev = elev00 * (1.0f - cellx) * (1.0f - celly) +
                                  elev10 * cellx * (1.0f - celly) +
                                  elev01 * (1.0f - cellx) * celly +
                                  elev11 * cellx * celly;

            return elev;
        }

        /// <summary>
        /// Returns the surface normal of the renderViewport at the position in map coordinates.
        /// </summary>
        public bool RenderViewportGetNormal(Vector2 mapPosition, out Vector3 normal) {
            if (_wrapHorizontally) {
                if (mapPosition.x >= 0.5f)
                    mapPosition.x -= 1f;
                else if (mapPosition.x <= -0.5f)
                    mapPosition.x += 1f;
            }
            Vector3 worldPos;
            if (renderViewportIsTerrain) {
                // Terrain mode
                worldPos = transform.TransformPoint(mapPosition);
                worldPos.y -= WMSK_TERRAIN_MODE_Y_OFFSET;
            } else {
                // Viewport mode
                worldPos = _renderViewport.transform.TransformPoint(mapPosition);
            }
            return RenderViewportGetNormal(worldPos, out normal);
        }

        /// <summary>
        /// Returns the surface normal of the renderViewport at the position in World coordinates.
        /// </summary>
        public bool RenderViewportGetNormal(Vector3 worldPosition, out Vector3 normal) {
            if (renderViewportIsTerrain) {
                float dist = terrain.terrainData.size.y;
                Ray ray = new Ray(worldPosition + Misc.Vector3up * dist, Misc.Vector3down);
                if (terrainHits == null)
                    terrainHits = new RaycastHit[20];
                int hitCount = Physics.RaycastNonAlloc(ray, terrainHits, dist + 1f);
                for (int i = 0; i < hitCount; i++) {
                    if (terrainHits[i].transform.gameObject == terrain.gameObject) {
                        normal = terrainHits[i].normal;
                        return true;
                    }
                }
            } else {
                RaycastHit hit;
                Ray ray = new Ray(worldPosition - _renderViewport.transform.forward * 50.0f, _renderViewport.transform.forward);
                if (Physics.Raycast(ray, out hit, 100.0f, layerMask)) {
                    normal = hit.normal;
                    return true;
                }
            }
            normal = Misc.Vector3zero;
            return false;
        }


        /// <summary>
        /// Returns the zoom level required to show the entire rect in local map coordinates
        /// </summary>
        /// <returns>The country zoom level of -1 if error.</returns>
        public float GetZoomExtents(Rect rect) {
            return GetFrustumZoomLevel(rect.width * mapWidth, rect.height * mapHeight);
        }


        /// <summary>
        /// Increases heightmap value at specific position and radius
        /// </summary>
        /// <param name="amount">amount to add/substract to heightmap. Heightmap values are in the 0-255 range (integer values).</param>
        public void EarthRaiseElevation(Vector2 localPosition, float radius, float amount) {
            localPosition.x += 0.5f;
            localPosition.y += 0.5f;
            int y0 = Mathf.FloorToInt((localPosition.y - radius) * heightmapTextureHeight);
            int y1 = Mathf.FloorToInt((localPosition.y + radius) * heightmapTextureHeight);
            int x0 = Mathf.FloorToInt((localPosition.x - radius) * heightmapTextureWidth);
            int x1 = Mathf.FloorToInt((localPosition.x + radius) * heightmapTextureWidth);
            float radiusSqr = radius * radius;
            bool changes = false;
            for (int j = y0; j <= y1 && j < heightmapTextureHeight; j++) {
                if (j < 0) continue;
                int jj = j * heightmapTextureWidth;
                float dy = (j + 0.5f) / heightmapTextureHeight - localPosition.y;
                for (int k = x0; k <= x1 && k < heightmapTextureWidth; k++) {
                    if (k < 0) continue;
                    float dx = (k + 0.5f) / heightmapTextureWidth - localPosition.x;
                    float distSqr = dy * dy + dx * dx;
                    if (distSqr > radiusSqr) continue;
                    float height = viewportElevationPoints[jj + k];
                    height += amount;
                    if (height < 0) height = 0; else if (height > 1f) height = 1f;
                    viewportElevationPoints[jj + k] = height;
                    changes = true;
                }
            }

            if (changes) {
                MarkCustomRouteMatrixDirty();
                if (renderViewportIsEnabled) {
                    shouldCheckBoundaries = true; // updates viewport
                }
            }
        }

        /// <summary>
        /// Reduces heightmap value at specific position and radius
        /// </summary>
        public void EarthLowerElevation(Vector2 position, float radius, float amount) {
            EarthRaiseElevation(position, radius, -amount);
        }


        /// <summary>
        /// Increases heightmap value at specific position and radius
        /// </summary>
        public void EarthSetCustomElevation(Vector2 localPosition, float radius, float elevation) {
            if (elevation < 0) elevation = 0; else if (elevation > 1f) elevation = 1f;
            localPosition.x += 0.5f;
            localPosition.y += 0.5f;
            int y0 = Mathf.FloorToInt((localPosition.y - radius) * heightmapTextureHeight);
            int y1 = Mathf.FloorToInt((localPosition.y + radius) * heightmapTextureHeight);
            int x0 = Mathf.FloorToInt((localPosition.x - radius) * heightmapTextureWidth);
            int x1 = Mathf.FloorToInt((localPosition.x + radius) * heightmapTextureWidth);
            float radiusSqr = radius * radius;
            bool changes = false;
            for (int j = y0; j <= y1 && j < heightmapTextureHeight; j++) {
                if (j < 0) continue;
                int jj = j * heightmapTextureWidth;
                float dy = (j + 0.5f) / heightmapTextureHeight - localPosition.y;
                for (int k = x0; k <= x1 && k < heightmapTextureWidth; k++) {
                    if (k < 0) continue;
                    float dx = (k + 0.5f) / heightmapTextureWidth - localPosition.x;
                    float distSqr = dy * dy + dx * dx;
                    if (distSqr > radiusSqr) continue;
                    viewportElevationPoints[jj + k] = elevation;
                    changes = true;
                }
            }

            if (changes) {
                MarkCustomRouteMatrixDirty();
                if (renderViewportIsEnabled) {
                    shouldCheckBoundaries = true; // updates viewport
                }
            }
        }



        /// <summary>
        /// Restores heightmap value at specific position and radius
        /// </summary>
        public void EarthResetCustomElevation(Vector2 localPosition, float radius) {
            const float baseElevation = 24.0f / 255.0f;
            localPosition.x += 0.5f;
            localPosition.y += 0.5f;
            int y0 = Mathf.FloorToInt((localPosition.y - radius) * heightmapTextureHeight);
            int y1 = Mathf.FloorToInt((localPosition.y + radius) * heightmapTextureHeight);
            int x0 = Mathf.FloorToInt((localPosition.x - radius) * heightmapTextureWidth);
            int x1 = Mathf.FloorToInt((localPosition.x + radius) * heightmapTextureWidth);
            float radiusSqr = radius * radius;
            bool changes = false;
            for (int j = y0; j <= y1 && j < heightmapTextureHeight; j++) {
                if (j < 0) continue;
                int jj = j * heightmapTextureWidth;
                float dy = (j + 0.5f) / heightmapTextureHeight - localPosition.y;
                for (int k = x0; k <= x1 && k < heightmapTextureWidth; k++) {
                    if (k < 0) continue;
                    float dx = (k + 0.5f) / heightmapTextureWidth - localPosition.x;
                    float distSqr = dy * dy + dx * dx;
                    if (distSqr > radiusSqr) continue;
                    float elevation = heightMapColors[jj + k].r / 255f - baseElevation;
                    if (elevation < 0) {
                        elevation = 0;
                    }
                    viewportElevationPoints[jj + k] = elevation;
                    changes = true;
                }
            }

            if (changes) {
                MarkCustomRouteMatrixDirty();
                if (renderViewportIsEnabled) {
                    shouldCheckBoundaries = true; // updates viewport
                }
            }
        }

        #endregion

    }

}