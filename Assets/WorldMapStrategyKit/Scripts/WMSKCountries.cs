// World Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using WorldMapStrategyKit.ClipperLib;

namespace WorldMapStrategyKit {

    public enum FRONTIERS_DETAIL {
        Low = 0,
        High = 1
    }

    public enum OUTLINE_DETAIL {
        Simple = 0,
        Textured = 1
    }

    public enum TEXT_ENGINE {
        TextMeshStandard = 0,
        TextMeshPro = 1
    }

    public enum TEXT_RENDERING_MODE {
        BlendedWithWorldTexture = 0,
        FloatingAboveViewport = 1
    }

    public delegate void OnCountryEvent(int countryIndex, int regionIndex);
    public delegate void OnCountryClickEvent(int countryIndex, int regionIndex, int buttonIndex);
    public delegate void OnCountryHighlightEvent(int countryIndex, int regionIndex, ref bool allowHighlight);

    public partial class WMSK : MonoBehaviour {

        #region Public properties

        public const string COUNTRY_POOL_NAME = "Pool";


        Country[] _countries;

        /// <summary>
        /// Complete array of countries and the continent name they belong to.
        /// </summary>
        public Country[] countries {
            get { return _countries; }
            set {
                _countries = value;
                lastCountryLookupCount = -1;
            }
        }

        Country _countryHighlighted;

        /// <summary>
        /// Returns Country under mouse position or null if none.
        /// </summary>
        public Country countryHighlighted { get { return _countryHighlighted; } }

        int _countryHighlightedIndex = -1;

        /// <summary>
        /// Returns currently highlighted country index in the countries list.
        /// </summary>
        public int countryHighlightedIndex { get { return _countryHighlightedIndex; } }

        Region _countryRegionHighlighted;

        /// <summary>
        /// Returns currently highlightd country's region.
        /// </summary>
        /// <value>The country region highlighted.</value>
        public Region countryRegionHighlighted { get { return _countryRegionHighlighted; } }

        int _countryRegionHighlightedIndex = -1;

        /// <summary>
        /// Returns currently highlighted region of the country.
        /// </summary>
        public int countryRegionHighlightedIndex { get { return _countryRegionHighlightedIndex; } }

        int _countryLastClicked = -1;

        /// <summary>
        /// Returns the last clicked country.
        /// </summary>
        public int countryLastClicked { get { return _countryLastClicked; } }

        int _countryRegionLastClicked = -1;

        /// <summary>
        /// Returns the last clicked country region index.
        /// </summary>
        public int countryRegionLastClicked { get { return _countryRegionLastClicked; } }

        /// <summary>
        /// Gets the country region's highlighted shape.
        /// </summary>
        public GameObject countryRegionHighlightedShape { get { return countryRegionHighlightedObj; } }

        int _countryLastOver = -1;

        /// <summary>
        /// Returns the last hovered country.
        /// </summary>
        public int countryLastOver { get { return _countryLastOver; } }

        int _countryRegionLastOver = -1;

        /// <summary>
        /// Returns the last hovered country region index.
        /// </summary>
        public int countryRegionLastOver { get { return _countryRegionLastOver; } }

        public event OnCountryEvent OnCountryEnter;
        public event OnCountryEvent OnCountryExit;
        public event OnCountryClickEvent OnCountryClick;
        public event OnCountryHighlightEvent OnCountryHighlight;

        /// <summary>
        /// Enable/disable country highlight when mouse is over.
        /// </summary>
        [SerializeField]
        bool
            _enableCountryHighlight = true;

        public bool enableCountryHighlight {
            get {
                return _enableCountryHighlight;
            }
            set {
                if (_enableCountryHighlight != value) {
                    _enableCountryHighlight = value;
                    isDirty = true;
                }
            }
        }

        /// <summary>
        /// Set whether all regions of active country should be highlighted.
        /// </summary>
        [SerializeField]
        bool
            _highlightAllCountryRegions;

        public bool highlightAllCountryRegions {
            get {
                return _highlightAllCountryRegions;
            }
            set {
                if (_highlightAllCountryRegions != value) {
                    _highlightAllCountryRegions = value;
                    DestroySurfaces();
                    isDirty = true;
                }
            }
        }


        /// <summary>
        /// When enabled, surface meshes belonging to a country will be combined when calling ToggleCountrySurface, reducing draw calls.
        /// </summary>
        [SerializeField]
        bool
            _combineCountrySurfaces;

        public bool combineCountrySurfaces {
            get {
                return _combineCountrySurfaces;
            }
            set {
                if (_combineCountrySurfaces != value) {
                    _combineCountrySurfaces = value;
                    DestroySurfaces();
                    isDirty = true;
                }
            }
        }


        public bool combineCountrySurfacesActive => _combineCountrySurfaces && isPlaying;

        /// <summary>
        /// Set whether country highlight will switch the color to highlight color.
        /// </summary>
        [SerializeField]
        bool
            _highlightCountryRecolor = true;

        public bool highlightCountryRecolor {
            get {
                return _highlightCountryRecolor;
            }
            set {
                if (_highlightCountryRecolor != value) {
                    _highlightCountryRecolor = value;
                    isDirty = true;
                }
            }
        }


        /// <summary>
        /// Set whether the highlight effect must preserve existing country texture
        /// </summary>
        [SerializeField]
        bool _highlightCountryKeepTexture = true;

        public bool highlightCountryKeepTexture {
            get {
                return _highlightCountryKeepTexture;
            }
            set {
                if (_highlightCountryKeepTexture != value) {
                    _highlightCountryKeepTexture = value;
                    isDirty = true;
                }
            }
        }




        [SerializeField]
        float
            _highlightMaxScreenAreaSize = 1f;

