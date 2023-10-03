// World Map Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

// Comment this macro to disable TextMesh Pro labels
#define USE_TEXTMESH_PRO

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if USE_TEXTMESH_PRO
using TMPro;
#endif

namespace WorldMapStrategyKit {

    public partial class WMSK : MonoBehaviour {

        const string OVERLAY_TEXT_ROOT = "TextRoot";
        const float WMSK_TERRAIN_MODE_Y_OFFSET = 5000f;

        GameObject textRoot;
        Font labelsFont;
        UnityEngine.Object labelsFontTMPro;
        Material labelsFontTMProMaterial;
        Material labelsShadowMaterial;

#if USE_TEXTMESH_PRO
        float[] labelsCurve;
#endif
        List<MeshRect> meshRects;

        int antarctica, greenland, brazil, india, usa, canada, china, russia;

        #region Country Labels

        void ResetDefaultCountryLabelOffset() {
            antarctica = -1;
            greenland = -1;
            brazil = -1;
            india = -1;
            usa = -1;
            canada = -1;
            china = -1;
            russia = -1;
        }

        /// <summary>
        /// Custom label offset for default map
        /// </summary>
        /// <returns>The default country label offset.</returns>
        Vector2 GetDefaultCountryLabelOffset(int countryIndex) {
            if (antarctica < 0)
                antarctica = GetCountryIndex("Antarctica");
            if (greenland < 0)
                greenland = GetCountryIndex("Greenland");
            if (brazil < 0)
                brazil = GetCountryIndex("Brazil");
            if (india < 0)
                india = GetCountryIndex("India");
            if (usa < 0)
                usa = GetCountryIndex("United States of America");
            if (canada < 0)
                canada = GetCountryIndex("Canada");
            if (china < 0)
                china = GetCountryIndex("China");
            if (russia < 0)
                russia = GetCountryIndex("Russia");

            float zoomFactor = transform.localScale.x / 200.0f;
            if (_frontiersDetail == FRONTIERS_DETAIL.Low) {
                if (countryIndex == antarctica)
                    return zoomFactor * new Vector2(0, 3f);
                if (countryIndex == greenland)
                    return zoomFactor * new Vector2(0, -3f);
                if (countryIndex == brazil)
                    return zoomFactor * new Vector2(1f, 4f);
                if (countryIndex == india)
                    return zoomFactor * new Vector2(-2f, 0);
                if (countryIndex == usa)
                    return zoomFactor * new Vector2(-1f, 0);
                if (countryIndex == canada)
                    return zoomFactor * new Vector2(-3f, 0);
                if (countryIndex == china)
                    return zoomFactor * new Vector2(-1f, -1f);
            } else {
                if (countryIndex == antarctica)
                    return zoomFactor * new Vector2(0, 2f);
                if (countryIndex == brazil)
                    return zoomFactor * new Vector2(2f, 4f);
                if (countryIndex == usa)
                    return zoomFactor * new Vector2(-2f, 0);
                if (countryIndex == canada)
                    return zoomFactor * new Vector2(-7f, 0);
                if (countryIndex == china)
                    return zoomFactor * new Vector2(-3f, -0f);
            }

            return Misc.Vector2zero;
        }


        void ReloadFont() {
            if (_countryLabelsFont != null && _countryLabelsFont.dynamic) {
                Debug.LogWarning("Dynamic fonts are not yet supported in WMSK.");
                _countryLabelsFont = null;
            }
            if (_countryLabelsFont == null) {
                Font font = Resources.Load<Font>("WMSK/Font/Lato");
                if (font != null) {
                    labelsFont = Instantiate(font);
                }
            } else {
                labelsFont = Instantiate(_countryLabelsFont);
            }
            if (labelsFont == null)
                return;
            if (disposalManager != null) {
                disposalManager.MarkForDisposal(labelsFont);
            }
            Material fontMaterial = Instantiate(Resources.Load<Material>("WMSK/Materials/Font")); // this material is linked to a shader that has into account zbuffer
            if (labelsFont.material != null) {
                fontMaterial.mainTexture = labelsFont.material.mainTexture;
            }
            if (disposalManager != null) {
                disposalManager.MarkForDisposal(fontMaterial); // fontMaterial.hideFlags = HideFlags.DontSave;
            }
            labelsFont.material = fontMaterial;
            labelsShadowMaterial = Instantiate(fontMaterial);
            if (disposalManager != null) {
                disposalManager.MarkForDisposal(labelsShadowMaterial); // labelsShadowMaterial.hideFlags = HideFlags.DontSave;
            }
            labelsShadowMaterial.renderQueue--;

            if (_countryLabelsFontTMPro == null) {
                labelsFontTMPro = null;  // dummy assignment to avoid compiler warning if TMPro is not installed
            }
#if USE_TEXTMESH_PRO
            if (_countryLabelsFontTMPro == null) {
                _countryLabelsFontTMPro = Resources.Load("WMSK/Font/TextMeshPro/Lato SDF");
                if (_countryLabelsFontTMPro == null) {
                    Debug.LogWarning("Please assign an SDF Font to World Map Strategy Kit inspector. You can create SDF Fonts using TextMesh Pro Font Asset creator tool.");
                }
            }
            labelsFontTMPro = Instantiate(_countryLabelsFontTMPro);
            labelsFontTMProMaterial = _countryLabelsFontTMProMaterial;
            if (labelsFontTMPro != null) {
                if (labelsFontTMProMaterial == null) {
                    labelsFontTMProMaterial = ((TMP_FontAsset)labelsFontTMPro).material;
                }
                ((TMP_FontAsset)labelsFontTMPro).material = labelsFontTMProMaterial;
            }
#endif
            if (labelsFontTMPro != null) {
                if (disposalManager != null) {
                    disposalManager.MarkForDisposal(labelsFontTMPro);
                }
            }
        }

