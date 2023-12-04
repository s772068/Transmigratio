﻿using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

    public partial class WMSK_Editor : MonoBehaviour {

        public int GUICityIndex;
        public string GUICityName = "";
        public string GUICityNewName = "";
        public string GUICityPopulation = "";
        public CITY_CLASS GUICityClass = CITY_CLASS.CITY;
        public int cityIndex = -1;
        public bool cityChanges;  // if there's any pending change to be saved
        public bool cityAttribChanges; // if there's any pending change to be saved

        // private fields
        int lastCityCount = -1;
        string[] _cityNames;


        public string[] cityNames {
            get {
                if (map.cities != null && lastCityCount != map.cities.Length) {
                    cityIndex = -1;
                    ReloadCityNames();
                }
                return _cityNames;
            }
        }


        #region Editor functionality


        public void ClearCitySelection() {
            map.HideCityHighlights();
            cityIndex = -1;
            GUICityName = "";
            GUICityIndex = -1;
            GUICityNewName = "";
            GUICityClass = CITY_CLASS.CITY;
        }


        /// <summary>
        /// Adds a new city to current country.
        /// </summary>
        public void CityCreate(Vector2 newPoint) {
            if (countryIndex < 0) return;
            string provinceName;
            if (provinceIndex >= 0) {
                provinceName = map.provinces[provinceIndex].name;
            } else {
                provinceName = "";
            }
            GUICityName = "New City " + (map.cities.Length + 1);
            City newCity = new City(GUICityName, provinceName, countryIndex, 100, newPoint, GUICityClass, map.GetUniqueId(new List<IExtendableAttribute>(map.cities)));
            map.CityAdd(newCity);
            map.DrawCities();
            lastCityCount = -1;
            ReloadCityNames();
            cityChanges = true;
        }


        public bool CityRename() {
            if (cityIndex < 0) return false;
            string prevName = map.cities[cityIndex].name;
            GUICityNewName = GUICityNewName.Trim();
            if (prevName.Equals(GUICityNewName)) return false;
            map.cities[cityIndex].name = GUICityNewName;
            GUICityName = GUICityNewName;
            lastCityCount = -1;
            ReloadCityNames();
            map.DrawCities();
            cityChanges = true;
            return true;
        }

        public bool CityClassChange() {
            if (cityIndex < 0) return false;
            map.cities[cityIndex].cityClass = GUICityClass;
            map.DrawCities();
            cityChanges = true;
            return true;
        }

        public bool CityChangePopulation(int newPopulation) {
            if (cityIndex < 0) return false;
            map.cities[cityIndex].population = newPopulation;
            cityChanges = true;
            return true;
        }



        public void CityMove(Vector3 destination) {
            if (cityIndex < 0) return;
            map.cities[cityIndex].unity2DLocation = destination;
            map.DrawCities();
            CityHighlightSelection();
            cityChanges = true;
        }

        public void CitySelectByCombo(int selection) {
            GUICityName = "";
            GUICityIndex = selection;
            if (GetCityIndexByGUISelection()) {
                if (Application.isPlaying) {
                    map.BlinkCity(cityIndex, Color.black, Color.green, 1.2f, 0.2f);
                }
            }
            CitySelect();
        }

        bool GetCityIndexByGUISelection() {
            if (GUICityIndex < 0 || GUICityIndex >= cityNames.Length) return false;
            string[] s = cityNames[GUICityIndex].Split(new char[] {
                '(',
                ')'
            }, System.StringSplitOptions.RemoveEmptyEntries);
            if (s.Length >= 2) {
                GUICityName = s[0].Trim();
                if (int.TryParse(s[1], out cityIndex)) {
                    return true;
                }
            }
            return false;
        }

        public void CitySelect() {
            if (cityIndex < 0 || cityIndex > map.cities.Length)
                return;

            // If no country is selected (the city could be at sea) select it
            City city = map.cities[cityIndex];
            int cityCountryIndex = city.countryIndex;
            if (cityCountryIndex < 0) {
                SetInfoMsg("City's country not found in this country file.");
            }

            if (countryIndex != cityCountryIndex && cityCountryIndex >= 0) {
                ClearSelection();
                countryIndex = cityCountryIndex;
                countryRegionIndex = map.countries[countryIndex].mainRegionIndex;
                CountryRegionSelect();
                if (editingMode == EDITING_MODE.PROVINCES) {
                    provinceIndex = map.GetProvinceIndex(countryIndex, city.province);
                    if (provinceIndex >= 0) {
                        provinceRegionIndex = map.provinces[provinceIndex].mainRegionIndex;
                        ProvinceRegionSelect();
                    }
                }
            }

            // Just in case makes GUICountryIndex selects appropiate value in the combobox
            GUICityName = city.name;
            GUICityPopulation = city.population.ToString();
            GUICityClass = city.cityClass;
            SyncGUICitySelection();
            if (cityIndex >= 0) {
                GUICityNewName = city.name;
                CityHighlightSelection();
            }
        }

        public bool CitySelectByScreenClick(Ray ray) {
            int targetCityIndex;
            if (map.GetCityIndex(ray, countryIndex, out targetCityIndex)) {
                cityIndex = targetCityIndex;
                CitySelect();
                return true;
            }
            return false;
        }

        void CityHighlightSelection() {

            if (cityIndex < 0 || cityIndex >= map.cities.Length) return;

            // Colorize city
            map.HideCityHighlights();
            map.ToggleCityHighlight(cityIndex, Color.blue, true);
        }


        public void ReloadCityNames() {
            if (map == null || map.cities == null) {
                lastCityCount = -1;
                return;
            }
            lastCityCount = map.cities.Length; // check this size, and not result from GetCityNames because it could return additional rows (separators and so)
            _cityNames = map.GetCityNames(countryIndex);
            SyncGUICitySelection();
            CitySelect(); // refresh selection
        }

        void SyncGUICitySelection() {
            // recover GUI country index selection
            if (GUICityName.Length > 0) {
                for (int k = 0; k < _cityNames.Length; k++) {  // don't use city names or the array will be reloaded again if grouped option is enabled causing an infinite loop
                    if (_cityNames[k].TrimStart().StartsWith(GUICityName)) {
                        GUICityIndex = k;
                        cityIndex = map.GetCityIndex(GUICityName, countryIndex);
                        return;
                    }
                }
                SetInfoMsg("City " + GUICityName + " not found in this bank.");
            }
            GUICityIndex = -1;
            GUICityName = "";
        }

        /// <summary>
        /// Deletes current city
        /// </summary>
        public void DeleteCity() {
            if (cityIndex < 0 || cityIndex >= map.cities.Length) return;

            map.HideCityHighlights();
            List<City> cities = new List<City>(map.cities);
            cities.RemoveAt(cityIndex);
            map.cities = cities.ToArray();
            cityIndex = -1;
            GUICityName = "";
            SyncGUICitySelection();
            map.DrawCities();
            cityChanges = true;
        }

        /// <summary>
        /// Deletes all cities of current selected country
        /// </summary>
        public void DeleteCountryCities() {
            if (countryIndex < 0) return;

            map.HideCityHighlights();
            int k = -1;
            List<City> cities = new List<City>(map.cities);
            while (++k < cities.Count) {
                if (cities[k].countryIndex == countryIndex) {
                    cities.RemoveAt(k);
                    k--;
                }
            }
            map.cities = cities.ToArray();
            cityIndex = -1;
            GUICityName = "";
            SyncGUICitySelection();
            map.DrawCities();
            cityChanges = true;
        }

        public string GetCityUniqueName(string proposedName) {

            string goodName = proposedName;
            int suffix = 0;

            while (_map.GetCityIndex(goodName) >= 0) {
                suffix++;
                goodName = proposedName + suffix.ToString();
            }
            return goodName;

        }

        /// <summary>
        /// Calculates correct province for cities
        /// </summary>
        public void FixOrphanCities() {
            if (_map.provinces == null)
                return;

            for (int c = 0; c < _map.cities.Length; c++) {
                City city = _map.cities[c];
                if (city.countryIndex == -1) {
                    for (int k = 0; k < _map.countries.Length; k++) {
                        Country co = _map.countries[k];
                        for (int kr = 0; kr < co.regions.Count; kr++) {
                            if (co.regions[kr].Contains(city.unity2DLocation)) {
                                city.countryIndex = k;
                                cityChanges = true;
                                k = 100000;
                                break;
                            }
                        }
                    }
                }
                if (city.countryIndex == -1) {
                    float minDist = float.MaxValue;
                    for (int k = 0; k < _map.countries.Length; k++) {
                        Country co = _map.countries[k];
                        for (int kr = 0; kr < co.regions.Count; kr++) {
                            float dist = FastVector.SqrDistance(ref co.regions[kr].center, ref city.unity2DLocation);
                            if (dist < minDist) {
                                minDist = dist;
                                city.countryIndex = k;
                                cityChanges = true;
                            }
                        }
                    }
                }

                if (city.province.Length == 0) {
                    Country country = _map.countries[city.countryIndex];
                    if (country.provinces == null)
                        continue;
                    for (int p = 0; p < country.provinces.Length; p++) {
                        Province province = country.provinces[p];
                        _map.EnsureProvinceDataIsLoaded(province);
                        for (int pr = 0; pr < province.regions.Count; pr++) {
                            Region reg = province.regions[pr];
                            if (reg.Contains(city.unity2DLocation)) {
                                city.province = province.name;
                                cityChanges = true;
                                p = 100000;
                                break;
                            }
                        }
                    }
                }
            }

            for (int c = 0; c < _map.cities.Length; c++) {
                City city = _map.cities[c];
                if (city.province.Length == 0) {
                    float minDist = float.MaxValue;
                    int pg = -1;
                    for (int p = 0; p < _map.provinces.Length; p++) {
                        Province province = _map.provinces[p];
                        _map.EnsureProvinceDataIsLoaded(province);
                        for (int pr = 0; pr < province.regions.Count; pr++) {
                            Region pregion = province.regions[pr];
                            for (int prp = 0; prp < pregion.points.Length; prp++) {
                                float dist = FastVector.SqrDistance(ref city.unity2DLocation, ref pregion.points[prp]);
                                if (dist < minDist) {
                                    minDist = dist;
                                    pg = p;
                                }
                            }
                        }
                    }
                    if (pg >= 0) {
                        city.province = _map.provinces[pg].name;
                        cityChanges = true;
                    }
                }
            }
        }

        #endregion

        #region IO stuff

        /// <summary>
        /// Returns the file name corresponding to the current country data file (countries10, countries110)
        /// </summary>
        public string GetCityGeoDataFileName() {
            return "cities10.txt";
        }

        #endregion

    }
}
