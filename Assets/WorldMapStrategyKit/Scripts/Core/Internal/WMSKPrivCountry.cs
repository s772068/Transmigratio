// World Map Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WorldMapStrategyKit.Poly2Tri;
using WorldMapStrategyKit.ClipperLib;

namespace WorldMapStrategyKit {

    public partial class WMSK : MonoBehaviour {

        delegate void TestNeighbourSegment(Region region, int i, int j);


        const string COUNTRY_OUTLINE_GAMEOBJECT_NAME = "countryOutline";
        const string COUNTRY_SURFACES_ROOT_NAME = "surfaces";
        const string COUNTRY_ATTRIB_DEFAULT_FILENAME = "countriesAttrib";

        /// <summary>
        /// Country look up dictionary. Used internally for fast searching of country names.
        /// </summary>
        Dictionary<string, int> _countryLookup;
        List<int> _countriesOrderedBySize;

        int _lastCountryLookupCount = -1;

        int lastCountryLookupCount {
            get { return _lastCountryLookupCount; }
            set {
                if (value == -1)
                    RefreshCountryLookUp();
                else
                    _lastCountryLookupCount = value;
            }
        }

        Dictionary<string, int> countryLookup {
            get {
                if (_countries == null)
                    return _countryLookup;
                if (_countryLookup != null && _countries.Length == _lastCountryLookupCount)
                    return _countryLookup;
                RefreshCountryLookUp();
                return _countryLookup;
            }
        }

        List<int> countriesOrderedBySize {
            get {
                if (_lastCountryLookupCount == -1) {
                    RefreshCountryLookUp();
                }
                return _countriesOrderedBySize;
            }
        }


        // resources
        Material frontiersMat, hudMatCountry;

        // gameObjects
        GameObject countryRegionHighlightedObj;
        GameObject frontiersLayer;

        // maintains a reference to the country outline to hide it when zooming too much
        GameObject lastCountryOutlineRef;

        // caché and gameObject lifetime control
        [NonSerialized]
        public Vector3[][] frontiers;
        [NonSerialized]
        public int[][] frontiersIndices;
        [NonSerialized]
        public bool needOptimizeFrontiers = true;

        /// <summary>
        /// Ensures country index is between country array limits
        /// </summary>
        /// <param name="countryIndex"></param>
        /// <returns></returns>
        bool CheckCountryIndex(int countryIndex) {
            return countryIndex >= 0 && countryIndex < countries.Length;
        }

        /// <summary>
        /// Must be called internally when country list is changed (ie: a country has been deleted or added)
        /// </summary>
        void RefreshCountryLookUp() {
            if (_countries != null && _countries.Length > 0 && _countries[0] != null) {
                // Build dictionary for fast country object look up
                // Also build ordered index list of countries for allowing to highlight countries surrounded by other greater countries (smaller countries checked first).
                int countryCount = _countries.Length;
                if (_countryLookup == null) {
                    _countryLookup = new Dictionary<string, int>(countryCount);
                } else {
                    _countryLookup.Clear();
                }
                if (_countriesOrderedBySize == null) {
                    _countriesOrderedBySize = new List<int>(countryCount);
                } else {
                    _countriesOrderedBySize.Clear();
                }
                for (int k = 0; k < countryCount; k++) {
                    Country c = _countries[k];
                    _countryLookup.Add(c.name, k);
                    if (c.regions != null && c.mainRegionIndex >= 0 && c.mainRegionIndex < c.regions.Count) {
                        _countriesOrderedBySize.Add(k);
                    }
                }

                // Sort countries based on size
                _countriesOrderedBySize.Sort((int cIndex1, int cIndex2) => {
                    Country c1 = _countries[cIndex1];
                    Region r1 = c1.regions[c1.mainRegionIndex];
                    Country c2 = _countries[cIndex2];
                    Region r2 = c2.regions[c2.mainRegionIndex];
                    if (r1.rect2DArea < r2.rect2DArea) {
                        return -1;
                    } else if (r1.rect2DArea > r2.rect2DArea) {
                        return 1;
                    } else {
                        return 0;
                    }
                });

                // Update enclaves neighbours
                if (_enableEnclaves) {
                    int countriesLength = _countriesOrderedBySize.Count;
                    for (int smaller = 0; smaller < countriesLength; smaller++) {
                        Country smallerCountry = _countries[smaller];
                        if (smallerCountry.mainRegion == null) continue;
                        for (int bigger = smaller + 1; bigger < countriesLength; bigger++) {
                            Country biggerCountry = _countries[bigger];
                            if (biggerCountry.mainRegion != null && biggerCountry.mainRegion.Contains(smallerCountry.mainRegion)) {
                                if (!CountryIsNeighbour(smaller, bigger)) {
                                    CountryMakeNeighbours(smaller, bigger);
                                    break;
                                }
                            }
                        }
                    }
                }
            } else {
                _countryLookup = new Dictionary<string, int>(250);
                _countriesOrderedBySize = new List<int>(250);
            }
            _lastCountryLookupCount = _countryLookup.Count;

        }

        void ReadCountriesPackedString() {
            string frontiersFileName = _geodataResourcesPath + (_frontiersDetail == FRONTIERS_DETAIL.Low ? "/countries110" : "/countries10");
            TextAsset ta = Resources.Load<TextAsset>(frontiersFileName);
            if (ta != null) {
                SetCountryGeoData(ta.text);
                ReloadCountryAttributes();
                Resources.UnloadAsset(ta);
            }
        }

