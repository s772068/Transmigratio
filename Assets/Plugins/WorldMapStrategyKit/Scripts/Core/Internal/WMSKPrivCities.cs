// World Map Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace WorldMapStrategyKit {

    public partial class WMSK : MonoBehaviour {

        const string CITY_ATTRIB_DEFAULT_FILENAME = "citiesAttrib";

        // resources & gameobjects
        Material citiesNormalMat, citiesRegionCapitalMat, citiesCountryCapitalMat;
        GameObject citiesLayer;

        // internal cache
        City[] visibleCities;
        bool citiesAreSortedByPopulation;

        /// <summary>
        /// City look up dictionary. Used internally for fast searching of city objects.
        /// </summary>
        Dictionary<City, int> _cityLookup;
        int lastCityLookupCount = -1;

        Dictionary<City, int> cityLookup {
            get {
                if (_cityLookup != null && cities.Length == lastCityLookupCount)
                    return _cityLookup;
                if (_cityLookup == null) {
                    _cityLookup = new Dictionary<City, int>();
                } else {
                    _cityLookup.Clear();
                }
                if (cities != null) {
                    int cityCount = cities.Length;
                    for (int k = 0; k < cityCount; k++)
                        _cityLookup[_cities[k]] = k;
                }
                lastCityLookupCount = _cityLookup.Count;
                return _cityLookup;
            }
        }

        List<City> tmpCities;

        public Vector3 currentCityScale;

        #region IO stuff

        void ReadCitiesPackedString() {
            ReadCitiesPackedString("cities10");
        }

        void ReadCitiesPackedString(string filename) {
            string cityCatalogFileName = _geodataResourcesPath + "/" + filename;
            TextAsset ta = Resources.Load<TextAsset>(cityCatalogFileName);
            if (ta != null) {
                SetCityGeoData(ta.text);
                ReloadCitiesAttributes();
                Resources.UnloadAsset(ta);
            }
        }


        void ReloadCitiesAttributes() {
            TextAsset ta = Resources.Load<TextAsset>(_geodataResourcesPath + "/" + _cityAttributeFile);
            if (ta == null)
                return;
            SetCitiesAttributes(ta.text);
        }

        #endregion


        #region Drawing stuff

        void CheckCityIcons() {
            if (_citySpot == null)
                _citySpot = Resources.Load<GameObject>("WMSK/Prefabs/CitySpot");
            if (_citySpotCapitalRegion == null)
                _citySpotCapitalRegion = Resources.Load<GameObject>("WMSK/Prefabs/CityCapitalRegionSpot");
            if (_citySpotCapitalCountry == null)
                _citySpotCapitalCountry = Resources.Load<GameObject>("WMSK/Prefabs/CityCapitalCountrySpot");
        }


        int SortByPopulationDesc(City c1, City c2) {
            if (c1.population < c2.population) return 1;
            if (c1.population > c2.population) return -1;
            return 0;
        }

        /// <summary>
        /// Redraws the cities. This is automatically called by Redraw(). Used internally by the Map Editor. You should not need to call this method directly.
        /// </summary>
        public void DrawCities() {

            if (!_showCities || !gameObject.activeInHierarchy)
                return;

            CheckCityIcons();

            // Create cities layer
            Transform t = transform.Find("Cities");
            if (t != null)
                DestroyImmediate(t.gameObject);
            citiesLayer = new GameObject("Cities");
            citiesLayer.transform.SetParent(transform, false);
            citiesLayer.transform.localPosition = Misc.Vector3back * 0.001f;
            citiesLayer.layer = gameObject.layer;

            // Create cityclass parents
            GameObject countryCapitals = new GameObject("Country Capitals");
            countryCapitals.transform.SetParent(citiesLayer.transform, false);
            GameObject regionCapitals = new GameObject("Region Capitals");
            regionCapitals.transform.SetParent(citiesLayer.transform, false);
            GameObject normalCities = new GameObject("Normal Cities");
            normalCities.transform.SetParent(citiesLayer.transform, false);

            if (cities == null || (_cities.Length > 0 && _cities[0] == null))
                return;

            if (disposalManager != null) {
                disposalManager.MarkForDisposal(citiesLayer);
                disposalManager.MarkForDisposal(countryCapitals); // countryCapitals.hideFlags = HideFlags.DontSave;
                disposalManager.MarkForDisposal(regionCapitals); // regionCapitals.hideFlags = HideFlags.DontSave;
                disposalManager.MarkForDisposal(normalCities); // normalCities.hideFlags = HideFlags.DontSave;
            }

            if (!citiesAreSortedByPopulation && _maxCitiesPerCountry > 0) {
                Array.Sort(_cities, SortByPopulationDesc);
                citiesAreSortedByPopulation = true;
            }


            // Draw city marks
            numCitiesDrawn = 0;
            int minPopulation = _minPopulation * 1000;
            int visibleCount = 0;
            int maxCitiesPerCountry = _maxCitiesPerCountry > 0 ? _maxCitiesPerCountry : 999999;
            if (_maxCitiesPerCountry > 0) {
                for (int k = 0; k < _countries.Length; k++) {
                    _countries[k].visibleCities = 0;
                }
            }
            int cityCount = cities.Length;
            for (int k = 0; k < cityCount; k++) {
                City city = _cities[k];
                int countryIndex = city.countryIndex;
                if (countryIndex < 0 || countryIndex >= _countries.Length) {
                    city.isVisible = false;
                    continue;
                }
                Country country = _countries[countryIndex];
                if (country.hidden) {
                    city.isVisible = false;
                    continue;
                }
                bool isCapital = ((int)city.cityClass & _cityClassAlwaysShow) != 0;
                if (isCapital) {
                    city.isVisible = true;
                } else { 
                    city.isVisible = country.visibleCities < maxCitiesPerCountry && (minPopulation == 0 || city.population >= minPopulation);
                }
                if (city.isVisible) {
                    if (!isCapital) {
                        country.visibleCities++;
                    }
                    GameObject cityObj, cityParent;
                    Material mat;
                    switch (city.cityClass) {
                        case CITY_CLASS.COUNTRY_CAPITAL:
                            cityObj = Instantiate(_citySpotCapitalCountry);
                            mat = citiesCountryCapitalMat;
                            cityParent = countryCapitals;
                            break;
                        case CITY_CLASS.REGION_CAPITAL:
                            cityObj = Instantiate(_citySpotCapitalRegion);
                            mat = citiesRegionCapitalMat;
                            cityParent = regionCapitals;
                            break;
                        default:
                            cityObj = Instantiate(_citySpot);
                            mat = citiesNormalMat;
                            cityParent = normalCities;
                            break;
                    }
                    cityObj.name = k.ToString();
                    if (disposalManager != null) {
                        disposalManager.MarkForDisposal(cityObj);
                    }
                    cityObj.hideFlags |= HideFlags.HideInHierarchy;
                    cityObj.layer = citiesLayer.layer;
                    cityObj.transform.SetParent(cityParent.transform, false);
                    cityObj.transform.localPosition = city.unity2DLocation;
                    Renderer rr = cityObj.GetComponent<Renderer>();
                    if (rr != null)
                        rr.sharedMaterial = mat;

                    numCitiesDrawn++;
                    visibleCount++;
                }
            }

            // Cache visible cities (this is faster than iterate through the entire collection)
            if (visibleCities == null || visibleCities.Length != visibleCount) {
                visibleCities = new City[visibleCount];
            }
            for (int k = 0; k < cityCount; k++) {
                City city = _cities[k];
                if (city.isVisible) {
                    visibleCities[--visibleCount] = city;
                }
            }

            // Toggle cities layer visibility according to settings
            citiesLayer.SetActive(_showCities);
            ScaleCities();
        }

        public string GetCityHierarchyName(int cityIndex) {
            if (cityIndex < 0 || cityIndex >= cities.Length)
                return "";
            switch (cities[cityIndex].cityClass) {
                case CITY_CLASS.COUNTRY_CAPITAL:
                    return "Cities/Country Capitals/" + cityIndex.ToString();
                case CITY_CLASS.REGION_CAPITAL:
                    return "Cities/Region Capitals/" + cityIndex.ToString();
                default:
                    return "Cities/Normal Cities/" + cityIndex.ToString();
            }
        }

        void ScaleCities() {
            if (!gameObject.activeInHierarchy)
                return;
            CityScaler cityScaler = citiesLayer.GetComponent<CityScaler>() ?? citiesLayer.AddComponent<CityScaler>();
            cityScaler.map = this;
            if (_showCities) {
                cityScaler.ScaleCities();
            }
        }

        void HighlightCity(int cityIndex) {
            _cityHighlightedIndex = cityIndex;
            _cityHighlighted = cities[cityIndex];

            // Raise event
            if (OnCityEnter != null)
                OnCityEnter(_cityHighlightedIndex);
        }

        void HideCityHighlight() {
            if (_cityHighlightedIndex < 0)
                return;

            // Raise event
            if (OnCityExit != null)
                OnCityExit(_cityHighlightedIndex);
            _cityHighlighted = null;
            _cityHighlightedIndex = -1;
        }


        #endregion

        #region Internal API

        /// <summary>
        /// Ensures the city index is between city array limits
        /// </summary>
        /// <param name="cityIndex"></param>
        /// <returns></returns>
        bool CheckCityIndex(int cityIndex) {
            return cityIndex >= 0 && cityIndex < cities.Length;
        }


        /// <summary>
        /// Returns any city near the point specified in local coordinates.
        /// </summary>
        /// <returns>The city near point.</returns>
        /// <param name="localPoint">Local point.</param>
        int GetCityNearPoint(Vector2 localPoint) {
            return GetCityNearPoint(localPoint, -1);
        }

        /// <summary>
        /// Returns any city near the point specified for a given country in local coordinates.
        /// </summary>
        /// <returns>The city near point.</returns>
        /// <param name="localPoint">Local point.</param>
        /// <param name="countryIndex">Country index.</param>
        int GetCityNearPoint(Vector2 localPoint, int countryIndex) {
            if (visibleCities == null)
                return -1;
            if (isPlaying) {
                //												float hitPrecission = CITY_HIT_PRECISION * _cityIconSize * _cityHitTestPrecision;
                float rl = localPoint.x - currentCityScale.x; // hitPrecission;
                float rr = localPoint.x + currentCityScale.x; // hitPrecission;
                float rt = localPoint.y + currentCityScale.y; // hitPrecission;
                float rb = localPoint.y - currentCityScale.y; // hitPrecission;
                for (int c = 0; c < visibleCities.Length; c++) {
                    City city = visibleCities[c];
                    if (countryIndex < 0 || city.countryIndex == countryIndex) {
                        Vector2 cityLoc = city.unity2DLocation;
                        if (cityLoc.x > rl && cityLoc.x < rr && cityLoc.y > rb && cityLoc.y < rt) {
                            return GetCityIndex(city);
                        }
                    }
                }
            } else {
                // Use alternate method since city scale is different in Scene View
                float minDist = float.MaxValue;
                City candidate = null;
                for (int c = 0; c < visibleCities.Length; c++) {
                    City city = visibleCities[c];
                    if (countryIndex < 0 || city.countryIndex == countryIndex) {
                        Vector2 cityLoc = city.unity2DLocation;
                        float dist = (cityLoc.x - localPoint.x) * (cityLoc.x - localPoint.x) + (cityLoc.y - localPoint.y) * (cityLoc.y - localPoint.y);
                        if (dist < minDist) {
                            minDist = dist;
                            candidate = city;
                        }
                    }
                }
                if (candidate != null) {
                    return GetCityIndex(candidate);
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the file name corresponding to the current city data file
        /// </summary>
        public string GetCityFileName() {
            return "cities10.txt";
        }


        #endregion


    }

}