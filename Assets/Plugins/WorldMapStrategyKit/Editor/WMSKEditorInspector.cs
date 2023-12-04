using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace WorldMapStrategyKit {
    [CustomEditor(typeof(WMSK_Editor))]
    public partial class WMSKEditorInspector : Editor {

        const string INFO_MSG_CHANGES_SAVED = "Changes saved. Original geodata files in /Backup folder.";
        const string INFO_MSG_REGION_DELETED = "Region deleted!";
        const string INFO_MSG_BACKUP_NOT_FOUND = "Backup folder not found!";
        const string INFO_MSG_BACKUP_RESTORED = "Backup restored.";
        const string INFO_MSG_GEODATA_LOW_QUALITY_CREATED = "Low quality geodata file created.";
        const string INFO_MSG_CITY_DELETED = "City deleted!";
        const string INFO_MSG_NO_CHANGES_TO_SAVE = "Nothing changed to save!";
        const string INFO_MSG_CHOOSE_COUNTRY = "Choose a country first.";
        const string INFO_MSG_CHOOSE_PROVINCE = "Choose a province first.";
        const string INFO_MSG_CONTINENT_DELETED = "Continent deleted!";
        const string INFO_MSG_COUNTRY_DELETED = "Country deleted!";
        const string INFO_MSG_PROVINCE_DELETED = "Province deleted!";
        const string INFO_MSG_MOUNT_POINT_DELETED = "Mount point deleted!";
        const string INFO_MSG_PROVINCE_REGION_ADDED_TO_COUNTRY = "Province regions added to country regions";

        static Vector3 pointSnap = Misc.Vector3one * 0.1f;
        const float HANDLE_SIZE = 0.05f;
        const float HIT_PRECISION = 0.00075f;
        const string EDITORPREF_SCALE_WARNED = "ScaleWarned";
        WMSK_Editor _editor;
        GUIStyle labelsStyle, attribHeaderStyle, warningLabelStyle, editorCaptionLabelStyle;
        GUIContent[] mainToolbarIcons;
        GUIContent[] reshapeRegionToolbarIcons, reshapeCityToolbarIcons, reshapeMountPointToolbarIcons, createToolbarIcons;
        int[] controlIds;
        bool startedReshapeRegion, startedReshapeCity, startedReshapeMountPoint, undoPushStarted;
        long tickStart;
        string[] reshapeRegionModeExplanation, reshapeCityModeExplanation, reshapeMountPointModeExplanation, editingModeOptions, editingCountryFileOptions, createModeExplanation, cityClassOptions;
        int[] cityClassValues;
        string[] emptyStringArray;
        WMSK_EditorAttribGroup mountPointAttribGroup, countryAttribGroup, provinceAttribGroup, cityAttribGroup;
        StringBuilder sb = new StringBuilder();
        Vector2 mapScale = Misc.Vector2one;
        Vector2 mapOffset;

        WMSK _map { get { return _editor.map; } }

        #region Inspector lifecycle

        void OnEnable() {
            // Setup basic inspector stuff
            _editor = (WMSK_Editor)target;
            if (_map.countries == null) {
                _map.Init();
            }

            // Load UI icons
            Texture2D[] icons = new Texture2D[22];
            icons[0] = Resources.Load<Texture2D>("WMSK/IconSelect");
            icons[1] = Resources.Load<Texture2D>("WMSK/IconPolygon");
            icons[2] = Resources.Load<Texture2D>("WMSK/IconUndo");
            icons[3] = Resources.Load<Texture2D>("WMSK/IconConfirm");
            icons[4] = Resources.Load<Texture2D>("WMSK/IconPoint");
            icons[5] = Resources.Load<Texture2D>("WMSK/IconCircle");
            icons[6] = Resources.Load<Texture2D>("WMSK/IconMagnet");
            icons[7] = Resources.Load<Texture2D>("WMSK/IconSplitVert");
            icons[8] = Resources.Load<Texture2D>("WMSK/IconSplitHoriz");
            icons[9] = Resources.Load<Texture2D>("WMSK/IconDelete");
            icons[10] = Resources.Load<Texture2D>("WMSK/IconEraser");
            icons[11] = Resources.Load<Texture2D>("WMSK/IconMorePoints");
            icons[12] = Resources.Load<Texture2D>("WMSK/IconCreate");
            icons[13] = Resources.Load<Texture2D>("WMSK/IconPenCountry");
            icons[14] = Resources.Load<Texture2D>("WMSK/IconTarget");
            icons[15] = Resources.Load<Texture2D>("WMSK/IconPenCountryRegion");
            icons[16] = Resources.Load<Texture2D>("WMSK/IconPenProvince");
            icons[17] = Resources.Load<Texture2D>("WMSK/IconPenProvinceRegion");
            icons[18] = Resources.Load<Texture2D>("WMSK/IconMove");
            icons[19] = Resources.Load<Texture2D>("WMSK/IconMountPoint");
            icons[20] = Resources.Load<Texture2D>("WMSK/IconWorld");
            icons[21] = Resources.Load<Texture2D>("WMSK/IconTools");

            // Setup main toolbar
            mainToolbarIcons = new GUIContent[7];
            mainToolbarIcons[0] = new GUIContent("Select", icons[0], "Selection mode");
            mainToolbarIcons[1] = new GUIContent("Edit", icons[1], "Change the shape of this entity or delete selection");
            mainToolbarIcons[2] = new GUIContent("Create", icons[12], "Add a new entity to this layer");
            mainToolbarIcons[3] = new GUIContent("Random", icons[20], "Randomly generates a map");
            mainToolbarIcons[4] = new GUIContent("Map Tools", icons[21], "Global Map Tools");
            mainToolbarIcons[5] = new GUIContent("Revert", icons[2], "Restore shape information");
            mainToolbarIcons[6] = new GUIContent("Save", icons[3], "Confirm changes and save to file");

            // Setup reshape region command toolbar
            int RESHAPE_REGION_TOOLS_COUNT = 8;
            reshapeRegionToolbarIcons = new GUIContent[RESHAPE_REGION_TOOLS_COUNT];
            reshapeRegionToolbarIcons[(int)RESHAPE_REGION_TOOL.POINT] = new GUIContent("Point", icons[4], "Single Point Tool");
            reshapeRegionToolbarIcons[(int)RESHAPE_REGION_TOOL.CIRCLE] = new GUIContent("Circle", icons[5], "Group Move Tool");
            reshapeRegionToolbarIcons[(int)RESHAPE_REGION_TOOL.SPLITV] = new GUIContent("SplitV", icons[7], "Split Vertically");
            reshapeRegionToolbarIcons[(int)RESHAPE_REGION_TOOL.SPLITH] = new GUIContent("SplitH", icons[8], "Split Horizontally");
            reshapeRegionToolbarIcons[(int)RESHAPE_REGION_TOOL.MAGNET] = new GUIContent("Magnet", icons[6], "Join frontiers between different regions");
            reshapeRegionToolbarIcons[(int)RESHAPE_REGION_TOOL.SMOOTH] = new GUIContent("Smooth", icons[11], "Add Point Tool");
            reshapeRegionToolbarIcons[(int)RESHAPE_REGION_TOOL.ERASER] = new GUIContent("Erase", icons[10], "Removes Point Tool");
            reshapeRegionToolbarIcons[(int)RESHAPE_REGION_TOOL.DELETE] = new GUIContent("Delete", icons[9], "Delete Region or Country");
            reshapeRegionModeExplanation = new string[RESHAPE_REGION_TOOLS_COUNT];
            reshapeRegionModeExplanation[(int)RESHAPE_REGION_TOOL.POINT] = "Drag a SINGLE point of currently selected region (and its neighbour)";
            reshapeRegionModeExplanation[(int)RESHAPE_REGION_TOOL.CIRCLE] = "Drag a GROUP of points of currently selected region (and from its neighbour region if present)";
            reshapeRegionModeExplanation[(int)RESHAPE_REGION_TOOL.SPLITV] = "Splits VERTICALLY currently selected region. One of the two splitted parts will form a new country.";
            reshapeRegionModeExplanation[(int)RESHAPE_REGION_TOOL.SPLITH] = "Splits HORIZONTALLY currently selected region. One of the two splitted parts will form a new country.";
            reshapeRegionModeExplanation[(int)RESHAPE_REGION_TOOL.MAGNET] = "Click several times on a group of points next to a neighbour frontier to makes them JOIN. You may need to add additional points on both sides using the smooth tool.";
            reshapeRegionModeExplanation[(int)RESHAPE_REGION_TOOL.SMOOTH] = "Click around currently selected region to ADD new points.";
            reshapeRegionModeExplanation[(int)RESHAPE_REGION_TOOL.ERASER] = "Click on target points of currently selected region to ERASE them.";
            reshapeRegionModeExplanation[(int)RESHAPE_REGION_TOOL.DELETE] = "DELETES currently selected region. If this is the last region of the country or province, then the country or province will be deleted completely.";

            // Setup create command toolbar
            int CREATE_TOOLS_COUNT = 6;
            createToolbarIcons = new GUIContent[CREATE_TOOLS_COUNT];
            createToolbarIcons[(int)CREATE_TOOL.CITY] = new GUIContent("City", icons[14], "Create a new city");
            createToolbarIcons[(int)CREATE_TOOL.COUNTRY] = new GUIContent("Country", icons[13], "Draw a new country");
            createToolbarIcons[(int)CREATE_TOOL.COUNTRY_REGION] = new GUIContent("Co. Region", icons[15], "Draw a new region for current selected country");
            createToolbarIcons[(int)CREATE_TOOL.PROVINCE] = new GUIContent("Province", icons[16], "Draw a new province for current selected country");
            createToolbarIcons[(int)CREATE_TOOL.PROVINCE_REGION] = new GUIContent("Prov. Region", icons[17], "Draw a new region for current selected province");
            createToolbarIcons[(int)CREATE_TOOL.MOUNT_POINT] = new GUIContent("Mount Point", icons[19], "Create a new mount point");
            createModeExplanation = new string[CREATE_TOOLS_COUNT];
            createModeExplanation[(int)CREATE_TOOL.CITY] = "Click over the map to create a NEW CITY for currrent COUNTRY";
            createModeExplanation[(int)CREATE_TOOL.COUNTRY] = "Click over the map to create a polygon and add points for a NEW COUNTRY";
            createModeExplanation[(int)CREATE_TOOL.COUNTRY_REGION] = "Click over the map to create a polygon and add points for a NEW REGION of currently selected COUNTRY";
            createModeExplanation[(int)CREATE_TOOL.PROVINCE] = "Click over the map to create a polygon and add points for a NEW PROVINCE of currently selected country";
            createModeExplanation[(int)CREATE_TOOL.PROVINCE_REGION] = "Click over the map to create a polygon and add points for a NEW REGION of currently selected PROVINCE";
            createModeExplanation[(int)CREATE_TOOL.MOUNT_POINT] = "Click over the map to create a NEW MOUNT POINT for current COUNTRY and optional PROVINCE";

            // Setup reshape city tools
            int RESHAPE_CITY_TOOLS_COUNT = 2;
            reshapeCityToolbarIcons = new GUIContent[RESHAPE_CITY_TOOLS_COUNT];
            reshapeCityToolbarIcons[(int)RESHAPE_CITY_TOOL.MOVE] = new GUIContent("Move", icons[18], "Move city");
            reshapeCityToolbarIcons[(int)RESHAPE_CITY_TOOL.DELETE] = new GUIContent("Delete", icons[9], "Delete city");
            reshapeCityModeExplanation = new string[RESHAPE_CITY_TOOLS_COUNT];
            reshapeCityModeExplanation[(int)RESHAPE_CITY_TOOL.MOVE] = "Click and drag currently selected CITY to change its POSITION";
            reshapeCityModeExplanation[(int)RESHAPE_CITY_TOOL.DELETE] = "DELETES currently selected CITY.";

            // Setup reshape mount point tools
            int RESHAPE_MOUNT_POINT_TOOLS_COUNT = 2;
            reshapeMountPointToolbarIcons = new GUIContent[RESHAPE_MOUNT_POINT_TOOLS_COUNT];
            reshapeMountPointToolbarIcons[(int)RESHAPE_MOUNT_POINT_TOOL.MOVE] = new GUIContent("Move", icons[18], "Move mount point");
            reshapeMountPointToolbarIcons[(int)RESHAPE_MOUNT_POINT_TOOL.DELETE] = new GUIContent("Delete", icons[9], "Delete mount point");
            reshapeMountPointModeExplanation = new string[RESHAPE_MOUNT_POINT_TOOLS_COUNT];
            reshapeMountPointModeExplanation[(int)RESHAPE_MOUNT_POINT_TOOL.MOVE] = "Click and drag currently selected MOUNT POINT to change its POSITION";
            reshapeMountPointModeExplanation[(int)RESHAPE_MOUNT_POINT_TOOL.DELETE] = "DELETES currently selected MOUNT POINT.";


            editingModeOptions = new string[] {
                "Only Countries",
                "Countries + Provinces"
            };

            editingCountryFileOptions = new string[] {
                "High Definition Geodata File",
                "Low Definition Geodata File"
            };
            cityClassOptions = new string[] {
                "City",
                "Country Capital",
                "Region Capital"
            };
            cityClassValues = new int[] {
                (int)CITY_CLASS.CITY,
                (int)CITY_CLASS.COUNTRY_CAPITAL,
                (int)CITY_CLASS.REGION_CAPITAL
            };

            InitMapGenerator();

            emptyStringArray = new string[0];

            _editor.snapPrecisionDigits = EditorPrefs.GetInt("SnapUnit", 7);
            _editor.operationMode = OPERATION_MODE.SELECTION;

            if (!Application.isPlaying) {
                _editor.SetEditorMapSettings();

                AdjustCityIconsScale();
                AdjustMountPointIconsScale();
                CheckScale();
                _map.ResetCountryLabelsAlpha();

                if (mapScale.x != 1 || mapOffset.x != 0) {
                    MapTransformApply();
                }
            }

#if UNITY_2019_1_OR_NEWER
            SceneView sv = SceneView.lastActiveSceneView;
            if (sv != null) {
                sv.drawGizmos = true;
            }
#endif
        }

        public override void OnInspectorGUI() {

            try {

                if (_editor == null)
                    return;

                if (_editor.issueRedraw) {
                    RedrawEditor();
                    return;
                }

                // Safety checks
                if (_editor.countryIndex >= 0 && _editor.countryIndex >= _editor.map.countries.Length) {
                    _editor.ClearSelection();
                    return;
                }
                if (_editor.provinceIndex >= 0 && _editor.provinceIndex >= _editor.map.provinces.Length) {
                    _editor.ClearSelection();
                    return;
                }

                // Setup UI styles
                CheckEditorStyles();

                // Show Editor UI
                if (_map.showProvinces) {
                    _editor.editingMode = EDITING_MODE.PROVINCES;
                } else {
                    _editor.editingMode = EDITING_MODE.COUNTRIES;
                    if (_map.frontiersDetail == FRONTIERS_DETAIL.High) {
                        _editor.editingCountryFile = EDITING_COUNTRY_FILE.COUNTRY_HIGHDEF;
                    } else {
                        _editor.editingCountryFile = EDITING_COUNTRY_FILE.COUNTRY_LOWDEF;
                    }
                }

                EditorGUILayout.Separator();
                EditorGUILayout.BeginVertical();

                if (_map.renderViewport != _map.gameObject) {
                    EditorGUILayout.BeginHorizontal();
                    DrawWarningLabel("Note: Map editing only works in Scene View and on normal map, not viewport.");
                    EditorGUILayout.EndHorizontal();
                }

                if (_editor.territoryImporterActive) {
                    EditorGUILayout.BeginHorizontal();
                    DrawWarningLabel("Waiting for Territories Importer to close.");
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }

                if (_editor.terrainImporterActive) {
                    EditorGUILayout.BeginHorizontal();
                    DrawWarningLabel("Waiting for Terrain Importer to close.");
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }

                EDITING_MODE prevEditingMode = _editor.editingMode;
                _editor.editingMode = (EDITING_MODE)EditorGUILayout.Popup("Show Layers", (int)_editor.editingMode, editingModeOptions);
                if (_editor.editingMode != prevEditingMode) {
                    _editor.ChangeEditingMode(_editor.editingMode);
                }
                EDITING_COUNTRY_FILE prevCountryFile = _editor.editingCountryFile;
                _editor.editingCountryFile = (EDITING_COUNTRY_FILE)EditorGUILayout.Popup("Country File", (int)_editor.editingCountryFile, editingCountryFileOptions);
                if (_editor.editingCountryFile != prevCountryFile) {
                    if (!EditorUtility.DisplayDialog("Switch Geodata File", "Choosing a different country file will reload definitions and any unsaved change to current file will be lost. Continue?", "Switch Geodata File", "Cancel")) {
                        _editor.editingCountryFile = prevCountryFile;
                        CheckScale();
                        return;
                    }
                    SwitchEditingFrontiersFile();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Separator();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.Separator();

                // main toolbar
                GUIStyle toolbarStyle = new GUIStyle(GUI.skin.button);

                OPERATION_MODE prevOp = _editor.operationMode;
                _editor.operationMode = (OPERATION_MODE)GUILayout.SelectionGrid((int)_editor.operationMode, mainToolbarIcons, 4, toolbarStyle, GUILayout.Height(48));
                if (prevOp != _editor.operationMode) {
                    NewShapeInit();
                    ProcessOperationMode();
                    if (_editor.operationMode == OPERATION_MODE.SELECTION) {
                        _editor.RedrawFrontiers(null, true); // make sure it draws all frontiers
                    }
                    FocusSceneView();
                }

                if (_editor.operationMode != OPERATION_MODE.CONFIRM && (_editor.countryChanges || _editor.provinceChanges || _editor.cityChanges || _editor.mountPointChanges)) {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    sb.Length = 0;
                    sb.Append("(You have unsaved changes in:");
                    if (_editor.countryChanges) {
                        sb.Append(" countries");
                    }
                    if (_editor.provinceChanges) {
                        sb.Append(" provinces");
                    }
                    if (_editor.cityChanges) {
                        sb.Append(" cities");
                    }
                    if (_editor.mountPointChanges) {
                        sb.Append(" mount points");
                    }
                    sb.Append(")");
                    DrawWarningLabel(sb.ToString());
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();
                }

                if (_editor.infoMsg.Length > 0) {
                    if (Event.current.type == EventType.Layout && (DateTime.Now - _editor.infoMsgStartTime).TotalSeconds > 5) {
                        _editor.infoMsg = "";
                    } else {
                        EditorGUILayout.HelpBox(_editor.infoMsg, MessageType.Info);
                    }
                }
                EditorGUILayout.Separator();
                switch (_editor.operationMode) {
                    case OPERATION_MODE.SELECTION:
                        if (_editor.editingMode == EDITING_MODE.COUNTRIES && _editor.countryIndex < 0) {
                            EditorGUILayout.HelpBox("Select a country clicking in Scene View.", MessageType.Info);
                        } else if (_editor.editingMode == EDITING_MODE.PROVINCES) {
                            if (_editor.countryIndex < 0) {
                                EditorGUILayout.HelpBox("Select a country or province clicking in Scene View.\nHold down Control key to select multiple provinces.", MessageType.Info);
                            }
                        }
                        break;
                    case OPERATION_MODE.REVERT:
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        DrawWarningLabel("Discard current changes?");
                        if (GUILayout.Button("Discard", GUILayout.Width(80))) {
                            _editor.DiscardChanges();
                            _editor.operationMode = OPERATION_MODE.SELECTION;
                        }
                        if (GUILayout.Button("Cancel", GUILayout.Width(80))) {
                            _editor.operationMode = OPERATION_MODE.SELECTION;
                            _editor.reshapeRegionMode = RESHAPE_REGION_TOOL.POINT;
                        }
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Separator();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Separator();
                        EditorGUILayout.BeginVertical();
                        break;
                    case OPERATION_MODE.CONFIRM:
                        if (_editor.countryChanges) {
                            DrawCenteredLabel("There're pending countries modifications.");
                        }
                        if (_editor.provinceChanges) {
                            DrawCenteredLabel("There're pending provinces modifications.");
                        }
                        if (_editor.cityChanges) {
                            DrawCenteredLabel("There're pending cities modifications.");
                        }
                        if (_editor.mountPointChanges) {
                            DrawCenteredLabel("There're pending mount points modifications.");
                        }

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        DrawWarningLabel("Save changes?");
                        if (GUILayout.Button("Save", GUILayout.Width(80))) {
                            if (SaveMapChanges()) {
                                _editor.SetInfoMsg(INFO_MSG_CHANGES_SAVED);
                            } else {
                                _editor.SetInfoMsg(INFO_MSG_NO_CHANGES_TO_SAVE);
                            }
                            _editor.operationMode = OPERATION_MODE.SELECTION;
                        }
                        if (GUILayout.Button("Cancel", GUILayout.Width(80))) {
                            _editor.operationMode = OPERATION_MODE.SELECTION;
                            _editor.reshapeRegionMode = RESHAPE_REGION_TOOL.POINT;
                        }
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Separator();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Separator();
                        EditorGUILayout.BeginVertical();
                        break;
                    case OPERATION_MODE.RESHAPE:
                        bool entitySelected = false;
                        if (_editor.countryIndex >= 0) {
                            entitySelected = true;
                            ShowReshapingRegionTools();
                        }
                        if (_editor.cityIndex >= 0) {
                            entitySelected = true;
                            ShowReshapingCityTools();
                        }
                        if (_editor.mountPointIndex >= 0) {
                            entitySelected = true;
                            ShowReshapingMountPointTools();
                        }
                        if (!entitySelected) {
                            DrawWarningLabel("No country, province nor city selected.");
                        }
                        break;
                    case OPERATION_MODE.MAP_TOOLS:
                        ShowReshapingMapTools();
                        break;

                    case OPERATION_MODE.CREATE:
                        ShowCreateTools();
                        break;
                    case OPERATION_MODE.MAP_GENERATOR:
                        if (ShowMapGeneratorOptions()) {
                            GUIUtility.ExitGUI();
                            return;
                        }
                        break;
                }
                EditorGUILayout.EndVertical();

                if (_editor.operationMode != OPERATION_MODE.MAP_GENERATOR && _editor.operationMode != OPERATION_MODE.MAP_TOOLS && _editor.operationMode != OPERATION_MODE.REVERT && _editor.operationMode != OPERATION_MODE.CONFIRM) {
                    ShowEntitySelectors();
                }

                CheckHideEditorMesh();

            } catch {
            }
        }

        void CheckEditorStyles() {

            if (labelsStyle == null) {
                labelsStyle = new GUIStyle();
                labelsStyle.normal.textColor = Color.green;
                labelsStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (attribHeaderStyle == null) {
                attribHeaderStyle = new GUIStyle(EditorStyles.foldout);
                Color color = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);
                attribHeaderStyle.SetFoldoutColor(color);
                attribHeaderStyle.margin = new RectOffset(0, 0, 0, 0);
            }

            // Draw cursor
            if (editorCaptionLabelStyle == null) {
                editorCaptionLabelStyle = new GUIStyle();
            }
            editorCaptionLabelStyle.normal.textColor = Color.white;
        }


        void OnSceneGUI() {
            if (_editor != null && Camera.current != null && Camera.current.cameraType == CameraType.SceneView) {
                _editor.sceneCamera = Camera.current;
            }

            if (_editor.issueRedraw) {
                RedrawEditor();
                return;
            }

            CheckEditorStyles();
            ProcessOperationMode();
        }

        public void RedrawEditor() {
            _editor.issueRedraw = false;
            _editor.ClearSelection();
            _map.Redraw(true);
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        void FocusSceneView() {
            if (Application.isPlaying) return;
            SceneView sceneView = (SceneView)SceneView.sceneViews[0];
            sceneView.Focus();
        }

        void ShowEntitySelectors() {

            // preprocesssing logic first to not interfere with layout and repaint events
            string[] provinceNames, countryNames = _editor.countryNames, cityNames = _editor.cityNames, mountPointNames = _editor.mountPointNames;
            if (_editor.editingMode != EDITING_MODE.PROVINCES) {
                provinceNames = emptyStringArray;
            } else {
                provinceNames = _editor.provinceNames;
                if (provinceNames == null)
                    provinceNames = emptyStringArray;
            }
            if (mountPointNames == null)
                mountPointNames = emptyStringArray;


            int selectedCountriesCount = _editor.selectedCountriesRegions.Count;
            if (selectedCountriesCount > 1) {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.HelpBox("Multiple regions selected.", MessageType.Info);
                sb.Length = 0;
                for (int k = 0; k < selectedCountriesCount; k++) {
                    if (sb.Length > 0) {
                        sb.Append(", ");
                    }
                    Region region = _editor.selectedCountriesRegions[k];
                    sb.Append(region.entity.name);
                    sb.Append(" ");
                    sb.Append("(region ");
                    sb.Append(region.regionIndex);
                    sb.Append(")");
                }
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Countries", GUILayout.Width(90));
                GUILayout.TextArea(sb.ToString(), GUILayout.ExpandHeight(true));
                if (GUILayout.Button("Delete")) {
                    RemoveSelectedRegions();
                    GUIUtility.ExitGUI();
                    return;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();


            } else {

                EditorGUILayout.Separator();
                EditorGUILayout.BeginVertical(GUI.skin.box);

                // country selector
                EditorGUILayout.BeginHorizontal();
                int selection = EditorGUILayout.Popup("Country", _editor.GUICountryIndex, countryNames);
                if (selection != _editor.GUICountryIndex) {
                    _editor.CountrySelectByCombo(selection);
                }
                bool prevc = _editor.groupByParentAdmin;
                _editor.groupByParentAdmin = EditorGUILayout.ToggleLeft("Grouped", _editor.groupByParentAdmin, GUILayout.Width(90));
                if (_editor.groupByParentAdmin != prevc) {
                    _editor.ReloadCountryNames();
                }
                EditorGUILayout.EndHorizontal();
                if (_editor.countryIndex >= 0) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Id", _editor.countryIndex.ToString());
                    EditorGUILayout.BeginHorizontal();
                    _editor.GUICountryNewName = EditorGUILayout.TextField("Name", _editor.GUICountryNewName);
                    if (GUILayout.Button("Change")) {
                        _editor.CountryRename();
                    }
                    if (GUILayout.Button("Delete")) {
                        if (EditorUtility.DisplayDialog("Delete Country", "This option will completely delete current country and all its dependencies (cities, provinces, mount points, ...)\n\nContinue?", "Yes", "No")) {
                            _editor.CountryDelete();
                            _editor.SetInfoMsg(INFO_MSG_COUNTRY_DELETED);
                            _editor.operationMode = OPERATION_MODE.SELECTION;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    _editor.GUICountryHidden = EditorGUILayout.Toggle("Hidden", _editor.GUICountryHidden);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Sanitize")) {
                        if (EditorUtility.DisplayDialog("Sanitize Frontiers", "This option detects polygon issues (like self-crossing polygon) and fix them. Only use if you encounter some problem with the shape of this country.\n\nContinue?", "Ok", "Cancel")) {
                            if (_editor.CountrySanitize())
                                _editor.map.Redraw();
                            _editor.CountryRegionSelect();
                        }
                    }
                    if (GUILayout.Button("Merge Adjacent Regions")) {
                        if (EditorUtility.DisplayDialog("Merge Adjacent Regions", "This option will merge adjacent regions of this country into the same region.\n\nContinue?", "Ok", "Cancel")) {
                            if (_editor.CountryMergeAdjacentRegions()) {
                                _editor.map.Redraw();
                            }
                            _editor.CountryRegionSelect();
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    _editor.GUICountryTransferToCountryIndex = EditorGUILayout.Popup("Sovereign", _editor.GUICountryTransferToCountryIndex, countryNames);
                    if (GUILayout.Button("Transfer")) {
                        if (_editor.GUICountryIndex >= 0 && _editor.GUICountryTransferToCountryIndex >= 0 && _editor.GUICountryIndex != _editor.GUICountryTransferToCountryIndex) {
                            string sourceCountry = countryNames[_editor.GUICountryIndex].Trim();
                            string targetCountry = countryNames[_editor.GUICountryTransferToCountryIndex].Trim();
                            if (EditorUtility.DisplayDialog("Change Country's Sovereignty", "Current country " + sourceCountry + " will join target country " + targetCountry + ".\n\nAre you sure?", "Ok", "Cancel")) {
                                _editor.CountryTransferTo();
                                _editor.operationMode = OPERATION_MODE.SELECTION;
                            }
                        } else
                            EditorUtility.DisplayDialog("Invalid destination", "Can't transfer to itself.", "Ok");
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    _editor.GUICountryNewContinent = EditorGUILayout.TextField("Continent", _editor.GUICountryNewContinent);
                    GUI.enabled = _editor.countryIndex >= 0;
                    if (GUILayout.Button("Update")) {
                        _editor.CountryChangeContinent();
                    }
                    if (GUILayout.Button("Rename")) {
                        if (EditorUtility.DisplayDialog("Continent Renaming", "This option will rename the continent affecting to all countries in same continent. Continue?", "Yes", "No")) {
                            _editor.ContinentRename();
                        }
                    }
                    if (GUILayout.Button("Delete")) {
                        if (EditorUtility.DisplayDialog("Delete all countries (in same continent)", "You're going to delete all countries and provinces in continent " + _map.countries[_editor.countryIndex].continent + ".\n\nAre you sure?", "Yes", "No")) {
                            _editor.CountryDeleteSameContinent();
                            _editor.SetInfoMsg(INFO_MSG_CONTINENT_DELETED);
                            _editor.operationMode = OPERATION_MODE.SELECTION;
                        }
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();

                    _editor.GUICountryNewFIPS10_4 = EditorGUILayout.TextField("FIPS 10 4", _editor.GUICountryNewFIPS10_4);
                    _editor.GUICountryNewISO_A2 = EditorGUILayout.TextField("ISO A2", _editor.GUICountryNewISO_A2);
                    _editor.GUICountryNewISO_A3 = EditorGUILayout.TextField("ISO A3", _editor.GUICountryNewISO_A3);

                    EditorGUILayout.BeginHorizontal();
                    _editor.GUICountryNewISO_N3 = EditorGUILayout.TextField("ISO N3", _editor.GUICountryNewISO_N3);
                    GUI.enabled = _editor.countryIndex >= 0;
                    if (GUILayout.Button("Update FIPS/ISO Codes")) {
                        _editor.CountryChangeFIPSAndISOCodes();
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();

                    // Country attributes
                    if (_editor.countryIndex >= 0 && _editor.countryIndex < _map.countries.Length) {
                        Country country = _map.countries[_editor.countryIndex];
                        if (countryAttribGroup == null) {
                            countryAttribGroup = new WMSK_EditorAttribGroup();
                        }
                        if (countryAttribGroup.itemGroup != country) {
                            countryAttribGroup.SetItemGroup(country);
                        }
                        if (ShowAttributeGroup(countryAttribGroup)) {
                            _editor.countryAttribChanges = true;
                            GUIUtility.ExitGUI();
                            return;
                        }

                        // Country Regions
                        if (ShowRegionsGroup(country, _editor.countryRegionIndex)) {
                            GUIUtility.ExitGUI();
                            return;
                        }
                    }

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();

                if (_editor.editingMode == EDITING_MODE.PROVINCES) {
                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    int selectedProvincesCount = _editor.selectedProvincesRegions.Count;
                    if (selectedProvincesCount > 1) {
                        EditorGUILayout.HelpBox("Multiple regions selected.", MessageType.Info);
                        sb.Length = 0;
                        for (int k = 0; k < selectedProvincesCount; k++) {
                            if (sb.Length > 0) {
                                sb.Append(", ");
                            }
                            Region region = _editor.selectedProvincesRegions[k];
                            sb.Append(region.entity.name);
                            sb.Append(" ");
                            sb.Append("(region ");
                            sb.Append(region.regionIndex);
                            sb.Append(")");
                        }
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Provinces", GUILayout.Width(90));
                        GUILayout.TextArea(sb.ToString(), GUILayout.ExpandHeight(true));
                        EditorGUILayout.BeginVertical();
                        if (GUILayout.Button("Merge")) {
                            _editor.ProvincesMerge();
                            _editor.operationMode = OPERATION_MODE.SELECTION;
                            GUIUtility.ExitGUI();
                            return;
                        }
                        if (GUILayout.Button("Delete")) {
                            _editor.SelectedProvincesRegionsDelete();
                            GUIUtility.ExitGUI();
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("   New Country", GUILayout.Width(90));
                        _editor.GUIProvinceToNewCountryName = EditorGUILayout.TextField(_editor.GUIProvinceToNewCountryName);
                        GUI.enabled = !string.IsNullOrEmpty(_editor.GUIProvinceToNewCountryName);
                        if (GUILayout.Button("Create")) {
                            if (EditorUtility.DisplayDialog("Convert Province Into a Country", "This command will extract provinces " + sb.ToString() + " from its country " + _editor.GUICountryName + " and create a new country named " + _editor.GUIProvinceToNewCountryName + ".\n\nContinue?", "Yes", "No")) {
                                _editor.ProvincesRegionsToNewCountry();
                                _editor.operationMode = OPERATION_MODE.SELECTION;
                                GUIUtility.ExitGUI();
                                return;
                            }
                        }
                        GUI.enabled = true;
                        EditorGUILayout.EndHorizontal();
                    } else {

                        int poolCountryIndex = _map.GetCountryIndex(WMSK.COUNTRY_POOL_NAME);
                        if (poolCountryIndex < 0) {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("", GUILayout.Width(90));
                            if (GUILayout.Button("Create Background/Pool Country...")) {
                                if (EditorUtility.DisplayDialog("Create Background/Pool Country", "This option will add an invisible country with name 'Pool' which can be used to add provinces in a generic way. Later you can move the provinces out from the pool to create new countries or reassign them to other countries (check demo scene 06 Provinces Pool under General Examples folder for details)", "Ok", "Cancel")) {
                                    _editor.CountryCreatePool();
                                    GUIUtility.ExitGUI();
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        } else if (_editor.countryIndex != poolCountryIndex) {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("", GUILayout.Width(90));
                            if (GUILayout.Button("Transfer All Provinces to Background/Pool Country...")) {
                                if (EditorUtility.DisplayDialog("Transfer All Provinces To Background/Pool Country", "This option will transfer all provinces of current country to the 'Pool' country.", "Ok", "Cancel")) {
                                    _editor.CountryTransferProvincesToPool();
                                    GUIUtility.ExitGUI();
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUI.BeginChangeCheck();
                        _editor.showProvinceNames = EditorGUILayout.Toggle("Show Province Names", _editor.showProvinceNames);
                        if (EditorGUI.EndChangeCheck()) {
                            SceneView.RepaintAll();
                        }

                        int provSelection = EditorGUILayout.Popup("Province/State", _editor.GUIProvinceIndex, provinceNames);
                        if (provSelection != _editor.GUIProvinceIndex) {
                            _editor.ProvinceSelectByCombo(provSelection);
                        }

                        if (_editor.provinceIndex >= 0) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField("Id", _editor.provinceIndex.ToString());
                            EditorGUILayout.BeginHorizontal();
                            _editor.GUIProvinceNewName = EditorGUILayout.TextField("Name", _editor.GUIProvinceNewName);
                            if (GUILayout.Button("Change")) {
                                _editor.ProvinceRename();
                                SceneView.RepaintAll();
                            }
                            if (GUILayout.Button("Sanitize")) {
                                if (EditorUtility.DisplayDialog("Sanitize Borders", "This option detects polygon issues (like self-crossing polygon) and fix them. Only use if you encounter some problem with the shape of this province.\n\nContinue?", "Ok", "Cancel")) {
                                    _editor.ProvinceSanitize();
                                    _editor.ProvinceRegionSelect();
                                }
                            }
                            if (GUILayout.Button("Delete")) {
                                if (EditorUtility.DisplayDialog("Delete Province", "This option will completely delete current province.\n\nContinue?", "Yes", "No")) {
                                    _editor.ProvinceDelete();
                                    _editor.SetInfoMsg(INFO_MSG_PROVINCE_DELETED);
                                    _editor.operationMode = OPERATION_MODE.SELECTION;
                                    GUIUtility.ExitGUI();
                                    return;
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            _editor.GUIProvinceTransferToCountryIndex = EditorGUILayout.Popup("Sovereign", _editor.GUIProvinceTransferToCountryIndex, countryNames);
                            if (GUILayout.Button("Transfer")) {
                                string sourceProvince = provinceNames[_editor.GUIProvinceIndex].Trim();
                                string targetCountry = countryNames[_editor.GUIProvinceTransferToCountryIndex].Trim();
                                if (_editor.editingCountryFile == EDITING_COUNTRY_FILE.COUNTRY_LOWDEF) {
                                    EditorUtility.DisplayDialog("Change Province's Sovereignty", "This command is only available with High-Definition Country File selected.", "Ok");
                                } else if (EditorUtility.DisplayDialog("Change Province's Sovereignty", "Current province " + sourceProvince + " will join target country " + targetCountry + ".\n\nAre you sure?", "Ok", "Cancel")) {
                                    _editor.ProvinceTransferTo();
                                    _editor.operationMode = OPERATION_MODE.SELECTION;
                                    GUIUtility.ExitGUI();
                                    return;
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            _editor.GUIProvinceTransferToProvinceIndex = EditorGUILayout.Popup("Merge With", _editor.GUIProvinceTransferToProvinceIndex, provinceNames);
                            if (GUILayout.Button("Merge")) {
                                string sourceProvince = provinceNames[_editor.GUIProvinceIndex].Trim();
                                string targetProvince = provinceNames[_editor.GUIProvinceTransferToProvinceIndex].Trim();
                                if (_editor.editingCountryFile == EDITING_COUNTRY_FILE.COUNTRY_LOWDEF) {
                                    EditorUtility.DisplayDialog("Merge Province", "This command is only available with High-Definition Country File selected.", "Ok");
                                } else if (EditorUtility.DisplayDialog("Merge Provinces", "Current province " + sourceProvince + " will join target province " + targetProvince + ".\n\nAre you sure?", "Ok", "Cancel")) {
                                    _editor.ProvinceMerge();
                                    _editor.operationMode = OPERATION_MODE.SELECTION;
                                    GUIUtility.ExitGUI();
                                    return;
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            _editor.GUIProvinceToNewCountryName = EditorGUILayout.TextField("New Country", _editor.GUIProvinceToNewCountryName);
                            GUI.enabled = !string.IsNullOrEmpty(_editor.GUIProvinceToNewCountryName);
                            if (GUILayout.Button("Create")) {
                                if (EditorUtility.DisplayDialog("Convert Province Into a Country", "This command will extract current province " + _editor.GUIProvinceName + " from its country " + _editor.GUICountryName + " and create a new country named " + _editor.GUIProvinceToNewCountryName + ".\n\nContinue?", "Yes", "No")) {
                                    _editor.ProvinceToNewCountry();
                                }
                            }
                            GUI.enabled = true;
                            EditorGUILayout.EndHorizontal();


                            EditorGUILayout.BeginVertical(GUI.skin.box);

                            if (GUILayout.Button("Add Province to Country Regions")) {
                                if (EditorUtility.DisplayDialog("Add Province Region to Country Regions", "This option ensures the province region is also part of country regions. This means that highlight of country will include this province region as well.\n\nContinue?", "Yes", "No")) {
                                    _editor.AddProvinceRegionToCountry();
                                    _editor.SetInfoMsg(INFO_MSG_PROVINCE_REGION_ADDED_TO_COUNTRY);
                                    _editor.operationMode = OPERATION_MODE.SELECTION;
                                    GUIUtility.ExitGUI();
                                    return;
                                }
                            }


                            if (GUILayout.Button("Delete All Country Provinces")) {
                                if (EditorUtility.DisplayDialog("Delete All Country Provinces", "This option will delete all provinces of current country.\n\nContinue?", "Yes", "No")) {
                                    _editor.DeleteCountryProvinces();
                                    _editor.SetInfoMsg(INFO_MSG_PROVINCE_DELETED);
                                    _editor.operationMode = OPERATION_MODE.SELECTION;
                                    GUIUtility.ExitGUI();
                                    return;
                                }
                            }

                            if (GUILayout.Button("Simplify/Reduce Provinces")) {
                                WMSKProvincesEqualizer.ShowWindow(_editor.countryIndex);
                                _editor.ClearSelection();
                                GUIUtility.ExitGUI();
                                return;
                            }

                            EditorGUILayout.EndVertical();

                            // Province attributes
                            Province province = _map.provinces[_editor.provinceIndex];
                            if (provinceAttribGroup == null) {
                                provinceAttribGroup = new WMSK_EditorAttribGroup();
                            }
                            if (provinceAttribGroup.itemGroup != province) {
                                provinceAttribGroup.SetItemGroup(province);
                            }
                            if (ShowAttributeGroup(provinceAttribGroup)) {
                                _editor.provinceAttribChanges = true;
                                GUIUtility.ExitGUI();
                                return;
                            }
                            // Province Regions
                            if (ShowRegionsGroup(province, _editor.provinceRegionIndex)) {
                                GUIUtility.ExitGUI();
                                return;
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    EditorGUILayout.EndVertical();
                }




                if (_editor.countryIndex >= 0) {
                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginVertical(GUI.skin.box);



                    int citySelection = EditorGUILayout.Popup("City", _editor.GUICityIndex, cityNames);
                    if (citySelection != _editor.GUICityIndex) {
                        _editor.CitySelectByCombo(citySelection);
                    }
                    if (_editor.cityIndex >= 0) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField("Id", _editor.cityIndex.ToString());
                        EditorGUILayout.BeginHorizontal();
                        _editor.GUICityNewName = EditorGUILayout.TextField("Name", _editor.GUICityNewName);
                        if (GUILayout.Button("Change")) {
                            UndoPushCityStartOperation("Undo Rename City");
                            _editor.CityRename();
                            UndoPushCityEndOperation();
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        _editor.GUICityClass = (CITY_CLASS)EditorGUILayout.IntPopup("Class", (int)_editor.GUICityClass, cityClassOptions, cityClassValues);
                        if (GUILayout.Button("Update")) {
                            UndoPushCityStartOperation("Undo Change City Class");
                            _editor.CityClassChange();
                            UndoPushCityEndOperation();
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        _editor.GUICityPopulation = EditorGUILayout.TextField("Population", _editor.GUICityPopulation);
                        if (GUILayout.Button("Change")) {
                            int pop = 0;
                            if (int.TryParse(_editor.GUICityPopulation, out pop)) {
                                UndoPushCityStartOperation("Undo Change Population");
                                _editor.CityChangePopulation(pop);
                                UndoPushCityEndOperation();
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        // City attributes
                        City city = _map.cities[_editor.cityIndex];
                        if (cityAttribGroup == null) {
                            cityAttribGroup = new WMSK_EditorAttribGroup();
                        }
                        if (cityAttribGroup.itemGroup != city) {
                            cityAttribGroup.SetItemGroup(city);
                        }
                        if (ShowAttributeGroup(cityAttribGroup)) {
                            _editor.cityAttribChanges = true;
                            GUIUtility.ExitGUI();
                            return;
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    int mpSelection = EditorGUILayout.Popup("Mount Point", _editor.GUIMountPointIndex, mountPointNames);
                    if (mpSelection != _editor.GUIMountPointIndex) {
                        _editor.MountPointSelectByCombo(mpSelection);
                    }
                    if (_editor.mountPointIndex >= 0 && _map.mountPoints != null) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginHorizontal();
                        _editor.GUIMountPointNewName = EditorGUILayout.TextField("Name", _editor.GUIMountPointNewName);
                        if (GUILayout.Button("Update")) {
                            UndoPushMountPointStartOperation("Undo Modify Mount Point");
                            _editor.MountPointRename();
                            _editor.MountPointUpdateType();
                            UndoPushMountPointEndOperation();
                        }
                        EditorGUILayout.EndHorizontal();

                        _editor.GUIMountPointNewType = EditorGUILayout.TextField("Type", _editor.GUIMountPointNewType, GUILayout.Width(120));

                        // Mount point attributes
                        MountPoint mp = _map.mountPoints[_editor.mountPointIndex];
                        if (mountPointAttribGroup == null) {
                            mountPointAttribGroup = new WMSK_EditorAttribGroup();
                        }
                        if (mountPointAttribGroup.itemGroup != mp) {
                            mountPointAttribGroup.SetItemGroup(mp);
                        }
                        if (ShowAttributeGroup(mountPointAttribGroup)) {
                            _editor.mountPointChanges = true;
                            GUIUtility.ExitGUI();
                            return;
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }
            }

        }

        /// <summary>
        /// Returns true if there're changes
        /// </summary>
        bool ShowAttributeGroup(WMSK_EditorAttribGroup attribGroup) {

            EditorGUILayout.BeginHorizontal();
            attribGroup.foldOut = EditorGUILayout.Foldout(attribGroup.foldOut, "Attributes", attribHeaderStyle);
            EditorGUILayout.EndHorizontal();
            if (!attribGroup.foldOut)
                return false;

            JSONObject attrib = attribGroup.itemGroup.attrib;
            if (attrib.keys != null) {
                foreach (string key in attrib.keys) {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("   Tag", GUILayout.Width(90));
                    string newKey = EditorGUILayout.TextField(key);
                    string currentValue = attrib[key];
                    if (!newKey.Equals(key)) {
                        attrib.RenameKey(key, newKey);
                        return true;
                    }
                    GUILayout.Label("Value");
                    string newValue = EditorGUILayout.TextField(currentValue);
                    if (!newValue.Equals(currentValue)) {
                        attrib[key] = newValue;
                        return true;
                    }
                    if (GUILayout.Button("Remove")) {
                        attrib.RemoveField(key);
                        return true;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            // new tag line
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("   Tag", GUILayout.Width(90));
            attribGroup.newTagKey = EditorGUILayout.TextField(attribGroup.newTagKey);
            GUILayout.Label("Value");
            attribGroup.newTagValue = EditorGUILayout.TextField(attribGroup.newTagValue);
            if (GUILayout.Button("Add") && attribGroup.newTagKey.Length > 0) {
                if (!attrib.HasField(attribGroup.newTagKey)) {
                    attrib[attribGroup.newTagKey] = attribGroup.newTagValue;
                    attribGroup.newTagKey = "";
                    attribGroup.newTagValue = "";
                    return true;
                }
            }
            EditorGUILayout.EndHorizontal();
            return false;
        }


        /// <summary>
        /// Returns true if there're changes
        /// </summary>
        bool ShowRegionsGroup(IAdminEntity entity, int currentSelectedRegionIndex) {

            EditorGUILayout.BeginHorizontal();
            entity.foldOut = EditorGUILayout.Foldout(entity.foldOut, "Regions", attribHeaderStyle);
            EditorGUILayout.EndHorizontal();
            if (!entity.foldOut)
                return false;

            int regionCount = entity.regions.Count;
            for (int k = 0; k < regionCount; k++) {
                EditorGUILayout.BeginHorizontal();
                sb.Length = 0;
                sb.Append("    ");
                sb.Append(k.ToString());
                if (k == entity.mainRegionIndex)
                    sb.Append(" (Main)");
                if (currentSelectedRegionIndex == k)
                    sb.Append(" (Selected)");
                GUILayout.Label(sb.ToString(), GUILayout.Width(100));
                if (GUILayout.Button("Select", GUILayout.Width(60))) {
                    if (entity is Country) {
                        _editor.countryRegionIndex = k;
                        _editor.CountryRegionSelect();
                    } else {
                        _editor.provinceRegionIndex = k;
                        _editor.ProvinceRegionSelect();
                    }
                    SceneView.RepaintAll();
                }
                if (GUILayout.Button("Remove", GUILayout.Width(60))) {
                    if (EditorUtility.DisplayDialog("Remove Region", "Are you sure you want to remove this region?", "Ok", "Cancel")) {
                        if (entity is Country) {
                            _editor.countryRegionIndex = k;
                            _editor.CountryRegionDelete();
                        } else {
                            _editor.provinceRegionIndex = k;
                            _editor.ProvinceRegionDelete();
                        }
                        _editor.ClearSelection();
                        GUIUtility.ExitGUI();
                        return true;
                    }
                }
                if (GUILayout.Button("New Country", GUILayout.Width(90))) {
                    if (EditorUtility.DisplayDialog("New Country From Region", "Are you sure you want to create a new country based on this region (note: any contained province will also be moved to the new country)?", "Ok", "Cancel")) {
                        _editor.CountryCreate(entity.regions[k]);
                        _editor.ClearSelection();
                        GUIUtility.ExitGUI();
                        return true;
                    }
                }
                if (GUILayout.Button("New Province", GUILayout.Width(90))) {
                    if (EditorUtility.DisplayDialog("New Province From Region", "Are you sure you want to create a new province based on this region (note: if a province already contains this region this process will be cancelled)?", "Ok", "Cancel")) {
                        _editor.ProvinceCreate(entity.regions[k]);
                        GUIUtility.ExitGUI();
                        return true;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            return false;
        }


        void ShowReshapingRegionTools() {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawWarningLabel("REGION MODIFYING TOOLS");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            RESHAPE_REGION_TOOL prevTool = _editor.reshapeRegionMode;
            int selectionGridRows = (reshapeRegionToolbarIcons.Length - 1) / 4 + 1;
            GUIStyle selectionGridStyle = new GUIStyle(GUI.skin.button);
            selectionGridStyle.margin = new RectOffset(2, 2, 2, 2);
            _editor.reshapeRegionMode = (RESHAPE_REGION_TOOL)GUILayout.SelectionGrid((int)_editor.reshapeRegionMode, reshapeRegionToolbarIcons, 4, selectionGridStyle, GUILayout.Height(24 * selectionGridRows), GUILayout.MaxWidth(300));
            if (_editor.reshapeRegionMode != prevTool) {
                if (_editor.countryIndex >= 0) {
                    tickStart = DateTime.Now.Ticks;
                }
                ProcessOperationMode();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUIStyle explanationStyle = new GUIStyle(GUI.skin.box);
            explanationStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.32f, 0.33f, 0.6f);
            GUILayout.Box(reshapeRegionModeExplanation[(int)_editor.reshapeRegionMode], explanationStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            if (_editor.reshapeRegionMode.hasCircle()) {
                _editor.reshapeCircleWidth = EditorGUILayout.Slider("Circle Width", _editor.reshapeCircleWidth, 0.0001f, 0.1f);
            }

            if (_editor.reshapeRegionMode == RESHAPE_REGION_TOOL.POINT || _editor.reshapeRegionMode == RESHAPE_REGION_TOOL.CIRCLE || _editor.reshapeRegionMode == RESHAPE_REGION_TOOL.ERASER || _editor.reshapeRegionMode == RESHAPE_REGION_TOOL.SMOOTH) {
                _editor.circleCurrentRegionOnly = EditorGUILayout.Toggle("Selected Region Only", _editor.circleCurrentRegionOnly);

                EditorGUILayout.LabelField("Cursor", "X: " + _editor.cursor.x.ToString("F7") + ", Y: " + _editor.cursor.y.ToString("F7"));

                string snapUnitStr = _editor.snapPrecisionDigits.ToString();
                string newSnapUnit = EditorGUILayout.TextField("Snap Precision Digits:", snapUnitStr);
                if (!newSnapUnit.Equals(snapUnitStr)) {
                    if (int.TryParse(newSnapUnit, out _editor.snapPrecisionDigits)) {
                        _editor.snapPrecisionDigits = Mathf.Clamp(_editor.snapPrecisionDigits, 3, 7);
                        EditorPrefs.SetInt("SnapUnit", _editor.snapPrecisionDigits);
                    }
                }
            }

            switch (_editor.reshapeRegionMode) {
                case RESHAPE_REGION_TOOL.CIRCLE:
                    _editor.circleMoveConstant = EditorGUILayout.Toggle(new GUIContent("Constant Move", "Apply the same amount of displacement to all points inside the circle regardless of the distance to the center of the circle."), _editor.circleMoveConstant);
                    break;
                case RESHAPE_REGION_TOOL.MAGNET:
                    _editor.magnetAgressiveMode = EditorGUILayout.Toggle(new GUIContent("Agressive Mode", "Collapses all points to the nearest point removing duplicates instead of just merging pair of points."), _editor.magnetAgressiveMode);
                    _editor.magnetIncludeCountries = EditorGUILayout.Toggle(new GUIContent("Include Countries", "Also affects points of country frontiers when editing provinces."), _editor.magnetIncludeCountries);
                    break;
                case RESHAPE_REGION_TOOL.SPLITV:
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    DrawWarningLabel("Confirm split vertically?");
                    if (GUILayout.Button("Split", GUILayout.Width(80))) {
                        _editor.SplitVertically();
                        _editor.operationMode = OPERATION_MODE.SELECTION;
                    }
                    if (GUILayout.Button("Cancel", GUILayout.Width(80))) {
                        _editor.reshapeRegionMode = RESHAPE_REGION_TOOL.POINT;
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    break;
                case RESHAPE_REGION_TOOL.SPLITH:
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    DrawWarningLabel("Confirm split horizontally?");
                    if (GUILayout.Button("Split", GUILayout.Width(80))) {
                        _editor.SplitHorizontally();
                        _editor.operationMode = OPERATION_MODE.SELECTION;
                    }
                    if (GUILayout.Button("Cancel", GUILayout.Width(80))) {
                        _editor.reshapeRegionMode = RESHAPE_REGION_TOOL.POINT;
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    break;
                case RESHAPE_REGION_TOOL.DELETE:
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (_editor.entityIndex < 0) {
                        DrawWarningLabel("Select a region to delete.");
                    } else {
                        if (_editor.editingMode == EDITING_MODE.COUNTRIES) {
                            bool deletingRegion = _map.countries[_editor.countryIndex].regions.Count > 1;
                            if (deletingRegion) {
                                DrawWarningLabel("Confirm delete this region?");
                            } else {
                                DrawWarningLabel("Confirm delete this country?");
                            }
                            if (GUILayout.Button("Delete", GUILayout.Width(80))) {
                                if (deletingRegion) {
                                    _editor.CountryRegionDelete();
                                    _editor.SetInfoMsg(INFO_MSG_REGION_DELETED);
                                } else {
                                    _editor.CountryDelete();
                                    _editor.SetInfoMsg(INFO_MSG_COUNTRY_DELETED);
                                }
                                _editor.operationMode = OPERATION_MODE.SELECTION;
                            }
                        } else {
                            if (_editor.provinceIndex >= 0 && _editor.provinceIndex < _map.provinces.Length) {
                                if (_editor.provinceIndex >= 0 && _editor.provinceIndex < _map.provinces.Length) {
                                    bool deletingRegion = _map.provinces[_editor.provinceIndex].regions != null && _map.provinces[_editor.provinceIndex].regions.Count > 1;
                                    if (deletingRegion) {
                                        DrawWarningLabel("Confirm delete this region?");
                                    } else {
                                        DrawWarningLabel("Confirm delete this province/state?");
                                    }
                                    if (GUILayout.Button("Delete", GUILayout.Width(80))) {
                                        if (deletingRegion) {
                                            _editor.ProvinceRegionDelete();
                                            _editor.SetInfoMsg(INFO_MSG_REGION_DELETED);
                                        } else {
                                            _editor.ProvinceDelete();
                                            _editor.SetInfoMsg(INFO_MSG_PROVINCE_DELETED);
                                        }
                                        _editor.operationMode = OPERATION_MODE.SELECTION;
                                    }
                                }
                            }
                        }

                        if (GUILayout.Button("Cancel", GUILayout.Width(80))) {
                            _editor.reshapeRegionMode = RESHAPE_REGION_TOOL.POINT;
                        }
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
        }

        void ShowReshapingCityTools() {
            GUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawWarningLabel("CITY MODIFYING TOOLS");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            RESHAPE_CITY_TOOL prevTool = _editor.reshapeCityMode;
            int selectionGridRows = (reshapeCityToolbarIcons.Length - 1) / 2 + 1;
            GUIStyle selectionGridStyle = new GUIStyle(GUI.skin.button);
            selectionGridStyle.margin = new RectOffset(2, 2, 2, 2);
            _editor.reshapeCityMode = (RESHAPE_CITY_TOOL)GUILayout.SelectionGrid((int)_editor.reshapeCityMode, reshapeCityToolbarIcons, 2, selectionGridStyle, GUILayout.Height(24 * selectionGridRows), GUILayout.MaxWidth(150));
            if (_editor.reshapeCityMode != prevTool) {
                ProcessOperationMode();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUIStyle explanationStyle = new GUIStyle(GUI.skin.box);
            explanationStyle.normal.textColor = new Color(0.52f, 0.66f, 0.9f);
            GUILayout.Box(reshapeCityModeExplanation[(int)_editor.reshapeCityMode], explanationStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndHorizontal();

            switch (_editor.reshapeCityMode) {
                case RESHAPE_CITY_TOOL.DELETE:
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (_editor.cityIndex < 0) {
                        DrawWarningLabel("Select a city to delete.");
                    } else {
                        DrawWarningLabel("Confirm delete this city?");
                        if (GUILayout.Button("Delete", GUILayout.Width(80))) {
                            UndoPushCityStartOperation("Undo Delete City");
                            _editor.DeleteCity();
                            UndoPushCityEndOperation();
                            _editor.SetInfoMsg(INFO_MSG_CITY_DELETED);
                            _editor.operationMode = OPERATION_MODE.SELECTION;
                        }
                        if (GUILayout.Button("Cancel", GUILayout.Width(80))) {
                            _editor.reshapeCityMode = RESHAPE_CITY_TOOL.MOVE;
                        }
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    break;
            }

            GUILayout.EndVertical();
        }

        void ShowReshapingMountPointTools() {
            GUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawWarningLabel("MOUNT POINT MODIFYING TOOLS");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            RESHAPE_MOUNT_POINT_TOOL prevTool = _editor.reshapeMountPointMode;
            int selectionGridRows = (reshapeMountPointToolbarIcons.Length - 1) / 2 + 1;
            GUIStyle selectionGridStyle = new GUIStyle(GUI.skin.button);
            selectionGridStyle.margin = new RectOffset(2, 2, 2, 2);
            _editor.reshapeMountPointMode = (RESHAPE_MOUNT_POINT_TOOL)GUILayout.SelectionGrid((int)_editor.reshapeMountPointMode, reshapeMountPointToolbarIcons, 2, selectionGridStyle, GUILayout.Height(24 * selectionGridRows), GUILayout.MaxWidth(150));
            if (_editor.reshapeMountPointMode != prevTool) {
                ProcessOperationMode();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUIStyle explanationStyle = new GUIStyle(GUI.skin.box);
            explanationStyle.normal.textColor = new Color(0.52f, 0.66f, 0.9f);
            GUILayout.Box(reshapeMountPointModeExplanation[(int)_editor.reshapeMountPointMode], explanationStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndHorizontal();

            switch (_editor.reshapeMountPointMode) {
                case RESHAPE_MOUNT_POINT_TOOL.DELETE:
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (_editor.mountPointIndex < 0) {
                        DrawWarningLabel("Select a mount point to delete.");
                    } else {
                        DrawWarningLabel("Confirm delete this mount point?");
                        if (GUILayout.Button("Delete", GUILayout.Width(80))) {
                            UndoPushMountPointStartOperation("Undo Delete Mount Point");
                            _editor.DeleteMountPoint();
                            UndoPushMountPointEndOperation();
                            _editor.SetInfoMsg(INFO_MSG_MOUNT_POINT_DELETED);
                            _editor.operationMode = OPERATION_MODE.SELECTION;
                        }
                        if (GUILayout.Button("Cancel", GUILayout.Width(80))) {
                            _editor.reshapeMountPointMode = RESHAPE_MOUNT_POINT_TOOL.MOVE;
                        }
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    break;
            }

            GUILayout.EndVertical();
        }

        void ShowReshapingMapTools() {
            EditorGUILayout.Separator();

            DrawWarningLabel("Global Map Transform");

            EditorGUI.BeginChangeCheck();
            mapScale = EditorGUILayout.Vector2Field(new GUIContent("Map Scale", "Applies a scale multiplier to all map entities."), mapScale);
            mapOffset = EditorGUILayout.Vector2Field(new GUIContent("Map Offset", "Applies an shift to all map entities."), mapOffset);
            if (EditorGUI.EndChangeCheck()) {
                MapTransformApply();
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset")) {
                MapTransformReset();
                _editor.DiscardChanges();
            }
            if (GUILayout.Button("Crop")) {
                MapTransformCrop();
            }
            if (GUILayout.Button("Apply & Save")) {
                if (EditorUtility.DisplayDialog("Apply Map Transform", "Scale and offset will be applied to coordinates of map entities. This change cannot be undone (you will need to restore the backup).\nContinue?", "Ok", "Cancel")) {
                    MapTransformReset();
                    if (SaveMapChanges()) {
                        _editor.SetInfoMsg(INFO_MSG_CHANGES_SAVED);
                    } else {
                        _editor.SetInfoMsg(INFO_MSG_NO_CHANGES_TO_SAVE);
                    }
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            bool textureCoordChanges = false;
            EditorGUI.BeginChangeCheck();
            _map.earthTextureScale = EditorGUILayout.Vector2Field(new GUIContent("Texture Scale", "Applies a scale multiplier to all map entities."), _map.earthTextureScale);
            _map.earthTextureOffset = EditorGUILayout.Vector2Field(new GUIContent("Texture Offset", "Applies an shift to all map entities."), _map.earthTextureOffset);
            if (EditorGUI.EndChangeCheck()) {
                textureCoordChanges = true;
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset Texture Transform")) {
                _map.earthTextureScale = Misc.Vector2one;
                _map.earthTextureOffset = Misc.Vector2zero;
                textureCoordChanges = true;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (textureCoordChanges) {
                EditorUtility.SetDirty(_map);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }

        }

        void ShowCreateTools() {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            CREATE_TOOL prevCTool = _editor.createMode;
            GUIStyle selectionCGridStyle = new GUIStyle(GUI.skin.button);
            int selectionCGridRows = (createToolbarIcons.Length - 1) / 3 + 1;
            selectionCGridStyle.margin = new RectOffset(2, 2, 2, 2);
            _editor.createMode = (CREATE_TOOL)GUILayout.SelectionGrid((int)_editor.createMode, createToolbarIcons, 3, selectionCGridStyle, GUILayout.Height(24 * selectionCGridRows), GUILayout.MaxWidth(310));
            if (_editor.createMode != prevCTool) {
                ProcessOperationMode();
                NewShapeInit();
                if (_editor.editingMode == EDITING_MODE.COUNTRIES && (_editor.createMode == CREATE_TOOL.PROVINCE || _editor.createMode == CREATE_TOOL.PROVINCE_REGION)) {
                    _editor.ChangeEditingMode(EDITING_MODE.PROVINCES);
                }
                FocusSceneView();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUIStyle explanationCStyle = new GUIStyle(GUI.skin.box);
            explanationCStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.22f, 0.36f, 0.6f);
            GUILayout.Box(createModeExplanation[(int)_editor.createMode], explanationCStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndHorizontal();

            if (_editor.createMode == CREATE_TOOL.MOUNT_POINT) {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Mount Point Mass Creator Tool");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("  Minimum", "Minimum number of mount points to create per region."), GUILayout.Width(90));
                _editor.GUIMountPointMassPopulationMin = EditorGUILayout.IntSlider(_editor.GUIMountPointMassPopulationMin, 0, 100);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("  Maximum", "Maximum number of mount points to create per region."), GUILayout.Width(90));
                _editor.GUIMountPointMassPopulationMax = EditorGUILayout.IntSlider(Mathf.Max(_editor.GUIMountPointMassPopulationMax, _editor.GUIMountPointMassPopulationMin), 0, 100);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("  Separation", "Separation threshold between existing mount points. Value is % of size of region."), GUILayout.Width(90));
                _editor.GUIMountPointMassPopulationSeparation = EditorGUILayout.Slider(_editor.GUIMountPointMassPopulationSeparation, 5, 25);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("  Type", "Type of the mount points to be created"), GUILayout.Width(90));
                _editor.GUIMountPointMassPopulationType = EditorGUILayout.TextField(_editor.GUIMountPointMassPopulationType);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Populate Continent Countries")) {
                    if (CheckSelectedCountry() && EditorUtility.DisplayDialog("Mount Point Mass Creator", "Between " + _editor.GUIMountPointMassPopulationMin + " and " + _editor.GUIMountPointMassPopulationMax + " mount points of type " + _editor.GUIMountPointMassPopulationTypeInteger + " will be created per country in current continent.\n\nDepending on separation specified, it may not be enough room for a new mount point in a country; in this case, it will be skipped.\n\nContinue?", "Yes", "No")) {
                        _editor.MountPointPopulateContinentCountries();
                    }
                }
                if (GUILayout.Button("Populate Country")) {
                    if (CheckSelectedCountry() && EditorUtility.DisplayDialog("Mount Point Mass Creator", "Between " + _editor.GUIMountPointMassPopulationMin + " and " + _editor.GUIMountPointMassPopulationMax + " mount points of type " + _editor.GUIMountPointMassPopulationTypeInteger + " will be created in currently selected country.\n\nDepending on separation specified, it may not be enough room for a new mount point in a country; in this case, it will be skipped.\n\nContinue?", "Yes", "No")) {
                        _editor.MountPointPopulateCountry();
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (_editor.editingMode == EDITING_MODE.PROVINCES) {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Populate Continent Provinces")) {
                        if (CheckSelectedCountry() && EditorUtility.DisplayDialog("Mount Point Mass Creator", "Between " + _editor.GUIMountPointMassPopulationMin + " and " + _editor.GUIMountPointMassPopulationMax + " mount points of type " + _editor.GUIMountPointMassPopulationTypeInteger + " will be created per province in current continent.\n\nDepending on separation specified, it may not be enough room for a new mount point in a province; in this case, it will be skipped.\n\nContinue?", "Yes", "No")) {
                            _editor.MountPointPopulateContinentProvinces();
                        }
                    }
                    if (GUILayout.Button("Populate Country Provinces")) {
                        if (CheckSelectedCountry() && EditorUtility.DisplayDialog("Mount Point Mass Creator", "Between " + _editor.GUIMountPointMassPopulationMin + " and " + _editor.GUIMountPointMassPopulationMax + " mount points of type " + _editor.GUIMountPointMassPopulationTypeInteger + " will be created in each province of currently selected country.\n\nDepending on separation specified, it may not be enough room for a new mount point in a province; in this case, it will be skipped.\n\nContinue?", "Yes", "No")) {
                            _editor.MountPointPopulateProvinces();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Populate Province")) {
                        if (CheckSelectedProvince() && EditorUtility.DisplayDialog("Mount Point Mass Creator", "Between " + _editor.GUIMountPointMassPopulationMin + " and " + _editor.GUIMountPointMassPopulationMax + " mount points of type " + _editor.GUIMountPointMassPopulationTypeInteger + " will be created in currently selected province.\n\nDepending on separation specified, it may not be enough room for a new mount point in the province; in this case, it will be skipped.\n\nContinue?", "Yes", "No")) {
                            _editor.MountPointPopulateProvince();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }


        void RemoveSelectedRegions() {
            _editor.SelectedProvincesRegionsDelete();
            _editor.SelectedCountriesRegionsDelete();
        }

        #endregion

        #region Menu items

        // Hard separator hack.
        [MenuItem("CONTEXT/WMSK_Editor/About Map Editor", false, 85)]
        static void HelpMenuOption(MenuCommand command) {
            EditorUtility.DisplayDialog("World Map Strategy Kit Map Editor", "For help and guidance please read the documentation included in the asset or contact us on our support forum on kronnect.com or send an email to contact@kronnect.com.", "Ok");
        }


        // Add a menu item called "Restore Backup".
        [MenuItem("CONTEXT/WMSK_Editor/New Map", false, 100)]
        static void NewMapMenuOption(MenuCommand command) {
            if (EditorUtility.DisplayDialog("New Map", "Are you sure you want to remove all existing countries, provinces, cities and mount points?", "YES!", "Cancel")) {
                WMSK.instance.editor.NewMap();
            }
        }

        // Add a menu item called "Restore Backup".
        [MenuItem("CONTEXT/WMSK_Editor/Terrain Importer", false, 101)]
        static void TerrainImporterMenuOption(MenuCommand command) {
            WMSK.instance.editor.terrainImporterActive = true;
            WMSKTerrainImporter.ShowWindow();
        }

#if !UNITY_WSA
        // Add a menu item called "Restore Backup".
        [MenuItem("CONTEXT/WMSK_Editor/Territories Importer", false, 102)]
        static void TerritoriesImporterMenuOption(MenuCommand command) {
            WMSK.instance.editor.territoryImporterActive = true;
            WMSKTerritoriesImporter.ShowWindow();
        }
#endif

        // Add a menu item called "Export Provinces Map".
        [MenuItem("CONTEXT/WMSK_Editor/Export Provinces Color Map", false, 103)]
        static void ExportProvincesMenuOption(MenuCommand command) {
            if (!EditorUtility.DisplayDialog("Export Provinces Map", "This command will color all provinces with a different color and export the texture to ProvincesMap.png file at the root of the Unity project (existing file from a previous bake texture operation will be replaced).\n\nYou can then modify this image to color all provinces from the same country and import the modified image using the ImportProvincesColorMap() method at runtime thus generating a new map with different countries and province distribution.\n\nGenerating the province color map can take some time depending on CPU speed.\n\nProceed?", "Ok", "Cancel"))
                return;

            // Proceed and restore
            string filename = "Assets/ProvincesMap.png";
            WMSKEditorInspector.ExportProvincesMap(filename);
            AssetDatabase.Refresh();
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(filename);
            if (tex != null) {
                Selection.activeObject = tex;
                EditorGUIUtility.PingObject(tex);
            }

        }



        // Add a menu item called "Restore Backup".
        [MenuItem("CONTEXT/WMSK_Editor/Restore Backup", false, 120)]
        static void RestoreBackupMenuOption(MenuCommand command) {
            if (!EditorUtility.DisplayDialog("Restore original geodata files?", "Current geodata files will be replaced by the original files from Backup folder. Any changes will be lost. This operation can't be undone.\n\nRestore files?", "Restore", "Cancel")) {
                return;
            }

            // Proceed and restore
            string[] paths = AssetDatabase.GetAllAssetPaths();
            bool backupFolderExists = false;
            string geoDataFolder = "", backupFolder = "";
            for (int k = 0; k < paths.Length; k++) {
                if (paths[k].EndsWith(WMSK.instance.geodataResourcesPath)) {
                    geoDataFolder = paths[k];
                } else if (paths[k].EndsWith("WorldMapStrategyKit/Backup")) {
                    backupFolder = paths[k];
                    backupFolderExists = true;
                }
            }

            WMSK_Editor editor = (WMSK_Editor)command.context;

            if (!backupFolderExists) {
                editor.SetInfoMsg(INFO_MSG_BACKUP_NOT_FOUND);
                return;
            }

            string fullFileName;
            // Countries110
            fullFileName = backupFolder + "/countries110.txt";
            if (File.Exists(fullFileName)) {
                AssetDatabase.DeleteAsset(geoDataFolder + "/countries110.txt");
                AssetDatabase.SaveAssets();
                AssetDatabase.CopyAsset(fullFileName, geoDataFolder + "/countries110.txt");
            }
            // Countries10
            fullFileName = backupFolder + "/countries10.txt";
            if (File.Exists(fullFileName)) {
                AssetDatabase.DeleteAsset(geoDataFolder + "/countries10.txt");
                AssetDatabase.SaveAssets();
                AssetDatabase.CopyAsset(fullFileName, geoDataFolder + "/countries10.txt");
            }
            // Country attributes
            fullFileName = backupFolder + "/" + WMSK.instance.countryAttributeFile + ".json";
            if (File.Exists(fullFileName)) {
                AssetDatabase.DeleteAsset(geoDataFolder + "/" + WMSK.instance.countryAttributeFile);
                AssetDatabase.SaveAssets();
                AssetDatabase.CopyAsset(fullFileName, geoDataFolder + "/" + WMSK.instance.countryAttributeFile + ".json");
            }
            // Provinces10
            fullFileName = backupFolder + "/provinces10.txt";
            if (File.Exists(fullFileName)) {
                AssetDatabase.DeleteAsset(geoDataFolder + "/provinces10.txt");
                AssetDatabase.SaveAssets();
                AssetDatabase.CopyAsset(fullFileName, geoDataFolder + "/provinces10.txt");
            }
            // Provinces attributes
            fullFileName = backupFolder + "/" + WMSK.instance.provinceAttributeFile + ".json";
            if (File.Exists(fullFileName)) {
                AssetDatabase.DeleteAsset(geoDataFolder + "/" + WMSK.instance.provinceAttributeFile);
                AssetDatabase.SaveAssets();
                AssetDatabase.CopyAsset(fullFileName, geoDataFolder + "/" + WMSK.instance.provinceAttributeFile + ".json");
            }
            // Cities10
            fullFileName = backupFolder + "/cities10.txt";
            if (File.Exists(fullFileName)) {
                AssetDatabase.DeleteAsset(geoDataFolder + "/cities10.txt");
                AssetDatabase.SaveAssets();
                AssetDatabase.CopyAsset(fullFileName, geoDataFolder + "/cities10.txt");
            }
            // Cities attributes
            fullFileName = backupFolder + "/" + WMSK.instance.cityAttributeFile + ".json";
            if (File.Exists(fullFileName)) {
                AssetDatabase.DeleteAsset(geoDataFolder + "/" + WMSK.instance.cityAttributeFile);
                AssetDatabase.SaveAssets();
                AssetDatabase.CopyAsset(fullFileName, geoDataFolder + "/" + WMSK.instance.cityAttributeFile + ".json");
            }
            // Mount points
            fullFileName = backupFolder + "/mountPoints.json";
            if (File.Exists(fullFileName)) {
                AssetDatabase.DeleteAsset(geoDataFolder + "/mountPoints.json");
                AssetDatabase.SaveAssets();
                AssetDatabase.CopyAsset(fullFileName, geoDataFolder + "/mountPoints.json");
            }

            AssetDatabase.Refresh();

            // Save changes
            editor.SetInfoMsg(INFO_MSG_BACKUP_RESTORED);
            editor.DiscardChanges();
        }


        // Add a menu item called "Create Low Definition Geodata File".
        [MenuItem("CONTEXT/WMSK_Editor/Create Low Definition Country Geodata File", false, 130)]
        static void CreateLowDefinitionFileMenuOption(MenuCommand command) {
            WMSK_Editor editor = (WMSK_Editor)command.context;
            if (editor.editingCountryFile != EDITING_COUNTRY_FILE.COUNTRY_HIGHDEF) {
                EditorUtility.DisplayDialog("Create Low Definition Country Geodata File", "Switch to the high definition country geodata file first.", "Ok");
                return;
            }
            if (!EditorUtility.DisplayDialog("Create Low Definition Country Geodata File", "The low definition geodata file will be replaced by a reduced quality version of the high definition geodata file.\n\nChanges to the low definition file will be lost. Continue?", "Proceed", "Cancel")) {
                return;
            }

            string geoDataFolder;
            CheckBackup(out geoDataFolder);

            // Save changes
            string dataFileName = "countries110.txt";
            string fullPathName = Application.dataPath;
            int pos = fullPathName.LastIndexOf("/Assets");
            if (pos > 0)
                fullPathName = fullPathName.Substring(0, pos + 1);
            fullPathName += geoDataFolder + "/" + dataFileName;
            string data = editor.GetCountryGeoDataLowQuality();
            File.WriteAllText(fullPathName, data, System.Text.Encoding.UTF8);
            AssetDatabase.Refresh();

            editor.SetInfoMsg(INFO_MSG_GEODATA_LOW_QUALITY_CREATED);

            editor.map.frontiersDetail = FRONTIERS_DETAIL.Low; // switch to low quality to see results
            editor.DiscardChanges();
        }


        // Add a menu item called "Create Low Definition Geodata File".
        [MenuItem("CONTEXT/WMSK_Editor/Create Low Definition Province Geodata File", false, 130)]
        static void CreateLowDefinitionProvinceFileMenuOption(MenuCommand command) {
            WMSK_Editor editor = (WMSK_Editor)command.context;
            if (!EditorUtility.DisplayDialog("Create Low Definition Province Geodata File", "The provinces geodata file will be replaced by a reduced quality version.\n\nChanges to the existing definition file will be lost. Continue?", "Proceed", "Cancel")) {
                return;
            }

            string geoDataFolder;
            CheckBackup(out geoDataFolder);

            // Save changes
            string dataFileName = "provinces10.txt";
            string fullPathName = Application.dataPath;
            int pos = fullPathName.LastIndexOf("/Assets");
            if (pos > 0)
                fullPathName = fullPathName.Substring(0, pos + 1);
            fullPathName += geoDataFolder + "/" + dataFileName;
            string data = editor.GetProvinceGeoDataLowQuality();
            File.WriteAllText(fullPathName, data, System.Text.Encoding.UTF8);
            AssetDatabase.Refresh();

            editor.SetInfoMsg(INFO_MSG_GEODATA_LOW_QUALITY_CREATED);

            editor.map.frontiersDetail = FRONTIERS_DETAIL.Low; // switch to low quality to see results
            editor.DiscardChanges();
        }

        [MenuItem("CONTEXT/WMSK_Editor/Hide Off-Screen Countries", false, 181)]
        static void HideCountriesOffScreenMenuOption(MenuCommand command) {
            if (!EditorUtility.DisplayDialog("Hide Off-Screen Countries", "All countries not currently visible in the Scene View will be marked as hidden (they won't be removed).\n\nContinue?", "Hide Countries", "Cancel")) {
                return;
            }
            WMSK_Editor editor = (WMSK_Editor)command.context;
            if (editor.sceneCamera == null) {
                EditorUtility.DisplayDialog("Scene camera not available", "The scene camera is not yet available. Click in the scene view and try again.", "Ok");
                return;
            }
            editor.CountryHideOffScreen();
            editor.ClearSelection();
            editor.map.Redraw(true);

            EditorUtility.DisplayDialog("Hide Off-Screen Countries", "Off-screen countries are now hidden. They still are selectable in the map editor. Remember to save your changes.", "Ok");

        }

        [MenuItem("CONTEXT/WMSK_Editor/Delete Off-Screen Countries", false, 182)]
        static void DeleteCountriesOffScreenMenuOption(MenuCommand command) {
            if (!EditorUtility.DisplayDialog("Delete Off-Screen Countries", "All countries not currently visible in the Scene View will be DELETED.\n\nContinue?", "Delete Countries", "Cancel")) {
                return;
            }
            WMSK_Editor editor = (WMSK_Editor)command.context;
            if (editor.sceneCamera == null) {
                EditorUtility.DisplayDialog("Scene camera not available", "The scene camera is not yet available. Click in the scene view and try again.", "Ok");
                return;
            }
            editor.ClearSelection();
            editor.CountryDeleteOffScreen();
            editor.map.Redraw(true);

            EditorUtility.DisplayDialog("Delete Off-Screen Countries", "Countries and related provices, cities and mount-points deleted. Remember to save your changes.", "Ok");
        }

        [MenuItem("CONTEXT/WMSK_Editor/Equalize Countries", false, 132)]
        static void EqualizeCountriesMenuOption(MenuCommand command) {

            WMSKCountriesEqualizer.ShowWindow();

            // no modal operation exists, so just clear selection
            WMSK_Editor editor = (WMSK_Editor)command.context;
            editor.ClearSelection();
        }

        [MenuItem("CONTEXT/WMSK_Editor/Equalize Provinces", false, 133)]
        static void EqualizeProvincesMenuOption(MenuCommand command) {

            WMSKProvincesEqualizer.ShowWindow();

            // no modal operation exists, so just clear selection
            WMSK_Editor editor = (WMSK_Editor)command.context;
            editor.ClearSelection();
        }

        #endregion


        #region Processing logic

        void SwitchEditingFrontiersFile() {
            if (_editor.editingCountryFile == EDITING_COUNTRY_FILE.COUNTRY_HIGHDEF) {
                _map.frontiersDetail = FRONTIERS_DETAIL.High;
            } else {
                _map.frontiersDetail = FRONTIERS_DETAIL.Low;
            }
            _editor.DiscardChanges();
        }

        void ProcessOperationMode() {

            AdjustCityIconsScale();
            AdjustMountPointIconsScale();

            // Check mouse buttons state and react to possible undo/redo operations
            bool mouseDown = false;
            Event e = Event.current;
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (GUIUtility.hotControl == controlID) {   // release hot control to allow standard navigation
                GUIUtility.hotControl = 0;
            }
            // locks control on map
            var eventType = e.GetTypeForControl(controlID);
            if (eventType == EventType.MouseDown && e.button == 0) {
                mouseDown = true;
                GUIUtility.hotControl = controlID;
                startedReshapeRegion = false;
                startedReshapeCity = false;
            } else if (eventType == EventType.MouseUp && e.button == 0) {
                if (undoPushStarted) {
                    if (startedReshapeRegion) {
                        UndoPushRegionEndOperation();
                    }
                    if (startedReshapeCity) {
                        UndoPushCityEndOperation();
                    }
                }
                if (_editor.operationMode == OPERATION_MODE.RESHAPE) {
                    // after executing a polygon modifier tool, ensure all frontiers are drawn again
                    _editor.RedrawFrontiers(null, true);
                    CheckHideEditorMesh();
                    return;
                }
            }

            if (e.type == EventType.ValidateCommand && e.commandName.Equals("UndoRedoPerformed")) {
                _editor.UndoHandle();
                EditorUtility.SetDirty(target);
                return;
            }

            switch (_editor.operationMode) {
                case OPERATION_MODE.SELECTION:
                    // do we click inside a country or province?
                    if (Camera.current == null) // can't ray-trace
                        return;
                    if (mouseDown) {
                        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                        bool selected = _editor.CountrySelectByScreenClick(ray);
                        if (!selected && !e.control) {
                            _editor.ClearSelection();
                        };
                        if (_editor.editingMode == EDITING_MODE.PROVINCES) {
                            selected = _editor.ProvinceSelectByScreenClick(ray);
                            if (!selected && !e.control)
                                _editor.ClearProvinceSelection();
                        }
                        if (!_editor.CitySelectByScreenClick(ray) && !e.control) {
                            _editor.ClearCitySelection();
                        }
                        if (_editor.MountPointSelectByScreenClick(ray)) {
                            if (e.control) {
                                UndoPushMountPointStartOperation("Undo Delete Mount Point");
                                _editor.DeleteMountPoint();
                                UndoPushMountPointEndOperation();
                                _editor.SetInfoMsg(INFO_MSG_MOUNT_POINT_DELETED);
                                GUIUtility.ExitGUI();
                            }
                        } else if (!e.control) {
                            _editor.ClearMountPointSelection();
                        }
                        // Reset the cursor if entity selected
                        if (_editor.countryIndex >= 0) {
                            if (_editor.editingMode == EDITING_MODE.PROVINCES) {
                                _map.DrawProvinces(_editor.countryIndex, true, false, false);
                            }
                        }
                        if (_editor.selectedRegion != null) {
                            _editor.cursor = _editor.selectedRegion.center;
                        }
                    }

                    ShowShapePoints(false);
                    ShowCitySelected();
                    ShowMountPointSelected();
                    DrawCellNumbers();

                    // Short cuts

                    // Show tooltip and handle hotkeys
                    if (Camera.current != null) {
                        Vector3 labelPos = Camera.current.ScreenToWorldPoint(new Vector3(10, 30, 1f));
                        Handles.Label(labelPos, "Hotkeys: Shift+X = Remove currently selected regions. Hold down Control key to select multiple regions.", editorCaptionLabelStyle);
                    }

                    if (Event.current != null && Event.current.type == EventType.KeyDown) {
                        if (Event.current.shift && Event.current.keyCode == KeyCode.X) {
                            RemoveSelectedRegions();
                            Event.current.Use();
                        }
                    }
                    break;

                case OPERATION_MODE.RESHAPE:
                    // do we move any handle to change frontiers?
                    switch (_editor.reshapeRegionMode) {
                        case RESHAPE_REGION_TOOL.POINT:
                        case RESHAPE_REGION_TOOL.CIRCLE:
                            ExecuteMoveTool(e.mousePosition);
                            break;
                        case RESHAPE_REGION_TOOL.MAGNET:
                        case RESHAPE_REGION_TOOL.ERASER:
                        case RESHAPE_REGION_TOOL.SMOOTH:
                            ExecuteClickTool(e.mousePosition, mouseDown);
                            break;
                        case RESHAPE_REGION_TOOL.SPLITH:
                        case RESHAPE_REGION_TOOL.SPLITV:
                        case RESHAPE_REGION_TOOL.DELETE:
                            ShowShapePoints(false);
                            break;
                    }
                    switch (_editor.reshapeCityMode) {
                        case RESHAPE_CITY_TOOL.MOVE:
                            ExecuteCityMoveTool();
                            break;
                    }
                    switch (_editor.reshapeMountPointMode) {
                        case RESHAPE_MOUNT_POINT_TOOL.MOVE:
                            ExecuteMountPointMoveTool();
                            break;
                    }
                    break;
                case OPERATION_MODE.CREATE:
                    switch (_editor.createMode) {
                        case CREATE_TOOL.CITY:
                            ExecuteCityCreateTool(e.mousePosition, mouseDown);
                            break;
                        case CREATE_TOOL.COUNTRY:
                            ExecuteShapeCreateTool(e.mousePosition, mouseDown);
                            break;
                        case CREATE_TOOL.COUNTRY_REGION:
                        case CREATE_TOOL.PROVINCE:
                            if (_editor.countryIndex >= 0 && _editor.countryIndex < _map.countries.Length) {
                                ExecuteShapeCreateTool(e.mousePosition, mouseDown);
                            } else {
                                _editor.SetInfoMsg(INFO_MSG_CHOOSE_COUNTRY);
                            }
                            break;
                        case CREATE_TOOL.PROVINCE_REGION:
                            if (_editor.countryIndex <= 0 || _editor.countryIndex >= _map.countries.Length) {
                                _editor.SetInfoMsg(INFO_MSG_CHOOSE_COUNTRY);
                            } else if (_editor.provinceIndex < 0 || _editor.provinceIndex >= _map.provinces.Length) {
                                _editor.SetInfoMsg(INFO_MSG_CHOOSE_PROVINCE);
                            } else {
                                ExecuteShapeCreateTool(e.mousePosition, mouseDown);
                            }
                            break;
                        case CREATE_TOOL.MOUNT_POINT:
                            ExecuteMountPointCreateTool(e.mousePosition, mouseDown);
                            break;
                    }
                    break;
                case OPERATION_MODE.CONFIRM:
                case OPERATION_MODE.REVERT:
                    break;
            }

            if (_editor.editingMode == EDITING_MODE.PROVINCES) {
                DrawEditorProvinceNames();
            }
            CheckHideEditorMesh();
        }

        bool onePointSelected;
        Vector3 selectedPoint;

        void ExecuteMoveTool(Vector2 mousePosition) {
            bool frontiersUnchanged = true;
            if (_editor.selectedRegion == null)
                return;

            // Update cursor
            if (Camera.current != null) {
                Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                int layerMask = 1 << _map.gameObject.layer; // MAP BELONGS TO LAYER UI
                RaycastHit[] hits = Physics.RaycastAll(ray, 500, layerMask);
                if (hits.Length > 0) {
                    for (int k = 0; k < hits.Length; k++) {
                        if (hits[k].collider.gameObject == _map.gameObject) {
                            Vector3 cursorPos = hits[k].point;
                            _editor.cursor = _map.transform.InverseTransformPoint(cursorPos);
                            _editor.cursor.z = 0;
                        }
                    }
                }
            }

            // Manage move tool
            Vector2[] points = _editor.selectedRegion.points;
            Vector2 sourcePosition = Misc.Vector2zero, displacement = Misc.Vector2zero, newCoor = Misc.Vector2zero;
            Vector3 oldPoint, newPoint;
            Transform mapTransform = _map.transform;
            if (controlIds == null || controlIds.Length < points.Length)
                controlIds = new int[points.Length];

            for (int i = 0; i < points.Length; i++) {
                oldPoint = mapTransform.TransformPoint(points[i]);
                float handleSize = HandleUtility.GetHandleSize(oldPoint) * HANDLE_SIZE;
                var fmh_1989_61_638321484499202370 = mapTransform.rotation; newPoint = Handles.FreeMoveHandle(oldPoint, handleSize, pointSnap,
                                                   (handleControlID, position, rotation, size, eventType) => {
                                                       controlIds[i] = handleControlID;
                                                       Handles.DotHandleCap(handleControlID, position, rotation, size, eventType);
                                                   });
                if (GUIUtility.hotControl == controlIds[i] && GUIUtility.hotControl != 0) {
                    onePointSelected = true;
                    selectedPoint = oldPoint;
                }
                if (frontiersUnchanged && oldPoint != newPoint) {
                    frontiersUnchanged = false;
                    newCoor = mapTransform.InverseTransformPoint(newPoint);
                    sourcePosition = points[i];
                    displacement = new Vector2(newCoor.x - points[i].x, newCoor.y - points[i].y);
                }
            }
            if (_editor.reshapeRegionMode.hasCircle()) {
                if (!onePointSelected) {
                    selectedPoint = mapTransform.TransformPoint(points[0]);
                }
                float size = _editor.reshapeCircleWidth * mapTransform.localScale.y;
                Handles.CircleHandleCap(0, selectedPoint, mapTransform.rotation, size, EventType.Repaint);
                HandleUtility.Repaint();
            }

            if (!frontiersUnchanged) {
                List<Region> affectedRegions = null;
                switch (_editor.reshapeRegionMode) {
                    case RESHAPE_REGION_TOOL.POINT:
                        if (!startedReshapeRegion)
                            UndoPushRegionStartOperation("Undo Point Move");
                        affectedRegions = _editor.MovePoint(sourcePosition, displacement);
                        break;
                    case RESHAPE_REGION_TOOL.CIRCLE:
                        if (!startedReshapeRegion)
                            UndoPushRegionStartOperation("Undo Group Move");
                        affectedRegions = _editor.MoveCircle(sourcePosition, displacement, _editor.reshapeCircleWidth);
                        break;
                }
                _editor.RedrawFrontiers(affectedRegions, highlightSelected: false, fastMode: true);
                HandleUtility.Repaint();
            }
        }

        void ExecuteClickTool(Vector2 mousePosition, bool clicked) {
            if (_editor.selectedRegion == null)
                return;

            // Show the mouse cursor
            if (Camera.current == null)
                return;

            // Show the points
            ShowShapePoints(_editor.reshapeRegionMode != RESHAPE_REGION_TOOL.SMOOTH);
            Transform mapTransform = _map.transform;

            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            int layerMask = 1 << _map.gameObject.layer; // MAP BELONGS TO LAYER UI
            RaycastHit[] hits = Physics.RaycastAll(ray, 500, layerMask);
            if (hits.Length > 0) {
                for (int k = 0; k < hits.Length; k++) {
                    if (hits[k].collider.gameObject == _map.gameObject) {
                        Vector3 cursorPos = hits[k].point;
                        _editor.cursor = mapTransform.InverseTransformPoint(cursorPos);
                        _editor.cursor.z = 0;
                        if (_editor.reshapeRegionMode == RESHAPE_REGION_TOOL.SMOOTH) {
                            ShowCandidatePoint();
                        } else {
                            // Show circle cursor
                            float seconds = (float)new TimeSpan(DateTime.Now.Ticks - tickStart).TotalSeconds;
                            seconds *= 4.0f;
                            float t = seconds % 2;
                            if (t >= 1)
                                t = 2 - t;
                            float effect = Mathf.SmoothStep(0, 1, t) / 10.0f;
                            float size = _editor.reshapeCircleWidth * mapTransform.localScale.y * (0.9f + effect);
                            Handles.CircleHandleCap(0, cursorPos, mapTransform.rotation, size, EventType.Repaint);
                        }

                        if (clicked) {
                            switch (_editor.reshapeRegionMode) {
                                case RESHAPE_REGION_TOOL.MAGNET:
                                    if (!startedReshapeRegion)
                                        UndoPushRegionStartOperation("Undo Magnet");
                                    _editor.Magnet(_editor.cursor, _editor.reshapeCircleWidth);
                                    break;
                                case RESHAPE_REGION_TOOL.ERASER:
                                    if (!startedReshapeRegion)
                                        UndoPushRegionStartOperation("Undo Eraser");
                                    _editor.Erase(_editor.cursor, _editor.reshapeCircleWidth);
                                    break;
                                case RESHAPE_REGION_TOOL.SMOOTH:
                                    if (!startedReshapeRegion)
                                        UndoPushRegionStartOperation("Undo Smooth");
                                    _editor.AddPointToRegion(_editor.cursor); // Addpoint manages the refresh
                                    break;
                            }
                        }
                        HandleUtility.Repaint();
                        break;
                    }
                }
            }
        }

        void ExecuteCityCreateTool(Vector2 mousePosition, bool clicked) {

            // Show the mouse cursor
            if (Camera.current == null)
                return;

            // Show the points
            Transform mapTransform = _map.transform;

            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            int layerMask = 1 << _map.gameObject.layer; // MAP BELONGS TO LAYER UI
            RaycastHit[] hits = Physics.RaycastAll(ray, 500, layerMask);
            if (hits.Length > 0) {
                for (int k = 0; k < hits.Length; k++) {
                    if (hits[k].collider.gameObject == _map.gameObject) {
                        Vector3 cursorPos = hits[k].point;
                        _editor.cursor = mapTransform.InverseTransformPoint(cursorPos);
                        _editor.cursor.z = 0;

                        Handles.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                        Vector3 pt = mapTransform.TransformPoint(_editor.cursor);
                        float handleSize = HandleUtility.GetHandleSize(pt) * HANDLE_SIZE * 4.0f;
                        Handles.SphereHandleCap(0, pt, mapTransform.rotation, handleSize, EventType.Repaint);
                        Handles.color = Color.white;

                        if (clicked) {
                            if (_editor.countryIndex < 0 || _editor.countryIndex >= _map.countries.Length) {
                                EditorUtility.DisplayDialog("Add new city", "Please choose a country first.", "Ok");
                                return;
                            }
                            UndoPushCityStartOperation("Undo Create City");
                            _editor.CityCreate(_editor.cursor);
                            UndoPushCityEndOperation();
                        }
                    }
                    HandleUtility.Repaint();
                }
            }
        }

        void AdjustCityIconsScale() {
            // Adjust city icons in scene view
            if (_map == null || _map.cities == null)
                return;

            Transform t = _map.transform.Find("Cities");
            if (t != null) {
                CityScaler scaler = t.GetComponent<CityScaler>();
                if (scaler != null) {
                    scaler.ScaleCities(0.1f);
                }
            } else {
                // This should not happen but maybe the user deleted the layer. Forces refresh.
                _map.DrawCities();
            }
        }

        void ShowCitySelected() {
            if (_map.cities == null || _editor.cityIndex < 0 || _editor.cityIndex >= _map.cities.Length)
                return;
            City city = _map.cities[_editor.cityIndex];
            if (city == null) return;
            Vector3 cityPos = city.unity2DLocation;
            Vector3 worldPos = _map.transform.TransformPoint(cityPos);
            float handleSize = HandleUtility.GetHandleSize(worldPos) * HANDLE_SIZE * 2.0f;

            Handles.RectangleHandleCap(0, worldPos, _map.transform.rotation, handleSize, EventType.Repaint);
        }

        void ExecuteCityMoveTool() {
            if (_editor.cityIndex < 0 || _editor.cityIndex >= _map.cities.Length)
                return;

            Transform mapTransform = _map.transform;
            City city = _map.cities[_editor.cityIndex];
            if (city == null) return;
            Vector3 cityPos = city.unity2DLocation;
            Vector3 oldPoint = mapTransform.TransformPoint(cityPos);
            float handleSize = HANDLE_SIZE * 1.2f;

            var fmh_2174_65_638321484499237023 = mapTransform.rotation; Vector3 newPoint = Handles.FreeMoveHandle(oldPoint, handleSize, pointSnap,
                                                   (handleControlID, position, rotation, size, eventType) => {
                                                       Handles.RectangleHandleCap(handleControlID, position, rotation, size, eventType);
                                                   });
            if (newPoint != oldPoint) {
                newPoint = mapTransform.InverseTransformPoint(newPoint);
                newPoint.z = 0;
                if (!startedReshapeCity)
                    UndoPushCityStartOperation("Undo City Move");
                _editor.CityMove(newPoint);
                HandleUtility.Repaint();
            }
        }

        void AdjustMountPointIconsScale() {
            // Adjust icons in scene view
            if (_map == null || _map.mountPoints == null)
                return;

            Transform t = _map.transform.Find("Mount Points");
            if (t != null) {
                MountPointScaler scaler = t.GetComponent<MountPointScaler>();
                scaler.ScaleMountPoints(0.1f);
            } else {
                // This should not happen but maybe the user deleted the layer. Forces refresh.
                _map.DrawMountPoints();
            }
        }

        void ExecuteMountPointCreateTool(Vector2 mousePosition, bool clicked) {

            // Show the mouse cursor
            if (Camera.current == null)
                return;

            // Show the points
            Transform mapTransform = _map.transform;

            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            int layerMask = 1 << _map.gameObject.layer; // MAP BELONGS TO LAYER UI
            RaycastHit[] hits = Physics.RaycastAll(ray, 5000, layerMask);
            if (hits.Length > 0) {
                for (int k = 0; k < hits.Length; k++) {
                    if (hits[k].collider.gameObject == _map.gameObject) {
                        Vector3 cursorPos = hits[k].point;
                        _editor.cursor = mapTransform.InverseTransformPoint(cursorPos);

                        Handles.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                        float handleSize = HandleUtility.GetHandleSize(cursorPos) * HANDLE_SIZE * 4.0f;

                        Handles.SphereHandleCap(0, cursorPos, mapTransform.rotation, handleSize, EventType.Repaint);
                        Handles.color = Color.white;

                        if (clicked) {
                            if (_editor.countryIndex < 0 || _editor.countryIndex >= _map.countries.Length) {
                                EditorUtility.DisplayDialog("Add new mount point", "Please choose a country first.", "Ok");
                                return;
                            }
                            UndoPushMountPointStartOperation("Undo Create Mount Point");
                            _editor.MountPointCreate(_editor.cursor);
                            UndoPushMountPointEndOperation();
                        }
                    }
                    HandleUtility.Repaint();
                }
            }
        }

        void ShowMountPointSelected() {
            if (_editor.mountPointIndex < 0 || _map.mountPoints == null || _editor.mountPointIndex >= _map.mountPoints.Count)
                return;
            Vector3 mountPointPos = _map.mountPoints[_editor.mountPointIndex].unity2DLocation;
            Vector3 worldPos = _map.transform.TransformPoint(mountPointPos);
            float handleSize = HandleUtility.GetHandleSize(worldPos) * HANDLE_SIZE * 2.0f;
            Handles.RectangleHandleCap(0, worldPos, _map.transform.rotation, handleSize, EventType.Repaint);
        }

        void ExecuteMountPointMoveTool() {
            if (_map.mountPoints == null || _editor.mountPointIndex < 0 || _editor.mountPointIndex >= _map.mountPoints.Count)
                return;

            Transform mapTransform = _map.transform;
            Vector3 mountPointPos = _map.mountPoints[_editor.mountPointIndex].unity2DLocation;
            Vector3 oldPoint = mapTransform.TransformPoint(mountPointPos);
            float handleSize = HandleUtility.GetHandleSize(oldPoint) * HANDLE_SIZE * 2.0f;

            var fmh_2260_65_638321484499242128 = mapTransform.rotation; Vector3 newPoint = Handles.FreeMoveHandle(oldPoint, handleSize, pointSnap,
                                                                   (handleControlID, position, rotation, size, eventType) => {
                                                                       Handles.RectangleHandleCap(handleControlID, position, rotation, size, eventType);
                                                                   });
            if (newPoint != oldPoint) {
                newPoint = mapTransform.InverseTransformPoint(newPoint);
                if (!startedReshapeMountPoint)
                    UndoPushMountPointStartOperation("Undo Mount Point Move");
                _editor.MountPointMove(newPoint);
                HandleUtility.Repaint();
            }
        }

        void UndoPushRegionStartOperation(string operationName) {
            startedReshapeRegion = !startedReshapeRegion;
            undoPushStarted = true;
            Undo.RecordObject(target, operationName);   // record changes to the undo dummy flag
            _editor.UndoRegionsPush(_editor.highlightedRegions);
        }

        void UndoPushRegionEndOperation() {
            undoPushStarted = false;
            _editor.UndoRegionsInsertAtCurrentPos(_editor.highlightedRegions);
            if (_editor.reshapeRegionMode != RESHAPE_REGION_TOOL.SMOOTH) { // Smooth operation doesn't need to refresh labels nor frontiers
                _map.DrawMapLabels();
                bool refreshAllFrontiers = _editor.reshapeRegionMode != RESHAPE_REGION_TOOL.CIRCLE && _editor.reshapeRegionMode != RESHAPE_REGION_TOOL.POINT;
                if (refreshAllFrontiers) {
                    _editor.RedrawFrontiers(null, true);
                } else {
                    _editor.RedrawFrontiers();
                }
            }
        }

        void UndoPushCityStartOperation(string operationName) {
            startedReshapeCity = !startedReshapeCity;
            undoPushStarted = true;
            Undo.RecordObject(target, operationName);   // record changes to the undo dummy flag
            _editor.UndoCitiesPush();
        }

        void UndoPushCityEndOperation() {
            undoPushStarted = false;
            _editor.UndoCitiesInsertAtCurrentPos();
        }

        void UndoPushMountPointStartOperation(string operationName) {
            startedReshapeMountPoint = !startedReshapeMountPoint;
            undoPushStarted = true;
            Undo.RecordObject(target, operationName);   // record changes to the undo dummy flag
            _editor.UndoMountPointsPush();
        }

        void UndoPushMountPointEndOperation() {
            undoPushStarted = false;
            _editor.UndoMountPointsInsertAtCurrentPos();
        }





        #endregion

        #region Editor UI handling

        void CheckHideEditorMesh() {
            if (!_editor.shouldHideEditorMesh)
                return;
            _editor.shouldHideEditorMesh = false;
            Transform s = _map.transform.Find(WMSK.SURFACE_LAYER);
            if (s == null)
                return;
            Renderer[] rr = s.GetComponentsInChildren<Renderer>(true);
            for (int k = 0; k < rr.Length; k++) {
                EditorUtility.SetSelectedRenderState(rr[k], EditorSelectedRenderState.Hidden);
            }
        }

        void ShowShapePoints(bool highlightInsideCircle) {
            if (_map.countries == null)
                return;
            Region region = _editor.selectedRegion;
            if (region != null) {
                Transform mapTransform = _map.transform;
                float circleSizeSqr = _editor.reshapeCircleWidth * _editor.reshapeCircleWidth;
                if (region.points != null) {
                    for (int i = 0; i < region.points.Length; i++) {
                        Vector3 rp = region.points[i];
                        Vector3 p = mapTransform.TransformPoint(rp);
                        float handleSize = HandleUtility.GetHandleSize(p) * HANDLE_SIZE;
                        if (highlightInsideCircle) {
                            float dist = (rp.x - _editor.cursor.x) * (rp.x - _editor.cursor.x) * 4.0f + (rp.y - _editor.cursor.y) * (rp.y - _editor.cursor.y);
                            if (dist < circleSizeSqr) {
                                Handles.color = Color.green;
                                Handles.DotHandleCap(0, p, mapTransform.rotation, handleSize, EventType.Repaint);
                                continue;
                            } else {
                                Handles.color = Color.white;
                            }
                        }
                        Handles.RectangleHandleCap(0, p, mapTransform.rotation, handleSize, EventType.Repaint);
                    }
                }
            }
            Handles.color = Color.white;
        }

        /// <summary>
        /// Shows a potential new point near from cursor location (point parameter, which is in local coordinates)
        /// </summary>
        void ShowCandidatePoint() {
            Region region = _editor.selectedRegion;
            if (region == null)
                return;
            int max = region.points.Length;
            float minDist = float.MaxValue;
            int nearest = -1, previous = 0;
            Vector2 rp = Misc.Vector2zero;
            for (int p = 0; p < max; p++) {
                int q = p == 0 ? max - 1 : p - 1;
                FastVector.Average(ref region.points[p], ref region.points[q], ref rp);
                float dist = (rp.x - _editor.cursor.x) * (rp.x - _editor.cursor.x) * 4 + (rp.y - _editor.cursor.y) * (rp.y - _editor.cursor.y);
                if (dist < minDist) {
                    // Get nearest point
                    minDist = dist;
                    nearest = p;
                    previous = q;
                }
            }

            if (nearest >= 0) {
                Transform mapTransform = _map.transform;
                Vector3 pointToInsert = (region.points[nearest] + region.points[previous]) * 0.5f;
                Handles.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                Vector3 pt = mapTransform.TransformPoint(pointToInsert);
                float handleSize = HandleUtility.GetHandleSize(pt) * HANDLE_SIZE;
                Handles.DotHandleCap(0, pt, mapTransform.rotation, handleSize, EventType.Repaint);
                Handles.color = Color.white;
            }
        }

        void NewShapeInit() {
            if (_editor.newShape == null)
                _editor.newShape = new List<Vector2>();
            else
                _editor.newShape.Clear();
        }

        void NewShapeRemoveLastPoint() {
            if (_editor.newShape != null && _editor.newShape.Count > 0)
                _editor.newShape.RemoveAt(_editor.newShape.Count - 1);
        }


        /// <summary>
        /// Returns any city near the point specified in local coordinates.
        /// </summary>
        int NewShapeGetIndexNearPoint(Vector3 localPoint) {
            float rl = localPoint.x - HIT_PRECISION;
            float rr = localPoint.x + HIT_PRECISION;
            float rt = localPoint.y + HIT_PRECISION;
            float rb = localPoint.y - HIT_PRECISION;
            for (int c = 0; c < _editor.newShape.Count; c++) {
                Vector3 cityLoc = _editor.newShape[c];
                if (cityLoc.x > rl && cityLoc.x < rr && cityLoc.y > rb && cityLoc.y < rt) {
                    return c;
                }
            }
            return -1;
        }

        /// <summary>
        /// Shows a potential point to be added to the new shape and draws current shape polygon
        /// </summary>
        void ExecuteShapeCreateTool(Vector3 mousePosition, bool mouseDown) {
            // Show the mouse cursor
            if (Camera.current == null)
                return;

            // Show the points
            Transform mapTransform = _map.transform;

            int numPoints = _editor.newShape.Count;
            Vector3[] shapePoints = new Vector3[numPoints + 1];
            for (int k = 0; k < numPoints; k++) {
                shapePoints[k] = mapTransform.TransformPoint(_editor.newShape[k]);
            }
            shapePoints[numPoints] = mapTransform.TransformPoint(_editor.cursor);

            // Draw shape polygon in same color as corresponding frontiers
            if (numPoints >= 1) {
                if (_editor.createMode == CREATE_TOOL.COUNTRY || _editor.createMode == CREATE_TOOL.COUNTRY_REGION) {
                    Handles.color = _map.frontiersColor;
                } else {
                    Handles.color = _map.provincesColor;
                }
                Handles.DrawPolyLine(shapePoints);
                Handles.color = Color.white;
            }

            // Draw handles
            for (int i = 0; i < shapePoints.Length - 1; i++) {
                float handleSize = HandleUtility.GetHandleSize(shapePoints[i]) * HANDLE_SIZE;
                Handles.RectangleHandleCap(0, shapePoints[i], mapTransform.rotation, handleSize, EventType.Repaint);
            }

            // Show tooltip and handle hotkeys
            if (Camera.current != null) {
                Vector3 labelPos = Camera.current.ScreenToWorldPoint(new Vector3(10, 60, 1f));
                Handles.Label(labelPos, "Hotkeys: Shift+C = Close polygon (requires +5 vertices, currently: " + numPoints + "), Esc = Remove last point, Shift+X = Remove all points", editorCaptionLabelStyle);
                labelPos = Camera.current.ScreenToWorldPoint(new Vector3(10, 30, 1f));
                Handles.Label(labelPos, " Shift+S: Snap to nearest vertex, Shift+B: Snap to map edge, Shift+G = Fast contour, Shift+Z = Divide existing region", editorCaptionLabelStyle);
            }

            bool snapRequested = false, contourRequested = false, snapToBorderRequested = false;
            if (Event.current != null && Event.current.type == EventType.KeyDown) {
                // Shift + X: remove last point
                if (Event.current.shift && Event.current.keyCode == KeyCode.X) {
                    _editor.newShape.Clear();
                    Event.current.Use();
                } else if (numPoints > 0 && Event.current.keyCode == KeyCode.Escape) {
                    NewShapeRemoveLastPoint();
                    Event.current.Use();
                } else if (Event.current.shift && Event.current.keyCode == KeyCode.S) {
                    snapRequested = true;
                    Event.current.Use();
                } else if (Event.current.shift && Event.current.keyCode == KeyCode.G) {
                    contourRequested = true;
                    Event.current.Use();
                } else if (Event.current.shift && Event.current.keyCode == KeyCode.B) {
                    snapToBorderRequested = true;
                    Event.current.Use();
                } else if (Event.current.shift && Event.current.keyCode == KeyCode.Z) {
                    if (numPoints < 2) {
                        EditorUtility.DisplayDialog("Divide", "2 or more points are required.", "Ok");
                    } else {
                        if (_editor.RegionDivide()) {
                            _editor.operationMode = OPERATION_MODE.SELECTION;
                            SceneView.RepaintAll();
                        }
                    }
                    Event.current.Use();
                }
            }

            // Draw handles
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            int layerMask = 1 << _map.gameObject.layer; // MAP BELONGS TO LAYER UI
            RaycastHit[] hits = Physics.RaycastAll(ray, 500, layerMask);
            bool canClosePolygon = false;
            if (hits.Length > 0) {
                for (int k = 0; k < hits.Length; k++) {
                    if (hits[k].collider.gameObject == _map.gameObject) {
                        Region region;
                        int pointIndex;

                        Vector3 cursorPos = hits[k].point;
                        Vector3 newPos = mapTransform.InverseTransformPoint(cursorPos);
                        newPos.z = 0;

                        // Shift + S: create a new vertex next to another near existing vertex
                        if (snapRequested) {
                            Vector2 nearPos;
                            if (_editor.GetNearestVertex(newPos, out nearPos, out region, out pointIndex)) {
                                newPos = nearPos;
                                mouseDown = true;
                                if (numPoints == 0) {
                                    _editor.newShapeSegmentRegion = region;
                                    _editor.newShapeSegmentPointIndex = pointIndex;
                                }
                            }
                        } else if (contourRequested) {
                            Vector2 nearPos;
                            if (_editor.GetNearestVertex(newPos, out nearPos, out region, out pointIndex)) {
                                if (numPoints == 0) {
                                    newPos = nearPos;
                                    mouseDown = true;
                                } else {
                                    Vector2 lastPos = _editor.newShape[numPoints - 1];
                                    if (_editor.AddNearestContour(lastPos, nearPos)) {
                                        mouseDown = true;
                                        canClosePolygon = true;
                                    } else {
                                        break;
                                    }
                                }
                            }
                        } else if (snapToBorderRequested) {
                            Vector2 nearPos;
                            if (_editor.GetMapBorderNearPos(newPos, out nearPos)) {
                                newPos = nearPos;
                                mouseDown = true;
                            }
                        }

                        _editor.cursor = newPos;
                        if (numPoints > 2) { // Check if we're over the first point
                            int i = NewShapeGetIndexNearPoint(newPos);
                            if (i == 0) {
                                Vector3 labelPos;
                                if (Camera.current != null) {
                                    Vector3 screenPos = Camera.current.WorldToScreenPoint(cursorPos);
                                    labelPos = Camera.current.ScreenToWorldPoint(screenPos + Vector3.up * 20f + Vector3.right * 12f);
                                } else {
                                    labelPos = cursorPos + Vector3.up * 0.17f;
                                }
                                if (numPoints > 5) {
                                    Handles.Label(labelPos, "Press Shift+C to close polygon", editorCaptionLabelStyle);
                                } else {
                                    Handles.Label(labelPos, "Add " + (6 - numPoints) + " more point(s)", editorCaptionLabelStyle);
                                }
                            }
                        }
                        Handles.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                        Vector3 pt = mapTransform.TransformPoint(_editor.cursor);
                        float handleSize = HandleUtility.GetHandleSize(pt) * HANDLE_SIZE;
                        Handles.DotHandleCap(0, pt, mapTransform.rotation, handleSize, EventType.Repaint);
                        Handles.color = Color.white;

                        // Hotkey for closing polygon (Control + C)
                        if (numPoints > 4 && (Event.current != null && Event.current.shift && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C)) {
                            mouseDown = true;
                            canClosePolygon = true;
                            Event.current.Use();
                        }

                        if (mouseDown) {
                            if (canClosePolygon) {
                                switch (_editor.createMode) {
                                    case CREATE_TOOL.COUNTRY:
                                        _editor.CountryCreate();
                                        break;
                                    case CREATE_TOOL.COUNTRY_REGION:
                                        _editor.CountryRegionCreate();
                                        break;
                                    case CREATE_TOOL.PROVINCE:
                                        _editor.ProvinceCreate();
                                        break;
                                    case CREATE_TOOL.PROVINCE_REGION:
                                        _editor.ProvinceRegionCreate();
                                        break;
                                }
                                NewShapeInit();
                            } else {
                                if (!_editor.newShape.Contains(_editor.cursor)) {
                                    _editor.newShape.Add(_editor.cursor);
                                }
                                break;
                            }
                        }
                        HandleUtility.Repaint();
                        break;
                    }
                }
            }
        }


        void DrawCenteredLabel(string s) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(s);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        void DrawWarningLabel(string s) {
            if (warningLabelStyle == null)
                warningLabelStyle = new GUIStyle(GUI.skin.label);
            warningLabelStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.22f, 0.36f, 0.6f);
            warningLabelStyle.wordWrap = true;
            GUILayout.Label(s, warningLabelStyle);
        }

        bool CheckSelectedCountry() {
            if (_editor.countryIndex >= 0)
                return true;
            EditorUtility.DisplayDialog("Tool", "Please select a country first.", "Ok");
            return false;
        }

        bool CheckSelectedProvince() {
            if (_editor.provinceIndex >= 0)
                return true;
            EditorUtility.DisplayDialog("Tool", "Please select a province first.", "Ok");
            return false;
        }

        void DrawEditorProvinceNames() {
            if (!_editor.showProvinceNames || _editor.highlightedRegions == null)
                return;
            Transform mapTransform = _map.transform;
            int hCount = _editor.highlightedRegions.Count;
            for (int p = 0; p < hCount; p++) {
                Region region = _editor.highlightedRegions[p];
                if (region != null && region.regionIndex == region.entity.mainRegionIndex) {
                    Vector3 regionCenter = mapTransform.TransformPoint(region.center);
                    if (labelsStyle != null && region.entity != null && region.entity.name != null) {
                        Handles.Label(regionCenter, region.entity.name, labelsStyle);
                    }
                }
            }
        }

        void DrawCellNumbers() {
            if (!_map.showGrid || _editor.cityIndex < 0)
                return;

            Transform mapTransform = _map.transform;
            int cellIndex = _map.GetCellIndex(_editor.map.cities[_editor.cityIndex].unity2DLocation);
            if (cellIndex >= 0) {
                Cell cell = _map.cells[cellIndex];
                int r0 = cell.row - 2;
                int r1 = cell.row + 2;
                int c0 = cell.column - 2;
                int c1 = cell.column + 2;
                for (int r = r0; r <= r1; r++) {
                    if (r < 0 || r >= _map.gridRows)
                        continue;
                    for (int c = c0; c <= c1; c++) {
                        if (c < 0 || c >= _map.gridColumns)
                            continue;
                        cellIndex = _map.GetCellIndex(r, c);
                        if (cellIndex >= 0) {
                            cell = _map.cells[cellIndex];
                            Vector2 pos = cell.center;
                            pos.y = (cell.center.y + cell.rect2D.yMax) * 0.5f;
                            Vector3 cellCenter = mapTransform.TransformPoint(pos);
                            Handles.Label(cellCenter, "r" + cell.row + " c" + cell.column + " " + cellIndex, labelsStyle);
                        }
                    }
                }
            }
        }

        void CheckScale() {
            if (EditorPrefs.HasKey(EDITORPREF_SCALE_WARNED))
                return;
            EditorPrefs.SetBool(EDITORPREF_SCALE_WARNED, true);
            if (_editor.editingCountryFile == EDITING_COUNTRY_FILE.COUNTRY_HIGHDEF && _map.transform.localScale.x < 2000) {
                EditorUtility.DisplayDialog("Tip", "Change the scale of the map gameobject to (X=2000,Y=1000,Z=1) to zoom and make selections easier.\n\nWhen you finish editing the map, remember to set its scale back to the original values (default scale is X=200, Y=100, Z=1).", "Good idea!");
            }
        }

        #endregion

        #region Map transform tools

        Country[] backupCountries;
        Province[] backupProvinces;
        List<City> backupCities;
        List<MountPoint> backupMountPoints;
        Vector2 backupTextureScale, backupTextureOffset;

        void MapTransformMakeBackup() {
            backupTextureOffset = _map.earthTextureOffset;
            backupTextureScale = _map.earthTextureScale;

            if (_map.countries != null) {
                backupCountries = new Country[_map.countries.Length];
                for (int k = 0; k < _map.countries.Length; k++) {
                    backupCountries[k] = _map.countries[k].Clone();
                }
            }
            if (_map.provinces != null) {
                backupProvinces = new Province[_map.provinces.Length];
                for (int k = 0; k < _map.provinces.Length; k++) {
                    Province prov = _map.provinces[k];
                    if (prov.regions == null) {
                        _map.ReadProvincePackedString(prov);
                    }
                    backupProvinces[k] = _map.provinces[k].Clone();
                }
            }
            if (_map.cities != null) {
                int cityCount = _map.cities.Length;
                backupCities = new List<City>(cityCount);
                for (int k = 0; k < cityCount; k++) {
                    backupCities.Add(_map.cities[k].Clone());
                }
            }
            if (_map.mountPoints != null) {
                int mpCount = _map.mountPoints.Count;
                backupMountPoints = new List<MountPoint>(mpCount);
                for (int k = 0; k < mpCount; k++) {
                    backupMountPoints.Add(_map.mountPoints[k].Clone());
                }
            }
        }

        void MapTransformApply() {
            _editor.ClearSelection();
            if (backupCountries == null) {
                MapTransformMakeBackup();
            }
            if (_map.countries != null) {
                for (int c = 0; c < _map.countries.Length; c++) {
                    Country country = _map.countries[c];
                    Country backupCountry = backupCountries[c];
                    country.center = TransformPoint(backupCountry.center);
                    if (country.regions == null) continue;
                    int regionsCount = country.regions.Count;
                    for (int r = 0; r < regionsCount; r++) {
                        Region region = country.regions[r];
                        if (region == null) continue;
                        Region backupRegion = backupCountry.regions[r];
                        TransformPoint(ref region.center, ref backupRegion.center);
                        for (int p = 0; p < region.points.Length; p++) {
                            TransformPoint(ref region.points[p], ref backupRegion.points[p]);
                        }
                    }
                }
                _editor.countryChanges = true;
            }
            if (_editor.editingMode == EDITING_MODE.PROVINCES && _map.provinces != null) {
                for (int c = 0; c < _map.provinces.Length; c++) {
                    Province province = _map.provinces[c];
                    Province backupProvince = backupProvinces[c];
                    province.center = TransformPoint(backupProvince.center);
                    if (province.regions == null) {
                        _map.ReadProvincePackedString(province);
                    }
                    if (province.regions == null) continue;
                    int regionsCount = province.regions.Count;
                    for (int r = 0; r < regionsCount; r++) {
                        Region region = province.regions[r];
                        if (region == null) continue;
                        Region backupRegion = backupProvince.regions[r];
                        TransformPoint(ref region.center, ref backupRegion.center);
                        for (int p = 0; p < region.points.Length; p++) {
                            TransformPoint(ref region.points[p], ref backupRegion.points[p]);
                        }
                    }
                }
                _editor.provinceChanges = true;
            }
            if (_map.cities != null) {
                int cityCount = _map.cities.Length;
                if (cityCount > 0) {
                    for (int c = 0; c < cityCount; c++) {
                        City city = _map.cities[c];
                        City backupCity = backupCities[c];
                        TransformPoint(ref city.unity2DLocation, ref backupCity.unity2DLocation);
                    }
                    _editor.cityChanges = true;
                }
            }
            if (_map.mountPoints != null) {
                int mountpointsCount = _map.mountPoints.Count;
                if (mountpointsCount > 0) {
                    for (int c = 0; c < mountpointsCount; c++) {
                        MountPoint mp = _map.mountPoints[c];
                        MountPoint backupMp = backupMountPoints[c];
                        TransformPoint(ref mp.unity2DLocation, ref backupMp.unity2DLocation);
                    }
                    _editor.mountPointChanges = true;
                }
            }
            _map.earthTextureScale = mapScale;
            _map.earthTextureOffset = mapOffset;
            _map.Redraw(true);
        }

        void TransformPoint(ref Vector2 p, ref Vector2 o) {
            p.x = (o.x + 0.5f + mapOffset.x) * mapScale.x - 0.5f;
            p.y = (o.y + 0.5f + mapOffset.y) * mapScale.y - 0.5f;
        }

        Vector2 TransformPoint(Vector2 o) {
            Vector2 p;
            p.x = (o.x + 0.5f + mapOffset.x) * mapScale.x - 0.5f;
            p.y = (o.y + 0.5f + mapOffset.y) * mapScale.y - 0.5f;
            return p;
        }

        void MapTransformReset() {
            backupCountries = null;
            mapScale = Misc.Vector2one;
            mapOffset = Misc.Vector2zero;
            _map.earthTextureScale = backupTextureScale;
            _map.earthTextureOffset = backupTextureOffset;

        }

        void MapTransformCrop() {
            for (int t = 0; t < 1000; t++) {
                bool changes = false;
                for (int k = 0; k < _map.countries.Length; k++) {
                    Country country = _map.countries[k];
                    if (country.center.x < -0.5f || country.center.x > 0.5f || country.center.y < -0.5f || country.center.y > 0.5f) {
                        _map.CountryDelete(k, true, false);
                        changes = true;
                        break;
                    }
                    // Check if any region is outside of area
                    if (country.regions == null) continue;
                    int regionCount = country.regions.Count;
                    for (int r = 0; r < regionCount; r++) {
                        Region region = country.regions[r];
                        if (region.center.x < -0.5f || region.center.x > 0.5f || region.center.y < -0.5f || region.center.y > 0.5f) {
                            country.regions.RemoveAt(r);
                            r--;
                            regionCount--;
                            changes = true;
                        }
                    }
                    if (changes) {
                        if (country.regions.Count == 0) {
                            _map.CountryDelete(k, true, false);
                        } else {
                            _map.RefreshCountryGeometry(country);
                        }
                        break;
                    }
                }
                if (!changes) break;
            }
            _map.Redraw(true);
        }

        #endregion

    }

}