        void ReloadCountryAttributes() {
            TextAsset ta = Resources.Load<TextAsset>(_geodataResourcesPath + "/" + _countryAttributeFile);
            if (ta == null)
                return;
            SetCountriesAttributes(ta.text);
        }

        /// <summary>
        /// Computes surfaces for big countries
        /// </summary>
        void CountriesPrewarmBigSurfaces() {
            for (int k = 0; k < _countries.Length; k++) {
                int points = _countries[k].regions[_countries[k].mainRegionIndex].points.Length;
                if (points > 6000) {
                    ToggleCountrySurface(k, true, Misc.ColorClear);
                    ToggleCountrySurface(k, false, Misc.ColorClear);
                }
            }
        }

        /// <summary>
        /// Used internally by the Map Editor. It will recalculate de boundaries and optimize frontiers based on new data of countries array
        /// Note: this not redraws the map. Redraw must be called afterwards.
        /// </summary>
        public void RefreshCountryDefinition(int countryIndex, List<Region> filterRegions) {
            if (!ValidCountryIndex(countryIndex)) return;
            Country country = _countries[countryIndex];
            // Optimize all country frontiers but only if provided country is not empty (avoids unnecessary call when adding new empty countries and this method is called)
            if (!country.IsEmpty()) {
                RefreshCountryGeometry(country);
                OptimizeFrontiers(filterRegions);
            }
        }

        /// <summary>
        /// Used internally by the Map Editor. It will recalculate de boundaries and optimize frontiers based on new data of countries array
        /// </summary>
        public void RefreshCountryGeometry(Country country) {
            float maxVol = 0;
            if (country.IsEmpty())
                return;
            int regionCount = country.regions.Count;
            Vector2 min = Misc.Vector2one * 10;
            Vector2 max = -min;
            Vector2 minCountry = Misc.Vector2one * 10;
            Vector2 maxCountry = -minCountry;
            for (int r = 0; r < regionCount; r++) {
                Region countryRegion = country.regions[r];
                if (countryRegion.points == null)
                    continue;
                countryRegion.entity = country; // just in case one country has been deleted
                countryRegion.regionIndex = r;              // just in case a region has been deleted
                int coorCount = countryRegion.points.Length;
                min.x = min.y = 10f;
                max.x = max.y = -10;
                for (int c = 0; c < coorCount; c++) {
                    float x = countryRegion.points[c].x;
                    float y = countryRegion.points[c].y;
                    if (x < min.x)
                        min.x = x;
                    if (x > max.x)
                        max.x = x;
                    if (y < min.y)
                        min.y = y;
                    if (y > max.y)
                        max.y = y;
                }
                FastVector.Average(ref min, ref max, ref countryRegion.center); // countryRegion.center = (min + max) * 0.5f;

                // Calculate country bounding rect
                if (min.x < minCountry.x)
                    minCountry.x = min.x;
                if (min.y < minCountry.y)
                    minCountry.y = min.y;
                if (max.x > maxCountry.x)
                    maxCountry.x = max.x;
                if (max.y > maxCountry.y)
                    maxCountry.y = max.y;

                // Calculate bounding rect
                countryRegion.rect2D = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
                countryRegion.rect2DArea = countryRegion.rect2D.width * countryRegion.rect2D.height;
                float vol = FastVector.SqrDistance(ref max, ref min); // (max - min).sqrMagnitude;
                if (vol > maxVol) {
                    maxVol = vol;
                    country.mainRegionIndex = r;
                    country.mainRegion.ResetCentroid();
                    country.center = countryRegion.center;
                }
            }
            country.regionsRect2D = new Rect(minCountry.x, minCountry.y, Math.Abs(maxCountry.x - minCountry.x), Mathf.Abs(maxCountry.y - minCountry.y));
            lastCountryLookupCount = -1;
        }


        void TestNeighbourSegmentAny(Region region, int i, int j) {
            double v = ((double)region.points[i].x + (double)region.points[j].x) + (double)MAP_PRECISION * ((double)region.points[i].y + (double)region.points[j].y);
            Region neighbour;
            if (frontiersCacheHit.TryGetValue(v, out neighbour)) { // add neighbour references
                if (neighbour != region) {
                    if (!region.neighbours.Contains(neighbour)) {
                        region.neighbours.Add(neighbour);
                        neighbour.neighbours.Add(region);
                    }
                }
            } else {
                frontiersCacheHit[v] = region;
                frontiersPoints.Add(region.points[i]);
                frontiersPoints.Add(region.points[j]);
            }
        }


        void TestNeighbourSegmentInland(Region region, int i, int j) {
            double v = ((double)region.points[i].x + (double)region.points[j].x) + (double)MAP_PRECISION * ((double)region.points[i].y + (double)region.points[j].y);
            Region neighbour;
            if (frontiersCacheHit.TryGetValue(v, out neighbour)) { // add neighbour references
                if (neighbour != region) {
                    if (!region.neighbours.Contains(neighbour)) {
                        region.neighbours.Add(neighbour);
                        neighbour.neighbours.Add(region);
                    }
                    frontiersPoints.Add(region.points[i]);
                    frontiersPoints.Add(region.points[j]);
                }
            } else {
                frontiersCacheHit[v] = region;
            }
        }

        /// <summary>
        /// Prepare and cache meshes for frontiers. Used internally by extra components (decorator). This is called just after loading data or when hidding a country.
        /// </summary>
        public void OptimizeFrontiers() {
            OptimizeFrontiers(null);
        }

