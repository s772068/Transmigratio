using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace WorldMapStrategyKit
{


    public partial class WMSK_Editor : MonoBehaviour {

        /// <summary>
        /// Builds a new geodata file for countries based on provinces data
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetCountryGeoDataFromProvinces(EditorOperationProgress progressOp, EditorOperationFinish finishOp) {

            WMSK map = this.map;
            List<Province> provinces = new List<Province>();
            int countriesCount = map.countries.Length;
            for (int k = 0; k < countriesCount; k++) {
                Country country = map.countries[k];
                if (progressOp != null) {
                    if (progressOp((float)k / countriesCount, "Rebuilding frontiers", "Processing country " + country.name + ", " + (k + 1) + " of " + countriesCount)) {
                        cancelled = true;
                        yield break;
                    }
                }
                provinces.Clear();
                provinces.AddRange(country.provinces);
                if (map.CountrySetProvinces(k, provinces, mergeRegions: true, updateCities: false, updateMountPoints: false)) {
                    countryChanges = true;
                }
                yield return null;
            }

            if (!cancelled) {
                if (progressOp != null) {
                    progressOp(0.98f, "Rebuilding countries in progress", "Verifying cities...");
                }
                // update cities
                int citiesCount = map.cities.Length;
                for (int k = 0; k < citiesCount; k++) {
                    City city = map.cities[k];
                    int countryIndex = map.GetCountryIndex(city.unity2DLocation);
                    if (countryIndex >= 0 && countryIndex != city.countryIndex) {
                        city.countryIndex = countryIndex;
                        cityChanges = true;
                    }
                    int provinceIndex = map.GetProvinceIndex(city.unity2DLocation);
                    if (provinceIndex >= 0) {
                        Province province = map.provinces[provinceIndex];
                        string provinceName = province.name;
                        if (city.province != provinceName) {
                            city.province = provinceName;
                            cityChanges = true;
                        }
                        if (city.countryIndex < 0) {
                            city.countryIndex = province.countryIndex;
                            cityChanges = true;
                        }
                    }
                }
            }

            if (!cancelled) {
                if (progressOp != null) {
                    progressOp(0.99f, "Rebuilding countries in progress", "Verifying mount points...");
                }
                // update mount points
                int mountPointsCount = map.mountPoints.Count;
                for (int k = 0; k < mountPointsCount; k++) {
                    MountPoint mp = map.mountPoints[k];
                    int countryIndex = map.GetCountryIndex(mp.unity2DLocation);
                    if (countryIndex >= 0 && countryIndex != mp.countryIndex) {
                        mp.countryIndex = countryIndex;
                        mountPointChanges = true;
                    }
                    int provinceIndex = map.GetProvinceIndex(mp.unity2DLocation);
                    if (provinceIndex >= 0) {
                        if (mp.provinceIndex != provinceIndex) {
                            mp.provinceIndex = provinceIndex;
                            mountPointChanges = true;
                        }
                        if (mp.countryIndex < 0) {
                            Province province = map.provinces[provinceIndex];
                            mp.countryIndex = province.countryIndex;
                            mountPointChanges = true;
                        }
                    }
                }
            }

            map.Redraw(true);

            if (finishOp != null) {
                finishOp(cancelled);
            }
        }

    }
}

