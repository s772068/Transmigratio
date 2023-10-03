// World Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WorldMapStrategyKit.ClipperLib;


namespace WorldMapStrategyKit {

    public delegate void OnRegionClickEvent(Region region, int buttonIndex);
    public delegate void OnRegionEvent(Region region);


    public partial class WMSK : MonoBehaviour {

        #region Public API area

        public event OnRegionClickEvent OnRegionClick;
        public event OnRegionEvent OnRegionEnter;
        public event OnRegionEvent OnRegionExit;


        /// <summary>
        /// Returns the colored surface (game object) of a region. If it has not been colored yet, it will return null.
        /// </summary>
        public GameObject GetRegionSurfaceGameObject(Region region) {
            return region.surface;
        }

        /// <summary>
        /// Returns the color of the surface of a region. If it has not been colored yet, it will return Transparent (0,0,0,0).
        /// </summary>
        public Color GetRegionColor(Region region) {
            GameObject go = GetRegionSurfaceGameObject(region);
            if (go != null) {
                Renderer renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                    return renderer.sharedMaterial.color;
            }
            return Misc.ColorClear;
        }

        /// <summary>
        /// Gets a list of regions that overlap with a given region
        /// </summary>
        public List<Region> GetRegionsOverlap(Region region, bool includeProvinces = false) {
            List<Region> rr = new List<Region>();
            for (int k = 0; k < _countries.Length; k++) {
                Country country = _countries[k];
                if (country.regions == null)
                    continue;
                int rCount = country.regions.Count;
                for (int r = 0; r < rCount; r++) {
                    Region otherRegion = country.regions[r];
                    if (region.Intersects(otherRegion)) {
                        rr.Add(otherRegion);
                    }
                }
            }

            if (includeProvinces) {
                int provinceCount = provinces.Length; // triggers lazy load
                for (int k = 0; k < provinceCount; k++) {
                    Province province = _provinces[k];
                    if (province.regions == null)
                        continue;
                    int rCount = province.regions.Count;
                    for (int r = 0; r < rCount; r++) {
                        Region otherRegion = province.regions[r];
                        if (region.Intersects(otherRegion)) {
                            rr.Add(otherRegion);
                        }
                    }
                }
            }
            return rr;
        }

        /// <summary>
        /// Paints the region pixels with a given eraseColor.
        /// </summary>
        /// <param name="region">Region.</param>
        /// <param name="eraseColor">Erase color.</param>
        /// <param name="redraw">If set to true, redraws the map. if you're calling RegionErase several times, pass false and call Redraw(true) manually at the end.</param>
        public void RegionErase(Region region, Color eraseColor) {
            if (region == null)
                return;
            List<Region> regions = new List<Region>();
            regions.Add(region);
            RegionErase(regions, eraseColor);
        }


        /// <summary>
        /// Paints the region pixels with a given eraseColor.
        /// </summary>
        /// <param name="region">Region.</param>
        /// <param name="eraseColor">Erase color.</param>
        /// <param name="redraw">If set to true, redraws the map. if you're calling RegionErase several times, pass false and call Redraw(true) manually at the end.</param>
        public void RegionErase(List<Region> regions, Color eraseColor) {
            RegionErase(regions, eraseColor, false);
        }