        void OptimizeFrontiers(List<Region> filterRegions) {
            if (frontiersPoints == null) {
                frontiersPoints = new List<Vector2>(325000); // needed for high-def resolution map
            } else {
                frontiersPoints.Clear();
            }
            if (frontiersCacheHit == null) {
                frontiersCacheHit = new Dictionary<double, Region>(163000); // needed for high-resolution map
            } else {
                frontiersCacheHit.Clear();
            }

            if (countries == null) return;
            int countriesCount = _countries.Length;
            for (int k = 0; k < countriesCount; k++) {
                Country country = _countries[k];
                int crCount = country.regions.Count;
                for (int r = 0; r < crCount; r++) {
                    Region region = country.regions[r];
                    if (filterRegions == null || filterRegions.Contains(region)) {
                        region.entity = country;
                        region.regionIndex = r;
                        region.neighbours.Clear();
                    }
                }
            }

            // Find neighbours by common frontiers
            TestNeighbourSegment testFunction;
            if (_frontiersCoastlines) {
                testFunction = TestNeighbourSegmentAny;
            } else {
                testFunction = TestNeighbourSegmentInland;
            }

            for (int k = 0; k < countriesCount; k++) {
                Country country = _countries[k];
                if (country.hidden)
                    continue;
                int crCount = country.regions.Count;
                for (int r = 0; r < crCount; r++) {
                    Region region = country.regions[r];
                    if (region.points == null || region.points.Length == 0)
                        continue;
                    if (filterRegions == null || filterRegions.Contains(region)) {
                        int numPoints = region.points.Length - 1;
                        for (int i = 0; i < numPoints; i++) {
                            testFunction(region, i, i + 1);
                        }
                        // Close the polygon
                        testFunction(region, numPoints, 0);
                    }
                }
            }

            // If frontier coastlines is disabled, make sure enclaves are also visible (countries without neighbours found)
            if (!_frontiersCoastlines) {
                for (int k = 0; k < countriesCount; k++) {
                    Country country = _countries[k];
                    if (country.hidden || country.neighbours.Length != 0)
                        continue;
                    int crCount = country.regions.Count;
                    for (int r = 0; r < crCount; r++) {
                        Region region = country.regions[r];
                        if (region.points == null || region.points.Length == 0)
                            continue;
                        if (filterRegions == null || filterRegions.Contains(region)) {
                            int numPoints = region.points.Length - 1;
                            for (int i = 0; i < numPoints; i++) {
                                frontiersPoints.Add(region.points[i]);
                                frontiersPoints.Add(region.points[i + 1]);
                            }
                            // Close the polygon
                            frontiersPoints.Add(region.points[numPoints]);
                            frontiersPoints.Add(region.points[0]);
                        }
                    }
                }
            }

            int meshGroups = (frontiersPoints.Count / 65000) + 1;
            int meshIndex = -1;
            frontiersIndices = new int[meshGroups][];
            frontiers = new Vector3[meshGroups][];
            for (int k = 0; k < frontiersPoints.Count; k += 65000) {
                int max = Mathf.Min(frontiersPoints.Count - k, 65000);
                frontiers[++meshIndex] = new Vector3[max];
                frontiersIndices[meshIndex] = new int[max];
                for (int j = k; j < k + max; j++) {
                    frontiers[meshIndex][j - k].x = frontiersPoints[j].x;
                    frontiers[meshIndex][j - k].y = frontiersPoints[j].y;
                    frontiersIndices[meshIndex][j - k] = j - k;
                }
            }
        }

        void ResortCountryProvinces(int countryIndex) {
            List<Province> provinces = new List<Province>(_countries[countryIndex].provinces);
            provinces.Sort(ProvinceSizeComparer);
            _countries[countryIndex].provinces = provinces.ToArray();
        }

        #region Drawing stuff

        const string FRONTIERS_MULTIPASS_SHADER = "WMSK/Unlit Country Frontiers Order 3";
        const string FRONTIERS_GEOMETRIC_SHADER = "WMSK/Unlit Country Frontiers Geom";

        void UpdateFrontiersMaterial() {
            if (frontiersMat == null)
                return;
            Shader actualShader = frontiersMat.shader;
            if (actualShader == null)
                return;
            bool supportsGeometryShaders = SystemInfo.supportsGeometryShaders;
            bool shaderIsGeometry = actualShader.name.Equals(FRONTIERS_GEOMETRIC_SHADER);
            if (_thickerFrontiers && !shaderIsGeometry && supportsGeometryShaders) {
                frontiersMat.shader = Shader.Find(FRONTIERS_GEOMETRIC_SHADER);
            } else if (!thickerFrontiers && (shaderIsGeometry || !supportsGeometryShaders)) {
                frontiersMat.shader = Shader.Find(FRONTIERS_MULTIPASS_SHADER);
            }
            frontiersMat.color = _frontiersColor;
            frontiersMat.SetColor(ShaderParams.OuterColor, frontiersColorOuter);
            frontiersMat.SetFloat(ShaderParams.Thickness, _frontiersWidth);
            frontiersMat.SetFloat(ShaderParams.MaxPixelWidth, _frontiersMaxPixelWidth);
            UpdateShadersLOD();
            shouldCheckBoundaries = true;
        }


        int GetCacheIndexForCountryRegion(int countryIndex, int regionIndex) {
            return countryIndex * 1000 + regionIndex;
        }


