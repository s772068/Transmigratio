using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {
    public partial class WMSK_Editor : MonoBehaviour {

        public int provinceIndex = -1, provinceRegionIndex = -1;
        public int GUIProvinceIndex;
        public string GUIProvinceName = "";
        public string GUIProvinceNewName = "";
        public string GUIProvinceToNewCountryName = "";
        public int GUIProvinceTransferToCountryIndex = -1;
        public int GUIProvinceTransferToProvinceIndex = -1;
        // if there's any pending change to be saved
        public bool provinceChanges;
        // if there's any pending change to be saved
        public bool provinceAttribChanges;
        public readonly List<Region> selectedProvincesRegions = new List<Region>();
        public bool showProvinceNames = true;

        int lastProvinceCount = -1;
        string[] _provinceNames;
        readonly string[] emptyStringArray = new string[0];

        public string[] provinceNames {
            get {
                if (countryIndex == -1 || map.countries[countryIndex].provinces == null) {
                    return emptyStringArray;
                }
                if (_provinceNames == null || lastProvinceCount != map.countries[countryIndex].provinces.Length) {
                    provinceIndex = -1;
                    ReloadProvinceNames();
                }
                return _provinceNames;
            }
        }


        #region Editor functionality


        public void ClearProvinceSelection() {
            map.HideProvinceRegionHighlights(true);
            map.HideProvinces();
            for (int k = 0; k < selectedProvincesRegions.Count; k++) {
                Region region = selectedProvincesRegions[k];
                Province province = (Province)region.entity;
                int provinceIndex = map.GetProvinceIndex(province);
                map.ToggleProvinceRegionSurface(provinceIndex, region.regionIndex, false, map.provincesFillColor);
            }
            selectedProvincesRegions.Clear();
            provinceIndex = -1;
            provinceRegionIndex = -1;
            GUIProvinceName = "";
            GUIProvinceNewName = "";
            GUIProvinceToNewCountryName = "";
            GUIProvinceIndex = -1;
        }

        public bool ProvinceSelectByScreenClick(Ray ray) {
            int targetProvinceIndex, targetRegionIndex;
            if (map.GetProvinceIndex(ray, out targetProvinceIndex, out targetRegionIndex)) {

                if (selectedCountriesRegions.Count > 0) {
                    ClearCountrySelection();
                }

                if (countryIndex != map.provinces[targetProvinceIndex].countryIndex) {
                    countryIndex = map.provinces[targetProvinceIndex].countryIndex;
                    countryRegionIndex = -1;
                    CountryRegionSelect();
                }
                provinceIndex = targetProvinceIndex;
                if (provinceIndex >= 0 && countryIndex != map.provinces[provinceIndex].countryIndex) { // sanity check
                    ClearSelection();
                    countryIndex = map.provinces[provinceIndex].countryIndex;
                    countryRegionIndex = -1;
                    CountryRegionSelect();
                }
                provinceRegionIndex = targetRegionIndex;

                Event e = Event.current;
                if (e != null) {
                    if (!e.control) {
                        selectedProvincesRegions.Clear();
                    }
                    Region region = map.provinces[provinceIndex].regions[provinceRegionIndex];
                    if (selectedProvincesRegions.Contains(region)) {
                        selectedProvincesRegions.Remove(region);
                    } else {
                        selectedProvincesRegions.Add(region);
                    }
                }

                ProvinceRegionSelect();
                return true;
            }
            return false;
        }

        bool GetProvinceIndexByGUISelection() {
            if (GUIProvinceIndex < 0 || GUIProvinceIndex >= provinceNames.Length)
                return false;
            string[] s = _provinceNames[GUIProvinceIndex].Split(new char[] {
                '(',
                ')'
            }, System.StringSplitOptions.RemoveEmptyEntries);
            if (s.Length >= 2) {
                GUIProvinceName = s[0].Trim();
                if (int.TryParse(s[1], out provinceIndex)) {
                    provinceRegionIndex = map.provinces[provinceIndex].mainRegionIndex;
                    return true;
                }
            }
            return false;
        }

        public void ProvinceSelectByCombo(int selection) {
            GUIProvinceName = "";
            GUIProvinceIndex = selection;
            selectedProvincesRegions.Clear();
            if (GetProvinceIndexByGUISelection()) {
                if (Application.isPlaying) {
                    map.BlinkProvince(provinceIndex, Color.black, Color.green, 1.2f, 0.2f);
                }
            }
            ProvinceRegionSelect();
        }

        public void ReloadProvinceNames() {
            if (map == null || map.provinces == null || countryIndex < 0 || countryIndex >= map.countries.Length) {
                return;
            }
            string oldProvinceTransferName = GetProvinceTransferIndexByGUISelection();
            string oldProvinceMergeTransferName = GetProvinceMergeIndexByGUISelection();
            _provinceNames = map.GetProvinceNames(countryIndex);
            lastProvinceCount = _provinceNames.Length;
            lastMountPointCount = -1;
            SyncGUIProvinceTransferSelection(oldProvinceTransferName);
            SyncGUIProvinceMergeSelection(oldProvinceMergeTransferName);
            ProvinceRegionSelect(); // refresh selection
        }

        public void ProvinceRegionSelect() {
            if (countryIndex < 0 || countryIndex >= map.countries.Length || provinceIndex < 0 || provinceIndex >= map.provinces.Length || editingMode != EDITING_MODE.PROVINCES)
                return;

            // Checks country selected is correct
            Province province = map.provinces[provinceIndex];
            if (province.countryIndex != countryIndex) {
                ClearSelection();
                countryIndex = province.countryIndex;
                countryRegionIndex = map.countries[countryIndex].mainRegionIndex;
                CountryRegionSelect();
            }

            // Just in case makes GUICountryIndex selects appropiate value in the combobox
            GUIProvinceName = province.name;
            SyncGUIProvinceSelection();
            if (provinceIndex >= 0 && provinceIndex < map.provinces.Length) {
                GUIProvinceNewName = province.name;
                ProvinceHighlightSelection();
            }
        }

        void ProvinceHighlightSelection() {

            if (highlightedRegions == null)
                highlightedRegions = new List<Region>();
            else
                highlightedRegions.Clear();
            map.HideProvinceRegionHighlights(true);

            if (provinceIndex < 0 || provinceIndex >= map.provinces.Length || countryIndex < 0 || countryIndex >= map.countries.Length || map.countries[countryIndex].provinces == null ||
                provinceRegionIndex < 0 || map.provinces[provinceIndex].regions == null || provinceRegionIndex >= map.provinces[provinceIndex].regions.Count)
                return;

            if (selectedProvincesRegions.Count > 0) {
                // Multi-select
                for (int k = 0; k < selectedProvincesRegions.Count; k++) {
                    Region region = selectedProvincesRegions[k];
                    int pindex = map.GetProvinceIndex((Province)region.entity);
                    if (pindex == provinceIndex && region.regionIndex == provinceRegionIndex)
                        continue;
                    map.ToggleProvinceRegionSurface(pindex, region.regionIndex, true, map.provincesFillColor);
                }
            }

            // Highlight current province
            for (int p = 0; p < map.countries[countryIndex].provinces.Length; p++) {
                Province province = map.countries[countryIndex].provinces[p];
                if (province.regions == null)
                    continue;
                // if province is current province then highlight it
                if (province.name.Equals(map.provinces[provinceIndex].name)) {
                    map.HighlightProvinceRegion(provinceIndex, provinceRegionIndex, false);
                    highlightedRegions.Add(map.provinces[provinceIndex].regions[provinceRegionIndex]);
                } else {
                    // if this province belongs to the country but it's not current province, add to the collection of highlighted (not colorize because country is already colorized and that includes provinces area)
                    if (province.mainRegionIndex >= 0 && province.mainRegionIndex < province.regions.Count) {
                        highlightedRegions.Add(province.regions[province.mainRegionIndex]);
                    }
                }
            }


            shouldHideEditorMesh = true;
        }

        void SyncGUIProvinceSelection() {
            // recover GUI country index selection
            if (GUIProvinceName.Length > 0 && provinceNames != null) {
                for (int k = 0; k < _provinceNames.Length; k++) {
                    if (_provinceNames[k].TrimStart().StartsWith(GUIProvinceName)) {
                        GUIProvinceIndex = k;
                        provinceIndex = map.GetProvinceIndex(countryIndex, GUIProvinceName);
                        return;
                    }
                }
            }
            GUIProvinceIndex = -1;
            GUIProvinceName = "";
        }

        string GetProvinceTransferIndexByGUISelection() {
            if (GUIProvinceTransferToCountryIndex < 0 || GUIProvinceTransferToCountryIndex >= _countryNames.Length)
                return "";
            string[] s = _countryNames[GUIProvinceTransferToCountryIndex].Split(new char[] {
                '(',
                ')'
            }, System.StringSplitOptions.RemoveEmptyEntries);
            if (s.Length >= 2) {
                return s[0].Trim();
            }
            return "";
        }

        void SyncGUIProvinceTransferSelection(string oldName) {
            // recover GUI province index selection
            if (oldName.Length > 0) {
                for (int k = 0; k < _countryNames.Length; k++) {  // don't use countryNames or the array will be reloaded again if grouped option is enabled causing an infinite loop
                    if (_countryNames[k].TrimStart().StartsWith(oldName)) {
                        GUIProvinceTransferToCountryIndex = k;
                        return;
                    }
                }
                SetInfoMsg("Country " + oldName + " not found in this geodata file.");
            }
            GUIProvinceTransferToCountryIndex = -1;
        }


        string GetProvinceMergeIndexByGUISelection() {
            if (GUIProvinceTransferToProvinceIndex < 0 || _provinceNames == null || GUIProvinceTransferToProvinceIndex >= _provinceNames.Length)
                return "";
            string[] s = _provinceNames[GUIProvinceTransferToProvinceIndex].Split(new char[] {
                '(',
                ')'
            }, System.StringSplitOptions.RemoveEmptyEntries);
            if (s.Length >= 2) {
                return s[0].Trim();
            }
            return "";
        }

        void SyncGUIProvinceMergeSelection(string oldName) {
            // recover GUI province index selection
            if (oldName.Length > 0) {
                for (int k = 0; k < _provinceNames.Length; k++) {  // don't use provinceNames or the array will be reloaded again if grouped option is enabled causing an infinite loop
                    if (_provinceNames[k].TrimStart().StartsWith(oldName)) {
                        GUIProvinceTransferToProvinceIndex = k;
                        return;
                    }
                }
                SetInfoMsg("Province " + oldName + " not found in this geodata file.");
            }
            GUIProvinceTransferToProvinceIndex = -1;
        }

        public bool ProvinceRename() {
            if (countryIndex < 0 || provinceIndex < 0)
                return false;
            string prevName = map.provinces[provinceIndex].name;
            GUIProvinceNewName = GUIProvinceNewName.Trim();
            if (prevName.Equals(GUIProvinceNewName))
                return false;
            if (map.ProvinceRename(countryIndex, prevName, GUIProvinceNewName)) {
                GUIProvinceName = GUIProvinceNewName;
                lastProvinceCount = -1;
                SyncGUIProvinceSelection();
                ProvinceHighlightSelection();
                provinceChanges = true;
                cityChanges = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Deletes current region or province if this was the last region
        /// </summary>
        public void ProvinceDelete() {
            if (provinceIndex < 0 || provinceIndex >= map.provinces.Length)
                return;
            map.ProvinceDelete(provinceIndex);
            ClearProvinceSelection();
            map.OptimizeFrontiers();
            map.Redraw();
            provinceChanges = true;
        }

        /// <summary>
        /// Deletes current region or province if this was the last region
        /// </summary>
        public bool ProvinceRegionDelete() {
            if (provinceIndex < 0 || provinceIndex >= map.provinces.Length)
                return false;

            Province province = _map.provinces[provinceIndex];
            if (province.regions == null || map.provinces[provinceIndex].regions.Count <= 0) return false;
            map.provinces[provinceIndex].regions.RemoveAt(provinceRegionIndex);
            map.RefreshProvinceDefinition(_map.GetProvinceIndex(province), false);

            ClearProvinceSelection();
            RedrawFrontiers();
            provinceChanges = true;
            return true;
        }

        /// <summary>
        /// Deletes selected provinces regions
        /// </summary>
        public bool SelectedProvincesRegionsDelete() {
            if (provinceIndex < 0 || provinceIndex >= map.provinces.Length)
                return false;

            for (int k = 0; k < selectedProvincesRegions.Count; k++) {
                Region region = selectedProvincesRegions[k];
                Province province = (Province)region.entity;
                if (province.regions == null || province.regions.Count <= 0) continue;
                province.regions.Remove(region);
                map.RefreshProvinceDefinition(_map.GetProvinceIndex(province), false);
            }

            ClearSelection();
            RedrawFrontiers();
            provinceChanges = true;
            return true;
        }

        /// <summary>
        /// Delete all provinces of current country. Called from DeleteCountry.
        /// </summary>
        void mDeleteCountryProvinces() {
            if (map.provinces == null)
                return;
            if (countryIndex < 0)
                return;

            map.HideProvinceRegionHighlights(true);
            map.countries[countryIndex].provinces = new Province[0];
            map.CountryDeleteProvinces(countryIndex);
            provinceChanges = true;
        }

        public void DeleteCountryProvinces() {
            mDeleteCountryProvinces();
            ClearSelection();
            RedrawFrontiers();
            map.DrawMapLabels();
        }


        /// <summary>
        /// Creates a new province with the current shape
        /// </summary>
        public void ProvinceCreate() {
            if (newShape.Count < 3 || countryIndex < 0)
                return;

            provinceIndex = map.provinces.Length;
            provinceRegionIndex = 0;
            string newProvinceName = GetProvinceUniqueName("New Province");
            Province newProvince = new Province(newProvinceName, countryIndex, map.GetUniqueId(new List<IExtendableAttribute>(map.provinces)));
            Region region = new Region(newProvince, 0);
            region.points = newShape.ToArray();
            newProvince.regions = new List<Region>();
            newProvince.regions.Add(region);
            map.ProvinceAdd(newProvince);
            map.RefreshProvinceDefinition(provinceIndex, false);
            lastProvinceCount = -1;
            GUIProvinceName = newProvince.name;
            SyncGUIProvinceSelection();
            ProvinceRegionSelect();
            provinceChanges = true;
        }

        public void AddProvinceRegionToCountry() {
            if (provinceIndex < 0 || provinceRegionIndex < 0 || countryIndex < 0) return;
            Country country = map.countries[countryIndex];
            Province province = map.provinces[provinceIndex];
            if (province.regions == null) return;
            foreach (Region provRegion in province.regions) {
                if (!country.regions.Contains(provRegion)) {
                    country.regions.Add(provRegion);
                }
            }
            map.RefreshCountryGeometry(country);
            map.Redraw();
            countryIndex = -1;
            ProvinceRegionSelect();
            countryChanges = true;
        }

        /// <summary>
        /// Adds a new province to current province
        /// </summary>
        public void ProvinceRegionCreate() {
            if (newShape.Count < 3 || provinceIndex < 0)
                return;

            Province province = map.provinces[provinceIndex];
            if (province.regions == null)
                province.regions = new List<Region>();
            provinceRegionIndex = province.regions.Count;
            Region region = new Region(province, provinceRegionIndex);
            region.points = newShape.ToArray();
            if (province.regions == null)
                province.regions = new List<Region>();
            province.regions.Add(region);
            map.RefreshProvinceDefinition(provinceIndex, false);
            provinceChanges = true;
            ProvinceRegionSelect();
        }


        /// <summary>
        /// Creates a new province with the given region
        /// </summary>
        public void ProvinceCreate(Region region) {
            if (region == null)
                return;

            // Remove region from source entity
            IAdminEntity entity = region.entity;
            if (entity != null) {
                entity.regions.Remove(region);
                Country country;
                // Refresh entity definition
                if (region.entity is Country) {
                    int countryIndex = _map.GetCountryIndex((Country)region.entity);
                    country = _map.countries[countryIndex];
                    _map.RefreshCountryGeometry(country);
                } else {
                    int provinceIndex = map.GetProvinceIndex((Province)region.entity);
                    country = _map.countries[_map.provinces[provinceIndex].countryIndex];
                    _map.RefreshProvinceGeometry(provinceIndex);
                }
            }

            provinceIndex = map.provinces.Length;
            provinceRegionIndex = 0;
            string newProvinceName = GetProvinceUniqueName("New Province");
            Province newProvince = new Province(newProvinceName, countryIndex, map.GetUniqueId(new List<IExtendableAttribute>(map.provinces)));
            Region newRegion = new Region(newProvince, 0);
            newRegion.UpdatePointsAndRect(region.points);
            newProvince.regions = new List<Region>();
            newProvince.regions.Add(newRegion);
            map.ProvinceAdd(newProvince);
            map.RefreshProvinceDefinition(provinceIndex, false);

            // Update cities
            List<City> cities = _map.GetCities(region);
            int citiesCount = cities.Count;
            if (citiesCount > 0) {
                for (int k = 0; k < citiesCount; k++) {
                    if (cities[k].province != newProvinceName) {
                        cities[k].province = newProvinceName;
                        cityChanges = true;
                    }
                }
            }

            lastProvinceCount = -1;
            GUIProvinceName = newProvince.name;
            SyncGUIProvinceSelection();
            ProvinceRegionSelect();
            provinceChanges = true;
        }

        /// <summary>
        /// Checks province's polygon points quality and fix artifacts.
        /// </summary>
        /// <returns><c>true</c>, if province was changed, <c>false</c> otherwise.</returns>
        public bool ProvinceSanitize() {
            bool changes = false;
            if (_map.MergeAdjacentRegions(_map.provinces[provinceIndex])) changes = true;
            if (_map.ProvinceSanitize(provinceIndex, 5)) changes = true;
            if (changes) {
                provinceChanges = true;
            }
            return changes;
        }


        /// <summary>
        /// Changes province's owner to specified country
        /// </summary>
        public void ProvinceTransferTo() {
            if (provinceIndex < 0 || GUIProvinceTransferToCountryIndex < 0 || GUIProvinceTransferToCountryIndex >= countryNames.Length)
                return;

            // Get target country
            // recover GUI country index selection
            int targetCountryIndex = -1;
            string[] s = countryNames[GUIProvinceTransferToCountryIndex].Split(new char[] {
                '(',
                ')'
            }, System.StringSplitOptions.RemoveEmptyEntries);
            if (s.Length >= 2) {
                if (!int.TryParse(s[1], out targetCountryIndex)) {
                    return;
                }
            }

            map.HideCountryRegionHighlights(true);
            map.HideProvinceRegionHighlights(true);
            _map.CountryTransferProvinceRegion(targetCountryIndex, map.provinces[provinceIndex].regions[provinceRegionIndex], true);

            countryChanges = true;
            provinceChanges = true;
            countryIndex = targetCountryIndex;
            countryRegionIndex = map.countries[targetCountryIndex].mainRegionIndex;
            ProvinceRegionSelect();
        }

        /// <summary>
        /// Merges province with target province
        /// </summary>
        public void ProvinceMerge() {
            if (provinceIndex < 0 || GUIProvinceTransferToProvinceIndex < 0 || GUIProvinceTransferToProvinceIndex >= provinceNames.Length)
                return;

            // Get target province
            // recover GUI country index selection
            int targetProvinceIndex = -1;
            string[] s = _provinceNames[GUIProvinceTransferToProvinceIndex].Split(new char[] {
                '(',
                ')'
            }, System.StringSplitOptions.RemoveEmptyEntries);
            if (s.Length >= 2) {
                if (!int.TryParse(s[1], out targetProvinceIndex)) {
                    return;
                }
            }

            Province targetProvince;
            if (targetProvinceIndex >= 0)
                targetProvince = map.provinces[targetProvinceIndex];
            else
                return;
            map.HideCountryRegionHighlights(true);
            map.HideProvinceRegionHighlights(true);
            if (_map.ProvinceTransferProvinceRegion(targetProvinceIndex, map.provinces[provinceIndex].regions[provinceRegionIndex], true)) {
                GC.Collect();
                countryChanges = true;
                provinceChanges = true;
                cityChanges = true;
                mountPointChanges = true;
                provinceIndex = map.GetProvinceIndex(targetProvince);
                provinceRegionIndex = map.provinces[provinceIndex].mainRegionIndex;
                CountryRegionSelect();
                ProvinceRegionSelect();
            }
        }

        /// <summary>
        /// Converts current province in a new country
        /// </summary>
        public void ProvinceToNewCountry() {
            if (map.GetCountryIndex(GUIProvinceToNewCountryName) >= 0) {
                Debug.LogError("Country name is already in use.");
                return;
            }
            Province sourceProvince = map.provinces[provinceIndex];
            int newCountryIndex = map.ProvinceToCountry(sourceProvince, GUIProvinceToNewCountryName);
            GC.Collect();
            countryIndex = newCountryIndex;
            countryRegionIndex = 0;
            countryChanges = true;
            provinceChanges = true;
            cityChanges = true;
            mountPointChanges = true;
            ReloadCountryNames();
            CountryRegionSelect();
            ProvinceRegionSelect();
        }


        /// <summary>
        /// Merges provinces with target province
        /// </summary>
        public void ProvincesMerge() {
            if (selectedProvincesRegions.Count < 2)
                return;
            Province province = (Province)selectedProvincesRegions[0].entity;
            int targetProvinceIndex = map.GetProvinceIndex(province);
            Province targetProvince = map.provinces[targetProvinceIndex];
            for (int k = 1; k < selectedProvincesRegions.Count; k++) {
                _map.ProvinceTransferProvinceRegion(targetProvinceIndex, selectedProvincesRegions[k], false);
                targetProvinceIndex = map.GetProvinceIndex(targetProvince);
                GC.Collect();
            }

            ClearProvinceSelection();
            countryChanges = true;
            provinceChanges = true;
            cityChanges = true;
            mountPointChanges = true;
            provinceIndex = map.GetProvinceIndex(targetProvince);
            provinceRegionIndex = map.provinces[provinceIndex].mainRegionIndex;
            map.Redraw(true);
            ProvinceRegionSelect();
        }

        /// <summary>
        /// Converts current province in a new country
        /// </summary>
        public void ProvincesRegionsToNewCountry() {
            if (map.GetCountryIndex(GUIProvinceToNewCountryName) >= 0) {
                Debug.LogError("Country name is already in use.");
                return;
            }
            int newCountryIndex = map.RegionsToCountry(selectedProvincesRegions, GUIProvinceToNewCountryName);
            GC.Collect();
            countryIndex = newCountryIndex;
            countryRegionIndex = 0;
            countryChanges = true;
            provinceChanges = true;
            cityChanges = true;
            mountPointChanges = true;
            CountryRegionSelect();
            ProvinceRegionSelect();
        }


        #endregion

        #region IO stuff

        /// <summary>
        /// Returns the file name corresponding to the current province data file
        /// </summary>
        public string GetProvinceGeoDataFileName() {
            return "provinces10.txt";
        }


        /// <summary>
        /// Exports the geographic data in packed string format with reduced quality.
        /// </summary>
        public string GetProvinceGeoDataLowQuality() {
            // step 1: duplicate data
            Province[] provinces = map.provinces;
            List<Province> provinces1 = new List<Province>(provinces);

            for (int k = 0; k < provinces1.Count; k++) {
                Province prov = provinces1[k];
                if (prov.regions == null) map.ReadProvincePackedString(prov);
                if (prov.regions == null) continue;
                provinces1[k].regions = new List<Region>(provinces1[k].regions);
                for (int r = 0; r < provinces[k].regions.Count; r++) {
                    provinces1[k].regions[r].points = new List<Vector2>(provinces1[k].regions[r].points).ToArray();
                }
            }
            // step 2: ensure near points between neighbours
            float MAX_DIST = 0.00000001f;
            for (int k = 0; k < provinces1.Count; k++) {
                for (int r = 0; r < provinces1[k].regions.Count; r++) {
                    Region region1 = provinces1[k].regions[r];
                    for (int p = 0; p < provinces1[k].regions[r].points.Length; p++) {
                        // Search near points
                        for (int k2 = 0; k2 < region1.neighbours.Count; k2++) {
                            for (int r2 = 0; r2 < provinces1[k2].regions.Count; r2++) {
                                Region region2 = provinces1[k2].regions[r2];
                                for (int p2 = 0; p2 < provinces1[k2].regions[r2].points.Length; p2++) {
                                    float dist = (region1.points[p].x - region2.points[p2].x) * (region1.points[p].x - region2.points[p2].x) +
                                                 (region1.points[p].y - region2.points[p2].y) * (region1.points[p].y - region2.points[p2].y);
                                    if (dist < MAX_DIST) {
                                        region2.points[p2] = region1.points[p];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // step 2: simplify
            Dictionary<Vector2, bool> frontiersHit = new Dictionary<Vector2, bool>();
            List<IAdminEntity> entities2 = new List<IAdminEntity>(provinces1.Count);
            int savings = 0, totalPoints = 0;
            float FACTOR = 1000f;
            for (int k = 0; k < provinces1.Count; k++) {
                IAdminEntity refEntity = provinces1[k];
                IAdminEntity newEntity = new Province(refEntity.name, ((Province)refEntity).countryIndex, map.GetUniqueId(new List<IExtendableAttribute>(map.provinces)));
                newEntity.regions = new List<Region>();
                for (int r = 0; r < refEntity.regions.Count; r++) {
                    Region region = refEntity.regions[r];
                    int numPoints = region.points.Length;
                    totalPoints += numPoints;
                    List<Vector2> points = new List<Vector2>(numPoints);
                    frontiersHit.Clear();

                    Vector3[] blockyPoints = new Vector3[numPoints];
                    for (int p = 0; p < numPoints; p++)
                        blockyPoints[p] = new Vector2(Mathf.RoundToInt(region.points[p].x * FACTOR) / FACTOR, Mathf.RoundToInt(region.points[p].y * FACTOR) / FACTOR);

                    points.Add(region.points[0] * WMSK.MAP_PRECISION);
                    for (int p = 1; p < numPoints - 1; p++) {
                        if (blockyPoints[p - 1].y == blockyPoints[p].y && blockyPoints[p].y == blockyPoints[p + 1].y ||
                            blockyPoints[p - 1].x == blockyPoints[p].x && blockyPoints[p].x == blockyPoints[p + 1].x) {
                            savings++;
                            continue;
                        }
                        if (!frontiersHit.ContainsKey(blockyPoints[p])) { // add neighbour references
                            frontiersHit.Add(blockyPoints[p], true);
                            points.Add(region.points[p] * WMSK.MAP_PRECISION);
                        } else {
                            savings++;
                        }
                    }
                    points.Add(region.points[numPoints - 1] * WMSK.MAP_PRECISION);
                    if (points.Count >= 5) {
                        Region newRegion = new Region(newEntity, newEntity.regions.Count);
                        newRegion.points = points.ToArray();
                        newEntity.regions.Add(newRegion);
                        _map.RegionSanitize(newRegion);
                    }
                }
                if (newEntity.regions.Count > 0)
                    entities2.Add(newEntity);
            }

            Debug.Log(savings + " points removed of " + totalPoints + " (" + (((float)savings / totalPoints) * 100.0f).ToString("F1") + "%)");

            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < entities2.Count; k++) {
                IAdminEntity entity = entities2[k];
                if (k > 0)
                    sb.Append("|");
                sb.Append(entity.name + "$");
                sb.Append(map.countries[((Province)entity).countryIndex].name + "$");
                for (int r = 0; r < entity.regions.Count; r++) {
                    if (r > 0)
                        sb.Append("*");
                    Region region = entity.regions[r];
                    for (int p = 0; p < region.points.Length; p++) {
                        if (p > 0)
                            sb.Append(";");
                        Vector2 point = region.points[p];
                        sb.Append(point.x.ToString(Misc.InvariantCulture) + ",");
                        sb.Append(point.y.ToString(Misc.InvariantCulture));
                    }
                }
                sb.Append("$");
                sb.Append(entity.uniqueId.ToString());
            }
            return sb.ToString();
        }


        /// <summary>
        /// Merges all provinces in each country so their number fits a given range
        /// </summary>
        /// <param name="min">Minimum number of provinces.</param>
        /// <param name="max">Maximum number of provinces.</param>
        public void ProvincesEqualize(int min, int max, int countryIndex) {
            if (min < 1 || countryIndex < 0 || countryIndex >= map.countries.Length)
                return;
            if (max < min)
                max = min;

            map.showProvinces = true;
            map.drawAllProvinces = true;

            Country country = map.countries[countryIndex];
            if (country == null || country.provinces == null)
                return;
            int targetProvCount = UnityEngine.Random.Range(min, max);
            int provCount = country.provinces.Length;
            float provStartSize = 0;
            while (provCount > targetProvCount) {
                // Take the smaller province and merges with a neighbour
                float minAreaSize = float.MaxValue;
                int provinceIndex = -1;
                for (int p = 0; p < provCount; p++) {
                    Province prov = country.provinces[p];
                    if (prov == null)
                        continue;
                    if (prov.regions == null)
                        map.ReadProvincePackedString(prov);
                    if (prov.regions == null || prov.regions.Count == 0 || prov.mainRegion.neighbours == null || prov.mainRegion.neighbours.Count == 0)
                        continue;
                    if (prov.regionsRect2DArea < minAreaSize && prov.regionsRect2DArea > provStartSize) {
                        minAreaSize = prov.regionsRect2DArea;
                        provinceIndex = map.GetProvinceIndex(prov);
                    }
                }

                if (provinceIndex < 0)
                    break;

                provStartSize = minAreaSize;

                // Get the smaller neighbour
                int neighbourIndex = -1;
                Province province = map.provinces[provinceIndex];
                int neighbourCount = province.mainRegion.neighbours.Count;
                minAreaSize = float.MaxValue;
                for (int n = 0; n < neighbourCount; n++) {
                    Region neighbour = province.mainRegion.neighbours[n];
                    Province neighbourProvince = (Province)neighbour.entity;
                    if (neighbourProvince != null && neighbourProvince != province && neighbourProvince.countryIndex == countryIndex && neighbour.rect2DArea < minAreaSize) {
                        int neighbourProvIndex = map.GetProvinceIndex(neighbourProvince);
                        if (neighbourProvIndex >= 0) {
                            minAreaSize = neighbour.rect2DArea;
                            neighbourIndex = neighbourProvIndex;
                        }
                    }
                }
                if (neighbourIndex < 0)
                    continue;

                // Merges province into neighbour
                string provinceSource = map.provinces[provinceIndex].name;
                string provinceTarget = map.provinces[neighbourIndex].name;
                int prevProvCount = country.provinces.Length;
                if (!map.ProvinceTransferProvinceRegion(neighbourIndex, map.provinces[provinceIndex].mainRegion, false)) {
                    Debug.LogWarning("Country: " + map.countries[countryIndex].name + " => " + provinceSource + " failed merge into " + provinceTarget + ".");
                    break;
                }
                provCount = country.provinces.Length;
                if (provCount == prevProvCount)
                    break; // can't merge more provinces
            }

            provinceChanges = true;
            cityChanges = true;
            mountPointChanges = true;
        }


        #endregion

    }
}
