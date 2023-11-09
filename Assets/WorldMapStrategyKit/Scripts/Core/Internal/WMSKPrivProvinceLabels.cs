// World Map Strategy Kit for Unity - Main Script
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

        const string OVERLAY_TEXT_PROVINCE_ROOT = "TextProvinceRoot";

        GameObject textProvinceRoot;
        Font provLabelsFont;
        Material provLabelsShadowMaterial;
        Country countryProvincesLabelsShown;

        #region Province Labels

        void ReloadProvinceFont() {
            if (_provinceLabelsFont != null && _provinceLabelsFont.dynamic) {
                Debug.LogWarning("Dynamic fonts are not yet supported in WMSK.");
                _provinceLabelsFont = null;
            }
            if (_provinceLabelsFont == null) {
                provLabelsFont = Instantiate(Resources.Load<Font>("WMSK/Font/Lato"));
            } else {
                provLabelsFont = Instantiate(_provinceLabelsFont);
            }
            if (disposalManager != null) disposalManager.MarkForDisposal(provLabelsFont); //provLabelsFont.hideFlags = HideFlags.DontSave;
            Material fontMaterial = Instantiate(Resources.Load<Material>("WMSK/Materials/Font")); // this material is linked to a shader that has into account zbuffer
            if (provLabelsFont.material != null) {
                fontMaterial.mainTexture = provLabelsFont.material.mainTexture;
            }
            if (disposalManager != null) disposalManager.MarkForDisposal(fontMaterial); // fontMaterial.hideFlags = HideFlags.DontSave;
            provLabelsFont.material = fontMaterial;
            provLabelsShadowMaterial = Instantiate(fontMaterial);
            if (disposalManager != null) disposalManager.MarkForDisposal(provLabelsShadowMaterial); // provLabelsShadowMaterial.hideFlags = HideFlags.DontSave;
            provLabelsShadowMaterial.renderQueue--;
        }


        void RedrawProvinceLabels(Country country) {
            DestroyProvinceLabels();
            if (_showProvinceNames) {
                DrawProvinceLabelsInt(country);
                UpdateProvinceFloatingLabels();
            }
        }


        /// <summary>
        /// Draws the province labels for a given country. Note that it will update cached textmesh objects if labels are already drawn. Used internally.
        /// </summary>
        void DrawProvinceLabelsInt(Country country, Color color = default(Color)) {

            if (!isPlaying || !gameObject.activeInHierarchy || country == null || country.hidden || provinces == null || country.provinces == null)
                return;

            countryProvincesLabelsShown = country;

            // Create texts
            GameObject overlay = GetOverlayLayer(true);
            Transform t = overlay.transform.Find(OVERLAY_TEXT_PROVINCE_ROOT);
            if (t == null) {
                textProvinceRoot = new GameObject(OVERLAY_TEXT_PROVINCE_ROOT);
                if (disposalManager != null) disposalManager.MarkForDisposal(textProvinceRoot); // textProvinceRoot.hideFlags = HideFlags.DontSave;
                textProvinceRoot.layer = overlay.layer;
            } else {
                textProvinceRoot = t.gameObject;
            }

            if (meshRects == null) {
                meshRects = new List<MeshRect>(country.provinces.Length);
            } else {
                meshRects.Clear();
            }
            float mw = mapWidth;
            float mh = mapHeight;

            for (int p = 0; p < country.provinces.Length; p++) {
                Province province = country.provinces[p];
                EnsureProvinceDataIsLoaded(province);
                if (province == null || province.hidden || !province.labelVisible || province.regions == null || province.mainRegionIndex < 0 || province.mainRegionIndex >= province.regions.Count)
                    continue;

                if (_provinceLabelsVisibility == PROVINCE_LABELS_VISIBILITY.Automatic && !_showAllCountryProvinceNames && province != _provinceHighlighted)
                    continue;

                Vector2 provinceCenter = _countryLabelsUseCentroid ? province.centroid : province.center;
                Vector2 center = new Vector2(provinceCenter.x * mapWidth, provinceCenter.y * mh) + province.labelOffset;
                Region region = province.regions[province.mainRegionIndex];

                // Adjusts province name length
                string provinceName = province.customLabel != null ? province.customLabel : province.name.ToUpper();
                bool introducedCarriageReturn = false;
                if (provinceName.Length > 15) {
                    int spaceIndex = provinceName.IndexOf(' ', provinceName.Length / 2);
                    if (spaceIndex >= 0) {
                        provinceName = provinceName.Substring(0, spaceIndex) + "\n" + provinceName.Substring(spaceIndex + 1);
                        introducedCarriageReturn = true;
                    }
                }

                // add caption
                GameObject textObj;
                TextMesh tm;
                Renderer tmRenderer;
                TextMesh tmShadow = null;

                Color labelColor = province.labelColorOverride ? province.labelColor : _provinceLabelsColor;
                if (color != default(Color)) {
                    labelColor = color;
                }

                if (province.labelTextMeshGO == null) {
                    Font customFont = province.labelFontOverride ?? provLabelsFont;
                    Material customLabelShadowMaterial = province.labelFontShadowMaterial ?? provLabelsShadowMaterial;
                    tm = Drawing.CreateText(provinceName, null, center, customFont, labelColor, _showProvinceLabelsShadow, customLabelShadowMaterial, _provinceLabelsShadowColor, _provinceLabelsShadowOffset, out tmShadow);
                    textObj = tm.gameObject;
                    province.labelTextMesh = tm;
                    province.labelTextMeshGO = tm.gameObject;
                    tmRenderer = textObj.GetComponent<Renderer>();
                    Bounds bounds = tmRenderer.bounds;
                    province.labelMeshWidth = bounds.size.x;
                    province.labelMeshHeight = bounds.size.y;
                    province.labelMeshCenter = center;
                    textObj.transform.SetParent(textProvinceRoot.transform, false);
                    textObj.transform.localPosition = center;
                    textObj.layer = textProvinceRoot.gameObject.layer;
                    if (_showProvinceLabelsShadow) {
                        province.labelShadowTextMesh = tmShadow;
                        province.labelShadowTextMesh.gameObject.layer = textObj.layer;
                    }
                } else {
                    tm = province.labelTextMesh;
                    tm.color = labelColor;
                    tmShadow = province.labelShadowTextMesh;
                    if (tmShadow != null) {
                        tmShadow.color = _provinceLabelsShadowColor;
                    }
                    textObj = tm.gameObject;
                    textObj.transform.localPosition = center;
                    tmRenderer = textObj.GetComponent<Renderer>();
                }

                province.labelMeshCenter = center;
                float meshWidth = province.labelMeshWidth;
                float meshHeight = province.labelMeshHeight;

                // adjusts caption
                Rect rect = new Rect(region.rect2D.xMin * mw, region.rect2D.yMin * mh, region.rect2D.width * mw, region.rect2D.height * mh);
                float absoluteHeight;
                if (province.labelRotation > 0) {
                    textObj.transform.localRotation = Quaternion.Euler(0, 0, province.labelRotation);
                    absoluteHeight = Mathf.Min(rect.height * _provinceLabelsSize, rect.width);
                } else if (rect.height > rect.width * 1.45f) {
                    float angle;
                    if (rect.height > rect.width * 1.5f) {
                        angle = 90;
                    } else {
                        angle = Mathf.Atan2(rect.height, rect.width) * Mathf.Rad2Deg;
                    }
                    textObj.transform.localRotation = Quaternion.Euler(0, 0, angle);
                    absoluteHeight = Mathf.Min(rect.width * _provinceLabelsSize, rect.height);
                } else {
                    absoluteHeight = Mathf.Min(rect.height * _provinceLabelsSize, rect.width);
                }

                province.labelMeshLocalRotation = textObj.transform.localRotation;

                // adjusts scale to fit width in rect
                float adjustedMeshHeight = introducedCarriageReturn ? meshHeight * 0.5f : meshHeight;
                float scale = absoluteHeight / adjustedMeshHeight;
                if (province.labelFontSizeOverride) {
                    scale = province.labelFontSize;
                } else {
                    float desiredWidth = meshWidth * scale;
                    if (desiredWidth > rect.width) {
                        scale = rect.width / meshWidth;
                    }
                    if (adjustedMeshHeight * scale < _provinceLabelsAbsoluteMinimumSize) {
                        scale = _provinceLabelsAbsoluteMinimumSize / adjustedMeshHeight;
                    }
                }

                // stretchs out the caption
                float displayedMeshWidth = meshWidth * scale;
                float displayedMeshHeight = meshHeight * scale;
                string wideName;
                int times = Mathf.FloorToInt(rect.width * 0.45f / (meshWidth * scale));
                if (times > 10)
                    times = 10;
                if (times > 0) {
                    StringBuilder sb = new StringBuilder();
                    string spaces = new string(' ', times * 2);
                    for (int c = 0; c < provinceName.Length; c++) {
                        sb.Append(provinceName[c]);
                        if (c < provinceName.Length - 1) {
                            sb.Append(spaces);
                        }
                    }
                    wideName = sb.ToString();
                } else {
                    wideName = provinceName;
                }

                if (tm.text.Length != wideName.Length) {
                    tm.text = wideName;
                    displayedMeshWidth = tmRenderer.bounds.size.x * scale;
                    displayedMeshHeight = tmRenderer.bounds.size.y * scale;
                    if (_showProvinceLabelsShadow) {
                        tmShadow.text = wideName;
                    }
                }

                // apply scale
                textObj.transform.localScale = province.labelMeshLocalScale = new Vector3(scale, scale, 1);

                // Save mesh rect for overlapping checking
                if (province.labelOffset == Misc.Vector2zero) {
                    int provinceIndex = GetProvinceIndex(province);
                    float xMin = center.x - displayedMeshWidth * 0.5f;
                    float yMin = center.y - displayedMeshHeight * 0.5f;
                    float xMax = xMin + displayedMeshWidth;
                    float yMax = yMin + displayedMeshHeight;
                    MeshRect mr = new MeshRect(provinceIndex, new Vector4(xMin, yMin, xMax, yMax));
                    meshRects.Add(mr);
                }
            }

            // Simple-fast overlapping checking
            int cont = 0;
            bool needsResort = true;

            int meshRectsCount = meshRects.Count;
            while (needsResort && ++cont < 10) {
                meshRects.Sort(overlapComparer);

                for (int c = 1; c < meshRectsCount; c++) {
                    Vector4 r1 = meshRects[c].rect;
                    for (int prevc = c - 1; prevc >= 0; prevc--) {
                        Vector4 r2 = meshRects[prevc].rect;
                        bool overlaps = !(r2.x > r1.z || r2.z < r1.x || r2.y > r1.w || r2.w < r1.y);
                        if (overlaps) {
                            needsResort = true;
                            int thisProvinceIndex = meshRects[c].entityIndex;
                            Province province = _provinces[thisProvinceIndex];
                            GameObject thisLabel = province.labelTextMeshGO;

                            // displaces this label
                            float offsety = r1.w - r2.y;
                            offsety = Mathf.Min(province.regions[province.mainRegionIndex].rect2D.height * mh * 0.35f, offsety);
                            thisLabel.transform.localPosition = new Vector3(province.labelMeshCenter.x, province.labelMeshCenter.y - offsety, thisLabel.transform.localPosition.z);
                            float width = r1.z - r1.x;
                            float height = r1.w - r1.y;
                            float xMin = thisLabel.transform.localPosition.x - width * 0.5f;
                            float yMin = thisLabel.transform.localPosition.y - height * 0.5f;
                            float xMax = xMin + width;
                            float yMax = yMin + height;
                            r1 = new Vector4(xMin, yMin, xMax, yMax);
                            meshRects[c].rect = r1;
                        }
                    }
                }
            }

            // Adjusts parent
            textProvinceRoot.transform.SetParent(overlay.transform, false);
            textProvinceRoot.transform.localPosition = new Vector3(0, 0, -0.001f);
            textProvinceRoot.transform.localRotation = Misc.QuaternionZero;
            textProvinceRoot.transform.localScale = new Vector3(1.0f / mw, 1.0f / mh, 1);

            // Adjusts alpha based on distance to camera
            if (isPlaying) {
                FadeProvinceLabels();
            }

        }


        void DestroyProvinceLabels() {
            if (_provinces != null) {
                for (int k = 0; k < _provinces.Length; k++) {
                    _provinces[k].labelTextMesh = null;
                    _provinces[k].labelTextMeshGO = null;
                }
            }
            if (textProvinceRoot != null) {
                DestroyImmediate(textProvinceRoot);
            }
            countryProvincesLabelsShown = null;
        }


        // Automatically fades in/out province labels based on their screen size
        void FadeProvinceLabels() {

            if (!_provinceLabelsEnableAutomaticFade)
                return;

            if (countryProvincesLabelsShown == null || countryProvincesLabelsShown.provinces == null)
                return;

            // Automatically fades in/out province labels based on their screen size

            float y0 = _currentCamera.WorldToViewportPoint(transform.TransformPoint(0, 0, 0)).y;
            float y1 = _currentCamera.WorldToViewportPoint(transform.TransformPoint(0, 1.0f, 0)).y;
            float th = y1 - y0;

            float maxAlpha = _provinceLabelsColor.a;
            float maxAlphaShadow = _provinceLabelsShadowColor.a;
            float labelFadeMinSize = _provinceLabelsAutoFadeMinHeight; // 0.018f;
            float labelFadeMaxSize = _provinceLabelsAutoFadeMaxHeight; // 0.2f;
            float labelFadeMinFallOff = _provinceLabelsAutoFadeMinHeightFallOff; // 0.005f;
            float labelFadeMaxFallOff = _provinceLabelsAutoFadeMaxHeightFallOff; // 0.5f;

            float mh = mapHeight;
            for (int k = 0; k < countryProvincesLabelsShown.provinces.Length; k++) {
                Province province = countryProvincesLabelsShown.provinces[k];
                TextMesh tm = province.labelTextMesh;
                if (tm != null) {
                    // Fade label
                    float labelSize = (province.labelMeshHeight + province.labelMeshWidth) * 0.5f;
                    float screenHeight = labelSize * province.labelMeshLocalScale.y * th / mh;
                    float ad;
                    if (screenHeight < labelFadeMinSize) {
                        ad = Mathf.Lerp(1.0f, 0, (labelFadeMinSize - screenHeight) / labelFadeMinFallOff);
                    } else if (screenHeight > labelFadeMaxSize) {
                        ad = Mathf.Lerp(1.0f, 0, (screenHeight - labelFadeMaxSize) / labelFadeMaxFallOff);
                    } else {
                        ad = 1.0f;
                    }
                    float newAlpha = ad * maxAlpha;
                    if (tm.color.a != newAlpha) {
                        tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, newAlpha);
                    }
                    // Fade label shadow
                    TextMesh tmShadow = province.labelShadowTextMesh;
                    if (tmShadow != null) {
                        newAlpha = ad * maxAlphaShadow;
                        if (tmShadow.color.a != newAlpha) {
                            tmShadow.color = new Color(tmShadow.color.r, tmShadow.color.g, tmShadow.color.b, maxAlphaShadow * ad);
                        }
                    }
                }
            }
        }

        void UpdateProvinceFloatingLabels() {
            if (_countryLabelsRenderingMode != TEXT_RENDERING_MODE.FloatingAboveViewport || countryProvincesLabelsShown == null || renderViewport == null || !renderViewportIsEnabled) return;

            int provincesCount = countryProvincesLabelsShown.provinces.Length;
            for (int k = 0; k < provincesCount; k++) {
                UpdateFloatingLabel(countryProvincesLabelsShown.provinces[k]);

            }

        }


        #endregion


    }

}