        /// <summary>
        /// Draws the map labels. Note that it will update cached textmesh objects if labels are already drawn. Used internally.
        /// </summary>
        public void DrawMapLabels() {
            if (!gameObject.activeInHierarchy)
                return;

            if (_provinceLabelsVisibility == PROVINCE_LABELS_VISIBILITY.Automatic) {
                if (_countryLabelsTextEngine == TEXT_ENGINE.TextMeshPro) {
                    foreach (Country country in countries) {
                        if (country.mainRegion != null) {
                            country.mainRegion.curvedLabelInfo.isDirty = true;
                        }
                    }
                }
                RedrawCountryLabels();
                RedrawProvinceLabels(_countryHighlighted);
            }

            if (renderViewportIsEnabled) {  // refresh labels if they're floating
                UpdateCountryFloatingLabels();
                UpdateProvinceFloatingLabels();
            }

        }


        /// <summary>
        /// Draws the country labels. Note that it will update cached textmesh objects if labels are already drawn. Used internally.
        /// </summary>
        void RedrawCountryLabels() {

            if (!_showCountryNames || !gameObject.activeInHierarchy || _countries == null)
                return;

            // Set colors
            labelsFont.material.color = _countryLabelsColor;
            labelsShadowMaterial.color = _countryLabelsShadowColor;

            // Create texts
            DestroyCountryLabels();
            GameObject overlay = GetOverlayLayer(true);
            Transform t = overlay.transform.Find(OVERLAY_TEXT_ROOT);
            if (t == null) {
                textRoot = new GameObject(OVERLAY_TEXT_ROOT);
                if (disposalManager != null)
                    disposalManager.MarkForDisposal(textRoot);
                textRoot.layer = overlay.layer;
            } else {
                textRoot = t.gameObject;
            }

            if (_countryLabelsTextEngine == TEXT_ENGINE.TextMeshPro) {
#if USE_TEXTMESH_PRO
                // assign root before text creation so it doesn't invoke mesh regeneration twice by Text Mesh Pro
                textRoot.transform.SetParent(overlay.transform, false);
                DrawTextMeshProLabels();
#else
				DrawUnityStandardTextLabels ();
#endif
            } else {
                DrawUnityStandardTextLabels();
            }

            // Adjusts parent
            if (textRoot.transform.parent != overlay.transform) {
                textRoot.transform.SetParent(overlay.transform, false);
            }
            float textElevation = renderViewportIsEnabled ? -0.001f : -labelsElevation;
            textRoot.transform.localPosition = new Vector3(0, 0, textElevation);
            textRoot.transform.localRotation = Misc.QuaternionZero;
            textRoot.transform.localScale = new Vector3(1.0f / mapWidth, 1.0f / mapHeight, 1);

            // Adjusts alpha based on distance to camera
            if (Application.isPlaying) {
                FadeCountryLabels();
            }
        }

        #region TextMesh Pro support

#if USE_TEXTMESH_PRO


