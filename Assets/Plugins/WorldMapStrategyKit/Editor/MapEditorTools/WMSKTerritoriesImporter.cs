#if !UNITY_WSA
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {
    public class WMSKTerritoriesImporter : EditorWindow {
        enum COLOR_PICKER_MODE {
            NONE,
            BACKGROUND_COLOR,
            GOOD_COLOR
        }

        enum DETAIL {
            Fine = 1,
            Coarse = 2
        }

        COLOR_PICKER_MODE colorPickerMode;
        string filePath;
        Texture2D texture, bgColorTex;
        bool working, committing;
        string committingStatus;
        float committingProgress;
        TerritoriesImporterMode mode;
        DETAIL detail = DETAIL.Coarse;
        bool snapToCountryFrontiers;
        bool additive;
        WMSK_TextureImporter ti;
        static readonly GUIContent[] modes = new GUIContent[] {
            new GUIContent ("Countries"),
            new GUIContent ("Provinces")
        };
        static readonly int[] modesValues = new int[] { 0, 1 };
        DateTime lastRefreshTime;
        Color32 bgColor = Color.white;
        Rect textureRect;
        Texture2D goodColorsTex;
        GUIStyle colorTexStyle;
        Country[] newCountries;
        Province[] newProvinces;
        int textureRectWidth, textureRectHeight;
        int overseasProvincesCountryGUIIndex;
        string[] countryNames;
        WMSK map;

        void OnEnable() {
            filePath = "";
            working = false;
            committing = false;
            texture = null;
            ti = null;
            EditorUtility.ClearProgressBar();
            colorPickerMode = COLOR_PICKER_MODE.NONE;
            map = WMSK.instance;

            countryNames = map.GetCountryNames(true);
        }

        void OnDestroy() {
            map.editor.territoryImporterActive = false;
        }

        public static void ShowWindow() {
            int w = 950;
            int h = 450;
            Rect rect = new Rect(Screen.currentResolution.width / 2 - w / 2, Screen.currentResolution.height / 2 - h / 2, w, h);
            GetWindowWithRect<WMSKTerritoriesImporter>(rect, true, "Territories Importer", true);
        }

        void OnGUI() {
            if (WMSK.instance == null) {
                DestroyImmediate(this);
                GUIUtility.ExitGUI();
                return;
            }

            EditorGUILayout.HelpBox("This tool will generate countries or provinces based on the color information of a given texture.", MessageType.Info);
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Texture File...", GUILayout.Width(140))) {
                filePath = EditorUtility.OpenFilePanelWithFilters("Select Texture File", "", new string[] { "Image files", "png,jpg,jpeg" });
                if (filePath.ToUpper().Contains("PROVINC")) { 
                    mode = TerritoriesImporterMode.Provinces;
                    }
                ReadTexture();
            }
            if (texture != null && GUILayout.Button("Reload Texture", GUILayout.Width(120))) {
                ReadTexture();
            }
            EditorGUILayout.EndHorizontal();

            if (texture != null) {
                textureRectWidth = (int)position.width - 400;
                textureRectHeight = (int)position.height - 70;
                textureRect = new Rect(390, 55, textureRectWidth, textureRectHeight);
                GUI.DrawTexture(textureRect, texture, ScaleMode.StretchToFill);
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Creation Mode", GUILayout.Width(150));
                mode = (TerritoriesImporterMode)EditorGUILayout.IntPopup((int)mode, modes, modesValues, GUILayout.Width(120));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Detail", GUILayout.Width(150));
                EditorGUI.BeginChangeCheck();
                detail = (DETAIL)EditorGUILayout.EnumPopup(detail, GUILayout.Width(120));
                if (EditorGUI.EndChangeCheck()) {
                    ReadTexture();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (mode == TerritoriesImporterMode.Countries) {
                    GUI.enabled = false;
                    snapToCountryFrontiers = false;
                    overseasProvincesCountryGUIIndex = -1;
                }
                GUILayout.Label("Snap To Country Frontiers", GUILayout.Width(150));
                snapToCountryFrontiers = EditorGUILayout.Toggle(snapToCountryFrontiers, GUILayout.Width(40));
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Additive", "Do not remove existing entities"), GUILayout.Width(150));
                additive = EditorGUILayout.Toggle(additive, GUILayout.Width(40));
                EditorGUILayout.EndHorizontal();

                if (mode == TerritoriesImporterMode.Provinces) {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Overseas Provinces Owner", GUILayout.Width(150));
                    overseasProvincesCountryGUIIndex = EditorGUILayout.Popup(overseasProvincesCountryGUIIndex, countryNames, GUILayout.Width(150));
                    EditorGUILayout.EndHorizontal();
                }

                // Background Color
                if (colorTexStyle == null) {
                    colorTexStyle = new GUIStyle(GUI.skin.box);
                    colorTexStyle.fixedWidth = 120;
                    colorTexStyle.fixedHeight = 17;
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Background Color", GUILayout.Width(150));
                if (bgColorTex == null) {
                    bgColorTex = MakeTex(150, 25, bgColor);
                    colorTexStyle.normal.background = bgColorTex;
                    Repaint();
                }
                GUILayout.Box(bgColorTex, colorTexStyle, GUILayout.Width(120), GUILayout.Height(17));

                if (colorPickerMode != COLOR_PICKER_MODE.BACKGROUND_COLOR) {
                    if (GUILayout.Button("Pick", GUILayout.Width(60))) {
                        colorPickerMode = COLOR_PICKER_MODE.BACKGROUND_COLOR;
                    }
                } else if (GUILayout.Button("Cancel", GUILayout.Width(60))) {
                    colorPickerMode = COLOR_PICKER_MODE.NONE;
                }
                EditorGUILayout.EndHorizontal();
                if (colorPickerMode == COLOR_PICKER_MODE.BACKGROUND_COLOR) {
                    GUILayout.Label("(Click on the texture to select background color)");
                }

                // Colors
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Territory Colors (" + ti.goodColorCount + ")", GUILayout.Width(150));
                if (goodColorsTex == null) {
                    goodColorsTex = MakeTexGoodColors(250, 25);
                    Repaint();
                }
                GUILayout.Box(goodColorsTex, colorTexStyle, GUILayout.Width(120), GUILayout.Height(17));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(150));
                if (colorPickerMode != COLOR_PICKER_MODE.GOOD_COLOR) {
                    if (GUILayout.Button("Add", GUILayout.Width(60))) {
                        colorPickerMode = COLOR_PICKER_MODE.GOOD_COLOR;
                    }
                    if (GUILayout.Button("Clear", GUILayout.Width(60))) {
                        ti.ClearGoodColors();
                        goodColorsTex = null;
                        GUIUtility.ExitGUI();
                        return;
                    }
                } else {
                    if (GUILayout.Button("Finish", GUILayout.Width(60))) {
                        colorPickerMode = COLOR_PICKER_MODE.NONE;
                    }
                    GUILayout.Label("(Click on the texture to add a new color)");
                }
                EditorGUILayout.EndHorizontal();

                // Operations
                if (colorPickerMode != COLOR_PICKER_MODE.NONE) {
                    Vector2 mousePos = Event.current.mousePosition;
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseDown && textureRect.Contains(mousePos)) {
                        Color32 color = GetColor(mousePos - textureRect.min);
                        if (colorPickerMode == COLOR_PICKER_MODE.BACKGROUND_COLOR) {
                            bgColor = color;
                            bgColorTex = null;
                            if (ti != null)
                            {
                                ti.backgroundColor = bgColor;
                                ReadTexture();
                            }
                            colorPickerMode = COLOR_PICKER_MODE.NONE;
                            GUIUtility.ExitGUI();
                            return;
                        } else if (colorPickerMode == COLOR_PICKER_MODE.GOOD_COLOR) {
                            if (!ti.IsGoodColor(color)) {
                                ti.AddGoodColor(color);
                                goodColorsTex = null;
                            }
                        }
                        GUIUtility.ExitGUI();
                    }
                }

                if (working) {
                    GUI.enabled = false;
                }
                if (ti != null) {
                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginHorizontal();
                    if (ti.goodColorCount == 0) {
                        GUI.enabled = false;
                    }
                    if (GUILayout.Button(mode == TerritoriesImporterMode.Countries ? "Generate Countries" : "Generate Provinces", GUILayout.Width(150))) {
                        if (AskConfirmation()) {
                            if (mode == TerritoriesImporterMode.Provinces) {
                                map.editor.ChangeEditingMode(EDITING_MODE.PROVINCES);
                                map.drawAllProvinces = true;
                            }
                            working = true;
                            int overseasProvincesCountryIndex = GetOverseasCountryIndex();
                            ti.StartProcess(mode, snapToCountryFrontiers, overseasProvincesCountryIndex);
                        }
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        bool AskConfirmation() {
            return EditorUtility.DisplayDialog("Confirm operation", "Countries or provinces (or both) including cities and mount points will be updated or removed according to the texture contents. Current map will be replaced!\n\nIMPORTANT:\n1) This feature is still experimental - unexpected errors may occur. Please report any issue on our support forum on kronnect.com.\n2) Changes to the map will not be saved to disk until you click 'Save' in Map Editor.\n3) Resulting borders will be a rough aproximation of an optimal map. You will need to adjust the contours in Map Editor.\n\nAre you sure you want to continue?", "YES!", "Cancel");
        }

        void OnInspectorUpdate() {
            if (working) {
                if (ti == null) {
                    Close();
                    return;
                }
                if (EditorUtility.DisplayCancelableProgressBar("Analyzing texture, please wait...", ti.status, ti.progress)) {
                    ti.CancelOperation();
                    EditorUtility.ClearProgressBar();
                    working = false;
                    return;
                }
                ti.Process();
                if (ti.progress >= 1f) {
                    EditorUtility.ClearProgressBar();
                    working = false;
                    committing = true;
                    committingProgress = 0f;
                    committingStatus = "";
                    Repaint();
                } else {
                    if ((DateTime.Now - lastRefreshTime).Seconds > 1f) {
                        lastRefreshTime = DateTime.Now;
                        ti.IssueTextureUpdate();
                        Repaint();
                    }
                }
            } else if (committing) {
                ti.IssueTextureUpdate();
                Repaint();
                EditorUtility.DisplayProgressBar("Applying changes, please wait...", committingStatus, committingProgress);
                CommitChanges();
                Repaint();
                if (committingProgress >= 1f) {
                    map.editor.issueRedraw = true;
                    EditorUtility.ClearProgressBar();
                    Close();
                }
            }
        }

        void ReadTexture() {
            if (!string.IsNullOrEmpty(filePath)) {
                CustomWWW www = new CustomWWW("file:///" + filePath);
                if (texture == null)
                    texture = new Texture2D(1, 1);
                www.LoadImageIntoTexture(texture);
                ti = new WMSK_TextureImporter(texture, bgColor, (int)detail);
                goodColorsTex = null;
            }
        }

        Texture2D MakeTex(int width, int height, Color col) {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            TextureFormat tf = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat) ? TextureFormat.RGBAFloat : TextureFormat.RGBA32;
            Texture2D result = new Texture2D(width, height, tf, false);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        Texture2D MakeTexGoodColors(int width, int height) {
            Color[] pix = new Color[width * height];
            if (ti.goodColorCount == 0)
                return null;
            for (int y = 0; y < height; y++) {
                int yy = y * width;
                for (int x = 0; x < width; x++) {
                    Color32 col = ti.goodColors[ti.goodColorCount * x / width];
                    pix[yy + x] = col;
                }
            }

            TextureFormat tf = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat) ? TextureFormat.RGBAFloat : TextureFormat.RGBA32;
            Texture2D result = new Texture2D(width, height, tf, false);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }


        Color GetColor(Vector2 pixelLocation) {
            pixelLocation.x = pixelLocation.x * (float)texture.width / textureRectWidth;
            pixelLocation.y = (textureRectHeight - pixelLocation.y) * (float)texture.height / textureRectHeight;
            int colorIndex = ((int)pixelLocation.y) * texture.width + (int)pixelLocation.x;
            Color[] colors = texture.GetPixels();
            if (colorIndex >= 0 && colorIndex < colors.Length) {
                return colors[colorIndex];
            }
            return Color.white;
        }

        void CommitChanges() {
            switch (mode) {
                case TerritoriesImporterMode.Countries:
                    UpdateCountries();
                    break;
                case TerritoriesImporterMode.Provinces:
                    UpdateProvinces();
                    break;
            }
        }

        void UpdateCountries() {
            if (committingProgress < 0.1f) {
                committingStatus = "Refreshing country frontiers...";
                committingProgress = 0.25f;
            } else if (committingProgress < 0.3f) {
                newCountries = ti.GetCountries();
                if (additive) {
                    for (int k = 0; k < newCountries.Length; k++) {
                        map.CountryAdd(newCountries[k]);
                    }
                } else {
                    map.countries = newCountries;
                }
                map.editor.countryChanges = true;
                map.editor.countryAttribChanges = true;
                for (int k = 0; k < map.countries.Length; k++) {
                    map.CountrySanitize(k, 5);
                }
                committingProgress = 0.4f;
            } else if (committingProgress < 0.5f) {
                committingStatus = "Refreshing provinces...";
                committingProgress = 0.6f;
            } else if (committingProgress < 0.7f) {
                // Update provinces references
                List<Province> newProvinces = new List<Province>(map.provinces.Length);
                for (int k = 0; k < map.provinces.Length; k++) {
                    Province prov = map.provinces[k];
                    if (prov.regions == null) {
                        map.ReadProvincePackedString(prov);
                        if (prov.regions == null)
                            prov.center = new Vector2(-1000, -1000);
                    }
                    int countryIndex = map.GetCountryIndex(prov.center);
                    if (countryIndex < 0) {
                        // Province deleted/ignored
                        map.editor.provinceChanges = true;
                    } else if (prov.countryIndex != countryIndex) {
                        prov.countryIndex = countryIndex;
                        map.editor.provinceChanges = true;
                        newProvinces.Add(prov);
                    }
                }
                map.provinces = newProvinces.ToArray();
                map.editor.provinceChanges = true;

                // Update country provinces
                newProvinces.Clear();
                for (int k = 0; k < map.countries.Length; k++) {
                    newProvinces.Clear();
                    for (int p = 0; p < map.provinces.Length; p++) {
                        if (map.provinces[p].countryIndex == k) {
                            newProvinces.Add(map.provinces[p]);
                        }
                    }
                    map.countries[k].provinces = newProvinces.ToArray();
                }
                committingProgress = 0.8f;
            } else if (committingProgress < 0.9f) {
                committingStatus = "Refreshing other dependencies...";
                committingProgress = 0.95f;
            } else {
                // Update city references
                int citiesCount = map.cities.Length;
                List<City> cities = new List<City>(map.cities);
                for (int k = 0; k < citiesCount; k++) {
                    City city = cities[k];
                    int countryIndex = map.GetCountryIndex(city.unity2DLocation);
                    if (countryIndex >= 0) {
                        if (city.countryIndex != countryIndex) {
                            city.countryIndex = countryIndex;
                            map.editor.cityChanges = true;
                        }
                    } else {
                        cities.RemoveAt(k);
                        map.editor.cityChanges = true;
                        k--;
                        citiesCount--;
                    }
                }
                map.cities = cities.ToArray();

                // Update mount points references
                if (map.mountPoints != null) {
                    int mpCount = map.mountPoints.Count;
                    for (int k = 0; k < mpCount; k++) {
                        MountPoint mp = map.mountPoints[k];
                        int countryIndex = map.GetCountryIndex(mp.unity2DLocation);
                        if (countryIndex >= 0) {
                            if (mp.countryIndex != countryIndex) {
                                mp.countryIndex = countryIndex;
                                map.editor.mountPointChanges = true;
                            }
                        } else {
                            map.mountPoints.RemoveAt(k);
                            map.editor.mountPointChanges = true;
                            k--;
                            mpCount--;
                        }
                    }
                }
                committingProgress = 1f;
            }
        }

        void UpdateProvinces() {
            if (committingProgress < 0.1f) {
                committingStatus = "Refreshing province borders...";
                committingProgress = 0.25f;
            } else if (committingProgress < 0.3f) {
                newProvinces = ti.GetProvinces();
                if (additive) {
                    for (int k = 0; k < newProvinces.Length; k++) {
                        map.ProvinceAdd(newProvinces[k]);
                    }
                } else {
                    map.provinces = newProvinces;
                }
                map.editor.provinceChanges = true;
                map.editor.provinceAttribChanges = true;
                for (int k = 0; k < map.provinces.Length; k++) {
                    map.ProvinceSanitize(k, 5);
                }
                committingProgress = 0.4f;
            } else if (committingProgress < 0.5f) {
                committingStatus = "Linking provinces to existing countries...";
                committingProgress = 0.6f;
            } else if (committingProgress < 0.7f) {
                List<Province> newProvinces = new List<Province>(map.provinces.Length);
                // Update country provinces
                for (int k = 0; k < map.countries.Length; k++) {
                    newProvinces.Clear();
                    for (int p = 0; p < map.provinces.Length; p++) {
                        Province province = map.provinces[p];
                        if (province.countryIndex == k) {
                            // add overseas province regions to the country regions array so they can be clicked
                            if (province.regions == null) continue;
                            int rr = province.regions.Count;
                            if (rr == 0) continue;
                            Country country = map.countries[province.countryIndex];
                            for (int pr = 0; pr < rr; pr++) {
                                Region region = province.regions[pr];
                                if (!country.Contains(region.center)) {
                                    country.regions.Add(region);
                                    map.editor.countryChanges = true;
                                }
                            }
                            newProvinces.Add(province);
                        }
                    }
                    map.countries[k].provinces = newProvinces.ToArray();
                }
                committingProgress = 0.8f;
            } else if (committingProgress < 0.9f) {
                committingStatus = "Refreshing other dependencies...";
                committingProgress = 0.95f;
            } else {
                // Update city references
                int citiesCount = map.cities.Length;
                for (int k = 0; k < citiesCount; k++) {
                    City city = map.cities[k];
                    int provinceIndex = map.GetProvinceIndex(city.unity2DLocation);
                    if (provinceIndex >= 0) {
                        Province prov = map.provinces[provinceIndex];
                        if (!city.province.Equals(prov.name)) {
                            city.province = prov.name;
                            city.countryIndex = prov.countryIndex;
                            map.editor.cityChanges = true;
                        }
                    } else if (city.province.Length > 0) {
                        city.province = "";
                        map.editor.cityChanges = true;
                    }
                }

                // Update mount points references
                int mpCount = map.mountPoints.Count;
                for (int k = 0; k < mpCount; k++) {
                    MountPoint mp = map.mountPoints[k];
                    int provinceIndex = map.GetProvinceIndex(mp.unity2DLocation);
                    if (provinceIndex >= 0) {
                        if (mp.provinceIndex != provinceIndex) {
                            mp.provinceIndex = provinceIndex;
                            map.editor.mountPointChanges = true;
                        }
                    } else {
                        map.mountPoints.RemoveAt(k);
                        map.editor.mountPointChanges = true;
                        k--;
                        mpCount--;
                    }
                }
                committingProgress = 1f;
            }

        }

        int GetOverseasCountryIndex() {

            if (overseasProvincesCountryGUIIndex < 0 || overseasProvincesCountryGUIIndex >= countryNames.Length)
                return -1;
            string[] s = countryNames[overseasProvincesCountryGUIIndex].Split(new char[]
                {
                    '(',
                    ')'
                }, System.StringSplitOptions.RemoveEmptyEntries);
            if (s.Length >= 2) {
                int countryIndex;
                if (int.TryParse(s[1], out countryIndex)) {
                    return countryIndex;
                }
            }
            return -1;
        }

    }

}
#endif