        /// <summary>
        /// Paints the region pixels with a given eraseColor.
        /// </summary>
        /// <param name="region">Region.</param>
        /// <param name="eraseColor">Erase color.</param>
        /// <param name="invertMode">If set to true, clears everything except the regions</param>
        /// <param name="redraw">If set to true, redraws the map. if you're calling RegionErase several times, pass false and call Redraw(true) manually at the end.</param>
        public void RegionErase(List<Region> regions, Color eraseColor, bool invertMode) {
            if (regions == null)
                return;

            // Get all triangles and its colors
            Texture2D texture;
            Color[] colors;
            Texture2D tex = (Texture2D)transform.GetComponent<Renderer>().sharedMaterial.mainTexture;
            if (_earthStyle == EARTH_STYLE.SolidColor || tex == null) {
                int tw = 2048;
                int th = 1024;
                texture = new Texture2D(tw, th, TextureFormat.RGB24, false);
                colors = new Color[tw * th];
                Color solidColor = _earthColor;
                for (int k = 0; k < colors.Length; k++) {
                    colors[k] = solidColor;
                }
            } else {
                int tw = tex.width;
                int th = tex.height;
                texture = new Texture2D(tw, th, TextureFormat.RGB24, false);
                colors = tex.GetPixels();
            }
            if (disposalManager != null) {
                disposalManager.MarkForDisposal(texture); //.hideFlags = HideFlags.DontSave;
            }
            int width = texture.width;
            int height = texture.height;
            int rCount = regions.Count;
            if (invertMode) {
                Color[] maskColors = new Color[width * height];
                Color maskColor = Color.white;
                List<IAdminEntity> entities = new List<IAdminEntity>();
                for (int k = 0; k < rCount; k++) {
                    if (regions[k] == null)
                        continue;
                    RegionPaint(maskColors, width, height, regions[k], maskColor, false);
                    entities.Add(regions[k].entity);
                }
                // Clears all colors from original texture except the masked colors
                for (int k = 0; k < colors.Length; k++) {
                    if (maskColors[k].a < 1f) {
                        colors[k] = eraseColor;
                    }
                }
                // Mark all other entities as hidden
                for (int k = 0; k < _countries.Length; k++) {
                    if (!entities.Contains(_countries[k])) {
                        _countries[k].hidden = true;
                    }
                }
            } else {
                for (int k = 0; k < rCount; k++) {
                    if (regions[k] == null) {
                        continue;
                    }
                    RegionPaint(colors, width, height, regions[k], eraseColor, false);
                    regions[k].entity.hidden = true;
                }
            }
            texture.SetPixels(colors);
            texture.Apply();
            transform.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
        }

        /// <summary>
        /// Marks a list of regions to extrude by a given amount (0..1)
        /// </summary>
        /// <param name="amount">Amount in the 0..1 range.</param>
        public void RegionSetCustomElevation(List<Region> regions, float amount) {
            if (regions == null)
                return;
            int count = regions.Count;
            bool changes = false;
            for (int k = 0; k < count; k++) {
                Region region = regions[k];
                if (region != null) {
                    if (region.extrusionAmount != amount) {
                        region.extrusionAmount = amount;
                        changes = true;
                    }
                    if (!extrudedRegions.Contains(region)) {
                        extrudedRegions.Add(region);
                        changes = true;
                    }
                }
            }
            if (changes) {
                earthLastElevation = -1;
                if (renderViewportIsEnabled) {
                    UpdateViewport();
                }
            }
        }

        /// <summary>
        /// Cancels extrusion effect of a set of regions
        /// </summary>
        /// <param name="regions">Regions.</param>
        public void RegionRemoveCustomElevation(List<Region> regions) {
            if (regions == null)
                return;
            int count = regions.Count;
            bool changes = false;
            for (int k = 0; k < count; k++) {
                regions[k].extrusionAmount = 0;
                if (extrudedRegions.Contains(regions[k])) {
                    changes = true;
                    extrudedRegions.Remove(regions[k]);
                }
            }
            if (changes) {
                earthLastElevation = -1;
                if (renderViewportIsEnabled) {
                    UpdateViewport();
                }
            }
        }


        /// <summary>
        /// Removes any extrusion from all regions
        /// </summary>
        public void RegionRemoveAllCustomElevations() {
            if (extrudedRegions == null)
                return;
            bool changes = false;
            int count = extrudedRegions.Count;
            for (int k = 0; k < count; k++) {
                Region region = extrudedRegions[k];
                if (region.extrusionAmount > 0) {
                    extrudedRegions[k].extrusionAmount = 0;
                    changes = true;
                }
            }
            extrudedRegions.Clear();
            if (changes) {
                earthLastElevation = -1;
                if (renderViewportIsEnabled) {
                    UpdateViewport();
                }
            }
        }