        void DrawTextMeshProLabels() {

            if (labelsFontTMPro == null) {
                ReloadFont();
                if (labelsFontTMPro == null) return;
            }

            float mw = mapWidth;
            float mh = mapHeight;

            if (labelsCurve == null || labelsCurve.Length == 0) {
                ComputeLabelsCurve();
            }

            Vector2 center = Misc.Vector2zero;
            for (int countryIndex = 0; countryIndex < _countries.Length; countryIndex++) {
                Country country = _countries[countryIndex];
                if (country.hidden || !country.labelVisible || country.mainRegionIndex < 0 || country.mainRegionIndex >= country.regions.Count)
                    continue;

                Region region = country.regions[country.mainRegionIndex];
                if (!ComputeCurvedLabelData(region))
                    continue;

                if (country.labelOffset.x != 0 || country.labelOffset.y != 0) {
                    Vector2 countryCenter = country.center;
                    center = countryCenter + country.labelOffset;
                } else if (_countryLabelsUseCentroid) {
                    center = country.centroid;
                } else {
                    FastVector.Average(ref region.curvedLabelInfo.axisStart, ref region.curvedLabelInfo.axisEnd, ref center);
                }
                center.x *= mw;
                center.y *= mh;

                // Adjusts country name length
                string countryName = country.customLabel != null ? country.customLabel : country.name.ToUpper();
                if (countryName.Length == 0)
                    continue;

                if (countryName.Length > 15) {
                    countryName = BreakOneLineString(countryName);
                }

                // add caption
                GameObject textObj;
                TextMeshPro tm;
                Color labelColor = country.labelColorOverride ? country.labelColor : _countryLabelsColor;
                if (country.labelTextMeshGO == null) {
                    // create base text
                    textObj = new GameObject(countryName);
                    textObj.hideFlags = HideFlags.DontSave;
                    tm = textObj.AddComponent<TextMeshPro>();
                    tm.alignment = TextAlignmentOptions.Center;
                    tm.enableWordWrapping = false;
                    country.labelTextMeshPro = tm;
                    country.labelTextMeshGO = tm.gameObject;
                    textObj.transform.SetParent(textRoot.transform, false);
                    textObj.layer = textRoot.gameObject.layer;
                } else {
                    tm = (TextMeshPro)country.labelTextMeshPro;
                    textObj = tm.gameObject;
                    textObj.transform.localPosition = center;
                }
                tm.font = (TMP_FontAsset)labelsFontTMPro;
                tm.color = labelColor;

                Material fontMat;
                if (_countryLabelsEnableAutomaticFade) {
                    // By using fontMaterial we're forcing to instantiate the material which will enable individual colors and alpha
                    fontMat = tm.fontMaterial;
                } else {
                    fontMat = tm.fontSharedMaterial;
                }

                if (_countryLabelsOutlineWidth > 0) {
                    fontMat.SetColor(ShaderParams.OutlineColor, _countryLabelsOutlineColor);
                    fontMat.SetFloat(ShaderParams.OutlineWidth, _countryLabelsOutlineWidth);
                    fontMat.EnableKeyword(ShaderParams.SKW_OUTLINE);
                } else {
                    fontMat.DisableKeyword(ShaderParams.SKW_OUTLINE);
                }

                tm.text = countryName;
                textObj.transform.localPosition = center;
                country.labelMeshWidth = tm.preferredWidth;
                country.labelMeshHeight = tm.preferredHeight;
                country.labelMeshCenter = center;

                float meshWidth = country.labelMeshWidth;
                float meshHeight = country.labelMeshHeight;

                // adjusts scale to fit in region
                Vector2 axis = region.curvedLabelInfo.axisEnd - region.curvedLabelInfo.axisStart;
                float scale;
                if (country.labelFontSizeOverride) {
                    scale = country.labelFontSize;
                } else {
                    // axisWidth represents the length of the label along the longest axis
                    float axisWidth = new Vector2(axis.x * mw, axis.y * mh).magnitude;
                    // axisAveragedWidth represents the average length of the region (used as a maximum height for the label)
                    float axisAveragedThickness = new Vector2(region.curvedLabelInfo.axisAveragedThickness.x * mw, region.curvedLabelInfo.axisAveragedThickness.y * mh).magnitude;
                    float scaleheight = axisAveragedThickness / meshHeight;
                    float scaleWidth = axisWidth / meshWidth;
                    scale = Mathf.Min(scaleWidth, scaleheight);
                    if (meshHeight * scale < _countryLabelsAbsoluteMinimumSize) {
                        scale = _countryLabelsAbsoluteMinimumSize / meshHeight;
                    }
                    scale *= _countryLabelsSize * 2f;
                }

                // apply scale
                textObj.transform.localScale = country.labelMeshLocalScale = new Vector3(scale, scale, 1);

                // Apply axis rotation or user defined rotation
                Quaternion labelRotation;
                if (country.labelRotation > 0) {
                    labelRotation = Quaternion.Euler(0, 0, country.labelRotation);
                } else {
                    labelRotation = Quaternion.Euler(0, 0, region.curvedLabelInfo.axisAngle);
                }
                textObj.transform.localRotation = country.labelMeshLocalRotation = labelRotation;

                // Compute fitting curve
                tm.havePropertiesChanged = true; // Need to force the TextMeshPro Object to be updated.
                tm.ForceMeshUpdate(); // Generate the mesh and populate the textInfo with data we can use and manipulate.

                if (_countryLabelsCurvature > 0) {

                    TMP_TextInfo textInfo = tm.textInfo;
                    int characterCount = textInfo.characterCount;

                    float boundsMinX = tm.bounds.min.x;
                    float boundsMaxX = tm.bounds.max.x;
                    // map bounds to axis length
                    float axisLengthWS = new Vector2(axis.x * mw / scale, axis.y * mh / scale).magnitude;
                    float boundsLength = boundsMaxX - boundsMinX;
                    float boundsMid = (boundsMaxX + boundsMinX) * 0.5f;
                    boundsMinX = boundsMid - (boundsMid - boundsMinX) * axisLengthWS / boundsLength;
                    boundsMaxX = boundsMid + (boundsMaxX - boundsMid) * axisLengthWS / boundsLength;

                    float curveMultiplier = new Vector2(region.curvedLabelInfo.axisMidDisplacement.x * mw / scale, region.curvedLabelInfo.axisMidDisplacement.y * mh / scale).magnitude * _countryLabelsCurvature;
                    // check if axisAveragedThickness is above or below axis
                    Vector2 a = axis * 0.5f + region.curvedLabelInfo.axisMidDisplacement;
                    float dot = a.x * -axis.y + a.y * axis.x;
                    if (dot < 0) {
                        curveMultiplier *= -1f;
                    }
                    float boundsWidth = boundsMaxX - boundsMinX;

                    // Get the index of the mesh used by this character.
                    int materialIndex = textInfo.characterInfo[0].materialReferenceIndex;
                    Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                    for (int i = 0; i < characterCount; i++) {
                        if (!textInfo.characterInfo[i].isVisible)
                            continue;

                        int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                        // Compute the baseline mid point for each character
                        Vector2 offsetToMidBaseline = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, textInfo.characterInfo[i].baseLine);
                        if (float.IsNaN(offsetToMidBaseline.x) || float.IsNaN(offsetToMidBaseline.y)) {
                            continue;
                            //offsetToMidBaseline.x = offsetToMidBaseline.y = 0;
                        }

                        // Apply offset to adjust our pivot point.
                        vertices[vertexIndex + 0].x -= offsetToMidBaseline.x;
                        vertices[vertexIndex + 0].y -= offsetToMidBaseline.y;
                        vertices[vertexIndex + 1].x -= offsetToMidBaseline.x;
                        vertices[vertexIndex + 1].y -= offsetToMidBaseline.y;
                        vertices[vertexIndex + 2].x -= offsetToMidBaseline.x;
                        vertices[vertexIndex + 2].y -= offsetToMidBaseline.y;
                        vertices[vertexIndex + 3].x -= offsetToMidBaseline.x;
                        vertices[vertexIndex + 3].y -= offsetToMidBaseline.y;

                        // Compute the angle of rotation for each character based on the animation curve
                        float x0 = (offsetToMidBaseline.x - boundsMinX) / boundsWidth; // Character's position relative to the bounds of the mesh.
                        float x1 = x0 + 0.01f;
                        const float minT = 0.0f;
                        const float maxT = 0.9999f;
                        if (x0 < minT) x0 = minT; else if (x0 > maxT) x0 = maxT;
                        int ix0 = (int)(x0 * labelsCurve.Length);
                        float y0 = labelsCurve[ix0] * curveMultiplier;
                        if (x1 < minT) x1 = minT; else if (x1 > maxT) x1 = maxT;
                        int ix1 = (int)(x1 * labelsCurve.Length);
                        float y1 = labelsCurve[ix1] * curveMultiplier;

                        float angle;
                        if (y1 == y0) {
                            angle = 0;
                        } else {
                            Vector2 tangent = new Vector2(x1 * boundsWidth + boundsMinX - offsetToMidBaseline.x, y1 - y0);
                            dot = Mathf.Acos(tangent.normalized.x) * Mathf.Rad2Deg;
                            angle = tangent.y > 0 ? dot : 360 - dot;
                        }

                        Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(offsetToMidBaseline.x, y0 + offsetToMidBaseline.y, 0), Quaternion.Euler(0, 0, angle), Misc.Vector3one);

                        vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
                        vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
                        vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
                        vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
                    }

                    // Upload the mesh with the revised information
                    tm.UpdateVertexData();

                }
                country.labelMeshWidth = tm.bounds.size.x;
                country.labelMeshHeight = tm.bounds.size.y;
            }

