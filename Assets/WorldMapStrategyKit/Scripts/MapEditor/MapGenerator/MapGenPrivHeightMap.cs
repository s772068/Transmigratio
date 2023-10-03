//#define SHOW_DEBUG_GIZMOS

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using WorldMapStrategyKit.MapGenerator.Geom;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WorldMapStrategyKit {

    public partial class WMSK_Editor : MonoBehaviour {

        float[] heights;
        Color[] heightColors, backgroundColors;
        int currentHeightmapWidth, currentHeightmapHeight;
        int currentBackgroundWidth, currentBackgroundHeight;

        public void GenerateHeightMap(bool preview = false) {

            try {
                if (userHeightMapTexture != null) {
                    Color[] hh = userHeightMapTexture.GetPixels();
                    heights = new float[hh.Length];
                    for (int k = 0; k < hh.Length; k++) {
                        heights[k] = hh[k].r;
                    }
                    return;
                }

                bool lowQuality = mapGenerationQuality == MapGenerationQuality.Draft || preview;
                currentHeightmapWidth = lowQuality ? 256 : this.heightMapWidth;
                currentHeightmapHeight = lowQuality ? 128 : this.heightMapHeight;

                int heightsLen = currentHeightmapWidth * currentHeightmapHeight;
                if (heights == null || heights.Length != heightsLen) {
                    heights = new float[heightsLen];
                } else {
                    for (int k = 0; k < heightsLen; k++) {
                        heights[k] = 0;
                    }
                }

                if (noiseOctaves == null)
                    return;

                // Fetch some noise
                float ratio = (float)currentHeightmapWidth / currentHeightmapHeight;
                for (int n = 0; n < noiseOctaves.Length; n++) {
                    if (noiseOctaves[n].disabled)
                        continue;
                    int c = 0;
                    float frequency = noiseOctaves[n].frequency;
                    float amplitude = noiseOctaves[n].amplitude;
                    float xmult = frequency * ratio / currentHeightmapWidth;
                    float ymult = frequency / currentHeightmapHeight;
                    if (noiseOctaves[n].ridgeNoise) {
                        for (int y = 0; y < currentHeightmapHeight; y++) {
                            float yy = y * ymult;
                            for (int x = 0; x < currentHeightmapWidth; x++, c++) {
                                float noise = Mathf.PerlinNoise(x * xmult, yy);
                                noise = 2f * (0.5f - Mathf.Abs(0.5f - noise));
                                noise *= amplitude;
                                if (n > 0) {
                                    noise *= heights[c];
                                }
                                heights[c] += noise;
                            }
                        }
                    } else {
                        for (int y = 0; y < currentHeightmapHeight; y++) {
                            float yy = y * ymult;
                            for (int x = 0; x < currentHeightmapWidth; x++, c++) {
                                float noise = Mathf.PerlinNoise(x * xmult, yy) * amplitude;
                                heights[c] += noise;
                            }
                        }
                    }
                }

                // Apply exponent & island factor
                float maxHeight = float.MinValue;
                for (int k = 0; k < heightsLen; k++) {
                    float h = heights[k];

                    // island factor
                    float x = k % currentHeightmapWidth;
                    float y = k / currentHeightmapWidth;
                    float nx = 2f * x / currentHeightmapWidth - 1f;
                    float ny = 2f * y / currentHeightmapHeight - 1f;
                    float dx = nx * nx;
                    float dy = ny * ny;
                    float d = dx + dy;

                    float mask = 1f - d * islandFactor;
                    if (mask < 0) {
                        mask = 0;
                    }
                    h *= mask;

                    // pow
                    h = Mathf.Pow(h, noisePower);

                    h = h + elevationShift;

                    if (h > maxHeight) {
                        maxHeight = h;
                    }

                    heights[k] = h;

                }

                // Normalize values
                if (maxHeight > 0) {
                    for (int k = 0; k < heightsLen; k++) {
                        if (heights[k] < 0) {
                            heights[k] = 0;
                        } else {
                            heights[k] /= maxHeight;
                        }
                    }
                }

                if (heightMapTexture == null || heightMapTexture.width != currentHeightmapWidth || heightMapTexture.height != currentHeightmapHeight) {
                    heightMapTexture = new Texture2D(currentHeightmapWidth, currentHeightmapHeight, TextureFormat.ARGB32, false);
                }
                if (heightColors == null || heightColors.Length != heightsLen) {
                    heightColors = new Color[heights.Length];
                }

                if (preview) {
                    for (int k = 0; k < heightsLen; k++) {
                        heightColors[k].r = heightColors[k].g = heightColors[k].b = heightColors[k].a = heights[k];
                    }
                    heightMapTexture.SetPixels(heightColors);
                    heightMapTexture.Apply();
                }

            } catch (Exception ex) {
                Debug.LogError("Error generating heightmap: " + ex.ToString());
            }
        }


        void GenerateWaterMask() {
            // Create water mask
            if (waterMaskTexture == null || waterMaskTexture.width != currentHeightmapWidth || waterMaskTexture.height != currentHeightmapHeight) {
                waterMaskTexture = new Texture2D(currentHeightmapWidth, currentHeightmapHeight, TextureFormat.ARGB32, false);
            }
            Color wc = new Color();
            int heightsLength = heights.Length;
            for (int k = 0; k < heightsLength; k++) {
                wc.r = wc.g = wc.b = wc.a = heights[k];
                heightColors[k] = wc;
            }
            GenerateMotionVectors();
            waterMaskTexture.SetPixels(heightColors);
            waterMaskTexture.Apply();
        }

        void GenerateMotionVectors() {

            // Per pixel, calculate foam intensity and motion vector
            // For motion vector, we calculate a weighted average of vectors surrounding the pixel for a custom sized kernel size
            // For foam intensity, we take the weighted average
            int hks = 2; // Half of kernel size

            Vector2[,] kernelWeight = new Vector2[hks * 2, hks * 2];
            for (int y = -hks; y < hks; y++) {
                for (int x = -hks; x < hks; x++) {
                    Vector2 v = new Vector2(x, y);
                    v.Normalize();
                    kernelWeight[y + hks, x + hks] = -v;
                }
            }

            int colorBufferSize = heightColors.Length;
            for (int j = 0; j < currentHeightmapHeight; j++) {
                for (int k = 0; k < currentHeightmapWidth; k++) {
                    int currentPixel = j * currentHeightmapWidth + k;
                    float sumElev = 0;
                    // Compute weighter vectors
                    Vector2 avgVector = Misc.Vector2zero;
                    for (int y = -hks; y < hks; y++) {
                        int pixelPos = currentPixel + y * currentHeightmapWidth - hks;
                        if (pixelPos >= 0 && pixelPos < colorBufferSize) {
                            for (int x = 0; x < hks * 2 && pixelPos < colorBufferSize; x++, pixelPos++) {
                                if (pixelPos != currentPixel) {
                                    float elev = heightColors[pixelPos].r;
                                    avgVector += kernelWeight[y + hks, x] * elev;
                                    sumElev += elev;
                                }
                            }
                        }
                    }
                    if (sumElev > 0)
                        avgVector /= sumElev;
                    heightColors[currentPixel].g = avgVector.x;
                    heightColors[currentPixel].b = avgVector.y;
                    heightColors[currentPixel].a = sumElev / (hks * hks * 4);
                }
            }

            // apply blur
            BlurPass(heightColors, currentHeightmapWidth, currentHeightmapHeight, 1, 0);
            BlurPass(heightColors, currentHeightmapWidth, currentHeightmapHeight, 0, 1);

            // clamp colors
            for (int j = 0; j < colorBufferSize; j++) {
                heightColors[j].g = (heightColors[j].g * 0.5f) + 0.5f;
                heightColors[j].b = (heightColors[j].b * 0.5f) + 0.5f;
            }
        }


        readonly float[] gaussian = {
            0.153170f,
            0.144893f,
            0.122649f,
            0.092902f,
            0.062970f
        };

        void BlurPass(Color[] colors, int width, int height, int incx, int incy) {

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    int pixelPos = y * width + x;
                    float denom = gaussian[0];
                    float[] sum = new float[] {
                        colors [pixelPos].g * denom,
                        colors [pixelPos].b * denom,
                        colors [pixelPos].a * denom
                    };
                    for (int k = 1; k < 5; k++) {
                        int y0 = y + incy * k;
                        if (y0 >= 0 && y0 < height) {
                            int x0 = x + incx * k;
                            if (x0 >= 0 && x0 < width) {
                                int pixelPos0 = y0 * width + x0;
                                float g = gaussian[k];
                                denom += g;
                                Color color = colors[pixelPos0];
                                sum[0] += color.g * g;
                                sum[1] += color.b * g;
                                sum[2] += color.a * g;
                            }
                        }
                    }
                    colors[pixelPos].g = sum[0] / denom;
                    colors[pixelPos].b = sum[1] / denom;
                    colors[pixelPos].a = sum[2] / denom;
                }
            }
        }

        void GenerateHeightGradient() {

            if (heightGradientPreset == HeightMapGradientPreset.Custom && heightGradient == null) {
                heightGradientPreset = HeightMapGradientPreset.ColorByHeight;
            }

            GradientColorKey[] colorKeys;
            switch (heightGradientPreset) {
                case HeightMapGradientPreset.ColorByHeight:
                    heightGradient = new Gradient();
                    colorKeys = new GradientColorKey[4];
                    colorKeys[0] = new GradientColorKey(Color.gray, 0f);
                    colorKeys[1] = new GradientColorKey(new Color(0.133f, 0.545f, 0.133f), seaLevel);
                    colorKeys[2] = new GradientColorKey(new Color(0.898f, 0.898f, 0.298f), (seaLevel + 1f) * 0.5f);
                    colorKeys[3] = new GradientColorKey(Color.white, 1f);
                    heightGradient.colorKeys = colorKeys;
                    break;
                case HeightMapGradientPreset.ColorByHeightLight:
                    heightGradient = new Gradient();
                    colorKeys = new GradientColorKey[4];
                    colorKeys[0] = new GradientColorKey(Color.gray, 0f);
                    colorKeys[1] = new GradientColorKey(new Color(0.333f, 0.745f, 0.333f), seaLevel);
                    colorKeys[2] = new GradientColorKey(new Color(0.998f, 0.998f, 0.498f), (seaLevel + 1f) * 0.5f);
                    colorKeys[3] = new GradientColorKey(Color.white, 1f);
                    heightGradient.colorKeys = colorKeys;
                    break;
                case HeightMapGradientPreset.Grayscale:
                    heightGradient = new Gradient();
                    colorKeys = new GradientColorKey[3];
                    colorKeys[0] = new GradientColorKey(Color.black, 0f);
                    colorKeys[1] = new GradientColorKey(Color.gray, seaLevel);
                    colorKeys[2] = new GradientColorKey(Color.white, 1f);
                    heightGradient.colorKeys = colorKeys;
                    break;
                case HeightMapGradientPreset.BlackAndWhite:
                    heightGradient = new Gradient();
                    colorKeys = new GradientColorKey[3];
                    colorKeys[0] = new GradientColorKey(Color.black, 0f);
                    colorKeys[1] = new GradientColorKey(Color.white, seaLevel);
                    colorKeys[2] = new GradientColorKey(Color.white, 1f);
                    heightGradient.colorKeys = colorKeys;
                    break;
            }
        }

        void AssignHeightMapToCells() {
            if (heights == null)
                return;

            if (currentHeightmapWidth == 0) {
                return;
            }

            int provCount = mapCells.Count;
            for (int k = 0; k < provCount; k++) {
                MapCell prov = mapCells[k];
                int x = (int)Mathf.Clamp((prov.center.x + 0.5f) * currentHeightmapWidth, 0, currentHeightmapWidth - 1);
                int y = (int)Mathf.Clamp((prov.center.y + 0.5f) * currentHeightmapHeight, 0, currentHeightmapHeight - 1);
                int j = y * currentHeightmapWidth + x;
                float h = heights[j];
                prov.visible = false;

                // province underwater? ignore it
                if (h < seaLevel) continue;

                // check margin
                Rect rect = prov.region.rect2D;
                if (rect.xMin < landRect.xMin || rect.yMin < landRect.yMin || rect.xMax > landRect.xMax || rect.yMax > landRect.yMax) continue;

                prov.visible = true;
            }
        }

        void AssignColorsToProvinces() {
            if (heights == null)
                return;

            if (currentHeightmapWidth == 0) {
                return;
            }

            int provincesCount = mapProvinces.Count;
            for (int k = 0; k < provincesCount; k++) {
                MapProvince province = mapProvinces[k];
                int x = (int)Mathf.Clamp((province.capitalCenter.x + 0.5f) * currentHeightmapWidth, 0, currentHeightmapWidth - 1);
                int y = (int)Mathf.Clamp((province.capitalCenter.y + 0.5f) * currentHeightmapHeight, 0, currentHeightmapHeight - 1);
                int j = y * currentHeightmapWidth + x;
                float h = heights[j];

                province.color = GetHeightMapColor(h);
            }
        }


        void AssignColorsToCountries() {
            if (heights == null)
                return;

            if (currentHeightmapWidth == 0) {
                return;
            }

            int countryCount = mapCountries.Count;
            for (int k = 0; k < countryCount; k++) {
                MapCountry country = mapCountries[k];
                int x = (int)Mathf.Clamp((country.capitalCenter.x + 0.5f) * currentHeightmapWidth, 0, currentHeightmapWidth - 1);
                int y = (int)Mathf.Clamp((country.capitalCenter.y + 0.5f) * currentHeightmapHeight, 0, currentHeightmapHeight - 1);
                int j = y * currentHeightmapWidth + x;
                float h = heights[j];

                country.color = GetHeightMapColor(h);
            }
        }


        Color GetHeightMapColor(float h) {
            Color color;
            if (heightGradientPreset == HeightMapGradientPreset.RandomHSVColors) {
                color = UnityEngine.Random.ColorHSV(heightGradientMinHue, heightGradientMaxHue, heightGradientMinSaturation, heightGradientMaxSaturation, heightGradientMinValue, heightGradientMaxValue);
            } else {
                color = heightGradient.Evaluate(h);
            }
            color.a = h;
            return color;
        }


        void GenerateWorldTextures() {

            // Create background texture
            currentBackgroundWidth = mapGenerationQuality == MapGenerationQuality.Draft ? 256 : this.backgroundTextureWidth;
            currentBackgroundHeight = mapGenerationQuality == MapGenerationQuality.Draft ? 128 : this.backgroundTextureHeight;

            if (backgroundTexture == null || backgroundTexture.width != currentBackgroundWidth || backgroundTexture.height != currentBackgroundHeight) {
                backgroundTexture = new Texture2D(currentBackgroundWidth, currentBackgroundHeight, TextureFormat.RGBA32, true);
            }
            int bufferLen = currentBackgroundWidth * currentBackgroundHeight;
            if (backgroundColors == null || backgroundColors.Length != bufferLen) {
                backgroundColors = new Color[bufferLen];
            }
            Color backColor = seaColor;
            backColor.a = 0;
            backgroundColors.Fill<Color>(backColor);

            if (colorProvinces) {
                int provincesCount = _map.provinces.Length;
                for (int k = 0; k < provincesCount; k++) {
                    Province prov = _map.provinces[k];
                    if (prov.regions == null) {
                        continue;
                    }
                    Region region = prov.regions[0];
                    if (gradientPerPixel) {
                        _map.RegionPaintHeights(backgroundColors, currentBackgroundWidth, currentBackgroundHeight, region, heights, seaLevel, currentHeightmapWidth, currentHeightmapHeight, heightGradient);
                    } else {
                        Color provColor = prov.attrib["mapColor"];
                        _map.RegionPaint(backgroundColors, currentBackgroundWidth, currentBackgroundHeight, region, provColor, false);
                    }
                }
            } else {
                int countriesCount = _map.countries.Length;
                for (int k = 0; k < countriesCount; k++) {
                    Country country = _map.countries[k];
                    if (country.regions == null) {
                        continue;
                    }
                    Region region = country.regions[0];
                    if (gradientPerPixel) {
                        _map.RegionPaintHeights(backgroundColors, currentBackgroundWidth, currentBackgroundHeight, region, heights, seaLevel, currentHeightmapWidth, currentHeightmapHeight, heightGradient);
                    } else {
                        Color countryColor = country.attrib["mapColor"];
                        _map.RegionPaint(backgroundColors, currentBackgroundWidth, currentBackgroundHeight, region, countryColor, false);
                    }
                }
            }

            Color[] newBackgroundColors = new Color[bufferLen];

            // adjusts heights so land is always >= seaLevel and sea is 0
            AdjustHeightmapToGeneratedLand();

            if (drawBorders) {
                float intensity = 1f - bordersIntensity;
                for (int i = 0; i < bordersWidth; i++) {
                    Array.Copy(backgroundColors, newBackgroundColors, bufferLen);
                    for (int k = currentBackgroundWidth * 2 + 2; k < bufferLen - currentBackgroundWidth * 2 - 2; k++) {
                        if (backgroundColors[k].a == 0) continue;
                        Color c0 = backgroundColors[k - currentBackgroundWidth * 2 - 2];
                        Color c1 = backgroundColors[k - currentBackgroundWidth * 2 + 2];
                        Color c2 = backgroundColors[k + currentBackgroundWidth * 2 - 2];
                        Color c3 = backgroundColors[k + currentBackgroundWidth * 2 + 2];
                        float dr = (c0.r + c1.r + c2.r + c3.r) / 4f;
                        float dg = (c0.g + c1.g + c2.g + c3.g) / 4f;
                        float db = (c0.b + c1.b + c2.b + c3.b) / 4f;
                        if (dr != c0.r || dg != c0.g || db != c0.b) {
                            newBackgroundColors[k].r *= intensity;
                            newBackgroundColors[k].g *= intensity;
                            newBackgroundColors[k].b *= intensity;
                        }
                    }
                    Array.Copy(newBackgroundColors, backgroundColors, bufferLen);
                }
            }

            // color sea
            if (drawSeaShoreline) {
                for (int i = 0; i < shorelineWidth; i++) {
                    Color backColorLight = Color.Lerp(Color.white, backColor, 0.75f + 0.25f * i / shorelineWidth);
                    backColorLight.a = 0.1f;
                    Array.Copy(backgroundColors, newBackgroundColors, bufferLen);
                    const int spread = 3;
                    for (int k = currentBackgroundWidth * spread + spread; k < bufferLen - currentBackgroundWidth * spread - spread; k++) {
                        if (backgroundColors[k].a == 0) {
                            Color c0 = backgroundColors[k - currentBackgroundWidth * spread - spread];
                            Color c1 = backgroundColors[k - currentBackgroundWidth * spread];
                            Color c2 = backgroundColors[k - currentBackgroundWidth * spread + spread];
                            Color c3 = backgroundColors[k - spread];
                            Color c4 = backgroundColors[k + spread];
                            Color c5 = backgroundColors[k + currentBackgroundWidth * spread - spread];
                            Color c6 = backgroundColors[k + currentBackgroundWidth * spread];
                            Color c7 = backgroundColors[k + currentBackgroundWidth * spread + spread];
                            if (c0.a > 0 || c1.a > 0 || c2.a > 0 || c3.a > 0 || c4.a > 0 || c5.a > 0 || c6.a > 0 || c7.a > 0) {
                                newBackgroundColors[k] = backColorLight;
                            }
                        }
                    }
                    Array.Copy(newBackgroundColors, backgroundColors, bufferLen);
                }
            }

            if (smoothBorders) {
                for (int i = 0; i < 4; i++) {
                    const float w0 = 0.0162162f;
                    const float w1 = 0.0540540f;
                    const float w2 = w0;
                    const float w3 = w1;
                    const float w4 = 0.2270270f;
                    const float w5 = w1;
                    const float w6 = w0;
                    const float w7 = w1;
                    const float w8 = w0;
                    const float sw = w0 + w1 + w2 + w3 + w4 + w5 + w6 + w7 + w8;
                    for (int k = currentBackgroundWidth + 1; k < bufferLen - currentBackgroundWidth - 1; k++) {
                        Color c0 = backgroundColors[k - currentBackgroundWidth - 1];
                        Color c1 = backgroundColors[k - currentBackgroundWidth];
                        Color c2 = backgroundColors[k - currentBackgroundWidth + 1];
                        Color c3 = backgroundColors[k - 1];
                        Color c4 = backgroundColors[k];
                        Color c5 = backgroundColors[k + 1];
                        Color c6 = backgroundColors[k + currentBackgroundWidth - 1];
                        Color c7 = backgroundColors[k + currentBackgroundWidth];
                        Color c8 = backgroundColors[k + currentBackgroundWidth + 1];
                        float r = c0.r * w0 + c1.r * w1 + c2.r * w2 + c3.r * w3 + c4.r * w4 + c5.r * w5 + c6.r * w6 + c7.r * w7 + c8.r * w8;
                        float g = c0.g * w0 + c1.g * w1 + c2.g * w2 + c3.g * w3 + c4.g * w4 + c5.g * w5 + c6.g * w6 + c7.g * w7 + c8.g * w8;
                        float b = c0.b * w0 + c1.b * w1 + c2.b * w2 + c3.b * w3 + c4.b * w4 + c5.b * w5 + c6.b * w6 + c7.b * w7 + c8.b * w8;
                        float a = c0.a * w0 + c1.a * w1 + c2.a * w2 + c3.a * w3 + c4.a * w4 + c5.a * w5 + c6.a * w6 + c7.a * w7 + c8.a * w8;
                        newBackgroundColors[k].r = r / sw;
                        newBackgroundColors[k].g = g / sw;
                        newBackgroundColors[k].b = b / sw;
                        newBackgroundColors[k].a = a / sw;
                    }
                    Array.Copy(newBackgroundColors, backgroundColors, bufferLen);
                }
            }

            backgroundTexture.SetPixels(backgroundColors);
            backgroundTexture.Apply();

            if (generateScenicWaterMask) {
                GenerateWaterMask();
            }
        }

        void AdjustHeightmapToGeneratedLand() {

            // Set height to 0 out of land areas
            if (heightMapTexture != null) {
                if (currentHeightmapWidth == currentBackgroundWidth && currentHeightmapHeight == currentBackgroundHeight) {
                    int backgroundColorsLength = backgroundColors.Length;
                    for (int k = 0; k < backgroundColorsLength; k++) {
                        float height = backgroundColors[k].a;
                        heights[k] = height;
                        if (height == 0) {
                            heightColors[k] = Misc.ColorClear;
                        } else {
                            heightColors[k].r = heightColors[k].g = heightColors[k].b = heightColors[k].a = height;
                        }
                    }
                } else {
                    for (int k = 0, y = 0; y < currentHeightmapHeight; y++) {
                        int backy = y * currentBackgroundHeight / currentHeightmapHeight;
                        int backyy = backy * currentBackgroundWidth;
                        for (int x = 0; x < currentHeightmapWidth; x++, k++) {
                            int backx = x * currentBackgroundWidth / currentHeightmapWidth;
                            float height = backgroundColors[backyy + backx].a;
                            heights[k] = height;
                            if (height == 0) {
                                heightColors[k] = Misc.ColorClear;
                            } else {
                                heightColors[k].r = heightColors[k].g = heightColors[k].b = heightColors[k].a = height;
                            }
                        }
                    }
                }
                heightMapTexture.SetPixels(heightColors);
                heightMapTexture.Apply();
            }
        }


        void GeneratePathFindingTextures() {

            // Create path-finding land & water map textures
            if (pathFindingLandMapTexture == null || pathFindingLandMapTexture.width != currentHeightmapWidth || pathFindingLandMapTexture.height != currentHeightmapHeight) {
                pathFindingLandMapTexture = new Texture2D(currentHeightmapWidth, currentHeightmapHeight, TextureFormat.R8, false);
            }
            if (pathFindingWaterMapTexture == null || pathFindingWaterMapTexture.width != currentHeightmapWidth || pathFindingWaterMapTexture.height != currentHeightmapHeight) {
                pathFindingWaterMapTexture = new Texture2D(currentHeightmapWidth, currentHeightmapHeight, TextureFormat.R8, false);
            }
            int heightsLength = heights.Length;
            Color32[] lm = new Color32[heightsLength];
            Color32[] wm = new Color32[heightsLength];
            Color32 wc = new Color32(255, 255, 255, 255);
            for (int k = 0; k < heightsLength; k++) {
                float h = heights[k];
                if (h >= seaLevel) lm[k] = wc; else wm[k] = wc;
            }
            pathFindingLandMapTexture.SetPixels32(lm);
            pathFindingLandMapTexture.Apply();
            pathFindingWaterMapTexture.SetPixels32(wm);
            pathFindingWaterMapTexture.Apply();
        }




    }
}