        void DrawFrontiers() {

            if (!gameObject.activeInHierarchy)
                return;
            if (!_showFrontiers)
                return;

            // Create frontiers layer
            Transform t = transform.Find("Frontiers");
            if (t != null) {
                DestroyRecursive(t.gameObject);
            }

            if (needOptimizeFrontiers) {
                OptimizeFrontiers();    // lazy optimization
            }

            frontiersLayer = new GameObject("Frontiers");
            if (disposalManager != null) {
                disposalManager.MarkForDisposal(frontiersLayer); // frontiersLayer.hideFlags = HideFlags.DontSave;
            }
            frontiersLayer.transform.SetParent(transform, false);
            frontiersLayer.transform.localPosition = Misc.Vector3zero;
            frontiersLayer.transform.localRotation = Quaternion.Euler(Misc.Vector3zero);
            frontiersLayer.layer = gameObject.layer;

            if (frontiers != null) {
                for (int k = 0; k < frontiers.Length; k++) {
                    GameObject flayer = new GameObject("flayer");
                    if (disposalManager != null)
                        disposalManager.MarkForDisposal(flayer); // flayer.hideFlags = HideFlags.DontSave;
                    flayer.transform.SetParent(frontiersLayer.transform, false);
                    flayer.transform.localPosition = Misc.Vector3zero;
                    flayer.transform.localRotation = Quaternion.Euler(Misc.Vector3zero);
                    flayer.layer = frontiersLayer.layer;

                    Mesh mesh = new Mesh();
                    mesh.vertices = frontiers[k];
                    mesh.SetIndices(frontiersIndices[k], MeshTopology.Lines, 0);
                    mesh.RecalculateBounds();
                    if (disposalManager != null)
                        disposalManager.MarkForDisposal(mesh); //mesh.hideFlags = HideFlags.DontSave;

                    MeshFilter mf = flayer.AddComponent<MeshFilter>();
                    mf.sharedMesh = mesh;

                    MeshRenderer mr = flayer.AddComponent<MeshRenderer>();
                    mr.receiveShadows = false;
                    mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    mr.sharedMaterial = frontiersMat;
                }
            }

            // Toggle frontiers visibility layer according to settings
            frontiersLayer.SetActive(_showFrontiers);
        }

        #endregion

        #region Country highlighting

        void HideCountryRegionHighlight() {

            HideProvinceRegionHighlight();
            HideCityHighlight();

            if (_provinceLabelsVisibility == PROVINCE_LABELS_VISIBILITY.Automatic) {
                DestroyProvinceLabels();
            }

            if (provincesObj != null && !_drawAllProvinces) {
                HideProvinces();
            }

            if (_enableCountryHighlight && ValidCountryIndex(_countryHighlightedIndex) && _countries[_countryHighlightedIndex].allowHighlight) {
                if (_countryRegionHighlightedIndex >= 0) {
                    if (_countryHighlighted != null) {
                        int rCount = _countryHighlighted.regions.Count;
                        for (int k = 0; k < rCount; k++) {
                            HideCountryRegionHighlightSingle(k);
                        }
                    }
                    countryRegionHighlightedObj = null;
                }
            }

            // Raise exit event
            if (ValidCountryRegionIndex(_countryLastOver, _countryRegionLastOver)) {
                if (OnCountryExit != null) {
                    OnCountryExit(_countryLastOver, _countryRegionLastOver);
                }
                if (OnRegionExit != null) {
                    OnRegionExit(_countries[_countryLastOver].regions[_countryRegionLastOver]);
                }
            }

            _countryHighlighted = null;
            _countryHighlightedIndex = -1;
            _countryRegionHighlighted = null;
            _countryRegionHighlightedIndex = -1;
            _countryLastOver = -1;
            _countryRegionLastOver = -1;
        }

        void HideCountryRegionHighlightSingle(int regionIndex) {
            int cacheIndex = GetCacheIndexForCountryRegion(_countryHighlightedIndex, regionIndex);
            Region region = _countryHighlighted.regions[regionIndex];
            GameObject surf = region.surface;
            if (surf != null) {
                Material mat = region.customMaterial;
                if (mat != null) {
                    ApplyMaterialToSurface(surf, mat);
                } else {
                    surf.SetActive(false);
                }
            }
            HideCountryRegionOutline(cacheIndex.ToString(), region);
            lastCountryOutlineRef = null;
        }

        void HideCountryRegionOutline(string entityId, Region region) {

            // Hides country outline
            if (region.customBorder.texture == null) {
                HideRegionObject(entityId, null, COUNTRY_OUTLINE_GAMEOBJECT_NAME);
            } else {
                // Stop animation?
                if (region.customBorder.animationSpeed == 0) {
                    Transform t = surfacesLayer.transform.Find(entityId + "/" + COUNTRY_OUTLINE_GAMEOBJECT_NAME);
                    if (t != null) {
                        Material mat = t.GetComponent<LineRenderer>().sharedMaterial;
                        mat.SetFloat("_AnimationSpeed", 0);
                        region.customBorder.animationAcumOffset += (time - region.customBorder.animationStartTime) * _outlineAnimationSpeed;
                        mat.SetFloat("_AnimationAcumOffset", region.customBorder.animationAcumOffset);
                    }
                }
            }
        }