            if (Application.isPlaying) {
                RepositionTextsDelayed();
                //StartCoroutine(RepositionTextsDelayed());
            } else {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += RepositionTexts;
#endif
            }
        }

        IEnumerator RepositionTextsDelayed() {
            yield return null;
            RepositionTexts();
        }

        void RepositionTexts() {
            // Workaround for TMPPro / Unity's RectTransform bug
            int countriesLength = _countries.Length;
            for (int k = 0; k < countriesLength; k++) {
                _countries[k].labelTextMeshGO.transform.localPosition = _countries[k].labelMeshCenter;
            }
        }

        void ComputeLabelsCurve() {
            const int NUM_KEYS = 255;
            labelsCurve = new float[NUM_KEYS + 1];
            for (int k = 1; k < NUM_KEYS; k++) { // intentionally left first and last entry = 0
                float x = (float)k / NUM_KEYS;
                float y = Mathf.Sin(x * Mathf.PI);
                labelsCurve[k] = y;
            }
        }

#endif
#endregion

                string BreakOneLineString(string s) {
            if (s.Length <= 15)
                return s;
            int spaceIndex = s.IndexOf(' ', s.Length / 2);
            if (spaceIndex < 0)
                spaceIndex = s.LastIndexOf(' ');
            if (spaceIndex >= 0) {
                s = s.Substring(0, spaceIndex) + "\n" + s.Substring(spaceIndex + 1);
            }
            return s;
        }

        void DrawUnityStandardTextLabels() {
            ResetDefaultCountryLabelOffset();

            if (meshRects == null) {
                meshRects = new List<MeshRect>(_countries.Length);
            } else {
                meshRects.Clear();
            }
            float mw = mapWidth;
            float mh = mapHeight;
            for (int countryIndex = 0; countryIndex < _countries.Length; countryIndex++) {
                Country country = _countries[countryIndex];
                if (country.hidden || !country.labelVisible || country.mainRegionIndex < 0 || country.mainRegionIndex >= country.regions.Count)
                    continue;

                Vector2 countryCenter = _countryLabelsUseCentroid ? country.centroid : country.center;
                Vector2 center = new Vector2(countryCenter.x * mapWidth, countryCenter.y * mh) + country.labelOffset;
                center += GetDefaultCountryLabelOffset(countryIndex);

                Region region = country.regions[country.mainRegionIndex];

                // Adjusts country name length
                string countryName = country.customLabel != null ? country.customLabel : country.name.ToUpper();
                bool introducedCarriageReturn = false;
                if (countryName.Length > 15) {
                    countryName = BreakOneLineString(countryName);
                    introducedCarriageReturn = true;
                }

                // add caption
                GameObject textObj;
                TextMesh tm;
                Renderer tmRenderer;
                TextMesh tmShadow = null;
                if (country.labelTextMeshGO == null) {
                    Color labelColor = country.labelColorOverride ? country.labelColor : _countryLabelsColor;
                    Font customFont = country.labelFontOverride ?? labelsFont;
                    if ((object)customFont == null)
                        continue;
                    Material customLabelShadowMaterial = country.labelFontShadowMaterial ?? labelsShadowMaterial;
                    tm = Drawing.CreateText(countryName, null, center, customFont, labelColor, _showLabelsShadow, customLabelShadowMaterial, _countryLabelsShadowColor, _countryLabelsShadowOffset, out tmShadow);
                    textObj = tm.gameObject;
                    country.labelTextMesh = tm;
                    country.labelTextMeshGO = tm.gameObject;
                    tmRenderer = textObj.GetComponent<Renderer>();
                    Bounds bounds = tmRenderer.bounds;
                    country.labelMeshWidth = bounds.size.x;
                    country.labelMeshHeight = bounds.size.y;
                    country.labelMeshCenter = center;
                    textObj.transform.SetParent(textRoot.transform, false);
                    textObj.transform.localPosition = center;
                    textObj.layer = textRoot.gameObject.layer;
                    if (_showLabelsShadow) {
                        country.labelShadowTextMesh = tmShadow;
                        country.labelShadowTextMesh.gameObject.layer = textObj.layer;
                    }
                } else {
                    tm = country.labelTextMesh;
                    textObj = tm.gameObject;
                    textObj.transform.localPosition = center;
                    tmRenderer = textObj.GetComponent<Renderer>();
                }

                float meshWidth = country.labelMeshWidth;
                float meshHeight = country.labelMeshHeight;

                // adjusts caption
                Rect rect = new Rect(region.rect2D.xMin * mw, region.rect2D.yMin * mh, region.rect2D.width * mw, region.rect2D.height * mh);
                float absoluteHeight;
                if (country.labelRotation > 0) {
                    textObj.transform.localRotation = Quaternion.Euler(0, 0, country.labelRotation);
                    absoluteHeight = Mathf.Min(rect.height * _countryLabelsSize, rect.width);
                } else if (rect.height > rect.width * 1.45f) {
                    float angle;
                    if (rect.height > rect.width * 1.5f) {
                        angle = 90;
                    } else {
                        angle = Mathf.Atan2(rect.height, rect.width) * Mathf.Rad2Deg;
                    }
                    textObj.transform.localRotation = Quaternion.Euler(0, 0, angle);
                    absoluteHeight = Mathf.Min(rect.width * _countryLabelsSize, rect.height);
                } else {
                    absoluteHeight = Mathf.Min(rect.height * _countryLabelsSize, rect.width);
                }
                country.labelMeshLocalRotation = textObj.transform.localRotation;

                // adjusts scale to fit width in rect
                float adjustedMeshHeight = introducedCarriageReturn ? meshHeight * 0.5f : meshHeight;
                float scale = absoluteHeight / adjustedMeshHeight;
                if (country.labelFontSizeOverride) {
                    scale = country.labelFontSize;
                } else {
                    float desiredWidth = meshWidth * scale;
                    if (desiredWidth > rect.width) {
                        scale = rect.width / meshWidth;
                    }
                    if (adjustedMeshHeight * scale < _countryLabelsAbsoluteMinimumSize) {
                        scale = _countryLabelsAbsoluteMinimumSize / adjustedMeshHeight;
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
                    for (int c = 0; c < countryName.Length; c++) {
                        sb.Append(countryName[c]);
                        if (c < countryName.Length - 1) {
                            sb.Append(spaces);
                        }
                    }
                    wideName = sb.ToString();
                } else {
                    wideName = countryName;
                }

                if (tm.text.Length != wideName.Length) {
                    tm.text = wideName;
                    displayedMeshWidth = tmRenderer.bounds.size.x * scale;
                    displayedMeshHeight = tmRenderer.bounds.size.y * scale;
                    if (_showLabelsShadow) {
                        tmShadow.text = wideName;
                    }
                }

                // apply scale
                textObj.transform.localScale = country.labelMeshLocalScale = new Vector3(scale, scale, 1);

                // Save mesh rect for overlapping checking
                if (country.labelOffset == Misc.Vector2zero) {
                    float xMin = center.x - displayedMeshWidth * 0.5f;
                    float yMin = center.y - displayedMeshHeight * 0.5f;
                    float xMax = xMin + displayedMeshWidth;
                    float yMax = yMin + displayedMeshHeight;
                    MeshRect mr = new MeshRect(countryIndex, new Vector4(xMin, yMin, xMax, yMax));
                    meshRects.Add(mr);
                }
            }

            // Simple-fast overlapping checking
            int cont = 0;
            bool needsResort = true;

            int meshRectsCount = meshRects.Count;
            while (needsResort && ++cont < 10) {
                meshRects.Sort(overlapComparer);
                needsResort = false;
                for (int c = 1; c < meshRectsCount; c++) {
                    Vector4 r1 = meshRects[c].rect;
                    for (int prevc = c - 1; prevc >= 0; prevc--) {
                        Vector4 r2 = meshRects[prevc].rect;
                        bool overlaps = !(r2.x > r1.z || r2.z < r1.x || r2.y > r1.w || r2.w < r1.y);
                        if (overlaps) {
                            needsResort = true;
                            int thisCountryIndex = meshRects[c].entityIndex;
                            Country country = _countries[thisCountryIndex];
                            GameObject thisLabel = country.labelTextMeshGO;

                            // displaces this label
                            float offsety = r1.w - r2.y;
                            offsety = Mathf.Min(country.regions[country.mainRegionIndex].rect2D.height * mh * 0.35f, offsety);
                            thisLabel.transform.localPosition = new Vector3(country.labelMeshCenter.x, country.labelMeshCenter.y - offsety, thisLabel.transform.localPosition.z);
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
        }

        int overlapComparer(MeshRect r1, MeshRect r2) {
            float r1MidY = (r1.rect.w + r1.rect.y) * 0.5f;
            float r2MidY = (r2.rect.w + r2.rect.y) * 0.5f;
            if (r2MidY < r1MidY) {
                return -1;
            } else if (r2MidY > r1MidY) {
                return 1;
            } else {
                return 0;
            }
        }

        class MeshRect {
            public int entityIndex;
            public Vector4 rect;

            public MeshRect(int entityIndex, Vector4 rect) {
                this.entityIndex = entityIndex;
                this.rect = rect;
            }
        }

        void DestroyCountryLabels() {
            if (_countries != null) {
                for (int k = 0; k < _countries.Length; k++) {
                    _countries[k].labelTextMeshGO = null;
                    _countries[k].labelTextMesh = null;
                }
            }
            if (textRoot != null) {
                DestroyImmediate(textRoot);
            }
            // Security check: if there're still gameObjects under TextRoot, also delete it
            if (overlayLayer != null) {
                Transform t = overlayLayer.transform.Find(OVERLAY_TEXT_ROOT);
                if (t != null && t.childCount > 0) {
                    DestroyImmediate(t.gameObject);
                }
            }
        }


        void FadeCountryLabels() {
            if (_countryLabelsTextEngine == TEXT_ENGINE.TextMeshPro) {
                FadeCountryLabelsTMPro();
            } else {
                FadeCountryLabelsStandard();
            }
        }

        public void ResetCountryLabelsAlpha() {
            if (_countryLabelsTextEngine == TEXT_ENGINE.TextMeshPro) {
                ResetCountryLabelsAlphaTMPro();
            } else {
                ResetCountryLabelsAlphaStandard();
            }
        }


#if USE_TEXTMESH_PRO

        // Automatically fades in/out country labels based on their screen size
        void FadeCountryLabelsTMPro() {

            if (!_countryLabelsEnableAutomaticFade)
                return;

            float th;

            Quaternion oldRot = _currentCamera.transform.rotation;
            if (renderViewportIsTerrain) { // workaround for terrain mode
                _currentCamera.transform.forward = transform.forward;
            }
            Vector2 y0 = _currentCamera.WorldToViewportPoint(transform.TransformPoint(0, -0.5f, 0));
            Vector2 y1 = _currentCamera.WorldToViewportPoint(transform.TransformPoint(0, 0.5f, 0));
            th = Vector2.Distance(y0, y1);
            if (renderViewportIsTerrain) {
                _currentCamera.transform.rotation = oldRot;
            }

            float maxAlpha = _countryLabelsColor.a;
            float labelFadeMinSize = _countryLabelsAutoFadeMinHeight; // 0.018f;
            float labelFadeMaxSize = _countryLabelsAutoFadeMaxHeight; // 0.2f;
            float labelFadeMinFallOff = _countryLabelsAutoFadeMinHeightFallOff; // 0.005f;
            float labelFadeMaxFallOff = _countryLabelsAutoFadeMaxHeightFallOff; // 0.5f;

            float mh = mapHeight;
            for (int k = 0; k < _countries.Length; k++) {
                Country country = _countries[k];
                TextMeshPro tm = (TextMeshPro)country.labelTextMeshPro;
                if (tm != null) {
                    // Fade label
                    float labelSize = (country.labelMeshHeight + country.labelMeshWidth) * 0.5f;
                    float screenHeight = labelSize * country.labelMeshLocalScale.y * th / mh;
                    float ad;
                    if (screenHeight < labelFadeMinSize)
                    {
                        float t = (labelFadeMinSize - screenHeight) / labelFadeMinFallOff;
                        if (t < 0) t = 0; else if (t > 1) t = 1;
                        ad = 1f - t;
                    }
                    else if (screenHeight > labelFadeMaxSize)
                    {
                        float t = (screenHeight - labelFadeMaxSize) / labelFadeMaxFallOff;
                        if (t < 0) t = 0; else if (t > 1) t = 1;
                        ad = 1f - t;
                    }
                    else
                    {
                        ad = 1.0f;
                    }
                    float newAlpha = ad * maxAlpha;
                    Color tmColor = tm.color;
                    if (tmColor.a != newAlpha) {
                        tmColor.a = newAlpha;
                        tm.fontSharedMaterial.SetColor(ShaderParams.FaceColor, tmColor);
                        tm.fontSharedMaterial.SetColor(ShaderParams.OutlineColor, new Color(_countryLabelsOutlineColor.r, _countryLabelsOutlineColor.g, _countryLabelsOutlineColor.b, _countryLabelsOutlineColor.a * newAlpha));
                        Color underlayColor = tm.fontSharedMaterial.GetColor(ShaderParams.UnderlayColor);
                        underlayColor.a = newAlpha;
                        tm.fontSharedMaterial.SetColor(ShaderParams.UnderlayColor, underlayColor);
                    }
                }
            }
        }

        /// <summary>
        /// Restores country labels alpha to 1.0. Used by editor since in edit mode one need to see labels!
        /// </summary>
        void ResetCountryLabelsAlphaTMPro() {
            for (int k = 0; k < _countries.Length; k++) {
                Country country = _countries[k];
                TextMeshPro tm = (TextMeshPro)country.labelTextMeshPro;
                if (tm != null) {
                    if (tm.color.a != 1f) {
                        tm.fontSharedMaterial.SetColor(ShaderParams.FaceColor, tm.color);
                        tm.fontSharedMaterial.SetColor(ShaderParams.OutlineColor, _countryLabelsOutlineColor);

                    }
                }
            }
        }



#endif

        void FadeCountryLabelsStandard() {

            // Automatically fades in/out country labels based on their screen size
            if (!_countryLabelsEnableAutomaticFade)
                return;

            float th;

            Quaternion oldRot = _currentCamera.transform.rotation;
            if (renderViewportIsTerrain) { // workaround for terrain mode
                _currentCamera.transform.forward = transform.forward;
            }
            Vector2 y0 = _currentCamera.WorldToViewportPoint(transform.TransformPoint(0, -0.5f, 0));
            Vector2 y1 = _currentCamera.WorldToViewportPoint(transform.TransformPoint(0, 0.5f, 0));
            th = Vector2.Distance(y0, y1);
            if (renderViewportIsTerrain) {
                _currentCamera.transform.rotation = oldRot;
            }

            float maxAlpha = _countryLabelsColor.a;
            float maxAlphaShadow = _countryLabelsShadowColor.a;
            float labelFadeMinSize = _countryLabelsAutoFadeMinHeight; // 0.018f;
            float labelFadeMaxSize = _countryLabelsAutoFadeMaxHeight; // 0.2f;
            float labelFadeMinFallOff = _countryLabelsAutoFadeMinHeightFallOff; // 0.005f;
            float labelFadeMaxFallOff = _countryLabelsAutoFadeMaxHeightFallOff; // 0.5f;

            float mh = mapHeight;
            for (int k = 0; k < _countries.Length; k++) {
                Country country = _countries[k];
                TextMesh tm = country.labelTextMesh;
                if (tm != null) {
                    // Fade label
                    float labelSize = (country.labelMeshHeight + country.labelMeshWidth) * 0.5f;
                    float screenHeight = labelSize * country.labelMeshLocalScale.y * th / mh;
                    float ad;
                    if (screenHeight < labelFadeMinSize) {
                        float t = (labelFadeMinSize - screenHeight) / labelFadeMinFallOff;
                        if (t < 0) t = 0; else if (t > 1) t = 1;
                        ad = 1f - t;
                    } else if (screenHeight > labelFadeMaxSize) {
                        float t = (screenHeight - labelFadeMaxSize) / labelFadeMaxFallOff;
                        if (t < 0) t = 0; else if (t > 1) t = 1;
                        ad = 1f - t;
                    } else {
                        ad = 1.0f;
                    }
                    float newAlpha = ad * maxAlpha;
                    Color tmColor = tm.color;
                    if (tmColor.a != newAlpha) {
                        tmColor.a = newAlpha;
                        tm.color = tmColor;
                    }
                    // Fade label shadow
                    TextMesh tmShadow = country.labelShadowTextMesh;
                    if (tmShadow != null) {
                        newAlpha = ad * maxAlphaShadow;
                        Color tmShadowColor = tmShadow.color;
                        if (tmShadowColor.a != newAlpha) {
                            tmShadowColor.a = newAlpha;
                            tmShadow.color = tmShadowColor;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Restores country labels alpha to 1.0. Used by editor since in edit mode one need to see labels!
        /// </summary>
        void ResetCountryLabelsAlphaStandard() {
            if (_countries == null) return;
            for (int k = 0; k < _countries.Length; k++) {
                Country country = _countries[k];
                TextMesh tm = country.labelTextMesh;
                if (tm != null) {
                    if (tm.color.a != 1f) {
                        tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, 1f);
                    }
                    // Fade label shadow
                    TextMesh tmShadow = country.labelShadowTextMesh;
                    if (tmShadow != null) {
                        if (tmShadow.color.a != 1f) {
                            tmShadow.color = new Color(tmShadow.color.r, tmShadow.color.g, tmShadow.color.b, 1f);
                        }
                    }
                }
            }
        }


        void DestroyMapLabels() {
            DestroyCountryLabels();
            DestroyProvinceLabels();
        }


        void UpdateCountryFloatingLabels() {
            if (_countryLabelsRenderingMode != TEXT_RENDERING_MODE.FloatingAboveViewport || _countries == null || renderViewport == null || !renderViewportIsEnabled) return;

            int countriesCount = _countries.Length;
            for (int k = 0; k < countriesCount; k++) {
                Country country = _countries[k];
                UpdateFloatingLabel(country);
            }
        }

        void UpdateFloatingLabel(AdminEntity entity) {
            GameObject labelGO = entity.labelTextMeshGO;
            if (labelGO == null) return;

            Vector2 labelPos = entity.labelMeshCenter;
            labelPos.x /= mapWidth;
            labelPos.y /= mapHeight;
            if (!renderViewportRect.Contains(labelPos)) {
                labelGO.SetActive(false);
                return;
            } else {
                labelGO.SetActive(true);
            }
            Transform t = labelGO.transform;
            t.position = Map2DToWorldPosition(labelPos, _countryLabelsElevation);
            t.rotation = renderViewport.transform.rotation * entity.labelMeshLocalRotation;
            Vector3 scale = entity.labelMeshLocalScale;
            scale.x *= _renderViewportScaleFactor;
            scale.y *= _renderViewportScaleFactor;
            t.localScale = scale;
        }

        #endregion


    }

}