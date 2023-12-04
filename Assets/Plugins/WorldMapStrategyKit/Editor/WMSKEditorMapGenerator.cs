using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

    public partial class WMSKEditorInspector : Editor {

        GUIStyle titleLabelStyle, infoLabelStyle, generateButtonStyle;
        SerializedProperty numCells, mapGenerationQuality, outputFolder, seed, seedNames, numProvincesPerCountryMin, numProvincesPerCountryMax, numCountries;
        SerializedProperty generateProvinces, generateCities;
        SerializedProperty userHeightMapTexture, heightmapWidth, heightmapHeight, octavesBySeed, noiseOctaves, seaColor, seaLevel, noisePower;
        SerializedProperty islandFactor, elevationShift;
        SerializedProperty backgroundTextureWidth, backgroundTextureHeight;
        SerializedProperty gridRelaxation, edgeMaxLength, edgeNoise;
        SerializedProperty numCitiesPerCountryMin, numCitiesPerCountryMax;
        SerializedProperty heightGradient, heightGradientPreset;
        SerializedProperty heightGradientMinHue, heightGradientMaxHue, heightGradientMinSaturation, heightGradientMaxSaturation, heightGradientMinValue, heightGradientMaxValue;
        SerializedProperty gradientPerPixel, changeStyle, generateNormalMap, normalMapBumpiness, generateScenicWaterMask, generatePathfindingMaps;
        SerializedProperty colorProvinces, drawBorders, bordersIntensity, bordersWidth, smoothBorders, drawSeaShoreline, shorelineWidth, landRect;

        Material previewMat;

        void InitMapGenerator() {

            outputFolder = serializedObject.FindProperty("outputFolder");
            numCells = serializedObject.FindProperty("numCells");
            mapGenerationQuality = serializedObject.FindProperty("mapGenerationQuality");

            seed = serializedObject.FindProperty("seed");
            seedNames = serializedObject.FindProperty("seedNames");
            numCountries = serializedObject.FindProperty("numCountries");

            generateProvinces = serializedObject.FindProperty("generateProvinces");
            numProvincesPerCountryMin = serializedObject.FindProperty("numProvincesPerCountryMin");
            numProvincesPerCountryMax = serializedObject.FindProperty("numProvincesPerCountryMax");

            generateCities = serializedObject.FindProperty("generateCities");
            numCitiesPerCountryMin = serializedObject.FindProperty("numCitiesPerCountryMin");
            numCitiesPerCountryMax = serializedObject.FindProperty("numCitiesPerCountryMax");

            backgroundTextureWidth = serializedObject.FindProperty("backgroundTextureWidth");
            backgroundTextureHeight = serializedObject.FindProperty("backgroundTextureHeight");

            userHeightMapTexture = serializedObject.FindProperty("userHeightMapTexture");
            heightmapWidth = serializedObject.FindProperty("heightMapWidth");
            heightmapHeight = serializedObject.FindProperty("heightMapHeight");

            octavesBySeed = serializedObject.FindProperty("octavesBySeed");
            noiseOctaves = serializedObject.FindProperty("noiseOctaves");
            seaColor = serializedObject.FindProperty("seaColor");
            seaLevel = serializedObject.FindProperty("seaLevel");
            noisePower = serializedObject.FindProperty("noisePower");

            heightGradient = serializedObject.FindProperty("heightGradient");
            heightGradientPreset = serializedObject.FindProperty("heightGradientPreset");
            heightGradientMinHue = serializedObject.FindProperty("heightGradientMinHue");
            heightGradientMaxHue = serializedObject.FindProperty("heightGradientMaxHue");
            heightGradientMinSaturation = serializedObject.FindProperty("heightGradientMinSaturation");
            heightGradientMaxSaturation = serializedObject.FindProperty("heightGradientMaxSaturation");
            heightGradientMinValue = serializedObject.FindProperty("heightGradientMinValue");
            heightGradientMaxValue = serializedObject.FindProperty("heightGradientMaxValue");

            gradientPerPixel = serializedObject.FindProperty("gradientPerPixel");
            changeStyle = serializedObject.FindProperty("changeStyle");

            islandFactor = serializedObject.FindProperty("islandFactor");
            elevationShift = serializedObject.FindProperty("elevationShift");

            gridRelaxation = serializedObject.FindProperty("gridRelaxation");
            edgeMaxLength = serializedObject.FindProperty("edgeMaxLength");
            edgeNoise = serializedObject.FindProperty("edgeNoise");

            generateNormalMap = serializedObject.FindProperty("generateNormalMap");
            normalMapBumpiness = serializedObject.FindProperty("normalMapBumpiness");
            generateScenicWaterMask = serializedObject.FindProperty("generateScenicWaterMask");
            generatePathfindingMaps = serializedObject.FindProperty("generatePathfindingMaps");

            colorProvinces = serializedObject.FindProperty("colorProvinces");
            drawBorders = serializedObject.FindProperty("drawBorders");
            bordersIntensity = serializedObject.FindProperty("bordersIntensity");
            bordersWidth = serializedObject.FindProperty("bordersWidth");
            smoothBorders = serializedObject.FindProperty("smoothBorders");
            drawSeaShoreline = serializedObject.FindProperty("drawSeaShoreline");
            shorelineWidth = serializedObject.FindProperty("shorelineWidth");
            landRect = serializedObject.FindProperty("landRect");

        }

        public bool ShowMapGeneratorOptions() {

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            DrawTitleLabel("General Settings");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(seed);
            if (GUILayout.Button("<", GUILayout.Width(20))) {
                if (seed.intValue > 0) {
                    seed.intValue--;
                }
            }
            if (GUILayout.Button(">", GUILayout.Width(20))) {
                if (seed.intValue < 10000) {
                    seed.intValue++;
                }
            }
            EditorGUILayout.EndHorizontal();

            bool requestNewHeightMap = false;
            if (EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
                _editor.ApplySeed();
                serializedObject.Update();
                requestNewHeightMap = true;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(seedNames);
            if (GUILayout.Button("<", GUILayout.Width(20))) {
                if (seedNames.intValue > 0) {
                    seedNames.intValue--;
                }
            }
            if (GUILayout.Button(">", GUILayout.Width(20))) {
                if (seedNames.intValue < 10000) {
                    seedNames.intValue++;
                }
            }
            EditorGUILayout.EndHorizontal();

            bool requestGeneration = false;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(outputFolder);
            if (GUILayout.Button("Open")) {
                EditorUtility.RevealInFinder(_editor.GetGenerationMapOutputPath());
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("Map data, including heightMap, background and water mask textures, will be stored in Resources/WMSK/Geodata/" + outputFolder.stringValue + " folder.", MessageType.Info);

            EditorGUILayout.PropertyField(mapGenerationQuality, new GUIContent("Generation Quality"));
            generateButtonStyle = new GUIStyle(GUI.skin.button);
            generateButtonStyle.normal.textColor = Color.yellow;
            generateButtonStyle.fontStyle = FontStyle.Bold;
            generateButtonStyle.fixedHeight = 30;

            if (GUILayout.Button("Generate & Save Map", generateButtonStyle)) {
                requestGeneration = true;
            }

            EditorGUILayout.Separator();

            DrawTitleLabel("Countries");
            EditorGUILayout.PropertyField(numCountries, new GUIContent("Count"));

            EditorGUILayout.Separator();

            DrawTitleLabel("Provinces");
            EditorGUILayout.PropertyField(generateProvinces, new GUIContent("Generate Provinces"));
            if (generateProvinces.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(numProvincesPerCountryMin, new GUIContent("Min Provinces Per Country"));
                EditorGUILayout.PropertyField(numProvincesPerCountryMax, new GUIContent("Max Provinces Per Country"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();
            DrawTitleLabel("Cities");

            EditorGUILayout.PropertyField(generateCities, new GUIContent("Generate Cities"));
            if (generateCities.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(numCitiesPerCountryMin, new GUIContent("Min Per Country"));
                EditorGUILayout.PropertyField(numCitiesPerCountryMax, new GUIContent("Max Per Country"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();

            DrawTitleLabel("Borders");

            EditorGUILayout.PropertyField(numCells, new GUIContent("Cells (Detail Level)"));
            EditorGUILayout.BeginHorizontal();
            if (numCells.intValue > WMSK_Editor.MAX_CELLS_FOR_RELAXATION) {
                GUILayout.Label("Relaxation", GUILayout.Width(120));
                DrawInfoLabel("not available with >" + WMSK_Editor.MAX_CELLS_FOR_RELAXATION + " cells");
            } else {
                EditorGUILayout.PropertyField(gridRelaxation, new GUIContent("Homogeneity"));
                if (GUILayout.Button("Reset", GUILayout.Width(80))) {
                    gridRelaxation.intValue = 1;
                }
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.PropertyField(edgeNoise, new GUIContent("Edge Noise"));
            GUI.enabled = edgeNoise.floatValue > 0;
            EditorGUILayout.PropertyField(edgeMaxLength, new GUIContent("Edge Max Length"));
            GUI.enabled = true;

            EditorGUILayout.PropertyField(landRect, new GUIContent("Land Boundary", "Restrict land to this rectangle"));

            EditorGUILayout.Separator();

            DrawTitleLabel("Height Map");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(userHeightMapTexture, new GUIContent("Custom Heightmap Texture", "Optional heightmap texture"));
            if (EditorGUI.EndChangeCheck()) {
                Texture2D tex = (Texture2D)userHeightMapTexture.objectReferenceValue;
                if (tex != null) {
                    string path = AssetDatabase.GetAssetPath(tex);
                    TextureImporter texImp = (TextureImporter)TextureImporter.GetAtPath(path);
                    if (!texImp.isReadable) {
                        texImp.isReadable = true;
                        texImp.SaveAndReimport();
                    }
                    heightmapWidth.intValue = tex.width;
                    heightmapHeight.intValue = tex.height;
                }
            }

            if (userHeightMapTexture.objectReferenceValue == null) {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(heightmapWidth);
                EditorGUILayout.PropertyField(heightmapHeight);
                EditorGUILayout.PropertyField(octavesBySeed, new GUIContent("Random Octaves", "Generate random octaves by seed."));
                EditorGUILayout.PropertyField(noiseOctaves, true);
                EditorGUILayout.PropertyField(islandFactor);
                EditorGUILayout.PropertyField(noisePower);
                EditorGUILayout.PropertyField(elevationShift);
                if (EditorGUI.EndChangeCheck()) {
                    requestNewHeightMap = true;
                }
            }
            Texture2D generatedHeightMapTexture = _editor.heightMapTexture;
            if (generatedHeightMapTexture != null) {
                Rect space = EditorGUILayout.BeginVertical();
                float aspect = (float)generatedHeightMapTexture.height / generatedHeightMapTexture.width;
                GUILayout.Space(EditorGUIUtility.currentViewWidth * aspect);
                EditorGUILayout.EndVertical();
                if (previewMat == null) {
                    previewMat = new Material(Shader.Find("WMSK/Editor/DepthTexPreview"));
                }
                previewMat.mainTexture = generatedHeightMapTexture;
                previewMat.color = seaColor.colorValue;
                previewMat.SetFloat("_SeaLevel", seaLevel.floatValue);
                EditorGUI.DrawPreviewTexture(space, generatedHeightMapTexture, previewMat, ScaleMode.StretchToFill);
            }

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(seaLevel);
            EditorGUILayout.PropertyField(seaColor);

            EditorGUILayout.Separator();

            DrawTitleLabel("World Textures");
            EditorGUILayout.PropertyField(backgroundTextureWidth, new GUIContent("Texture Width"));
            EditorGUILayout.PropertyField(backgroundTextureHeight, new GUIContent("Texture Height"));
            EditorGUILayout.PropertyField(generateNormalMap, new GUIContent("Generate Normal Map"));
            if (generateNormalMap.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(normalMapBumpiness, new GUIContent("Bumpiness"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(heightGradientPreset, new GUIContent("Color Style"));
            if (heightGradientPreset.intValue == (int)HeightMapGradientPreset.Custom) {
                EditorGUILayout.PropertyField(heightGradient, new GUIContent("Gradient"));
            } else if (heightGradientPreset.intValue == (int)HeightMapGradientPreset.RandomHSVColors) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(heightGradientMinHue, new GUIContent("Min Hue"));
                EditorGUILayout.PropertyField(heightGradientMaxHue, new GUIContent("Max Hue"));
                EditorGUILayout.PropertyField(heightGradientMinSaturation, new GUIContent("Min Saturation"));
                EditorGUILayout.PropertyField(heightGradientMaxSaturation, new GUIContent("Max Saturation"));
                EditorGUILayout.PropertyField(heightGradientMinValue, new GUIContent("Min Value"));
                EditorGUILayout.PropertyField(heightGradientMaxValue, new GUIContent("Max Value"));
                EditorGUI.indentLevel--;
            }
            if (generateProvinces.boolValue) {
                EditorGUILayout.PropertyField(colorProvinces, new GUIContent("Color Provinces", "Color provinces or countries"));
            }
            EditorGUILayout.PropertyField(gradientPerPixel, new GUIContent("Per Pixel Gradient", "Apply height gradient per pixel instead of per province"));
            EditorGUILayout.PropertyField(drawBorders, new GUIContent("Draw Borders", "Make borders darker"));
            if (drawBorders.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(bordersWidth, new GUIContent("Width"));
                EditorGUILayout.PropertyField(bordersIntensity, new GUIContent("Intensity"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(smoothBorders, new GUIContent("Smooth Borders", "Blur/smooth borders"));
            EditorGUILayout.PropertyField(drawSeaShoreline, new GUIContent("Draw Shoreline", "Draws coast shoreline"));
            if (drawSeaShoreline.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(shorelineWidth, new GUIContent("Width"));
                EditorGUI.indentLevel--;
            }

            if (heightGradientPreset.intValue != (int)HeightMapGradientPreset.Custom) {
                EditorGUILayout.PropertyField(changeStyle, new GUIContent("Apply Style", "Change frontier and label colors to match selected land colors preset. Disable to keep your current settings."));
            }
            EditorGUILayout.PropertyField(generateScenicWaterMask, new GUIContent("Generate Scenic Water Mask", "Generates water mask for the scenic style. Only useful if you're going to use the scenic styles."));
            EditorGUILayout.PropertyField(generatePathfindingMaps, new GUIContent("Generate Pathfinding Maps", "Generates land and water pathfinding textures. Only useful if you're using pathfinding functionality."));
            
            serializedObject.ApplyModifiedProperties();

            if (requestGeneration) {
                _editor.GenerateMap(saveData: true, changeStyle: this.changeStyle.boolValue);
                return true;
            } else if (requestNewHeightMap) {
                _editor.GenerateHeightMap(preview: true);
                return true;
            }

            return false;
        }

        #region Utility functions


        void DrawTitleLabel(string s) {
            if (titleLabelStyle == null)
                titleLabelStyle = new GUIStyle(GUI.skin.label);
            titleLabelStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.22f, 0.33f, 0.6f);
            titleLabelStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label(s, titleLabelStyle);
        }

        void DrawInfoLabel(string s) {
            if (infoLabelStyle == null)
                infoLabelStyle = new GUIStyle(GUI.skin.label);
            infoLabelStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.76f, 0.52f, 0.52f) : new Color(0.46f, 0.22f, 0.22f);
            GUILayout.Label(s, infoLabelStyle);
        }

        bool CheckTextureImportSettings(Texture2D tex) {
            if (tex == null)
                return false;
            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter imp = (TextureImporter)AssetImporter.GetAtPath(path);
            if (!imp.isReadable) {
                EditorGUILayout.HelpBox("Texture is not readable. Fix it?", MessageType.Warning);
                if (GUILayout.Button("Fix texture import setting")) {
                    imp.isReadable = true;
                    imp.SaveAndReimport();
                    return true;
                }
            }
            return false;
        }

        #endregion


    }

}