        /// <summary>
        /// Disables all country regions highlights. This doesn't remove custom materials.
        /// </summary>
        public void HideCountryRegionHighlights(bool destroyCachedSurfaces) {
            HideCountryRegionHighlight();
            if (_countries == null)
                return;
            int countriesLength = _countries.Length;
            for (int c = 0; c < countriesLength; c++) {
                Country country = _countries[c];
                if (country == null || country.regions == null) continue;
                int regionsCount = country.regions.Count;
                for (int cr = 0; cr < regionsCount; cr++) {
                    Region region = country.regions[cr];
                    GameObject surf = region.surface;
                    if (surf != null) {
                        if (destroyCachedSurfaces) {
                            DestroyImmediate(surf);
                        } else {
                            if (region.customMaterial == null) {
                                surf.SetActive(false);
                            } else {
                                ApplyMaterialToSurface(surf, region.customMaterial);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Highlights the country region specified. Returns the generated highlight surface gameObject.
        /// Internally used by the Map UI and the Editor component, but you can use it as well to temporarily mark a country region.
        /// </summary>
        /// <param name="refreshGeometry">Pass true only if you're sure you want to force refresh the geometry of the highlight (for instance, if the frontiers data has changed). If you're unsure, pass false.</param>
        public GameObject HighlightCountryRegion(int countryIndex, int regionIndex, bool refreshGeometry, bool drawOutline) {
            if (_countryHighlightedIndex == countryIndex && _countryRegionHighlightedIndex == regionIndex && !refreshGeometry)
                return countryRegionHighlightedObj;
            if (_countryHighlightedIndex >= 0) // hides both surface and outline
                HideCountryRegionHighlight();
            if (countryIndex < 0 || countryIndex >= _countries.Length || regionIndex < 0 || regionIndex >= _countries[countryIndex].regions.Count || _countries[countryIndex].isPool)
                return null;

            if (_enableCountryHighlight && _countries[countryIndex].allowHighlight) {

                if (OnCountryHighlight != null) {
                    bool allowHighlight = true;
                    OnCountryHighlight(countryIndex, regionIndex, ref allowHighlight);
                    if (!allowHighlight)
                        return null;
                }

                Country country = _countries[countryIndex];
                Region region = country.regions[regionIndex];
                if (region.surfaceisMerged) {
                    countryRegionHighlightedObj = HighlightCountryRegionSingle(countryIndex, country.mainRegionIndex, refreshGeometry, false);
                    if (drawOutline) {
                        int cacheIndex = GetCacheIndexForCountryRegion(countryIndex, regionIndex);
                        DrawCountryRegionOutline(cacheIndex.ToString(), _countries[countryIndex].regions[regionIndex], true, _outlineAnimationSpeed);
                    }
                } else {
                    countryRegionHighlightedObj = HighlightCountryRegionSingle(countryIndex, regionIndex, refreshGeometry, drawOutline);
                    if (_highlightAllCountryRegions) {
                        int rCount = country.regions.Count;
                        for (int r = 0; r < rCount; r++) {
                            if (r != regionIndex && !country.regions[r].surfaceisMerged) {
                                HighlightCountryRegionSingle(countryIndex, r, refreshGeometry, drawOutline);
                            }
                        }
                    }
                }
            }

            _countryHighlightedIndex = countryIndex;
            _countryRegionHighlighted = _countries[countryIndex].regions[regionIndex];
            _countryRegionHighlightedIndex = regionIndex;
            _countryHighlighted = _countries[countryIndex];
            return countryRegionHighlightedObj;
        }

        GameObject HighlightCountryRegionSingle(int countryIndex, int regionIndex, bool refreshGeometry, bool drawOutline) {

            int cacheIndex = GetCacheIndexForCountryRegion(countryIndex, regionIndex);

            // Draw outline?
            if (drawOutline) {
                DrawCountryRegionOutline(cacheIndex.ToString(), _countries[countryIndex].regions[regionIndex], true, _outlineAnimationSpeed);
            }

            Region region = countries[countryIndex].regions[regionIndex];
            GameObject surf = region.surface;
            bool existsInCache = surf != null;
            if (refreshGeometry && existsInCache) {
                DestroyImmediate(surf);
                existsInCache = false;
            }
            bool doHighlight = _highlightCountryRecolor;
            if (doHighlight) {
                if (_highlightMaxScreenAreaSize < 1f) {
                    // Check screen area size
                    doHighlight = CheckScreenAreaSizeOfRegion(region);
                }
                if (doHighlight) {
                    hudMatCountry.mainTexture = null;
                    if (existsInCache) {
                        if (!surf.activeSelf) {
                            surf.SetActive(true);
                        }
                        if (_highlightCountryKeepTexture && region.customMaterial != null) {
                            hudMatCountry.mainTexture = region.customMaterial.mainTexture;
                        }
                        Renderer rr = surf.GetComponent<Renderer>();
                        if (rr.sharedMaterial != hudMatCountry) {
                            rr.sharedMaterial = hudMatCountry;
                        }
                        return surf;
                    }
                    surf = GenerateCountryRegionSurface(countryIndex, regionIndex, hudMatCountry, Misc.Vector2one, Misc.Vector2zero, 0);
                }
            }
            return surf;
        }

        GameObject GenerateCountryRegionSurface(int countryIndex, int regionIndex, Material material) {
            return GenerateCountryRegionSurface(countryIndex, regionIndex, material, Misc.Vector2one, Misc.Vector2zero, 0);
        }

        void CountrySubstractProvinceEnclaves(int countryIndex, Region region, Poly2Tri.Polygon poly) {
            List<Region> negativeRegions = new List<Region>();
            for (int op = 0; op < _countries.Length; op++) {
                if (op == countryIndex)
                    continue;
                Country opCountry = _countries[op];
                if (opCountry.provinces == null)
                    continue;
                if (opCountry.regionsRect2D.Overlaps(region.rect2D, true)) {
                    int provCount = opCountry.provinces.Length;
                    for (int p = 0; p < provCount; p++) {
                        Province oProv = opCountry.provinces[p];
                        if (oProv.regions == null)
                            ReadProvincePackedString(oProv);
                        if (oProv.regions == null) {
                            continue;
                        }
                        if (oProv.mainRegionIndex < 0 || oProv.mainRegionIndex >= oProv.regions.Count) {
                            continue;
                        }
                        Region oProvRegion = oProv.regions[oProv.mainRegionIndex];
                        if (RegionCanBeHole(oProvRegion) && region.Contains(oProvRegion)) { // just check main region of province for speed purposes
                            negativeRegions.Add(oProvRegion.Clone());
                        }
                    }
                }
            }
            // Collapse negative regions in big holes
            for (int nr = 0; nr < negativeRegions.Count - 1; nr++) {
                for (int nr2 = nr + 1; nr2 < negativeRegions.Count; nr2++) {
                    if (negativeRegions[nr].Intersects(negativeRegions[nr2])) {
                        Clipper clipper = new Clipper();
                        clipper.AddPath(negativeRegions[nr], PolyType.ptSubject);
                        clipper.AddPath(negativeRegions[nr2], PolyType.ptClip);
                        clipper.Execute(ClipType.ctUnion);
                        negativeRegions.RemoveAt(nr2);
                        nr = -1;
                        break;
                    }
                }
            }

            // Substract holes
            for (int r = 0; r < negativeRegions.Count; r++) {
                int pointCount = negativeRegions[r].points.Length;
                Vector2[] pp = new Vector2[pointCount];
                for (int p = 0; p < pointCount; p++) {
                    Vector2 point = negativeRegions[r].points[p];
                    Vector2 midPoint = negativeRegions[r].center;
                    pp[p] = point + (midPoint - point) * 0.0001f; // prevents Poly2Tri issues when enclave boarders are to near from region borders
                }
                Poly2Tri.Polygon polyHole = new Poly2Tri.Polygon(pp);
                poly.AddHole(polyHole);
            }
        }

        void CountrySubstractCountryEnclaves(int countryIndex, Region region, Poly2Tri.Polygon poly) {
            List<Region> negativeRegions = new List<Region>();
            int countryCount = countriesOrderedBySize.Count;
            for (int ops = 0; ops < countryCount; ops++) {
                int op = _countriesOrderedBySize[ops];
                if (op == countryIndex)
                    continue;
                Country opCountry = _countries[op];
                Region opCountryRegion = opCountry.regions[opCountry.mainRegionIndex];
                if (opCountryRegion.points.Length >= 5 && opCountry.mainRegion.rect2DArea < region.rect2DArea && opCountryRegion.rect2D.Overlaps(region.rect2D, true)) {
                    if (RegionCanBeHole(opCountryRegion) && region.Contains(opCountryRegion)) { // just check main region of province for speed purposes
                        negativeRegions.Add(opCountryRegion.Clone());
                    }
                }
            }
            // Collapse negative regions in big holes
            for (int nr = 0; nr < negativeRegions.Count - 1; nr++) {
                for (int nr2 = nr + 1; nr2 < negativeRegions.Count; nr2++) {
                    if (negativeRegions[nr].Intersects(negativeRegions[nr2])) {
                        Clipper clipper = new Clipper();
                        clipper.AddPath(negativeRegions[nr], PolyType.ptSubject);
                        clipper.AddPath(negativeRegions[nr2], PolyType.ptClip);
                        clipper.Execute(ClipType.ctUnion);
                        negativeRegions.RemoveAt(nr2);
                        nr = -1;
                        break;
                    }
                }
            }

            // Substract holes
            for (int r = 0; r < negativeRegions.Count; r++) {
                Poly2Tri.Polygon polyHole = new Poly2Tri.Polygon(negativeRegions[r].points);
                poly.AddHole(polyHole);
            }
        }


        GameObject DrawCountryRegionOutline(string entityId, Region region, bool overridesAnimationSpeed = false, float animationSpeed = 0f) {
            if (_showTiles && _currentZoomLevel > _tileLinesMaxZoomLevel)
                return null;

            GameObject boldFrontiers = DrawRegionOutlineMesh(COUNTRY_OUTLINE_GAMEOBJECT_NAME, region, overridesAnimationSpeed, animationSpeed);
            ParentObjectToRegion(entityId, null, boldFrontiers);
            lastCountryOutlineRef = boldFrontiers;
            return boldFrontiers;
        }


        GameObject GenerateCountryRegionSurface(int countryIndex, int regionIndex, Material material, Vector2 textureScale, Vector2 textureOffset, float textureRotation) {
            if (!ValidCountryRegionIndex(countryIndex, regionIndex)) return null;
            Country country = _countries[countryIndex];

            Region region = country.regions[regionIndex];
            if (region.points.Length < 3) {
                return null;
            }

            CheckPendingRedraw();

            Poly2Tri.Polygon poly = new Poly2Tri.Polygon(region.points);
            // Extracts enclaves from main region
            if (_enableEnclaves && regionIndex == country.mainRegionIndex) {
                // Remove negative provinces
                if (_showProvinces) {
                    CountrySubstractProvinceEnclaves(countryIndex, region, poly);
                } else {
                    CountrySubstractCountryEnclaves(countryIndex, region, poly);
                }
            }
            P2T.Triangulate(poly);

            // Prepare surface cache entry and deletes older surface if exists
            int cacheIndex = GetCacheIndexForCountryRegion(countryIndex, regionIndex);
            string id = cacheIndex.ToString();

            // Creates surface mesh
            GameObject surf = Drawing.CreateSurface(id, poly, material, region.rect2D, textureScale, textureOffset, textureRotation, disposalManager);
            ParentObjectToRegion(id, COUNTRY_SURFACES_ROOT_NAME, surf);
            region.surface = surf;

            return surf;
        }

        /// <summary>
        /// Colorize specified region of a country by indexes.
        /// </summary>
        GameObject internal_ToggleCountryRegionSurface(int countryIndex, int regionIndex, bool visible, Color color, Texture2D texture = null, Vector2 textureScale = default, Vector2 textureOffset = default, float textureRotation = 0) {
            if (!ValidCountryRegionIndex(countryIndex, regionIndex)) return null;

            if (!visible) {
                HideCountryRegionSurface(countryIndex, regionIndex);
                return null;
            }

            Region region = _countries[countryIndex].regions[regionIndex];
            GameObject surf = region.surface;

            if (region.surfaceIsDirty) {
                DestroySafe(surf);
            }

            // Should the surface be recreated?
            Material surfMaterial;

            if (surf != null) {
                surfMaterial = surf.GetComponent<Renderer>().sharedMaterial;
                if (texture != null && (textureScale != region.customTextureScale || textureOffset != region.customTextureOffset || textureRotation != region.customTextureRotation || surfMaterial.name.Equals(coloredMat.name))) {
                    DestroyImmediate(surf);
                    surf = null;
                }
            }
            // If it exists, activate and check proper material, if not create surface
            bool isHighlighted = countryHighlightedIndex == countryIndex && (countryRegionHighlightedIndex == regionIndex || _highlightAllCountryRegions) && _enableCountryHighlight && _countries[countryIndex].allowHighlight == true;
            if (surf != null) {
                if (!surf.activeSelf)
                    surf.SetActive(true);
                // Check if material is ok
                Material goodMaterial = GetColoredTexturedMaterial(color, texture);
                region.customMaterial = goodMaterial;
                Renderer renderer = surf.GetComponent<Renderer>();
                surfMaterial = renderer.sharedMaterial;
                if ((texture == null && !surfMaterial.name.Equals(coloredMat.name)) || (texture != null && !surfMaterial.name.Equals(texturizedMat.name))
                    || (surfMaterial.color != color && !isHighlighted) || (texture != null && region.customMaterial.mainTexture != texture)) {
                    ApplyMaterialToSurface(surf, goodMaterial);
                }
            } else {
                surfMaterial = GetColoredTexturedMaterial(color, texture);
                surf = GenerateCountryRegionSurface(countryIndex, regionIndex, surfMaterial, textureScale, textureOffset, textureRotation);
                region.customMaterial = surfMaterial;
                region.customTextureOffset = textureOffset;
                region.customTextureRotation = textureRotation;
                region.customTextureScale = textureScale;
            }
            // If it was highlighted, highlight it again
            if (region.customMaterial != null && isHighlighted && region.customMaterial.color != hudMatCountry.color) {
                Material clonedMat = Instantiate(region.customMaterial);
                if (disposalManager != null)
                    disposalManager.MarkForDisposal(clonedMat); // clonedMat.hideFlags = HideFlags.DontSave;
                clonedMat.name = region.customMaterial.name;
                clonedMat.color = hudMatCountry.color;
                ApplyMaterialToSurface(surf, clonedMat);
                countryRegionHighlightedObj = surf;
            }
            return surf;
        }

        #endregion

        #region Country manipulation

        void internal_CountryTransferProvinces(int targetCountryIndex, List<Province> provinces, int sourceCountryIndex) {

            // Remove provinces form source country
            Country sourceCountry = _countries[sourceCountryIndex];
            if (sourceCountry.provinces != null) {
                List<Province> sourceProvinces = new List<Province>(sourceCountry.provinces);
                foreach (Province p in provinces) {
                    if (p.countryIndex == sourceCountryIndex && sourceProvinces.Contains(p)) {
                        sourceProvinces.Remove(p);
                    }
                }
                if (sourceCountry.provinces.Length != sourceProvinces.Count) {
                    sourceCountry.provinces = sourceProvinces.ToArray();
                }
            }

            // Adds provinces to target country
            Country targetCountry = _countries[targetCountryIndex];
            List<Province> destProvinces;
            if (targetCountry.provinces == null) {
                destProvinces = new List<Province>();
            } else {
                destProvinces = new List<Province>(targetCountry.provinces);
            }
            foreach (Province p in provinces) {
                if (p.countryIndex == sourceCountryIndex) {
                    destProvinces.Add(p);
                }
            }
            destProvinces.Sort(ProvinceSizeComparer);
            targetCountry.provinces = destProvinces.ToArray();

            // Add all province regions to target country's polygon - only if the province is touching or crossing target country frontier
            foreach (Province p in provinces) {
                if (p.countryIndex != sourceCountryIndex) continue;
                foreach (Region pr in p.regions) {
                    bool regionAddedToCountry = false;
                    foreach (Region cr in targetCountry.regions) {
                        if (pr.Intersects(cr)) {
                            RegionMagnet(pr, cr);
                            regionAddedToCountry = true;
                            Clipper clipper = new Clipper();
                            clipper.AddPath(cr, PolyType.ptSubject);
                            clipper.AddPath(pr, PolyType.ptClip);
                            clipper.Execute(ClipType.ctUnion);
                            break;
                        }
                    }
                    if (!regionAddedToCountry) {
                        // Add new region to country: this is a new physical region at the country frontier level - the province will maintain its region at the province level
                        Region newCountryRegion = new Region(targetCountry, targetCountry.regions.Count);
                        newCountryRegion.UpdatePointsAndRect(pr);
                        targetCountry.regions.Add(newCountryRegion);
                    }
                }
            }

            // Fusion any adjacent regions that results from merge operation
            MergeAdjacentRegions(targetCountry);

            // Finds the source country region that could overlap with target country, then substract
            // This handles province extraction but also extraction of two or more provinces than get merged with CountryMergeAdjacentRegions - the result of this merge, needs to be substracted from source country
            bool mustSanitizeSourceCountry = false;
            if (!sourceCountry.isPool) {
                Clipper clipper = new Clipper();
                bool mustSubstract = false;
                for (int k = 0; k < targetCountry.regions.Count; k++) {
                    if (sourceCountry.Intersects(targetCountry.regions[k])) {
                        if (!mustSubstract) {
                            mustSubstract = true;
                            clipper.AddPaths(sourceCountry.regions, PolyType.ptSubject);
                        }
                        clipper.AddPath(targetCountry.regions[k], PolyType.ptClip);
                    }
                }
                if (mustSubstract) {
                    clipper.Execute(ClipType.ctDifference, sourceCountry);
                    mustSanitizeSourceCountry = true;
                }

                // Remove invalid regions from source country
                for (int k = 0; k < sourceCountry.regions.Count; k++) {
                    Region otherSourceRegion = sourceCountry.regions[k];
                    if (!otherSourceRegion.sanitized && otherSourceRegion.points.Length < 5) {
                        sourceCountry.regions.RemoveAt(k);
                        k--;
                    }
                }
            }

            // Update cities
            int cityCount = cities.Length;
            for (int k = 0; k < cityCount; k++) {
                City city = _cities[k];
                if (city.countryIndex == sourceCountryIndex) {
                    foreach (Province p in provinces) {
                        if (p.countryIndex == sourceCountryIndex && city.province.Equals(p.name)) {
                            city.countryIndex = targetCountryIndex;
                            break;
                        }
                    }
                }
            }

            // Update mount points
            int mountPointsCount = mountPoints.Count;
            for (int k = 0; k < mountPointsCount; k++) {
                MountPoint mp = mountPoints[k];
                if (mp.countryIndex == sourceCountryIndex) {
                    foreach (Province p in provinces) {
                        if (p.countryIndex == sourceCountryIndex && mp.provinceIndex == GetProvinceIndex(p)) {
                            mp.countryIndex = targetCountryIndex;
                            break;
                        }
                    }
                }
            }

            // Update source country definition
            if (!sourceCountry.isPool) {
                if (sourceCountry.regions.Count == 0) {
                    internal_CountryDelete(sourceCountryIndex, false);
                    if (targetCountryIndex > sourceCountryIndex)
                        targetCountryIndex--;
                } else {
                    if (mustSanitizeSourceCountry) {
                        RegionSanitize(sourceCountry.regions);
                    }
                    RefreshCountryGeometry(sourceCountry);
                }
            }

            // Update target country definition
            RefreshCountryGeometry(targetCountry);
            for (int k = 0; k < provinces.Count; k++) {
                Province p = provinces[k];
                if (p.countryIndex == sourceCountryIndex) {
                    p.countryIndex = targetCountryIndex;
                    provinces.RemoveAt(k);
                    k--;
                }
            }
        }

        /// <summary>
        /// Deletes the country. Optionally also delete its dependencies (provinces, cities, mountpoints).
        /// This internal method does not refresh cachés.
        /// </summary>
        bool internal_CountryDelete(int countryIndex, bool deleteDependencies) {
            if (!ValidCountryIndex(countryIndex)) return false;

            Country country = countries[countryIndex];
            country.DestroySurfaces();

            // Update dependencies
            if (deleteDependencies) {
                List<Province> newProvinces = new List<Province>(provinces.Length);
                int k;
                for (k = 0; k < provinces.Length; k++) {
                    if (provinces[k].countryIndex != countryIndex)
                        newProvinces.Add(provinces[k]);
                }
                provinces = newProvinces.ToArray();
                lastProvinceLookupCount = -1;

                k = -1;
                List<City> cities = new List<City>(this.cities);
                while (++k < cities.Count) {
                    if (cities[k].countryIndex == countryIndex) {
                        cities.RemoveAt(k);
                        k--;
                    }
                }
                this.cities = cities.ToArray();
                lastCityLookupCount = -1;

                k = -1;
                while (++k < mountPoints.Count) {
                    if (mountPoints[k].countryIndex == countryIndex) {
                        mountPoints.RemoveAt(k);
                        k--;
                    }
                }
            }

            // Updates provinces reference to country
            for (int k = 0; k < provinces.Length; k++) {
                if (provinces[k].countryIndex > countryIndex)
                    provinces[k].countryIndex--;
            }

            // Updates country index in cities
            for (int k = 0; k < cities.Length; k++) {
                if (_cities[k].countryIndex > countryIndex) {
                    _cities[k].countryIndex--;
                }
            }
            // Updates country index in mount points
            if (mountPoints != null) {
                for (int k = 0; k < mountPoints.Count; k++) {
                    if (mountPoints[k].countryIndex > countryIndex) {
                        mountPoints[k].countryIndex--;
                    }
                }
            }

            // Excludes country from new array
            List<Country> newCountries = new List<Country>(_countries.Length);
            for (int k = 0; k < _countries.Length; k++) {
                if (k != countryIndex)
                    newCountries.Add(_countries[k]);
            }
            countries = newCountries.ToArray();
            return true;
        }


        #endregion

    }

}