        /// <summary>
        /// Returns a region which is the result of mergin region1 and region2. Original regions are not modified.
        /// </summary>
        /// <returns>The merge.</returns>
        /// <param name="region1">Region1.</param>
        /// <param name="region2">Region2.</param>
        public Region RegionMerge(Region region1, Region region2) {
            Region newRegion = region1.Clone();

            RegionMagnet(newRegion, region2);
            Clipper clipper = new Clipper();
            clipper.AddPath(newRegion, PolyType.ptSubject);
            clipper.AddPath(region2, PolyType.ptClip);
            clipper.Execute(ClipType.ctUnion, newRegion);
            return newRegion;
        }


        /// <summary>
        /// Creates a new country from a list of regions. Existing regions will be extracted from previous sovereign. If a region belongs to a province, entire province is transferred. Returns the index of the new country.
        /// </summary>
        /// <returns>The new country index or -1 if failed.</returns>
        public int RegionsToCountry(List<Region> regions, string newCountryName, bool redraw = true) {

            if (regions == null || regions.Count == 0)
                return -1;

            // Checks if newCountryName already exists
            int countryIndex = GetCountryIndex(newCountryName);
            if (countryIndex >= 0)
                return -1;

            // Add new country
            Region firstRegion = regions[0];
            IAdminEntity entity = firstRegion.entity;
            Country firstCountry = entity is Country ? (Country)entity : GetCountry(((Province)entity).countryIndex);
            string continent = firstCountry.continent;
            Country newCountry = new Country(newCountryName, continent, GetUniqueId(new List<IExtendableAttribute>(_countries)));

            // Create dummy region
            newCountry.regions.Add(new Region(newCountry, 0));
            newCountry.mainRegionIndex = 0;
            newCountry.provinces = new Province[0];
            int newCountryIndex = CountryAdd(newCountry);

            // Transfer regions
            for (int k = 0; k < regions.Count; k++) {
                Region region = regions[k];
                entity = region.entity;
                if (entity is Province) {
                    Province province = (Province)entity;
                    if (province.countryIndex == newCountryIndex) continue;
                    EnsureProvinceDataIsLoaded(province);
                    if (province.regions == null) continue;
                    if (!CountryTransferProvinceRegion(newCountryIndex, region, false)) {
                        CountryDelete(newCountryIndex, false, false);
                        return -1;
                    }
                } else {
                    if (!CountryTransferCountryRegion(newCountryIndex, region, false)) {
                        CountryDelete(newCountryIndex, false, false);
                        return -1;
                    }
                }
            }

            // Remove dummy region
            newCountry.regions.RemoveAt(0);

            // Update geometries and refresh
            RefreshCountryDefinition(newCountryIndex, null);
            if (redraw)
                Redraw();

            return newCountryIndex;
        }


        /// <summary>
        /// Draws an independent outline for a given region and returns the borders gameobject
        /// </summary>
        /// <returns>The region outline.</returns>
        /// <param name="name">Name for the outline gameobject.</param>
        /// <param name="region">Region.</param>
        /// <param name="overridesAnimationSpeed">If set to <c>true</c> overrides animation speed.</param>
        /// <param name="animationSpeed">Animation speed.</param>
        /// <param name="reuseMaterial">If true, the system will try to use a previously border material with same properties. This reduces draw calls when dynamic batching is enabled.</param>
        public GameObject DrawRegionOutline(string name, Region region, Texture borderTexture = null, float texturedBorderWidth = 0.1f, Color tintColor = default(Color), float textureTiling = 1f, float animationSpeed = 0f, bool forceUseTexturedBorder = false, bool reuseMaterial = true) {
            if (borderTexture == null && forceUseTexturedBorder) {
                borderTexture = outlineMatTextured.mainTexture;
            }
            region.customBorder.texture = borderTexture;
            region.customBorder.width = texturedBorderWidth;
            region.customBorder.textureTiling = textureTiling;
            region.customBorder.animationSpeed = animationSpeed;
            if (tintColor != default(Color)) {
                region.customBorder.tintColor = tintColor;
            }
            GameObject boldFrontiers = DrawRegionOutlineMesh(name, region, true, animationSpeed, reuseMaterial);
            boldFrontiers.layer = gameObject.layer;
            boldFrontiers.transform.SetParent(transform, false);
            return boldFrontiers;
        }


        #endregion

    }

}