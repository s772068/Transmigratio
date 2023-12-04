// World Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WorldMapStrategyKit {

    public partial class WMSK : MonoBehaviour {

        #region Public properties

        [SerializeField]
        bool _fogOfWarLayer = false;

        /// <summary>
        /// Enables/disables the fog of war layer when viewport is used.
        /// </summary>
        public bool fogOfWarLayer {
            get {
                return _fogOfWarLayer;
            }
            set {
                if (value != _fogOfWarLayer) {
                    _fogOfWarLayer = value;
                    isDirty = true;
                    UpdateFogOfWarLayer();
                }
            }
        }

        [SerializeField]
        Color _fogOfWarColor = new Color(0, 0, 0, 0.2f);

        /// <summary>
        /// Sets fog of war color.
        /// </summary>
        public Color fogOfWarColor {
            get {
                return _fogOfWarColor;
            }
            set {
                if (value != _fogOfWarColor) {
                    _fogOfWarColor = value;
                    isDirty = true;
                    UpdateFogOfWarLayer();
                }
            }
        }


        [SerializeField]
        float _fogOfWarLayerElevation = 1f;

        /// <summary>
        /// Elevation of fog of war layer when viewport is used.
        /// </summary>
        public float fogOfWarLayerElevation {
            get {
                return _fogOfWarLayerElevation;
            }
            set {
                if (value != _fogOfWarLayerElevation) {
                    _fogOfWarLayerElevation = value;
                    isDirty = true;
                    UpdateFogOfWarLayer();
                }
            }
        }


        #endregion

        #region Public Fog of War API

        /// <summary>
        /// Resets the fog of war making everything dark again if visible is set to true, or everything clear if visible is false.
        /// </summary>
        public void FogOfWarClear(bool visible) {
            Texture2D tex = (Texture2D)fogOfWarMat.mainTexture;
            Color32[] colors = tex.GetPixels32();
            Color32 newColor = visible ? new Color32(0, 0, 0, 255) : new Color32(0, 0, 0, 0);
            for (int k = 0; k < colors.Length; k++)
                colors[k] = newColor;
            tex.SetPixels32(colors);
            tex.Apply();
        }

        /// <summary>
        /// Gets fog of war transparency level on specified local map coordinate.
        /// </summary>
        /// <param name="x">The x coordinate in the map plane (local coordinate).</param>
        /// <param name="y">The y coordinate in the map plane (local coordinate).</param>
        public float FogOfWarGet(float x, float y) {
            if (x < -0.5f || x > 0.5f || y < -0.5f || y > 0.5f)
                return 0;

            Texture2D tex = (Texture2D)fogOfWarMat.mainTexture;
            int row = Mathf.FloorToInt(tex.height * (y + 0.5f));
            int col = Mathf.FloorToInt(tex.width * (x + 0.5f));

            Color32[] colors = tex.GetPixels32();
            return colors[row * tex.width + col].a / 255.0f;
        }

        /// <summary>
        /// Sets fog of war transparency level on specified coordinate.
        /// </summary>
        /// <param name="x">The x coordinate in the map plane (local coordinate).</param>
        /// <param name="y">The y coordinate in the map plane (local coordinate).</param>
        /// <param name="alpha">The transparency level from 0 (fully transparent) to 1 (opaque).</param>
        public void FogOfWarSet(float x, float y, float alpha) {
            if (x < -0.5f || x > 0.5f || y < -0.5f || y > 0.5f)
                return;

            Texture2D tex = (Texture2D)fogOfWarMat.mainTexture;
            Color32[] colors = tex.GetPixels32();
            Color32 newColor = (Color32)_fogOfWarColor;
            newColor.a = (byte)(alpha * 255.0f);
            int row = Mathf.FloorToInt(tex.height * (y + 0.5f));
            int col = Mathf.FloorToInt(tex.width * (x + 0.5f));
            colors[row * tex.width + col] = newColor;
            tex.SetPixels32(colors);
            tex.Apply();
        }

        /// <summary>
        /// Adjust fog of war transparency level for a circle of a given radius and specified coordinates.
        /// </summary>
        /// <param name="x">The x coordinate in the map plane (local coordinate).</param>
        /// <param name="y">The y coordinate in the map plane (local coordinate).</param>
        /// <param name="alphaIncrement">The amount to sum or substract to fog alpha. Result will always be clamped to 0-1..</param>
        /// <param name="alpha">The transparency level from 0 (fully transparent) to 1 (opaque).</param>
        public void FogOfWarIncrement(float x, float y, float alphaIncrement, float radius) {
            Texture2D tex = (Texture2D)fogOfWarMat.mainTexture;
            Color32[] colors = tex.GetPixels32();
            int row = Mathf.FloorToInt(tex.height * (y + 0.5f));
            int col = Mathf.FloorToInt(tex.width * (x + 0.5f));

            int delta = Mathf.FloorToInt(tex.height * radius);
            for (int r = row - delta; r < row + delta; r++) {
                for (int c = col - delta; c < col + delta; c++) {
                    float distance = Mathf.Sqrt((row - r) * (row - r) + (col - c) * (col - c));
                    if (distance <= delta) {
                        float newAlpha = Mathf.Clamp01(colors[r * tex.width + c].a / 255.0f + alphaIncrement);
                        colors[r * tex.width + c].a = (byte)(newAlpha * 255.0f);
                    }
                }
            }
            tex.SetPixels32(colors);
            tex.Apply();
        }

        /// <summary>
        /// Sets alpha of fog over an entire country, including all regions.
        /// </summary>
        public void FogOfWarSetCountry(int countryIndex, float alpha) {
            Country country = _countries[countryIndex];
            for (int k = 0; k < country.regions.Count; k++) {
                FogOfWarSetCountryRegion(countryIndex, k, alpha);
            }
        }

        /// <summary>
        /// Sets alpha of fog over a specific country region.
        /// </summary>
        public void FogOfWarSetCountryRegion(int countryIndex, int regionIndex, float alpha) {
            Texture2D tex = (Texture2D)fogOfWarMat.mainTexture;
            Color32[] colors = tex.GetPixels32();

            float stepy = 1f / tex.height;
            float stepx = 1f / tex.width;
            Region region = _countries[countryIndex].regions[regionIndex];
            Rect rect = region.rect2D;
            byte newAlpha = (byte)(alpha * 255.0f);
            for (float y = rect.yMin; y < rect.yMax; y += stepy) {
                int rowPos = Mathf.FloorToInt(tex.height * (y + 0.5f)) * tex.width;
                for (float x = rect.xMin; x < rect.xMax; x += stepx) {
                    if (region.Contains(new Vector2(x, y))) {
                        int col = Mathf.FloorToInt(tex.width * (x + 0.5f));
                        colors[rowPos + col].a = newAlpha;
                    }
                }
            }
            tex.SetPixels32(colors);
            tex.Apply();
        }

        /// <summary>
        /// Sets alpha of fog over an entire province, including all regions.
        /// </summary>
        public void FogOfWarSetProvince(int provinceIndex, float alpha) {
            Province province = provinces[provinceIndex];
            for (int k = 0; k < province.regions.Count; k++) {
                FogOfWarSetProvinceRegion(provinceIndex, k, alpha);
            }
        }


        /// <summary>
        /// Sets alpha of fog over a specific province region.
        /// </summary>
        public void FogOfWarSetProvinceRegion(int provinceIndex, int regionIndex, float alpha) {
            Texture2D tex = (Texture2D)fogOfWarMat.mainTexture;
            Color32[] colors = tex.GetPixels32();

            float stepy = 1f / tex.height;
            float stepx = 1f / tex.width;
            Region region = provinces[provinceIndex].regions[regionIndex];
            Rect rect = region.rect2D;
            byte newAlpha = (byte)(alpha * 255.0f);
            for (float y = rect.yMin; y < rect.yMax; y += stepy) {
                int rowPos = Mathf.FloorToInt(tex.height * (y + 0.5f)) * tex.width;
                for (float x = rect.xMin; x < rect.xMax; x += stepx) {
                    if (region.Contains(new Vector2(x, y))) {
                        int col = Mathf.FloorToInt(tex.width * (x + 0.5f));
                        colors[rowPos + col].a = newAlpha;
                    }
                }
            }
            tex.SetPixels32(colors);
            tex.Apply();
        }


        /// <summary>
        /// Sets alpha of fog for a list of cells
        /// </summary>
        public void FogOfWarSetCell(Cell cell, float alpha) {
            Texture2D tex = (Texture2D)fogOfWarMat.mainTexture;
            Color32[] colors = tex.GetPixels32();

            float stepy = 0.33f / Mathf.Max(tex.height, _gridRows);
            float stepx = 0.33f / Mathf.Max(tex.width, _gridColumns);
            Rect rect = cell.rect2D;
            byte newAlpha = (byte)(alpha * 255.0f);
            for (float y = rect.yMin; y < rect.yMax; y += stepy) {
                int rowPos = Mathf.FloorToInt(tex.height * (y + 0.5f)) * tex.width;
                for (float x = rect.xMin; x < rect.xMax; x += stepx) {
                    if (cell.Contains(new Vector2(x, y))) {
                        int col = Mathf.FloorToInt(tex.width * (x + 0.5f));
                        colors[rowPos + col].a = newAlpha;
                    }
                }
            }
            tex.SetPixels32(colors);
            tex.Apply();
        }

        /// <summary>
        /// Sets alpha of fog for a list of cells
        /// </summary>
        public void FogOfWarSetCells(List<int> cells, float alpha) {
            Texture2D tex = (Texture2D)fogOfWarMat.mainTexture;
            Color32[] colors = tex.GetPixels32();

            byte newAlpha = (byte)(alpha * 255.0f);
            float stepy = 0.33f / Mathf.Max(tex.height, _gridRows);
            float stepx = 0.33f / Mathf.Max(tex.width, _gridColumns);
            int cellCount = cells.Count;
            Vector2 pos;
            for (int k = 0; k < cellCount; k++) {
                Cell cell = this.cells[cells[k]];
                Rect rect = cell.rect2D;
                for (float y = rect.yMin; y < rect.yMax; y += stepy) {
                    int rowPos = (int)(tex.height * (y + 0.5f)) * tex.width;
                    pos.y = y;
                    for (float x = rect.xMin; x < rect.xMax; x += stepx) {
                        pos.x = x;
                        if (cell.Contains(pos)) {
                            int col = (int)(tex.width * (x + 0.5f) + 0.5f);
                            colors[rowPos + col].a = newAlpha;
                        }
                    }
                }
            }
            tex.SetPixels32(colors);
            tex.Apply();
        }


        /// <summary>
        /// Gets or sets the fog of war texture.
        /// </summary>
        /// <value>The fog of war texture.</value>
        public Texture2D fogOfWarTexture {
            get { return (Texture2D)fogOfWarMat.mainTexture; }
            set {
                if (fogOfWarMat.mainTexture != value) {
                    fogOfWarMat.mainTexture = value;
                    UpdateFogOfWarLayer();
                }
            }
        }

        #endregion

    }

}