        /// <summary>
        /// Defines the maximum area of a highlighted country or province. To prevent filling the whole screen with the highlight color, you can reduce this value and if the highlighted screen area size is greater than this factor (1=whole screen) the country won't be filled (it will behave as selected though)
        /// </summary>
        public float highlightMaxScreenAreaSize {
            get {
                return _highlightMaxScreenAreaSize;
            }
            set {
                if (_highlightMaxScreenAreaSize != value) {
                    _highlightMaxScreenAreaSize = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool
            _showFrontiers = true;

        /// <summary>
        /// Toggle frontiers visibility.
        /// </summary>
        public bool showFrontiers {
            get {
                return _showFrontiers;
            }
            set {
                if (value != _showFrontiers) {
                    _showFrontiers = value;
                    isDirty = true;

                    if (frontiersLayer != null) {
                        frontiersLayer.SetActive(_showFrontiers);
                    } else if (_showFrontiers) {
                        DrawFrontiers();
                    }
                }
            }
        }


        [SerializeField]
        bool
            _frontiersCoastlines = true;

        /// <summary>
        /// Include coasts in frontier lines
        /// </summary>
        public bool frontiersCoastlines {
            get {
                return _frontiersCoastlines;
            }
            set {
                if (value != _frontiersCoastlines) {
                    _frontiersCoastlines = value;
                    isDirty = true;
                    needOptimizeFrontiers = true;
                    DrawFrontiers();
                }
            }
        }

        /// <summary>
        /// Fill color to use when the mouse hovers a country's region.
        /// </summary>
        [SerializeField]
        Color
            _fillColor = new Color(1, 0, 0, 0.7f);

        public Color fillColor {
            get {
                if (hudMatCountry != null) {
                    return hudMatCountry.color;
                } else {
                    return _fillColor;
                }
            }
            set {
                if (value != _fillColor) {
                    _fillColor = value;
                    isDirty = true;
                    if (hudMatCountry != null && _fillColor != hudMatCountry.color) {
                        hudMatCountry.color = _fillColor;
                    }
                }
            }
        }

        /// <summary>
        /// Inner Color for country frontiers.
        /// </summary>
        [SerializeField]
        Color
            _frontiersColor = Color.green;

        public Color frontiersColor {
            get {
                if (frontiersMat != null) {
                    return frontiersMat.color;
                } else {
                    return _frontiersColor;
                }
            }
            set {
                if (value != _frontiersColor) {
                    _frontiersColor = value;
                    isDirty = true;

                    if (frontiersMat != null && _frontiersColor != frontiersMat.color) {
                        frontiersMat.color = _frontiersColor;
                    }
                }
            }
        }


        /// <summary>
        /// Outer color for country frontiers.
        /// </summary>
        [SerializeField]
        Color
            _frontiersColorOuter = new Color(0, 1, 0, 0.5f);

        public Color frontiersColorOuter {
            get {
                return _frontiersColorOuter;
            }
            set {
                if (value != _frontiersColorOuter) {
                    _frontiersColorOuter = value;
                    isDirty = true;

                    if (frontiersMat != null) {
                        frontiersMat.SetColor("_OuterColor", _frontiersColorOuter);
                    }
                }
            }
        }

        [SerializeField]
        bool
            _thickerFrontiers;

        /// <summary>
        /// Enable alternate frontiers shader.
        /// </summary>
        public bool thickerFrontiers {
            get {
                return _thickerFrontiers;
            }
            set {
                if (value != _thickerFrontiers) {
                    _thickerFrontiers = value;
                    isDirty = true;
                    UpdateFrontiersMaterial();
                    Redraw();
                }
            }
        }

        [SerializeField]
        bool
            _frontiersDynamicWidth = true;

        /// <summary>
        /// Enable dynamic width of country frontiers when zooming in/out
        /// </summary>
        public bool frontiersDynamicWidth {
            get {
                return _frontiersDynamicWidth;
            }
            set {
                if (value != _frontiersDynamicWidth) {
                    _frontiersDynamicWidth = value;
                    isDirty = true;
                    UpdateFrontiersMaterial();
                    Redraw();
                }
            }
        }

        [SerializeField]
        float _frontiersWidth = 0.05f;

        public float frontiersWidth {
            get {
                return _frontiersWidth;
            }
            set {
                if (value != _frontiersWidth) {
                    _frontiersWidth = value;
                    isDirty = true;
                    if (frontiersMat != null) {
                        frontiersMat.SetFloat(ShaderParams.Thickness, _frontiersWidth);
                    }
                }
            }
        }


        [SerializeField]
        int _frontiersMaxPixelWidth = 50;

        public int frontiersMaxPixelWidth {
            get {
                return _frontiersMaxPixelWidth;
            }
            set {
                if (value != _frontiersMaxPixelWidth) {
                    _frontiersMaxPixelWidth = value;
                    isDirty = true;
                    if (frontiersMat != null) {
                        frontiersMat.SetFloat(ShaderParams.MaxPixelWidth, _frontiersMaxPixelWidth);
                    }
                }
            }
        }


        [SerializeField]
        bool
            _showOutline = true;

        /// <summary>
        /// Toggle frontiers thicker outline visibility.
        /// </summary>
        public bool showOutline {
            get {
                return _showOutline;
            }
            set {
                if (value != _showOutline) {
                    _showOutline = value;
                    Redraw(); // recreate surfaces layer
                    isDirty = true;
                }
            }
        }

        /// <summary>
        /// Color for country frontiers outline.
        /// </summary>
        [SerializeField]
        Color
            _outlineColor = Color.black;

        public Color outlineColor {
            get {
                if (outlineMat != null) {
                    return outlineMat.color;
                } else {
                    return _outlineColor;
                }
            }
            set {
                if (value != _outlineColor) {
                    _outlineColor = value;
                    isDirty = true;

                    if (outlineMat != null && _outlineColor != outlineMat.color) {
                        outlineMat.color = _outlineColor;
                    }
                }
            }
        }

        [SerializeField]
        OUTLINE_DETAIL
            _outlineDetail = OUTLINE_DETAIL.Simple;

        /// <summary>
        /// Quality level for outline.
        /// </summary>
        public OUTLINE_DETAIL outlineDetail {
            get {
                return _outlineDetail;
            }
            set {
                if (value != _outlineDetail) {
                    _outlineDetail = value;
                    Redraw(); // recreate surfaces layer
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        Texture2D
            _outlineTexture;

        /// <summary>
        /// Texture for the outline when outlineDetail is set to Textured.
        /// </summary>
        public Texture2D outlineTexture {
            get {
                return _outlineTexture;
            }
            set {
                if (value != _outlineTexture) {
                    _outlineTexture = value;
                    if (outlineMatTextured != null)
                        outlineMatTextured.mainTexture = _outlineTexture;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _outlineWidth = 0.1f;

        public float outlineWidth {
            get {
                return _outlineWidth;
            }
            set {
                if (value != _outlineWidth) {
                    _outlineWidth = value;
                    Redraw(); // recreate surfaces layer
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _outlineTilingScale = 1f;

        public float outlineTilingScale {
            get {
                return _outlineTilingScale;
            }
            set {
                if (value != _outlineWidth) {
                    _outlineTilingScale = value;
                    if (outlineMatTextured != null)
                        outlineMatTextured.mainTextureScale = new Vector2(_outlineTilingScale, 1f);
                    isDirty = true;
                }
            }
        }


        [SerializeField, Range(-5f, 5f)]
        float _outlineAnimationSpeed = -1f;

        public float outlineAnimationSpeed {
            get {
                return _outlineAnimationSpeed;
            }
            set {
                if (value != _outlineAnimationSpeed) {
                    _outlineAnimationSpeed = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        FRONTIERS_DETAIL
            _frontiersDetail = FRONTIERS_DETAIL.Low;

        public FRONTIERS_DETAIL frontiersDetail {
            get { return _frontiersDetail; }
            set {
                if (_frontiersDetail != value) {
                    _frontiersDetail = value;
                    isDirty = true;
                    ReloadData();
                    Redraw();
                }
            }
        }



        [SerializeField]
        bool
            _showCountryNames = false;

        public bool showCountryNames {
            get {
                return _showCountryNames;
            }
            set {
                if (value != _showCountryNames) {
                    _showCountryNames = value;
                    isDirty = true;
                    if (textRoot != null) {
                        textRoot.SetActive(_showCountryNames);
                    } else {
                        DrawMapLabels();
                    }
                }
            }
        }

        [SerializeField]
        TEXT_ENGINE _countryLabelsTextEngine = TEXT_ENGINE.TextMeshStandard;

        public TEXT_ENGINE countryLabelsTextEngine {
            get {
                return _countryLabelsTextEngine;
            }
            set {
                if (_countryLabelsTextEngine != value) {
                    _countryLabelsTextEngine = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }


        [SerializeField]
        TEXT_RENDERING_MODE _countryLabelsRenderingMode = TEXT_RENDERING_MODE.BlendedWithWorldTexture;

        public TEXT_RENDERING_MODE countryLabelsRenderingMode {
            get {
                return _countryLabelsRenderingMode;
            }
            set {
                if (_countryLabelsRenderingMode != value) {
                    _countryLabelsRenderingMode = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }


        [SerializeField]
        float
            _countryLabelsElevation = 0.5f;

        public float countryLabelsElevation {
            get {
                return _countryLabelsElevation;
            }
            set {
                if (value != _countryLabelsElevation) {
                    _countryLabelsElevation = Math.Max(0, value);
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }


        [SerializeField]
        float
            _countryLabelsLength = 0.8f;

        public float countryLabelsLength {
            get {
                return _countryLabelsLength;
            }
            set {
                if (value != _countryLabelsLength) {
                    _countryLabelsLength = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }

        [SerializeField]
        float
            _countryLabelsHorizontality = 2f;

        public float countryLabelsHorizontality {
            get {
                return _countryLabelsHorizontality;
            }
            set {
                if (value != _countryLabelsHorizontality) {
                    _countryLabelsHorizontality = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }


        [SerializeField]
        float
            _countryLabelsCurvature = 1f;

        public float countryLabelsCurvature {
            get {
                return _countryLabelsCurvature;
            }
            set {
                if (value != _countryLabelsCurvature) {
                    _countryLabelsCurvature = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }

        [SerializeField]
        float
            _countryLabelsAbsoluteMinimumSize = 0.5f;

        public float countryLabelsAbsoluteMinimumSize {
            get {
                return _countryLabelsAbsoluteMinimumSize;
            }
            set {
                if (value != _countryLabelsAbsoluteMinimumSize) {
                    _countryLabelsAbsoluteMinimumSize = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }

        [SerializeField]
        float
            _countryLabelsSize = 0.25f;

        public float countryLabelsSize {
            get {
                return _countryLabelsSize;
            }
            set {
                if (value != _countryLabelsSize) {
                    _countryLabelsSize = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }



        [SerializeField]
        bool _countryLabelsEnableAutomaticFade = true;

        /// <summary>
        /// Automatic fading of country labels depending on camera distance and label screen size
        /// </summary>
        public bool countryLabelsEnableAutomaticFade {
            get { return _countryLabelsEnableAutomaticFade; }
            set {
                if (_countryLabelsEnableAutomaticFade != value) {
                    _countryLabelsEnableAutomaticFade = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }

        [SerializeField]
        float
            _countryLabelsAutoFadeMaxHeight = 0.2f;

        /// <summary>
        /// Max height of a label relative to screen height (0..1) at which fade out starts
        /// </summary>
        public float countryLabelsAutoFadeMaxHeight {
            get {
                return _countryLabelsAutoFadeMaxHeight;
            }
            set {
                if (value != _countryLabelsAutoFadeMaxHeight) {
                    _countryLabelsAutoFadeMaxHeight = value;
                    _countryLabelsAutoFadeMinHeight = Mathf.Min(_countryLabelsAutoFadeMaxHeight, _countryLabelsAutoFadeMinHeight);
                    isDirty = true;
                    FadeCountryLabels();
                }
            }
        }

        [SerializeField]
        float
            _countryLabelsAutoFadeMaxHeightFallOff = 0.5f;

        /// <summary>
        /// Fall off for fade labels when height is greater than min height
        /// </summary>
        public float countryLabelsAutoFadeMaxHeightFallOff {
            get {
                return _countryLabelsAutoFadeMaxHeightFallOff;
            }
            set {
                if (value != _countryLabelsAutoFadeMaxHeightFallOff) {
                    _countryLabelsAutoFadeMaxHeightFallOff = value;
                    isDirty = true;
                    FadeCountryLabels();
                }
            }
        }


        [SerializeField]
        float
            _countryLabelsAutoFadeMinHeight = 0.018f;

        /// <summary>
        /// Min height of a label relative to screen height (0..1) at which fade out starts
        /// </summary>
        public float countryLabelsAutoFadeMinHeight {
            get {
                return _countryLabelsAutoFadeMinHeight;
            }
            set {
                if (value != _countryLabelsAutoFadeMinHeight) {
                    _countryLabelsAutoFadeMinHeight = value;
                    _countryLabelsAutoFadeMaxHeight = Mathf.Max(_countryLabelsAutoFadeMaxHeight, _countryLabelsAutoFadeMinHeight);
                    isDirty = true;
                    FadeCountryLabels();
                }
            }
        }

        [SerializeField]
        float
            _countryLabelsAutoFadeMinHeightFallOff = 0.005f;

        /// <summary>
        /// Fall off for fade labels when height is less than min height
        /// </summary>
        public float countryLabelsAutoFadeMinHeightFallOff {
            get {
                return _countryLabelsAutoFadeMinHeightFallOff;
            }
            set {
                if (value != _countryLabelsAutoFadeMinHeightFallOff) {
                    _countryLabelsAutoFadeMinHeightFallOff = value;
                    isDirty = true;
                    FadeCountryLabels();
                }
            }
        }

        [SerializeField]
        bool
            _showLabelsShadow = true;

        /// <summary>
        /// Draws a shadow under map labels. Specify the color using labelsShadowColor.
        /// </summary>
        /// <value><c>true</c> if show labels shadow; otherwise, <c>false</c>.</value>
        public bool showLabelsShadow {
            get {
                return _showLabelsShadow;
            }
            set {
                if (value != _showLabelsShadow) {
                    _showLabelsShadow = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }

        [SerializeField]
        Color
            _countryLabelsColor = Color.white;

        /// <summary>
        /// Color for map labels.
        /// </summary>
        public Color countryLabelsColor {
            get {
                return _countryLabelsColor;
            }
            set {
                if (value != _countryLabelsColor) {
                    _countryLabelsColor = value;
                    isDirty = true;
                    if (gameObject.activeInHierarchy) {
                        if (!isPlaying || _countryLabelsTextEngine != TEXT_ENGINE.TextMeshStandard) {
                            DrawMapLabels();
                        } else {
                            if (labelsFont != null && labelsFont.material != null) {
                                labelsFont.material.color = _countryLabelsColor;
                            } else {
                                DrawMapLabels();
                            }
                        }
                    }
                }
            }
        }


        [SerializeField]
        Color
            _countryLabelsShadowColor = new Color(0, 0, 0, 0.5f);

        /// <summary>
        /// Color for map labels.
        /// </summary>
        public Color countryLabelsShadowColor {
            get {
                return _countryLabelsShadowColor;
            }
            set {
                if (value != _countryLabelsShadowColor) {
                    _countryLabelsShadowColor = value;
                    isDirty = true;
                    if (gameObject.activeInHierarchy) {
                        labelsShadowMaterial.color = _countryLabelsShadowColor;
                    }
                }
            }
        }


        [SerializeField]
        Vector2 _countryLabelsShadowOffset = new Vector2(0.4f, -0.4f);

        /// <summary>
        /// Shadow offset for country labels
        /// </summary>
        public Vector2 countryLabelsShadowOffset {
            get {
                return _countryLabelsShadowOffset;
            }
            set {
                if (value != _countryLabelsShadowOffset) {
                    _countryLabelsShadowOffset = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }


        [SerializeField]
        bool
            _countryLabelsUseCentroid;

        /// <summary>
        /// Use the country centroid to position the label, instead of the geometric center.
        /// </summary>
        public bool countryLabelsUseCentroid {
            get {
                return _countryLabelsUseCentroid;
            }
            set {
                if (value != _countryLabelsUseCentroid) {
                    _countryLabelsUseCentroid = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }


        [SerializeField]
        Font _countryLabelsFont;

        /// <summary>
        /// Gets or sets the default font for country labels
        /// </summary>
        public Font countryLabelsFont {
            get {
                return _countryLabelsFont;
            }
            set {
                if (value != _countryLabelsFont) {
                    _countryLabelsFont = value;
                    isDirty = true;
                    ReloadFont();
                    DrawMapLabels();
                }
            }
        }


        [SerializeField]
        UnityEngine.Object _countryLabelsFontTMPro;

        public UnityEngine.Object countryLabelsFontTMPro {
            get {
                return _countryLabelsFontTMPro;
            }
            set {
                if (_countryLabelsFontTMPro != value) {
                    _countryLabelsFontTMPro = value;
                    ReloadFont();
                    DrawMapLabels();
                }
            }
        }


        [SerializeField]
        Material _countryLabelsFontTMProMaterial;

        public Material countryLabelsFontTMProMaterial {
            get {
                return _countryLabelsFontTMProMaterial;
            }
            set {
                if (_countryLabelsFontTMProMaterial != value) {
                    _countryLabelsFontTMProMaterial = value;
                    ReloadFont();
                    DrawMapLabels();
                }
            }
        }

        [SerializeField]
        Color
            _countryLabelsOutlineColor = Color.black;

        /// <summary>
        /// Color for the label outline. Only used with TextMesh Pro text engine.
        /// </summary>
        public Color countryLabelsOutlineColor {
            get {
                return _countryLabelsOutlineColor;
            }
            set {
                if (value != _countryLabelsOutlineColor) {
                    _countryLabelsOutlineColor = value;
                    isDirty = true;
                    if (gameObject.activeInHierarchy) {
                        if (_countryLabelsTextEngine == TEXT_ENGINE.TextMeshPro) {
                            DrawMapLabels();
                        }
                    }
                }
            }
        }

        [SerializeField]
        float
            _countryLabelsOutlineWidth = 0.1f;

        /// <summary>
        /// Width for the label outline. Only used with TextMesh Pro text engine.
        /// </summary>
        public float countryLabelsOutlineWidth {
            get {
                return _countryLabelsOutlineWidth;
            }
            set {
                if (value != _countryLabelsOutlineWidth) {
                    _countryLabelsOutlineWidth = value;
                    isDirty = true;
                    if (gameObject.activeInHierarchy) {
                        if (_countryLabelsTextEngine == TEXT_ENGINE.TextMeshPro) {
                            DrawMapLabels();
                        }
                    }
                }
            }
        }

        [SerializeField]
        string _countryAttributeFile = COUNTRY_ATTRIB_DEFAULT_FILENAME;

        public string countryAttributeFile {
            get { return _countryAttributeFile; }
            set {
                if (value != _countryAttributeFile) {
                    _countryAttributeFile = value;
                    if (_countryAttributeFile == null)
                        _countryAttributeFile = COUNTRY_ATTRIB_DEFAULT_FILENAME;
                    isDirty = true;
                    ReloadCountryAttributes();
                }
            }
        }


        [SerializeField]
        float
            _labelsElevation = 0.001f;

        /// <summary>
        /// Labels elevation for normal 2D flat mode
        /// </summary>
        public float labelsElevation {
            get {
                return _labelsElevation;
            }
            set {
                if (value != _labelsElevation) {
                    if (value < 0.001f)
                        value = 0.001f;
                    _labelsElevation = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }


        #endregion

        #region Public API area

        /// <summary>
        /// Returns true if the countryIndex is valid (ie. within country array range)
        /// </summary>
        public bool ValidCountryIndex(int countryIndex) {
            return countryIndex >= 0 && countries != null && countryIndex < _countries.Length;
        }

        /// <summary>
        /// Returns true if the countryIndex and regionIndex are valie (ie. within country and country regions array range)
        /// </summary>
        public bool ValidCountryRegionIndex(int countryIndex, int regionIndex) {
            return countryIndex >= 0 && regionIndex >= 0 && countries != null && countryIndex < _countries.Length && _countries[countryIndex].regions != null && regionIndex < _countries[countryIndex].regions.Count;
        }


        /// <summary>
        /// Returns the index of a country in the countries array by its name.
        /// </summary>
        public int GetCountryIndex(string countryName) {
            int countryIndex;
            if (countryLookup != null && countryLookup.TryGetValue(countryName, out countryIndex))
                return countryIndex;
            else
                return -1;
        }

        /// <summary>
        /// Returns the index of a country in the countries collection by its reference.
        /// </summary>
        public int GetCountryIndex(Country country) {
            int countryIndex;
            if (countryLookup != null && country != null && countryLookup.TryGetValue(country.name, out countryIndex))
                return countryIndex;
            else
                return -1;
        }


        /// <summary>
        /// Returns the index of a country in the countries by its FIPS 10 4 code.
        /// </summary>
        public int GetCountryIndexByFIPS10_4(string fips) {
            for (int k = 0; k < _countries.Length; k++) {
                if (_countries[k].fips10_4.Equals(fips)) {
                    return k;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of a country in the countries by its ISO A-2 code.
        /// </summary>
        public int GetCountryIndexByISO_A2(string iso_a2) {
            for (int k = 0; k < _countries.Length; k++) {
                if (_countries[k].iso_a2.Equals(iso_a2)) {
                    return k;
                }
            }
            return -1;
        }


        /// <summary>
        /// Returns the index of a country in the countries by its ISO A-3 code.
        /// </summary>
        public int GetCountryIndexByISO_A3(string iso_a3) {
            for (int k = 0; k < _countries.Length; k++) {
                if (_countries[k].iso_a3.Equals(iso_a3)) {
                    return k;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of a country in the countries by its ISO N-3 code.
        /// </summary>
        public int GetCountryIndexByISO_N3(string iso_n3) {
            for (int k = 0; k < _countries.Length; k++) {
                if (_countries[k].iso_n3.Equals(iso_n3)) {
                    return k;
                }
            }
            return -1;
        }



        /// <summary>
        /// Gets the index of the country region.
        /// </summary>
        /// <returns>The country region index.</returns>
        /// <param name="countryIndex">Country index.</param>
        /// <param name="region">Region.</param>
        public int GetCountryRegionIndex(int countryIndex, Region region) {
            if (countryIndex < 0 || countryIndex >= countries.Length)
                return -1;
            Country country = _countries[countryIndex];
            int rc = country.regions.Count;
            for (int k = 0; k < rc; k++) {
                if (country.regions[k] == region) {
                    return k;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the country object by its name.
        /// </summary>
        public Country GetCountry(string countryName) {
            int countryIndex = GetCountryIndex(countryName);
            return GetCountry(countryIndex);
        }

        /// <summary>
        /// Returns the center of a country
        /// </summary>
        /// <param name="countryIndex"></param>
        /// <returns></returns>
        public Vector2 GetCountryCenter(int countryIndex) {
            if (!CheckCountryIndex(countryIndex)) return Misc.Vector2zero;
            return countries[countryIndex].center;
        }


        /// <summary>
        /// Returns the centroid of a country
        /// </summary>
        public Vector2 GetCountryCentroid(int countryIndex) {
            if (!CheckCountryIndex(countryIndex)) return Misc.Vector2zero;
            return countries[countryIndex].centroid;
        }

        /// <summary>
        /// Returns the centroid of a country
        /// </summary>
        public Vector2 GetCountryCentroid(Country country) {
            if (country == null) return Misc.Vector2zero;
            return country.centroid;
        }


        /// <summary>
        /// Returns the country object by its position.
        /// </summary>
        public Country GetCountry(Vector2 mapPosition) {
            int countryIndex = GetCountryIndex(mapPosition);
            if (countryIndex >= 0)
                return GetCountry(countryIndex);
            return null;
        }

        /// <summary>
        /// Returns the country object by its index. This is same than doing countries[countryIndex] but does a safety check.
        /// </summary>
        public Country GetCountry(int countryIndex) {
            if (countryIndex < 0 || countryIndex >= _countries.Length)
                return null;
            return _countries[countryIndex];
        }

        /// <summary>
        /// Gets the country index with that unique Id.
        /// </summary>
        public int GetCountryIndex(int uniqueId) {
            if (countries == null)
                return -1;
            for (int k = 0; k < _countries.Length; k++) {
                if (_countries[k].uniqueId == uniqueId)
                    return k;
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of the country that contains the provided map coordinates. This will ignore hidden countries.
        /// </summary>
        /// <returns>The country index.</returns>
        /// <param name="localPosition">Map coordinates in the range of (-0.5 .. 0.5)</param>
        public int GetCountryIndex(Vector2 localPosition) {
            // verify if hitPos is inside any country polygon
            int countryCount = countriesOrderedBySize.Count;
            for (int oc = 0; oc < countryCount; oc++) {
                int c = _countriesOrderedBySize[oc];
                Country country = _countries[c];
                if (country.hidden && isPlaying)
                    continue;
                if (!country.regionsRect2D.Contains(localPosition))
                    continue;
                int crCount = country.regions.Count;
                for (int cr = 0; cr < crCount; cr++) {
                    if (country.regions[cr].Contains(localPosition)) {
                        lastRegionIndex = cr;
                        return c;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of the country that contains the provided map coordinates. This will ignore hidden countries.
        /// </summary>
        /// <returns>The country index and regionIndex by reference.</returns>
        /// <param name="localPosition">Map coordinates in the range of (-0.5 .. 0.5)</param>
        public int GetCountryIndex(Vector2 localPosition, out int regionIndex) {
            // verify if hitPos is inside any country polygon
            int countryCount = countriesOrderedBySize.Count;
            for (int oc = 0; oc < countryCount; oc++) {
                int c = _countriesOrderedBySize[oc];
                Country country = _countries[c];
                if (country.hidden)
                    continue;
                if (!country.regionsRect2D.Contains(localPosition))
                    continue;
                int crCount = country.regions.Count;
                for (int cr = 0; cr < crCount; cr++) {
                    if (country.regions[cr].Contains(localPosition)) {
                        regionIndex = cr;
                        return c;
                    }
                }
            }
            regionIndex = -1;
            return -1;
        }

        /// <summary>
        /// Gets the region of the country that contains the provided map coordinates. This will ignore hidden countries.
        /// </summary>
        /// <returns>The Region object.</returns>
        /// <param name="localPosition">Map coordinates in the range of (-0.5 .. 0.5)</param>
        public Region GetCountryRegion(Vector2 localPosition) {
            // verify if hitPos is inside any country polygon
            int countryCount = countriesOrderedBySize.Count;
            for (int oc = 0; oc < countryCount; oc++) {
                int c = _countriesOrderedBySize[oc];
                Country country = _countries[c];
                if (country.hidden)
                    continue;
                if (!country.regionsRect2D.Contains(localPosition))
                    continue;
                for (int cr = 0; cr < country.regions.Count; cr++) {
                    Region region = country.regions[cr];
                    if (region.Contains(localPosition)) {
                        return region;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Gets the region index of the country that contains the provided map coordinates. This will ignore hidden countries.
        /// </summary>
        /// <returns>The Region index or -1 if no region found.</returns>
        /// <param name="localPosition">Map coordinates in the range of (-0.5 .. 0.5)</param>
        public int GetCountryRegionIndex(Vector2 localPosition) {
            Region region = GetCountryRegion(localPosition);
            if (region == null) return -1;
            return region.regionIndex;
        }


        /// <summary>
        /// Returns all neighbour countries
        /// </summary>
        public List<Country> CountryNeighbours(int countryIndex) {

            List<Country> countryNeighbours = new List<Country>();

            // Get country object
            Country country = _countries[countryIndex];

            // Iterate for all regions (a country can have several separated regions)
            for (int countryRegionIndex = 0; countryRegionIndex < country.regions.Count; countryRegionIndex++) {
                Region countryRegion = country.regions[countryRegionIndex];
                List<Country> neighbours = CountryNeighboursOfRegion(countryRegion);
                neighbours.ForEach(c => {
                    if (!countryNeighbours.Contains(c))
                        countryNeighbours.Add(c);
                });
            }

            return countryNeighbours;
        }


        /// <summary>
        /// Get neighbours of the main region of a country
        /// </summary>
        public List<Country> CountryNeighboursOfMainRegion(int countryIndex) {
            // Get main region
            Country country = _countries[countryIndex];
            Region countryRegion = country.regions[country.mainRegionIndex];
            return CountryNeighboursOfRegion(countryRegion);
        }


        /// <summary>
        /// Get neighbours of the currently selected region
        /// </summary>
        public List<Country> CountryNeighboursOfCurrentRegion() {
            return CountryNeighboursOfRegion(countryRegionHighlighted);
        }

        /// <summary>
        /// Get neighbours of a given country region
        /// </summary>
        public List<Country> CountryNeighboursOfRegion(Region countryRegion) {
            List<Country> countryNeighbours = new List<Country>();
            if (countryRegion == null)
                return countryNeighbours;

            // Get the neighbours for this region
            for (int neighbourIndex = 0; neighbourIndex < countryRegion.neighbours.Count; neighbourIndex++) {
                Region neighbour = countryRegion.neighbours[neighbourIndex];
                Country neighbourCountry = (Country)neighbour.entity;
                if (!countryNeighbours.Contains(neighbourCountry)) {
                    countryNeighbours.Add(neighbourCountry);
                }
            }

            // Find neighbours due to enclaves
            if (_enableEnclaves) {
                Country country = (Country)countryRegion.entity;
                for (int c = 0; c < _countries.Length; c++) {
                    Country c2 = _countries[c];
                    if (!country.regionsRect2D.Contains(c2.center) && !c2.regionsRect2D.Contains(country.center))
                        continue;
                    if (c2 == country)
                        continue;
                    if (countryNeighbours.Contains(c2))
                        continue;
                    int crc = c2.regions.Count;
                    for (int cr = 0; cr < crc; cr++) {
                        Region cregion = c2.regions[cr];
                        if (countryRegion.Contains(cregion) || cregion.Contains(countryRegion)) {
                            countryNeighbours.Add(c2);
                            break;
                        }
                    }
                }
            }
            return countryNeighbours;
        }

        /// <summary>
        /// Returns true if two given countries are neighbours
        /// </summary>
        public bool CountryIsNeighbour(int countryIndex, int otherCountryIndex) {
            if (!ValidCountryIndex(countryIndex) || !ValidCountryIndex(otherCountryIndex)) return false;
            Country country = countries[countryIndex];
            Country otherCountry = countries[otherCountryIndex];
            int ocount = otherCountry.neighbours.Length;
            for (int j = 0; j < ocount; j++) {
                if (otherCountry.neighbours[j] == countryIndex) return true;
            }
            return false;
        }

        /// <summary>
        /// Makes one country neighbour of another
        /// </summary>
        public void CountryMakeNeighbours(int countryIndex, int otherCountryIndex) {
            if (countryIndex == otherCountryIndex || !ValidCountryIndex(countryIndex) || !ValidCountryIndex(otherCountryIndex)) return;
            Country country = countries[countryIndex];
            Country otherCountry = countries[otherCountryIndex];
            List<int> newNeighbours = new List<int>(country.neighbours);
            if (!newNeighbours.Contains(otherCountryIndex)) {
                newNeighbours.Add(otherCountryIndex);
                country.neighbours = newNeighbours.ToArray();
            }
            newNeighbours.Clear();
            newNeighbours.AddRange(otherCountry.neighbours);
            if (!newNeighbours.Contains(countryIndex)) {
                newNeighbours.Add(countryIndex);
                otherCountry.neighbours = newNeighbours.ToArray();
            }
        }

        /// <summary>
        /// Returns a list of countries that are visible in the game view
        /// </summary>
        public List<Country> GetVisibleCountries() {
            Camera cam = isPlaying ? currentCamera : Camera.current;
            return GetVisibleCountries(cam);
        }

        /// <summary>
        /// Returns a list of countries that are visible by provided camera
        /// </summary>
        public List<Country> GetVisibleCountries(Camera camera) {
            if (camera == null)
                return null;
            List<Country> vc = new List<Country>(30);
            for (int k = 0; k < _countries.Length; k++) {
                Country country = _countries[k];
                if (country.hidden)
                    continue;

                // Check if country is facing camera
                Vector3 center = transform.TransformPoint(country.center);
                // Check if center of country is inside viewport
                Vector3 vpos = camera.WorldToViewportPoint(center);
                if (vpos.x >= 0 && vpos.x <= 1 && vpos.y >= 0 && vpos.y <= 1) {
                    vc.Add(country);
                } else {
                    // Check if some frontier point is inside viewport
                    Vector2[] frontier = country.regions[country.mainRegionIndex].points;
                    int step = 1 + frontier.Length / 25;
                    for (int p = 0; p < frontier.Length; p += step) {
                        Vector3 pos = transform.TransformPoint(frontier[p]);
                        vpos = camera.WorldToViewportPoint(pos);
                        if (vpos.x >= 0 && vpos.x <= 1 && vpos.y >= 0 && vpos.y <= 1) {
                            vc.Add(country);
                            break;
                        }
                    }
                }
            }
            return vc;
        }

        /// <summary>
        /// Returns a list of countries that are visible inside the window rectangle (rect constraints)
        /// </summary>
        public List<Country> GetVisibleCountriesInWindowRect() {
            List<Country> vc = new List<Country>(30);
            for (int k = 0; k < _countries.Length; k++) {
                Country country = _countries[k];
                if (country.hidden)
                    continue;
                // Check if center of country is inside window rect
                if (_windowRect.Contains(country.center)) {
                    vc.Add(country);
                } else {
                    // Check if some frontier point is inside viewport
                    Vector2[] frontier = country.regions[country.mainRegionIndex].points;
                    int step = 1 + frontier.Length / 25;
                    for (int p = 0; p < frontier.Length; p += step) {
                        if (_windowRect.Contains(frontier[p])) {
                            vc.Add(country);
                            break;
                        }
                    }
                }
            }
            return vc;
        }


        /// <summary>
        /// Returns the zoom level required to show the entire country region on screen
        /// </summary>
        /// <returns>The country zoom level of -1 if error.</returns>
        /// <param name="countryIndex">Country index.</param>
        public float GetCountryRegionZoomExtents(int countryIndex) {
            if (countryIndex < 0 || countries == null || countryIndex >= countries.Length)
                return -1;
            return GetCountryRegionZoomExtents(countryIndex, countries[countryIndex].mainRegionIndex);
        }

        /// <summary>
        /// Returns the zoom level required to show the entire country region on screen
        /// </summary>
        /// <returns>The country zoom level of -1 if error.</returns>
        /// <param name="countryIndex">Country index.</param>
        /// <param name="regionIndex">Region index of the country.</param>
        public float GetCountryRegionZoomExtents(int countryIndex, int regionIndex) {
            if (countryIndex < 0 || countries == null || countryIndex >= countries.Length)
                return -1;
            Country country = countries[countryIndex];
            if (regionIndex < 0 || regionIndex >= country.regions.Count)
                return -1;
            Region region = country.regions[regionIndex];
            return GetFrustumZoomLevel(region.rect2D.width * mapWidth, region.rect2D.height * mapHeight);
        }


        /// <summary>
        /// Returns the zoom level required to show the entire country (including all regions or only the main region) on screen
        /// </summary>
        /// <returns>The country zoom level of -1 if error.</returns>
        /// <param name="countryIndex">Country index.</param>
        /// <param name="onlyMainRegion">If set to true, only the main region will be considered. A value of false (default) ensures entire country including islands fits into the screen.</param>> 
        public float GetCountryZoomExtents(int countryIndex, bool onlyMainRegion = false) {
            if (countryIndex < 0 || countries == null || countryIndex >= countries.Length)
                return -1;
            Country country = _countries[countryIndex];
            Rect rect = onlyMainRegion ? country.mainRegion.rect2D : country.regionsRect2D;
            return GetFrustumZoomLevel(rect.width * mapWidth, rect.height * mapHeight);
        }


        /// <summary>
        /// Renames the country. Name must be unique, different from current and one letter minimum.
        /// </summary>
        /// <returns><c>true</c> if country was renamed, <c>false</c> otherwise.</returns>
        public bool CountryRename(string oldName, string newName) {
            if (newName == null || newName.Length == 0)
                return false;
            int countryIndex = GetCountryIndex(oldName);
            int newCountryIndex = GetCountryIndex(newName);
            if (countryIndex < 0 || newCountryIndex >= 0)
                return false;
            _countries[countryIndex].name = newName;
            // Look for any decorator
            decorator.UpdateDecoratorsCountryName(oldName, newName);
            lastCountryLookupCount = -1;
            return true;

        }


        /// <summary>
        /// Deletes the country. Optionally also delete its dependencies (provinces, cities, mountpoints).
        /// </summary>
        /// <returns><c>true</c> if country was deleted, <c>false</c> otherwise.</returns>
        public bool CountryDelete(int countryIndex, bool deleteDependencies, bool redraw = true) {
            if (internal_CountryDelete(countryIndex, deleteDependencies)) {
                // Update lookup dictionaries
                lastCountryLookupCount = -1;
                return true;
            }
            if (redraw) {
                Redraw();
            }
            return false;
        }



        /// <summary>
        /// Deletes all provinces from a country.
        /// </summary>
        /// <returns><c>true</c>, if provinces where deleted, <c>false</c> otherwise.</returns>
        public bool CountryDeleteProvinces(int countryIndex) {
            int numProvinces = provinces.Length;
            List<Province> newProvinces = new List<Province>(numProvinces);
            for (int k = 0; k < numProvinces; k++) {
                if (provinces[k] != null && provinces[k].countryIndex != countryIndex) {
                    newProvinces.Add(provinces[k]);
                }
            }
            provinces = newProvinces.ToArray();
            return true;
        }


        public void CountriesDeleteFromContinent(string continentName) {

            HideCountryRegionHighlights(true);

            ProvincesDeleteOfSameContinent(continentName);
            CitiesDeleteFromContinent(continentName);
            MountPointsDeleteFromSameContinent(continentName);

            List<Country> newAdmins = new List<Country>(_countries.Length - 1);
            for (int k = 0; k < _countries.Length; k++) {
                if (!_countries[k].continent.Equals(continentName)) {
                    newAdmins.Add(_countries[k]);
                } else {
                    int lastIndex = newAdmins.Count - 1;
                    // Updates country index in provinces
                    if (provinces != null) {
                        for (int p = 0; p < _provinces.Length; p++) {
                            if (_provinces[p].countryIndex > lastIndex) {
                                _provinces[p].countryIndex--;
                            }
                        }
                    }
                    // Updates country index in cities
                    int cityCount = cities.Length;
                    if (cities != null) {
                        for (int c = 0; c < cityCount; c++) {
                            if (_cities[c].countryIndex > lastIndex) {
                                _cities[c].countryIndex--;
                            }
                        }
                    }
                    // Updates country index in mount points
                    if (mountPoints != null) {
                        for (int c = 0; c < mountPoints.Count; c++) {
                            if (mountPoints[c].countryIndex > lastIndex) {
                                mountPoints[c].countryIndex--;
                            }
                        }
                    }
                }
            }

            countries = newAdmins.ToArray();
        }


        /// <summary>
        /// Creates a country and adds it to the country list.
        /// </summary>
        /// <param name="name">Name must be unique!</param>
        /// <param name="continent">Continent.</param>
        public int CountryCreate(string name, string continent) {
            Country newCountry = new Country(name, continent, GetUniqueId(new List<IExtendableAttribute>(countries)));
            return CountryAdd(newCountry);
        }



        /// <summary>
        /// Adds a new country which has been properly initialized. Used by the Map Editor. Name must be unique.
        /// </summary>
        /// <returns><c>country index</c> if country was added, <c>-1</c> otherwise.</returns>
        public int CountryAdd(Country country) {
            int countryIndex = GetCountryIndex(country.name);
            if (countryIndex >= 0 || _countries == null)
                return -1;
            Country[] newCountries = new Country[_countries.Length + 1];
            Array.Copy(_countries, newCountries, _countries.Length);
            countryIndex = newCountries.Length - 1;
            newCountries[countryIndex] = country;
            countries = newCountries;
            // Refresh definition but only if provided country is not empty
            RefreshCountryDefinition(countryIndex, null);
            return countryIndex;
        }

        /// <summary>
        /// Creates a special hidden country that acts as a pool for all provinces in the map.
        /// You can then create new countries from a single province from this pool using ProvinceToCountry function
        /// Or attach a province from the pool to a new country using CountryTransferProvince function
        /// </summary>
        /// <returns>The country index of the new country that acts as the province pool.</returns>
        /// <param name="countryName">A name for this special country (eg: "Pool").</param>
        public int CountryCreateProvincesPool(string countryName, bool removeAllOtherCountries) {
            if (_dontLoadGeodataAtStart) {
                // needs country and province data!
                ReloadData();
            }
            int bgCountryIndex = GetCountryIndex(COUNTRY_POOL_NAME);
            if (bgCountryIndex >= 0)
                return bgCountryIndex;

            Country bgCountry = new Country(countryName, "<Background>", 1);
            bgCountry.hidden = true;
            bgCountry.isPool = true;
            Region dummyRegion = new Region(bgCountry, 0);
            dummyRegion.UpdatePointsAndRect(new Vector2[] {
                new Vector2 (0.5f, 0.5f),
                new Vector2 (0.5f, -0.5f),
                new Vector2 (-0.5f, -0.5f),
                new Vector2 (-0.5f, 0.5f)
            });
            bgCountry.regions.Add(dummyRegion);
            CountryAdd(bgCountry);
            bgCountryIndex = GetCountryIndex(bgCountry);
            if (removeAllOtherCountries) {
                _countries[bgCountryIndex].provinces = provinces;
                for (int k = 0; k < _provinces.Length; k++) {
                    _provinces[k].countryIndex = bgCountryIndex;
                }
                // Delete all countries except the background country
                while (_countries.Length > 1) {
                    CountryDelete(0, false, false);
                }
            }
            return bgCountryIndex;
        }


        /// <summary>
        /// Returns the country index by screen position.
        /// </summary>
        public bool GetCountryIndex(Ray ray, out int countryIndex, out int regionIndex) {
            int hitCount = Physics.RaycastNonAlloc(ray, tempHits, 500, layerMask);
            if (hitCount > 0) {
                int countryCount = countriesOrderedBySize.Count;
                for (int k = 0; k < hitCount; k++) {
                    if (tempHits[k].collider.gameObject == gameObject) {
                        Vector2 localHit = transform.InverseTransformPoint(tempHits[k].point);
                        for (int oc = 0; oc < countryCount; oc++) {
                            int c = _countriesOrderedBySize[oc];
                            Country country = _countries[c];
                            int crCount = country.regions.Count;
                            for (int cr = 0; cr < crCount; cr++) {
                                Region region = country.regions[cr];
                                if (region.Contains(localHit)) {
                                    countryIndex = c;
                                    regionIndex = cr;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            countryIndex = -1;
            regionIndex = -1;
            return false;
        }

        /// <summary>
        /// Starts navigation to target country. Returns false if country is not found.
        /// </summary>
        public bool FlyToCountry(string name) {
            int countryIndex = GetCountryIndex(name);
            if (countryIndex >= 0) {
                FlyToCountry(countryIndex);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Starts navigation to target country. with specified duration, ignoring NavigationTime property.
        /// Set duration to zero to go instantly.
        /// Returns false if country is not found. 
        /// </summary>
        public bool FlyToCountry(string name, float duration) {
            return FlyToCountry(name, duration, GetZoomLevel());
        }

        /// <summary>
        /// Starts navigation to target country. with specified duration and zoom level, ignoring NavigationTime property.
        /// Set duration to zero to go instantly.
        /// Returns false if country is not found. 
        /// </summary>
        public bool FlyToCountry(string name, float duration, float zoomLevel) {
            int countryIndex = GetCountryIndex(name);
            if (countryIndex >= 0) {
                FlyToCountry(countryIndex, duration, zoomLevel);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Starts navigation to target country by index in the countries collection. Returns false if country is not found.
        /// </summary>
        public void FlyToCountry(int countryIndex) {
            FlyToCountry(countryIndex, _navigationTime);
        }

        /// <summary>
        /// Starts navigating to target country by index in the countries collection with specified duration, ignoring NavigationTime property.
        /// Set duration to zero to go instantly.
        /// </summary>
        public void FlyToCountry(int countryIndex, float duration) {
            FlyToCountry(countryIndex, duration, GetZoomLevel());
        }

        /// <summary>
        /// Starts navigating to target country by index in the countries collection with specified duration, ignoring NavigationTime property.
        /// Set duration to zero to go instantly.
        /// </summary>
        public void FlyToCountry(int countryIndex, float duration, float zoomLevel) {
            if (countryIndex < 0 || countryIndex >= countries.Length)
                return;
            SetDestination(_countries[countryIndex].center, duration, zoomLevel);
        }

        /// <summary>
        /// Colorize all regions of specified country by name. Returns false if not found.
        /// </summary>
        public bool ToggleCountrySurface(string name, bool visible, Color color) {
            int countryIndex = GetCountryIndex(name);
            if (countryIndex >= 0) {
                ToggleCountrySurface(countryIndex, visible, color);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Iterates for the countries list and colorizes those belonging to specified continent name.
        /// </summary>
        public void ToggleContinentSurface(string continentName, bool visible, Color color) {
            for (int colorizeIndex = 0; colorizeIndex < _countries.Length; colorizeIndex++) {
                if (_countries[colorizeIndex].continent.Equals(continentName)) {
                    ToggleCountrySurface(_countries[colorizeIndex].name, visible, color);
                }
            }
        }

        /// <summary>
        /// Uncolorize/hide specified countries beloning to a continent.
        /// </summary>
        public void HideContinentSurface(string continentName) {
            for (int colorizeIndex = 0; colorizeIndex < _countries.Length; colorizeIndex++) {
                if (_countries[colorizeIndex].continent.Equals(continentName)) {
                    HideCountrySurface(colorizeIndex);
                }
            }
        }

        /// <summary>
        /// Colorize all regions of specified country by index in the countries collection.
        /// </summary>
        public bool ToggleCountrySurface(int countryIndex, bool visible, Color color) {
            return ToggleCountrySurface(countryIndex, visible, color, null, Misc.Vector2one, Misc.Vector2zero, 0, false);
        }

        /// <summary>
        /// Colorize all regions of specified country and assings a texture.
        /// </summary>
        public bool ToggleCountrySurface(int countryIndex, bool visible, Texture2D texture, bool applyTextureToAllRegions = false) {
            return ToggleCountrySurface(countryIndex, visible, Misc.ColorWhite, texture, Misc.Vector2one, Misc.Vector2zero, 0, applyTextureToAllRegions);
        }

        /// <summary>
        /// Colorize all regions of specified country and assings a color and texture.
        /// </summary>
        public bool ToggleCountrySurface(int countryIndex, bool visible, Color color, Texture2D texture, bool applyTextureToAllRegions = false) {
            return ToggleCountrySurface(countryIndex, visible, color, texture, Misc.Vector2one, Misc.Vector2zero, 0, applyTextureToAllRegions);
        }


        readonly List<MeshFilter> mfs = new List<MeshFilter>();

        /// <summary>
        /// Colorize all regions of specified country and assings a texture.
        /// </summary>
        /// <param name="countryIndex">Country index.</param>
        /// <param name="visible">If set to <c>true</c> visible.</param>
        /// <param name="color">Color.</param>
        /// <param name="texture">Texture.</param>
        /// <param name="textureScale">Texture scale.</param>
        /// <param name="textureOffset">Texture offset.</param>
        /// <param name="textureRotation">Texture rotation.</param>
        /// <param name="applyTextureToAllRegions">If set to <c>true</c> the texture will be applied to all regions, otherwise only the main region will get the texture and the remaining regions will get the color.</param>
        public bool ToggleCountrySurface(int countryIndex, bool visible, Color color, Texture2D texture, Vector2 textureScale, Vector2 textureOffset, float textureRotation, bool applyTextureToAllRegions) {
            if (!ValidCountryIndex(countryIndex)) return false;
            if (!visible) {
                HideCountrySurface(countryIndex);
                return true;
            }

            if (combineCountrySurfacesActive) applyTextureToAllRegions = true;

            Country country = _countries[countryIndex];
            int rCount = country.regions.Count;

            Region mainRegion = country.mainRegion;
            if (mainRegion == null) {
                return false;
            }

            GameObject mainSurface = internal_ToggleCountryRegionSurface(countryIndex, country.mainRegionIndex, visible, color, texture, textureScale, textureOffset, textureRotation);
            if (mainSurface == null || mainRegion.surfaceisMerged) {
                return false;
            }

            MeshFilter mf = mainSurface.GetComponent<MeshFilter>();
            if (mf == null) {
                return false;
            }
            mfs.Clear();
            mfs.Add(mf);

            int totalVertexCount = mf.sharedMesh.vertexCount;

            for (int r = 0; r < rCount; r++) {
                if (r == country.mainRegionIndex) continue;
                GameObject surf;
                if (applyTextureToAllRegions) {
                    surf = internal_ToggleCountryRegionSurface(countryIndex, r, visible, color, texture, textureScale, textureOffset, textureRotation);
                } else {
                    surf = internal_ToggleCountryRegionSurface(countryIndex, r, visible, color);
                }
                if (combineCountrySurfacesActive && surf != null) {
                    mf = surf.GetComponent<MeshFilter>();
                    if (mf != null && mf.sharedMesh.vertexCount + totalVertexCount < 65000) {
                        country.regions[r].surfaceisMerged = true;
                        totalVertexCount += mf.sharedMesh.vertexCount;
                        mfs.Add(mf);
                    }
                } else {
                    country.regions[r].surfaceisMerged = false;
                }
            }

            int meshesCount = mfs.Count;
            if (combineCountrySurfacesActive && meshesCount > 1) {
                CombineInstance[] combine = new CombineInstance[meshesCount];

                for (int i = 0; i < meshesCount; i++) {
                    combine[i].mesh = mfs[i].sharedMesh;
                    mfs[i].gameObject.SetActive(false);
                }

                Mesh mesh = new Mesh();
                mesh.CombineMeshes(combine, true, false, false);
                mainSurface.GetComponent<MeshFilter>().sharedMesh = mesh;
                mainSurface.SetActive(true);

                country.mainRegion.surfaceisMerged = true;
                country.mainRegion.surfaceIsCombinedRegions = true;
            }

            return true;
        }

        /// <summary>
        /// Uncolorize/hide specified country by index in the countries collection.
        /// </summary>
        public void HideCountrySurface(int countryIndex) {
            int rCount = _countries[countryIndex].regions.Count;
            for (int r = 0; r < rCount; r++) {
                HideCountryRegionSurface(countryIndex, r);
            }
        }

        /// <summary>
        /// Highlights the country region specified.
        /// Internally used by the Editor component, but you can use it as well to temporarily mark a country region.
        /// </summary>
        /// <param name="refreshGeometry">Pass true only if you're sure you want to force refresh the geometry of the highlight (for instance, if the frontiers data has changed). If you're unsure, pass false.</param>
        public GameObject ToggleCountryRegionSurfaceHighlight(int countryIndex, int regionIndex, Color color, bool drawOutline) {
            if (!ValidCountryRegionIndex(countryIndex, regionIndex)) return null;
            Material mat = Instantiate(hudMatCountry);
            if (disposalManager != null)
                disposalManager.MarkForDisposal(mat);
            mat.color = color;
            mat.renderQueue--;

            Region region = countries[countryIndex].regions[regionIndex];
            GameObject surf = region.surface;

            if (surf != null) {
                surf.SetActive(true);
                surf.GetComponent<Renderer>().sharedMaterial = mat;
            } else {
                surf = GenerateCountryRegionSurface(countryIndex, regionIndex, mat, Misc.Vector2one, Misc.Vector2zero, 0);
            }
            return surf;
        }

        /// <summary>
        /// Colorize main region of a country by index in the countries collection.
        /// </summary>
        public GameObject ToggleCountryMainRegionSurface(int countryIndex, bool visible, Color color = default(Color)) {
            return ToggleCountryMainRegionSurface(countryIndex, visible, color, null, Misc.Vector2zero, Misc.Vector2zero, 0);
        }

        /// <summary>
        /// Add texture to main region of a country by index in the countries collection.
        /// </summary>
        /// <param name="texture">Optional texture or null to colorize with single color</param>
        public GameObject ToggleCountryMainRegionSurface(int countryIndex, bool visible, Texture2D texture) {
            return ToggleCountryRegionSurface(countryIndex, _countries[countryIndex].mainRegionIndex, visible, Color.white, texture, Misc.Vector2one, Misc.Vector2zero, 0);
        }


        /// <summary>
        /// Colorize main region of a country by index in the countries collection.
        /// </summary>
        /// <param name="texture">Optional texture or null to colorize with single color</param>
        public GameObject ToggleCountryMainRegionSurface(int countryIndex, bool visible, Color color, Texture2D texture, Vector2 textureScale, Vector2 textureOffset, float textureRotation) {
            if (!ValidCountryIndex(countryIndex)) return null;
            return ToggleCountryRegionSurface(countryIndex, _countries[countryIndex].mainRegionIndex, visible, color, texture, textureScale, textureOffset, textureRotation);
        }

        public GameObject ToggleCountryRegionSurface(int countryIndex, int regionIndex, bool visible, Color color = default(Color)) {
            return ToggleCountryRegionSurface(countryIndex, regionIndex, visible, color, null, Misc.Vector2one, Misc.Vector2zero, 0);
        }

        /// <summary>
        /// Colorize specified region of a country by indexes.
        /// </summary>
        public GameObject ToggleCountryRegionSurface(int countryIndex, int regionIndex, bool visible, Color color, Texture2D texture, Vector2 textureScale, Vector2 textureOffset, float textureRotation) {
            if (combineCountrySurfacesActive) {
                if (ToggleCountrySurface(countryIndex, visible, color, texture, textureScale, textureOffset, textureRotation, true)) {
                    return countries[countryIndex].mainRegion.surface;
                }
                return null;
            }
            return internal_ToggleCountryRegionSurface(countryIndex, regionIndex, visible, color, texture, textureScale, textureOffset, textureRotation);
        }

        /// <summary>
        /// Draws an outline around all regions of a country
        /// </summary>
        /// <returns>The country region outline.</returns>
        public void ToggleCountryOutline(int countryIndex, bool visible, Texture2D borderTexure = null, float borderWidth = 0.1f, Color tintColor = default(Color), float textureTiling = 1f, float animationSpeed = 0f) {
            if (countryIndex < 0 || countryIndex >= _countries.Length)
                return;
            int regionsCount = _countries[countryIndex].regions.Count;
            for (int k = 0; k < regionsCount; k++) {
                ToggleCountryRegionOutline(countryIndex, k, visible, borderTexure, borderWidth, tintColor, textureTiling, animationSpeed);
            }
        }

        /// <summary>
        /// Draws an outline around the main region of a country
        /// </summary>
        /// <returns>The country region outline.</returns>
        public GameObject ToggleCountryMainRegionOutline(int countryIndex, bool visible, Texture2D borderTexure = null, float borderWidth = 0.1f, Color tintColor = default(Color), float textureTiling = 1f, float animationSpeed = 0f) {
            if (countryIndex < 0 || countryIndex >= _countries.Length)
                return null;
            Region region = _countries[countryIndex].mainRegion;
            int regionIndex = _countries[countryIndex].mainRegionIndex;
            return ToggleCountryRegionOutline(countryIndex, regionIndex, visible, borderTexure, borderWidth, tintColor, textureTiling, animationSpeed);
        }

        /// <summary>
        /// Draws an outline around a region
        /// </summary>
        /// <returns>The country region outline.</returns>
        /// <param name="countryIndex">Country index.</param>
        public GameObject ToggleCountryRegionOutline(int countryIndex, int regionIndex, bool visible, Texture2D borderTexure = null, float borderWidth = 0.1f, Color tintColor = default(Color), float textureTiling = 1f, float animationSpeed = 0f) {
            if (countryIndex < 0 || countryIndex >= _countries.Length || regionIndex < 0 || regionIndex >= _countries[countryIndex].regions.Count)
                return null;
            // try get surface for this country region
            int cacheIndex = GetCacheIndexForCountryRegion(countryIndex, regionIndex);
            if (!visible) {
                HideRegionObject(cacheIndex.ToString(), null, COUNTRY_OUTLINE_GAMEOBJECT_NAME);
                return null;
            }
            Region region = _countries[countryIndex].regions[regionIndex];
            region.customBorder.texture = borderTexure;
            region.customBorder.width = borderWidth;
            region.customBorder.textureTiling = textureTiling;
            region.customBorder.animationSpeed = animationSpeed;
            if (tintColor != default(Color)) {
                region.customBorder.tintColor = tintColor;
            }
            return DrawCountryRegionOutline(cacheIndex.ToString(), region);
        }


        /// <summary>
        /// Uncolorize/hide specified country by index in the countries collection.
        /// </summary>
        public void HideCountryRegionSurface(int countryIndex, int regionIndex) {
            if (!ValidCountryRegionIndex(countryIndex, regionIndex)) return;
            Region region = countries[countryIndex].regions[regionIndex];
            if (_countryHighlightedIndex != countryIndex || _countryRegionHighlightedIndex != regionIndex) {
                GameObject obj = region.surface;
                if (obj != null) {
                    obj.SetActive(false);
                }
            }
            region.customMaterial = null;
        }

        /// <summary>
        /// Hides all colorized regions of all countries.
        /// </summary>
        public void HideCountrySurfaces() {
            for (int c = 0; c < _countries.Length; c++) {
                HideCountrySurface(c);
            }
        }

        /// <summary>
        /// Flashes specified country by index in the countries collection.
        /// </summary>
        public void BlinkCountry(string countryName, Color color1, Color color2, float duration, float blinkingSpeed) {
            int countryIndex = GetCountryIndex(countryName);
            BlinkCountry(countryIndex, color1, color2, duration, blinkingSpeed);
        }

        /// <summary>
        /// Flashes specified country by index in the countries collection.
        /// </summary>
        public void BlinkCountry(int countryIndex, Color color1, Color color2, float duration, float blinkingSpeed, bool smoothBlink = true) {
            if (!ValidCountryIndex(countryIndex)) return;
            if (countryIndex < 0 || countryIndex >= _countries.Length)
                return;
            int mainRegionIndex = _countries[countryIndex].mainRegionIndex;
            BlinkCountry(countryIndex, mainRegionIndex, color1, color2, duration, blinkingSpeed, smoothBlink);
        }

        /// <summary>
        /// Flashes specified country's region.
        /// </summary>
        public void BlinkCountry(int countryIndex, int regionIndex, Color color1, Color color2, float duration, float blinkingSpeed, bool smoothBlink = true) {
            if (!ValidCountryRegionIndex(countryIndex, regionIndex)) return;
            Region region = countries[countryIndex].regions[regionIndex];
            GameObject surf = region.surface;
            if (surf == null) {
                surf = GenerateCountryRegionSurface(countryIndex, regionIndex, hudMatCountry);
            }
            SurfaceBlinker sb = surf.AddComponent<SurfaceBlinker>();
            sb.blinkMaterial = hudMatCountry;
            sb.color1 = color1;
            sb.color2 = color2;
            sb.duration = duration;
            sb.speed = blinkingSpeed;
            sb.smoothBlink = smoothBlink;
            sb.customizableSurface = _countries[countryIndex].regions[regionIndex];
            surf.SetActive(true);
        }

        /// <summary>
        /// Returns an array of country names. The returning list can be grouped by continent.
        /// </summary>
        public string[] GetCountryNames(bool groupByContinent, bool addCountryIndex = true) {
            List<string> c = new List<string>();
            if (_countries == null)
                return c.ToArray();
            Dictionary<string, bool> continentsAdded = new Dictionary<string, bool>();
            for (int k = 0; k < _countries.Length; k++) {
                Country country = _countries[k];
                if (groupByContinent) {
                    if (!continentsAdded.ContainsKey(country.continent)) {
                        continentsAdded.Add(country.continent, true);
                        c.Add(country.continent);
                    }
                    if (addCountryIndex) {
                        c.Add(country.continent + "|" + country.name + " (" + k + ")");
                    } else {
                        c.Add(country.continent + "|" + country.name);
                    }
                } else {
                    if (addCountryIndex) {
                        c.Add(country.name + " (" + k + ")");
                    } else {
                        c.Add(country.name);
                    }
                }
            }
            c.Sort();

            if (groupByContinent) {
                int k = -1;
                while (++k < c.Count) {
                    int i = c[k].IndexOf('|');
                    if (i > 0) {
                        c[k] = "  " + c[k].Substring(i + 1);
                    }
                }
            }
            return c.ToArray();
        }


        /// <summary>
        /// Returns a list of countries whose attributes matches predicate
        /// </summary>
        public List<Country> GetCountries(AttribPredicate predicate) {
            List<Country> selectedCountries = new List<Country>();
            for (int k = 0; k < _countries.Length; k++) {
                Country country = _countries[k];
                if (predicate(country.attrib))
                    selectedCountries.Add(country);
            }
            return selectedCountries;
        }


        /// <summary>
        /// Gets a list of countries that overlap with a given region
        /// </summary>
        public List<Country> GetCountriesOverlap(Region region) {
            List<Country> rr = new List<Country>();
            for (int k = 0; k < _countries.Length; k++) {
                Country country = _countries[k];
                if (!country.regionsRect2D.Overlaps(region.rect2D))
                    continue;
                if (country.regions == null)
                    continue;
                int rCount = country.regions.Count;
                for (int r = 0; r < rCount; r++) {
                    Region otherRegion = country.regions[r];
                    if (region.Intersects(otherRegion)) {
                        rr.Add(country);
                        break;
                    }
                }
            }
            return rr;
        }


        /// <summary>
        /// Gets a list of country regions that overlap with a given region
        /// </summary>
        public List<Region> GetCountryRegionsOverlap(Region region) {
            List<Region> rr = new List<Region>();
            for (int k = 0; k < _countries.Length; k++) {
                Country country = _countries[k];
                if (!country.regionsRect2D.Overlaps(region.rect2D))
                    continue;
                if (country.regions == null)
                    continue;
                int rCount = country.regions.Count;
                for (int r = 0; r < rCount; r++) {
                    Region otherRegion = country.regions[r];
                    if (otherRegion.points.Length > 0 && region.Intersects(otherRegion)) {
                        rr.Add(otherRegion);
                    }
                }
            }
            return rr;
        }




        /// <summary>
        /// Returns the list of costal positions of a given country
        /// </summary>
        public List<Vector2> GetCountryCoastalPoints(int countryIndex, float minDistance = 0.005f, bool includeAllRegions = false) {
            if (countryIndex < 0 || countryIndex >= _countries.Length || _countries[countryIndex].regions == null)
                return null;

            List<Vector2> coastalPoints = new List<Vector2>();
            minDistance *= minDistance;
            bool onlyMainRegion = !includeAllRegions;
            for (int r = 0; r < _countries[countryIndex].regions.Count; r++) {
                if (onlyMainRegion)
                    r = _countries[countryIndex].mainRegionIndex;
                Region region = _countries[countryIndex].regions[r];
                for (int p = 0; p < region.points.Length; p++) {
                    Vector2 position = region.points[p];
                    Vector2 dummy;
                    if (ContainsWater(position, 4, out dummy)) {
                        bool valid = true;
                        for (int s = coastalPoints.Count - 1; s >= 0; s--) {
                            float sqrDist = FastVector.SqrDistanceByValue(coastalPoints[s], position); // (coastalPoints [s] - position).sqrMagnitude;
                            if (sqrDist < minDistance) {
                                valid = false;
                                break;
                            }
                        }
                        if (valid) {
                            coastalPoints.Add(position);
                        }
                    }
                }
                if (onlyMainRegion)
                    break;
            }
            return coastalPoints;
        }


        /// <summary>
        /// Returns the list of costal positions of a given country
        /// </summary>
        public List<Vector2> GetCountryCoastalPoints(int countryIndex, int regionIndex, float minDistance = 0.005f) {
            if (countryIndex < 0 || countryIndex >= _countries.Length || regionIndex < 0 || _countries[countryIndex].regions == null || regionIndex >= _countries[countryIndex].regions.Count)
                return null;
            List<Vector2> coastalPoints = new List<Vector2>();
            minDistance *= minDistance;
            Region region = _countries[countryIndex].regions[regionIndex];
            for (int p = 0; p < region.points.Length; p++) {
                Vector2 position = region.points[p];
                Vector2 dummy;
                if (ContainsWater(position, 4, out dummy)) {
                    bool valid = true;
                    for (int s = coastalPoints.Count - 1; s >= 0; s--) {
                        float sqrDist = (coastalPoints[s] - position).sqrMagnitude;
                        if (sqrDist < minDistance) {
                            valid = false;
                            break;
                        }
                    }
                    if (valid) {
                        coastalPoints.Add(position);
                    }
                }
            }
            return coastalPoints;
        }

        /// <summary>
        /// Returns a list of common frontier points between two countries.
        /// Use extraWidth to widen the points, useful when using the result to block pass in pathfinding
        /// </summary>
        public List<Vector2> GetCountryFrontierPoints(int countryIndex1, int countryIndex2, int extraWidth = 0) {

            if (countryIndex1 < 0 || countryIndex1 >= _countries.Length || countryIndex2 < 0 || countryIndex2 >= _countries.Length)
                return null;

            Country country1 = _countries[countryIndex1];
            Country country2 = _countries[countryIndex2];
            List<Vector2> samePoints = new List<Vector2>();

            int country1RegionsCount = country1.regions.Count;
            for (int cr = 0; cr < country1RegionsCount; cr++) {
                Region region1 = country1.regions[cr];
                int region1neighboursCount = region1.neighbours.Count;
                for (int n = 0; n < region1neighboursCount; n++) {
                    Region otherRegion = region1.neighbours[n];
                    if (country2.regions.Contains(otherRegion)) {
                        for (int p = 0; p < region1.points.Length; p++) {
                            for (int o = 0; o < otherRegion.points.Length; o++) {
                                if (region1.points[p] == otherRegion.points[o]) {
                                    samePoints.Add(region1.points[p]);
                                }
                            }
                        }
                    }
                }
            }

            // Adds optional width to the line
            int count = samePoints.Count;
            Vector2 dx = new Vector2(1.0f / EARTH_ROUTE_SPACE_WIDTH, 0);
            Vector2 dy = new Vector2(0, 1.0f / EARTH_ROUTE_SPACE_HEIGHT);
            for (int p = 0; p < count; p++) {
                Vector2 point = MatrixCostPositionToMap2D(Map2DToMatrixCostPosition(samePoints[p]));
                for (int k = -extraWidth; k <= extraWidth; k++) {
                    for (int j = -extraWidth; j <= extraWidth; j++) {
                        if (k == 0 && j == 0)
                            continue;
                        Vector2 pw = point + k * dx + j * dy;
                        if (!samePoints.Contains(pw)) {
                            samePoints.Add(pw);
                        }
                    }
                }
            }

            return samePoints;
        }

        /// <summary>
        /// Returns the points for the given country region. Optionally in world space coordinates (normal map, not viewport).
        /// </summary>
        public Vector3[] GetCountryFrontierPoints(int countryIndex, bool worldSpace) {
            if (countryIndex < 0 || countryIndex >= countries.Length)
                return null;
            return GetCountryFrontierPoints(countryIndex, countries[countryIndex].mainRegionIndex, worldSpace);
        }

        /// <summary>
        /// Returns the points for the given country region. Optionally in world space coordinates (normal map, not viewport).
        /// </summary>
        public Vector3[] GetCountryFrontierPoints(int countryIndex, int regionIndex, bool worldSpace) {
            if (countryIndex < 0 || countryIndex >= countries.Length)
                return null;
            if (regionIndex < 0 || regionIndex >= countries[countryIndex].regions.Count)
                return null;

            Region region = countries[countryIndex].regions[regionIndex];
            int pointsCount = region.points.Length;
            Vector3[] points = new Vector3[pointsCount];
            if (worldSpace) {
                for (int k = 0; k < pointsCount; k++) {
                    points[k] = transform.TransformPoint(region.points[k]);
                }
            } else {
                for (int k = 0; k < pointsCount; k++) {
                    points[k] = region.points[k];
                }
            }
            return points;
        }



        /// <summary>
        /// Checks quality of country's polygon points. Useful before using polygon clipping operations.
        /// </summary>
        /// <returns><c>true</c>, if country was sanitized (there was any change), <c>false</c> if country data has not changed.</returns>
        public bool CountrySanitize(int countryIndex, int minimumPoints = 3, bool refresh = true) {
            if (countryIndex < 0 || countryIndex >= _countries.Length)
                return false;

            bool changes = false;
            Country country = _countries[countryIndex];
            for (int k = 0; k < country.regions.Count; k++) {
                Region region = country.regions[k];
                if (RegionSanitize(region)) {
                    changes = true;
                }
                if (region.points.Length < minimumPoints) {
                    country.regions.Remove(region);
                    if (country.regions == null) {
                        return true;
                    }
                    k--;
                    changes = true;
                }
            }
            if (changes && refresh) {
                RefreshCountryDefinition(countryIndex, null);
            }
            return changes;
        }



        /// <summary>
        /// Returns the colored surface (game object) of a country. If it has not been colored yet, it will optionally create it.
        /// </summary>
        public GameObject GetCountryRegionSurfaceGameObject(string countryName, int regionIndex, bool forceCreation = false) {
            int countryIndex = GetCountryIndex(countryName);
            return GetCountryRegionSurfaceGameObject(countryIndex, regionIndex, forceCreation);

        }

        /// <summary>
        /// Returns the colored surface (game object) of a country. If it has not been colored yet, it will optionally create it.
        /// </summary>
        public GameObject GetCountryRegionSurfaceGameObject(int countryIndex, int regionIndex, bool forceCreation = false) {
            if (!ValidCountryRegionIndex(countryIndex, regionIndex)) return null;
            GameObject surf = countries[countryIndex].regions[regionIndex].surface;
            if (surf == null) {
                surf = ToggleCountryRegionSurface(countryIndex, regionIndex, true, Misc.ColorClear);
            }
            return surf;
        }


        /// <summary>
        /// Returns the colored surface (game object) of the main region of a country. If it has not been colored yet, it will optionally create it.
        /// </summary>
        public GameObject GetCountryMainRegionSurfaceGameObject(string countryName, bool forceCreation = false) {
            int countryIndex = GetCountryIndex(countryName);
            return GetCountryMainRegionSurfaceGameObject(countryIndex, forceCreation);

        }

        /// <summary>
        /// Returns the colored surface (game object) of the main region of a country. If it has not been colored yet, it will optionally create it.
        /// </summary>
        public GameObject GetCountryMainRegionSurfaceGameObject(int countryIndex, bool forceCreation = false) {
            if (!ValidCountryIndex(countryIndex)) return null;
            return GetCountryRegionSurfaceGameObject(countryIndex, countries[countryIndex].mainRegionIndex, forceCreation);
        }

        /// <summary>
        /// Makes countryIndex absorb another country. All regions are transfered to target country.
        /// This function is quite slow with high definition frontiers.
        /// </summary>
        /// <param name="countryIndex">Country index of the conquering country.</param>
        /// <param name="sourceCountryIndex">Source country of the loosing country.</param>
        /// <param name="mergeRegions">Merge resulting regions into a single region (if they're overlapping or intersect). If false, source country will be added as new region without any merging (faster).</param>
        public bool CountryTransferCountry(int countryIndex, int sourceCountryIndex, bool redraw, bool mergeRegions = true) {
            if (sourceCountryIndex < 0 || sourceCountryIndex >= countries.Length)
                return false;
            Region sourceCountryRegion = countries[sourceCountryIndex].mainRegion;
            return CountryTransferCountryRegion(countryIndex, sourceCountryRegion, redraw, mergeRegions);
        }



        /// <summary>
        /// Makes countryIndex absorb another country providing any of its regions. All regions are transfered to target country.
        /// This function is quite slow with high definition frontiers.
        /// </summary>
        /// <param name="countryIndex">Country index of the conquering country.</param>
        /// <param name="sourceRegion">Source region of the loosing country.</param>
        /// <param name="mergeRegions">Merge resulting regions into a single region (if they're overlapping or intersect). If false, source country will be added as new region without any merging (faster).</param>
        public bool CountryTransferCountryRegion(int countryIndex, Region sourceCountryRegion, bool redraw, bool mergeRegions = true) {
            if (sourceCountryRegion == null) return false;
            Country sourceCountry = (Country)sourceCountryRegion.entity;
            int sourceCountryIndex = GetCountryIndex(sourceCountry);
            if (countryIndex < 0 || sourceCountryIndex < 0 || countryIndex == sourceCountryIndex)
                return false;

            if (_provinces == null && !_showProvinces) {
                ReadProvincesPackedString(); // Forces loading of provinces
            }

            sourceCountry.DestroySurfaces();

            // Transfer all provinces records to target country
            Country targetCountry = _countries[countryIndex];

            targetCountry.DestroySurfaces();

            if (targetCountry.provinces != null && sourceCountry.provinces != null) {
                List<Province> destProvinces = new List<Province>(targetCountry.provinces);
                for (int k = 0; k < sourceCountry.provinces.Length; k++) {
                    Province province = sourceCountry.provinces[k];
                    province.countryIndex = countryIndex;
                    destProvinces.Add(province);
                }
                destProvinces.Sort(ProvinceSizeComparer);
                targetCountry.provinces = destProvinces.ToArray();
            }

            // Transfer cities
            int cityCount = cities.Length;
            for (int k = 0; k < cityCount; k++) {
                if (_cities[k].countryIndex == sourceCountryIndex)
                    _cities[k].countryIndex = countryIndex;
            }

            // Transfer mount points
            int mountPointCount = mountPoints.Count;
            for (int k = 0; k < mountPointCount; k++) {
                if (mountPoints[k].countryIndex == sourceCountryIndex)
                    mountPoints[k].countryIndex = countryIndex;
            }

            // Add main region of the source country to target if they are joint
            if (targetCountry.regions == null) {
                targetCountry.regions = new List<Region>();
                targetCountry.regions.Add(sourceCountryRegion);
                sourceCountryRegion.entity = targetCountry;
                sourceCountryRegion.regionIndex = 0;
            } else if (targetCountry.mainRegionIndex >= 0 && targetCountry.mainRegionIndex < targetCountry.regions.Count) {
                Region targetRegion = targetCountry.regions[targetCountry.mainRegionIndex];

                // Add region to target country's polygon - only if the country is touching or crossing target country frontier
                if (mergeRegions && sourceCountryRegion.Intersects(targetRegion)) {
                    RegionMagnet(sourceCountryRegion, targetRegion);
                    Clipper clipper = new Clipper();
                    clipper.AddPath(targetRegion, PolyType.ptSubject);
                    clipper.AddPath(sourceCountryRegion, PolyType.ptClip);
                    clipper.Execute(ClipType.ctUnion);
                } else {
                    // Add new region to country
                    sourceCountryRegion.entity = targetRegion.entity;
                    sourceCountryRegion.regionIndex = targetCountry.regions.Count;
                    targetCountry.regions.Add(sourceCountryRegion);
                }
            }

            // Transfer additional regions
            if (sourceCountry.regions.Count > 1) {
                List<Region> targetRegions = new List<Region>(targetCountry.regions);
                for (int k = 0; k < sourceCountry.regions.Count; k++) {
                    Region otherRegion = sourceCountry.regions[k];
                    if (otherRegion != sourceCountryRegion) {
                        targetRegions.Add(sourceCountry.regions[k]);
                    }
                }
                targetCountry.regions = targetRegions;
            }

            // Fusion any adjacent regions that results from merge operation
            if (mergeRegions) {
                MergeAdjacentRegions(targetCountry);
                RegionSanitize(targetCountry.regions);
            }

            // Finish operation
            internal_CountryDelete(sourceCountryIndex, false);
            if (countryIndex > sourceCountryIndex)
                countryIndex--;
            RefreshCountryDefinition(countryIndex, null);
            if (redraw) {
                Redraw();
            }
            return true;
        }


        /// <summary>
        /// Changes province's owner to specified country and modifies frontiers/borders.
        /// </summary>
        public bool CountryTransferProvince(int targetCountryIndex, int provinceIndex, bool redraw) {
            if (provinces == null || provinceIndex < 0 || provinceIndex >= _provinces.Length)
                return false;
            Province province = _provinces[provinceIndex];
            EnsureProvinceDataIsLoaded(province);
            if (province.regions == null) return false;
            return CountryTransferProvinceRegion(targetCountryIndex, province.mainRegion, redraw);
        }

        /// <summary>
        /// Changes province's owner to specified country and modifies frontiers/borders.
        /// Note: provinceRegion parameter usually is the province main region - although it does not matter since all regions will transfer as well. 
        /// </summary>
        public bool CountryTransferProvinceRegion(int targetCountryIndex, Region provinceRegion, bool redraw) {

            if (provinceRegion == null)
                return false;

            Province province = (Province)provinceRegion.entity;
            int provinceIndex = GetProvinceIndex(province);
            if (provinceIndex < 0 || targetCountryIndex < 0 || targetCountryIndex >= _countries.Length)
                return false;

            // Province must belong to another country

            int sourceCountryIndex = province.countryIndex;
            if (sourceCountryIndex == targetCountryIndex)
                return false;

            CountryRemoveProvince(sourceCountryIndex, province, mergeRegions: true);

            CountryAddProvince(targetCountryIndex, province, mergeRegions: true, updateCities: true, updateMountPoints: true);

            if (redraw) {
                Redraw(true);
            }
            return true;

        }

        /// <summary>
        /// Changes provinces' owner to specified country and modifies frontiers/borders.
        /// </summary>
        public bool CountryTransferProvinces(int targetCountryIndex, List<Province> provinces, bool redraw) {

            if (provinces == null)
                return false;
            if (targetCountryIndex < 0 || targetCountryIndex >= _countries.Length)
                return false;

            // Provinces must belong to another country
            List<Province> provincesToTransfer = new List<Province>();
            foreach (Province province in provinces) {
                int sourceCountryIndex = province.countryIndex;
                if (sourceCountryIndex != targetCountryIndex) {
                    provincesToTransfer.Add(province);
                }
            }
            if (provincesToTransfer.Count == 0) return false;

            // Group provinces by owner
            while (provincesToTransfer.Count > 0) {
                int sourceCountryIndex = provincesToTransfer[0].countryIndex;
                // this internal method will remove transferred provinces from the list
                internal_CountryTransferProvinces(targetCountryIndex, provincesToTransfer, sourceCountryIndex);
            }
            Country targetCountry = _countries[targetCountryIndex];
            RegionSanitize(targetCountry.regions);
            OptimizeFrontiers();
            lastCountryLookupCount = -1;

            if (redraw) {
                Redraw();
            }

            return true;
        }


        /// <summary>
        /// Sets the provinces for an existing country. The country regions will be updated to reflect the regions of the new provinces.
        /// </summary>
        /// <param name="mergeRegions">If true, adjacent regions will be merged (default).</param>
        /// <returns></returns>
        public bool CountrySetProvinces(int countryIndex, List<Province> provinces, bool mergeRegions = true, bool updateCities = true, bool updateMountPoints = true) {
            if (countryIndex < 0 || countryIndex >= countries.Length || provinces == null) return false;

            Country country = countries[countryIndex];

            List<Region> oldRegions = new List<Region>();
            if (country.regions != null) {
                foreach (Region region in country.regions) {
                    if (region != null && region.surface != null) {
                        if (region.surfaceIsDirty) {
                            region.Clear();
                        } else {
                            oldRegions.Add(region);
                        }
                    }
                }
            }

            country.regions.Clear();

            List<Province> newProvinces = new List<Province>();

            foreach (Province province in provinces) {
                if (province == null) continue;
                EnsureProvinceDataIsLoaded(province);
                if (province.regions == null) continue;
                int r = 0;
                foreach (Region region in province.regions) {
                    if (region == null) continue;
                    Region newRegion = region.Clone();
                    newRegion.entity = country;
                    newRegion.regionIndex = r++;
                    newRegion.neighbours.Clear();
                    country.regions.Add(newRegion);
                }
                newProvinces.Add(province);
            }
            country.provinces = newProvinces.ToArray();
            if (mergeRegions) {
                MergeAdjacentRegions(country);
            }

            // Optimization: try to find new regions in old regions; if old region matches new region, reuse its surface if it's already created
            if (country.regions != null) {
                int countryRegionsCount = country.regions.Count;
                foreach (Region oldRegion in oldRegions) {
                    bool foundSameRegion = false;
                    for (int k = 0; k < countryRegionsCount; k++) {
                        Region region = country.regions[k];
                        if (region.surface == null && region.HasSameShapeThan(oldRegion)) {
                            oldRegion.regionIndex = k;
                            country.regions[k] = oldRegion;
                            foundSameRegion = true;
                            break;
                        }
                    }
                    if (!foundSameRegion) {
                        // if old main region was result of a merge and is not found in new regions, clear its surface and all of the old regions
                        if (oldRegion.surfaceIsCombinedRegions) {
                            // clear all merged surfaces
                            foreach (Region r in oldRegions) {
                                r.DestroySurface();
                            }
                            break;
                        }
                        oldRegion.Clear();
                    }
                }
            }

            // Update provinces country index and country neighbours
            UpdateProvincesCountryIndex(countryIndex);

            int provincesCount = country.provinces.Length;

            if (updateCities) {
                int citiesCount = cities.Length;
                for (int p = 0; p < provincesCount; p++) {
                    Province province = country.provinces[p];
                    for (int k = 0; k < citiesCount; k++) {
                        City city = _cities[k];
                        if (city.countryIndex == province.countryIndex && city.province.Equals(province.name)) {
                            city.countryIndex = countryIndex;
                        }
                    }
                }
            }

            if (updateMountPoints) {
                int mpCount = mountPoints.Count;
                for (int p = 0; p < provincesCount; p++) {
                    Province province = country.provinces[p];
                    int provinceIndex = GetProvinceIndex(province);
                    for (int k = 0; k < mpCount; k++) {
                        MountPoint mp = mountPoints[k];
                        if (mp.countryIndex == province.countryIndex && mp.provinceIndex == provinceIndex) {
                            mp.countryIndex = countryIndex;
                        }
                    }
                }
            }

            RefreshCountryGeometry(country);

            return true;
        }


        void UpdateProvincesCountryIndex(int countryIndex) {
            Country country = _countries[countryIndex];
            List<int> newNeighbours = new List<int>(country.neighbours);
            int provincesCount = country.provinces.Length;
            bool changes = false;
            for (int p = 0; p < provincesCount; p++) {
                Province province = country.provinces[p];
                int formerCountryIndex = province.countryIndex;
                if (formerCountryIndex != countryIndex) {
                    province.countryIndex = countryIndex;
                    foreach (Region provRegion in province.regions) {
                        foreach (Region neighbour in provRegion.neighbours) {
                            Province neighbourProvince = (Province)neighbour.entity;
                            int neighbourCountryIndex = neighbourProvince.countryIndex;
                            if (neighbourCountryIndex != countryIndex && !newNeighbours.Contains(neighbourCountryIndex)) {
                                newNeighbours.Add(neighbourCountryIndex);
                                changes = true;
                            }
                        }
                    }
                }
            }
            if (changes) {
                country.neighbours = newNeighbours.ToArray();
            }
        }


        public bool CountryAddProvince(int countryIndex, Province province, bool mergeRegions = true, bool updateCities = true, bool updateMountPoints = true) {

            // Add province to target country
            Country targetCountry = _countries[countryIndex];
            List<Province> newProvinces = new List<Province>();
            newProvinces.Add(province);
            return CountryAddProvinces(countryIndex, newProvinces, mergeRegions: true, updateCities: true, updateMountPoints: true);
        }


        /// <summary>
        /// Add provinces to an existing country. The country regions will be updated to reflect the regions of the new provinces.
        /// Please note that if the provinces currently belong to another country, that other country won't be updated. Call CountrySetProvinces or CountryRemoveProvinces on the other country as well.
        /// </summary>
        /// <param name="mergeRegions">If true, adjacent regions will be merged (default).</param>
        /// <returns></returns>
        public bool CountryAddProvinces(int countryIndex, List<Province> provinces, bool mergeRegions = true, bool updateCities = true, bool updateMountPoints = true) {
            if (countryIndex < 0 || countryIndex >= countries.Length || provinces == null) return false;

            Country country = countries[countryIndex];

            List<Province> newProvinces = new List<Province>();

            if (country.provinces != null) {
                newProvinces.AddRange(country.provinces);
            }

            // ensure there's at least one new province
            foreach (Province province in provinces) {
                if (province == null) continue;
                bool alreadyExists = false;
                foreach (Province existingProvince in country.provinces) {
                    if (existingProvince == province) {
                        alreadyExists = true;
                        break;
                    }
                }
                if (alreadyExists) continue;
                EnsureProvinceDataIsLoaded(province);
                if (province.regions == null) continue;
                int r = 0;
                foreach (Region region in province.regions) {
                    if (region == null) continue;
                    Region newRegion = region.Clone();
                    newRegion.entity = country;
                    newRegion.regionIndex = r++;
                    newRegion.neighbours.Clear();
                    country.regions.Add(newRegion);
                }
                newProvinces.Add(province);

                if (updateCities) {
                    int citiesCount = cities.Length;
                    for (int k = 0; k < citiesCount; k++) {
                        City city = _cities[k];
                        if (city.countryIndex == province.countryIndex && city.province.Equals(province.name)) {
                            city.countryIndex = countryIndex;
                        }
                    }
                }

                if (updateMountPoints) {
                    int mpCount = mountPoints.Count;
                    int provinceIndex = GetProvinceIndex(province);
                    for (int k = 0; k < mpCount; k++) {
                        MountPoint mp = mountPoints[k];
                        if (mp.countryIndex == province.countryIndex && mp.provinceIndex == provinceIndex) {
                            mp.countryIndex = countryIndex;
                        }
                    }
                }
            }
            country.provinces = newProvinces.ToArray();
            if (mergeRegions) {
                MergeAdjacentRegions(country);
            }

            // Update provinces country index
            UpdateProvincesCountryIndex(countryIndex);

            RefreshCountryGeometry(country);
            return true;
        }


        /// <summary>
        /// Remove a province from an existing country.
        /// </summary>
        /// <param name="mergeRegions">If true, adjacent regions will be merged (default).</param>
        /// <returns></returns>
        public bool CountryRemoveProvince(int countryIndex, Province province, bool mergeRegions = true) {
            if (countryIndex < 0 || countryIndex >= countries.Length || provinces == null) return false;

            Country country = countries[countryIndex];
            List<Province> newProvinces = new List<Province>(country.provinces);
            if (!newProvinces.Contains(province)) return false;

            newProvinces.Remove(province);

            if (country.isPool) {
                // remove province from pool but do not waster time with polygons as pool country is not visible
                country.provinces = newProvinces.ToArray();
                return true;
            }

            return CountrySetProvinces(countryIndex, newProvinces, mergeRegions, false, false);
        }


        /// <summary>
        /// Remove provinces from an existing country. The country regions will be updated to reflect the regions of the new provinces.
        /// </summary>
        /// <param name="mergeRegions">If true, adjacent regions will be merged (default).</param>
        /// <returns></returns>
        public bool CountryRemoveProvinces(int countryIndex, List<Province> provincesToRemove, bool mergeRegions = true) {
            if (countryIndex < 0 || countryIndex >= countries.Length || provincesToRemove == null) return false;

            Country country = countries[countryIndex];
            List<Province> newProvinces = new List<Province>();
            foreach (Province province in country.provinces) {
                if (province == null || provincesToRemove.Contains(province)) {
                    province.countryIndex = -1;
                    continue;
                }
                newProvinces.Add(province);
            }

            if (country.isPool) {
                // remove province from pool but do not waste time with polygons as pool country is not visible
                country.provinces = newProvinces.ToArray();
                return true;
            }

            return CountrySetProvinces(countryIndex, newProvinces, mergeRegions, false, false);
        }



        /// <summary>
        /// Makes countryIndex absorb an hexagonal portion of the map. If that portion belong to another country, it will be substracted from that country as well.
        /// This function is quite slow with high definition frontiers.
        /// </summary>
        /// <param name="countryIndex">Country index of the conquering country.</param>
        /// <param name="cellIndex">Index of the cell to add to the country.</param>
        public bool CountryTransferCell(int countryIndex, int cellIndex, bool redraw = true) {
            if (countryIndex < 0 || cellIndex < 0 || cells == null || cellIndex >= cells.Length)
                return false;

            // Start process
            Country country = countries[countryIndex];
            Cell cell = cells[cellIndex];

            // Create a region for the cell
            Region sourceRegion = new Region(country, country.regions.Count);
            sourceRegion.UpdatePointsAndRect(cell.points, true);

            // Transfer cities
            List<City> citiesInCell = GetCities(sourceRegion);
            int cityCount = citiesInCell.Count;
            for (int k = 0; k < cityCount; k++) {
                City city = citiesInCell[k];
                if (city.countryIndex != countryIndex) {
                    city.countryIndex = countryIndex;
                    city.province = ""; // clear province since it does not apply anymore
                }
            }

            // Transfer mount points
            List<MountPoint> mountPointsInCell = GetMountPoints(sourceRegion);
            int mountPointCount = mountPointsInCell.Count;
            for (int k = 0; k < mountPointCount; k++) {
                MountPoint mp = mountPointsInCell[k];
                if (mp.countryIndex != countryIndex) {
                    mp.countryIndex = countryIndex;
                    mp.provinceIndex = -1;  // same as cities - province cleared in case it's informed since it does not apply anymore
                }
            }

            // Add region to target country's polygon - only if the country is touching or crossing target country frontier
            Region targetRegion = country.mainRegion;
            if (targetRegion != null && sourceRegion.Intersects(targetRegion)) {
                RegionMagnet(sourceRegion, targetRegion);
                Clipper clipper = new Clipper();
                clipper.AddPath(targetRegion, PolyType.ptSubject);
                clipper.AddPath(sourceRegion, PolyType.ptClip);
                clipper.Execute(ClipType.ctUnion);
            } else {
                // Add new region to country
                sourceRegion.entity = country;
                sourceRegion.regionIndex = country.regions.Count;
                country.regions.Add(sourceRegion);
            }

            // Fusion any adjacent regions that results from merge operation
            MergeAdjacentRegions(country);
            RegionSanitize(country.regions);

            // Finish operation with the country
            RefreshCountryGeometry(country);

            // Substract cell region from any other country
            List<Country> otherCountries = GetCountriesOverlap(sourceRegion);
            int orCount = otherCountries.Count;
            for (int k = 0; k < orCount; k++) {
                Country otherCountry = otherCountries[k];
                if (otherCountry == country)
                    continue;
                Clipper clipper = new Clipper();
                clipper.AddPaths(otherCountry.regions, PolyType.ptSubject);
                clipper.AddPath(sourceRegion, PolyType.ptClip);
                clipper.Execute(ClipType.ctDifference, otherCountry);
                if (otherCountry.regions.Count == 0) {
                    int otherCountryIndex = GetCountryIndex(otherCountry);
                    CountryDelete(otherCountryIndex, true, false);
                } else {
                    RegionSanitize(otherCountry.regions);
                    RefreshCountryGeometry(otherCountry);
                }
            }

            OptimizeFrontiers();

            if (redraw) {
                Redraw();
            }
            return true;
        }


        /// <summary>
        /// Removes a cell from a country.
        /// </summary>
        /// <param name="countryIndex">Country index.</param>
        /// <param name="cellIndex">Index of the cell to remove from the country.</param>
        public bool CountryRemoveCell(int countryIndex, int cellIndex, bool redraw = true) {
            if (countryIndex < 0 || countryIndex >= countries.Length || cellIndex < 0 || cells == null || cellIndex >= cells.Length)
                return false;

            Country country = countries[countryIndex];
            Cell cell = cells[cellIndex];
            Region sourceRegion = new Region(country, country.regions.Count);
            sourceRegion.UpdatePointsAndRect(cell.points, true);

            Clipper clipper = new Clipper();
            clipper.AddPaths(country.regions, PolyType.ptSubject);
            clipper.AddPath(sourceRegion, PolyType.ptClip);
            clipper.Execute(ClipType.ctDifference, country);
            if (country.regions.Count == 0) {
                CountryDelete(countryIndex, true, false);
            } else {
                RegionSanitize(country.regions);
                RefreshCountryGeometry(country);
            }

            OptimizeFrontiers();

            if (redraw) {
                Redraw();
            }
            return true;
        }

        #endregion

        #region IO functions area

        public void SetCountryGeoData(string s) {
            string[] countryList = s.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            int countryCount = countryList.Length;
            countries = new Country[countryCount];
            Vector2 min = Misc.Vector2one * 10;
            Vector2 max = -min;

            char[] separatorCountries = new char[] { '$' };
            for (int k = 0; k < countryCount; k++) {
                string[] countryInfo = countryList[k].Split(separatorCountries, StringSplitOptions.None);
                string name = countryInfo[0];
                string continent = countryInfo[1];
                int uniqueId;
                if (countryInfo.Length >= 5) {
                    uniqueId = int.Parse(countryInfo[4]);
                } else {
                    uniqueId = GetUniqueId(new List<IExtendableAttribute>(_countries));
                }
                Country country = new Country(name, continent, uniqueId);
                string[] regions = countryInfo[2].Split(SPLIT_SEP_ASTERISK, StringSplitOptions.RemoveEmptyEntries);
                int regionCount = regions.Length;
                country.regions = new List<Region>();
                float maxVol = 0;
                Vector2 minCountry = new Vector2(10, 10);
                Vector2 maxCountry = -minCountry;
                for (int r = 0; r < regionCount; r++) {
                    string[] coordinates = regions[r].Split(SPLIT_SEP_SEMICOLON, StringSplitOptions.RemoveEmptyEntries);
                    int coorCount = coordinates.Length;
                    if (coorCount < 3)
                        continue;
                    min.x = min.y = 10;
                    max.x = max.y = -10;
                    Region countryRegion = new Region(country, country.regions.Count);
                    Vector2[] newPoints = new Vector2[coorCount];
                    for (int c = 0; c < coorCount; c++) {
                        float x, y;
                        GetPointFromPackedString(ref coordinates[c], out x, out y);
                        if (x < min.x)
                            min.x = x;
                        if (x > max.x)
                            max.x = x;
                        if (y < min.y)
                            min.y = y;
                        if (y > max.y)
                            max.y = y;
                        newPoints[c].x = x;
                        newPoints[c].y = y;
                    }
                    countryRegion.UpdatePointsAndRect(newPoints);
                    countryRegion.sanitized = true;

                    // Calculate country bounding rect
                    if (min.x < minCountry.x)
                        minCountry.x = min.x;
                    if (min.y < minCountry.y)
                        minCountry.y = min.y;
                    if (max.x > maxCountry.x)
                        maxCountry.x = max.x;
                    if (max.y > maxCountry.y)
                        maxCountry.y = max.y;
                    float vol = FastVector.SqrDistance(ref min, ref max);
                    if (vol > maxVol) {
                        maxVol = vol;
                        country.mainRegionIndex = country.regions.Count;
                        country.center = countryRegion.center;
                    }
                    country.regions.Add(countryRegion);
                }
                // hidden
                if (countryInfo.Length >= 4) {
                    int hidden = 0;
                    if (int.TryParse(countryInfo[3], out hidden)) {
                        country.hidden = hidden > 0;
                    }
                }
                // fip 10 4
                if (countryInfo.Length >= 6) {
                    country.fips10_4 = countryInfo[5];
                }
                // iso A2
                if (countryInfo.Length >= 7) {
                    country.iso_a2 = countryInfo[6];
                }
                // iso A3
                if (countryInfo.Length >= 8) {
                    country.iso_a3 = countryInfo[7];
                }
                // iso N3
                if (countryInfo.Length >= 9) {
                    country.iso_n3 = countryInfo[8];
                }
                country.regionsRect2D = new Rect(minCountry.x, minCountry.y, Math.Abs(maxCountry.x - minCountry.x), Mathf.Abs(maxCountry.y - minCountry.y));
                _countries[k] = country;
            }
            lastCountryLookupCount = -1;
            needOptimizeFrontiers = true;
        }


        /// <summary>
        /// Exports the geographic data in packed string format.
        /// </summary>
        public string GetCountryGeoData() {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < countries.Length; k++) {
                Country country = countries[k];
                if (country.regions.Count < 1)
                    continue;
                if (k > 0)
                    sb.Append("|");
                sb.Append(country.name);
                sb.Append("$");
                sb.Append(country.continent);
                sb.Append("$");
                for (int r = 0; r < country.regions.Count; r++) {
                    if (r > 0)
                        sb.Append("*");
                    Region region = country.regions[r];
                    for (int p = 0; p < region.points.Length; p++) {
                        if (p > 0) {
                            sb.Append(";");
                        }
                        int x = (int)(region.points[p].x * WMSK.MAP_PRECISION);
                        int y = (int)(region.points[p].y * WMSK.MAP_PRECISION);
                        sb.Append(x.ToString(Misc.InvariantCulture));
                        sb.Append(",");
                        sb.Append(y.ToString(Misc.InvariantCulture));
                    }
                }
                sb.Append("$");
                sb.Append((country.hidden ? "1" : "0"));
                sb.Append("$");
                sb.Append(country.uniqueId.ToString());
                sb.Append("$");
                sb.Append(country.fips10_4);
                sb.Append("$");
                sb.Append(country.iso_a2);
                sb.Append("$");
                sb.Append(country.iso_a3);
                sb.Append("$");
                sb.Append(country.iso_n3);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets XML attributes of all countries in jSON format.
        /// </summary>
        public string GetCountriesAttributes(bool prettyPrint = true) {
            return GetCountriesAttributes(new List<Country>(_countries), prettyPrint);
        }

        /// <summary>
        /// Gets XML attributes of provided countries in jSON format.
        /// </summary>
        public string GetCountriesAttributes(List<Country> countries, bool prettyPrint = true) {
            JSONObject composed = new JSONObject();
            for (int k = 0; k < countries.Count; k++) {
                Country country = countries[k];
                if (country.attrib.keys != null)
                    composed.AddField(country.uniqueId.ToString(), country.attrib);
            }
            return composed.Print(prettyPrint);
        }

        /// <summary>
        /// Sets countries attributes from a jSON formatted string.
        /// </summary>
        public void SetCountriesAttributes(string jSON) {
            JSONObject composed = new JSONObject(jSON);
            if (composed.keys == null)
                return;
            int keyCount = composed.keys.Count;
            for (int k = 0; k < keyCount; k++) {
                int uniqueId = int.Parse(composed.keys[k]);
                int countryIndex = GetCountryIndex(uniqueId);
                if (countryIndex >= 0) {
                    _countries[countryIndex].attrib = composed[k];
                }
            }
        }

        /// <summary>
        /// Returns country data (geodata and any attributes) in jSON format.
        /// </summary>
        public string GetCountriesDataJSON(bool prettyPrint = true) {
            CountriesJSONData exported = new CountriesJSONData();
            for (int k = 0; k < countries.Length; k++) {
                Country country = countries[k];
                CountryJSON cjson = new CountryJSON();
                cjson.name = country.name;
                cjson.continent = country.continent;
                if (country.regions != null) {
                    cjson.regions = new List<RegionJSON>();
                    foreach (Region region in country.regions) {
                        RegionJSON regionJSON = new RegionJSON();
                        regionJSON.points = region.points;
                        cjson.regions.Add(regionJSON);
                    }
                }
                cjson.hidden = country.hidden;
                cjson.fips10_4 = country.fips10_4;
                cjson.iso_a2 = country.iso_a2;
                cjson.iso_a3 = country.iso_a3;
                cjson.iso_n3 = country.iso_n3;
                cjson.attrib = country.attrib;
                cjson.uniqueId = country.uniqueId;
                exported.countries.Add(cjson);
            }
            return JsonUtility.ToJson(exported, prettyPrint);
        }

        /// <summary>
        /// Sets country data (geodata and any attributes) in jSON format.
        /// </summary>
        public void SetCountriesDataJSON(string json) {
            CountriesJSONData imported = JsonUtility.FromJson<CountriesJSONData>(json);
            int countryCount = imported.countries.Count;
            countries = new Country[countryCount];
            Vector2 min = Misc.Vector2one * 10;
            Vector2 max = -min;

            for (int k = 0; k < countryCount; k++) {
                CountryJSON cjson = imported.countries[k];
                Country country = new Country(cjson.name, cjson.continent, cjson.uniqueId);
                country.regions = new List<Region>();
                float maxVol = 0;
                Vector2 minCountry = new Vector2(10, 10);
                Vector2 maxCountry = -minCountry;
                int regionCount = cjson.regions.Count;
                for (int r = 0; r < regionCount; r++) {
                    Vector2[] coordinates = cjson.regions[r].points;
                    int coorCount = coordinates.Length;
                    if (coorCount < 3)
                        continue;
                    min.x = min.y = 10;
                    max.x = max.y = -10;
                    Region countryRegion = new Region(country, country.regions.Count);
                    Vector2[] newPoints = new Vector2[coorCount];
                    for (int c = 0; c < coorCount; c++) {
                        float x = coordinates[c].x;
                        float y = coordinates[c].y;
                        if (x < min.x)
                            min.x = x;
                        if (x > max.x)
                            max.x = x;
                        if (y < min.y)
                            min.y = y;
                        if (y > max.y)
                            max.y = y;
                        newPoints[c].x = x;
                        newPoints[c].y = y;
                    }
                    countryRegion.UpdatePointsAndRect(newPoints);
                    countryRegion.sanitized = true;

                    // Calculate country bounding rect
                    if (min.x < minCountry.x)
                        minCountry.x = min.x;
                    if (min.y < minCountry.y)
                        minCountry.y = min.y;
                    if (max.x > maxCountry.x)
                        maxCountry.x = max.x;
                    if (max.y > maxCountry.y)
                        maxCountry.y = max.y;
                    float vol = FastVector.SqrDistance(ref min, ref max);
                    if (vol > maxVol) {
                        maxVol = vol;
                        country.mainRegionIndex = country.regions.Count;
                        country.center = countryRegion.center;
                    }
                    country.regions.Add(countryRegion);
                }
                country.hidden = cjson.hidden;
                country.fips10_4 = cjson.fips10_4;
                country.iso_a2 = cjson.iso_a2;
                country.iso_a3 = cjson.iso_a3;
                country.iso_n3 = cjson.iso_n3;
                country.regionsRect2D = new Rect(minCountry.x, minCountry.y, Math.Abs(maxCountry.x - minCountry.x), Mathf.Abs(maxCountry.y - minCountry.y));
                _countries[k] = country;
            }
            lastCountryLookupCount = -1;
            needOptimizeFrontiers = true;
        }


        #endregion
    }

}