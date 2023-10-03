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

    public enum PROVINCE_LABELS_VISIBILITY {
        Automatic = 0,
        Scripting = 1
    }

    public delegate void OnProvinceEvent(int provinceIndex, int regionIndex);
    public delegate void OnProvinceClickEvent(int provinceIndex, int regionIndex, int buttonIndex);
    public delegate void OnProvinceHighlightEvent(int provinceIndex, int regionIndex, ref bool allowHighlight);

    public partial class WMSK : MonoBehaviour {

        #region Public properties

        Province[] _provinces;

        /// <summary>
        /// Complete array of states and provinces and the country name they belong to.
        /// </summary>
        public Province[] provinces {
            get {
                EnsureProvincesDataIsLoaded();
                return _provinces;
            }
            set {
                _provinces = value;
                lastProvinceLookupCount = -1;
            }
        }

        Province _provinceHighlighted;

        /// <summary>
        /// Returns Province under mouse position or null if none.
        /// </summary>
        public Province provinceHighlighted { get { return _provinceHighlighted; } }

        int _provinceHighlightedIndex = -1;

        /// <summary>
        /// Returns current highlighted province index.
        /// </summary>
        public int provinceHighlightedIndex { get { return _provinceHighlightedIndex; } }

        Region _provinceRegionHighlighted;

        /// <summary>
        /// Returns currently highlightd province's region.
        /// </summary>
        /// <value>The country region highlighted.</value>
        public Region provinceRegionHighlighted { get { return _provinceRegionHighlighted; } }

        int _provinceRegionHighlightedIndex = -1;

        /// <summary>
        /// Returns current highlighted province's region index.
        /// </summary>
        public int provinceRegionHighlightedIndex { get { return _provinceRegionHighlightedIndex; } }

        int _provinceLastClicked = -1;

        /// <summary>
        /// Returns the last clicked province index.
        /// </summary>
        public int provinceLastClicked { get { return _provinceLastClicked; } }

        int _provinceRegionLastClicked = -1;

        /// <summary>
        /// Returns the last clicked province region index.
        /// </summary>
        public int provinceRegionLastClicked { get { return _provinceRegionLastClicked; } }

        /// <summary>
        /// Gets the province region's highlighted shape.
        /// </summary>
        public GameObject provinceRegionHighlightedShape { get { return provinceRegionHighlightedObj; } }

        int _provinceLastOver = -1;

        /// <summary>
        /// Returns the last hovered province.
        /// </summary>
        public int provinceLastOver { get { return _provinceLastOver; } }

        int _provinceRegionLastOver = -1;

        /// <summary>
        /// Returns the last hovered province region index.
        /// </summary>
        public int provinceRegionLastOver { get { return _provinceRegionLastOver; } }

        public event OnProvinceEvent OnProvinceEnter;
        public event OnProvinceEvent OnProvinceExit;
        public event OnProvinceClickEvent OnProvinceClick;
        public event OnProvinceHighlightEvent OnProvinceHighlight;

        [SerializeField]
        bool _showProvinces;

        /// <summary>
        /// Toggle province borders visibility.
        /// </summary>
        public bool showProvinces {
            get {
                return _showProvinces;
            }
            set {
                if (value != _showProvinces) {
                    _showProvinces = value;
                    isDirty = true;

                    if (_showProvinces) {
                        if (provinces == null) {
                            ReadProvincesPackedString();
                        }
                        if (_drawAllProvinces) {
                            DrawAllProvinceBorders(true, false);
                        }
                    } else {
                        HideProvinces();
                    }
                }
            }
        }

        [SerializeField]
        bool _enableProvinceHighlight = true;

        /// <summary>
        /// Enable/disable province highlight when mouse is over and ShowProvinces is true.
        /// </summary>
        public bool enableProvinceHighlight {
            get {
                return _enableProvinceHighlight;
            }
            set {
                if (_enableProvinceHighlight != value) {
                    _enableProvinceHighlight = value;
                    isDirty = true;
                    if (_enableProvinceHighlight) {
                        if (provinces == null) {
                            ReadProvincesPackedString();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Set whether all regions of active province should be highlighted.
        /// </summary>
        [SerializeField]
        bool _highlightAllProvinceRegions;

        public bool highlightAllProvinceRegions {
            get {
                return _highlightAllProvinceRegions;
            }
            set {
                if (_highlightAllProvinceRegions != value) {
                    _highlightAllProvinceRegions = value;
                    DestroySurfaces();
                    isDirty = true;
                }
            }
        }


        /// <summary>
        /// Set whether the highlight effect must preserve existing province texture
        /// </summary>
        [SerializeField]
        bool _highlightProvinceKeepTexture = true;

        public bool highlightProvinceKeepTexture {
            get {
                return _highlightProvinceKeepTexture;
            }
            set {
                if (_highlightProvinceKeepTexture != value) {
                    _highlightProvinceKeepTexture = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _drawAllProvinces;

        /// <summary>
        /// Forces drawing of all provinces and not only thouse of currently selected country.
        /// </summary>
        public bool drawAllProvinces {
            get {
                return _drawAllProvinces;
            }
            set {
                if (value != _drawAllProvinces) {
                    _drawAllProvinces = value;
                    isDirty = true;
                    DrawAllProvinceBorders(true, false);
                }
            }
        }


        [SerializeField]
        bool
            _provincesDashBorders;

        /// <summary>
        /// Use a dash line style when drawing province borders.
        /// </summary>
        public bool provincesDashBorders {
            get {
                return _provincesDashBorders;
            }
            set {
                if (value != _provincesDashBorders) {
                    _provincesDashBorders = value;
                    isDirty = true;
                    DrawAllProvinceBorders(true, false);
                }
            }
        }



        [SerializeField]
        [Range(0.05f, 1)]
        float
            _provincesDashAmount = 0.5f;

        /// <summary>
        /// Controls the dash appearance.
        /// </summary>
        public float provincesDashAmount {
            get {
                return _provincesDashAmount;
            }
            set {
                if (value != _provincesDashAmount) {
                    _provincesDashAmount = value;
                    isDirty = true;
                    DrawAllProvinceBorders(true, false);
                }
            }
        }


        /// <summary>
        /// Fill color to use when the mouse hovers a country's region.
        /// </summary>
        [SerializeField]
        Color
            _provincesFillColor = new Color(0, 0, 1, 0.7f);

        public Color provincesFillColor {
            get {
                if (hudMatProvince != null) {
                    return hudMatProvince.color;
                } else {
                    return _provincesFillColor;
                }
            }
            set {
                if (value != _provincesFillColor) {
                    _provincesFillColor = value;
                    isDirty = true;
                    if (hudMatProvince != null && _provincesFillColor != hudMatProvince.color) {
                        hudMatProvince.color = _provincesFillColor;
                    }
                }
            }
        }

        /// <summary>
        /// Global color for provinces.
        /// </summary>
        [SerializeField]
        Color
            _provincesColor = Color.white;

        public Color provincesColor {
            get {
                if (provincesMat != null) {
                    return provincesMat.color;
                } else {
                    return _provincesColor;
                }
            }
            set {
                if (value != _provincesColor) {
                    _provincesColor = value;
                    isDirty = true;

                    if (provincesMat != null && _provincesColor != provincesMat.color) {
                        provincesMat.color = _provincesColor;
                    }
                }
            }
        }

        [SerializeField]
        string _provinceAttributeFile = PROVINCE_ATTRIB_DEFAULT_FILENAME;

        public string provinceAttributeFile {
            get { return _provinceAttributeFile; }
            set {
                if (value != _provinceAttributeFile) {
                    _provinceAttributeFile = value;
                    if (_provinceAttributeFile == null)
                        _provinceAttributeFile = PROVINCE_ATTRIB_DEFAULT_FILENAME;
                    isDirty = true;
                    ReloadProvincesAttributes();
                }
            }
        }



        [SerializeField]
        bool
        _provincesCoastlines = true;

        public bool provincesCoastlines {
            get {
                return _provincesCoastlines;
            }
            set {
                if (value != _provincesCoastlines) {
                    _provincesCoastlines = value;
                    isDirty = true;
                    DrawAllProvinceBorders(true, false);
                }
            }
        }


        [SerializeField]
        bool
            _showProvinceNames = false;

        public bool showProvinceNames {
            get {
                return _showProvinceNames;
            }
            set {
                if (value != _showProvinceNames) {
                    _showProvinceNames = value;
                    isDirty = true;
                    if (textProvinceRoot != null) {
                        textProvinceRoot.SetActive(_showProvinceNames);
                    } else if (_showProvinceNames) {
                        RedrawProvinceLabels(countryProvincesLabelsShown);
                    }
                }
            }
        }


        [SerializeField]
        bool
            _showAllCountryProvinceNames = true;

        public bool showAllCountryProvinceNames {
            get {
                return _showAllCountryProvinceNames;
            }
            set {
                if (value != _showAllCountryProvinceNames) {
                    _showAllCountryProvinceNames = value;
                    isDirty = true;
                    RedrawProvinceLabels(countryProvincesLabelsShown);
                }
            }
        }


        [SerializeField]
        PROVINCE_LABELS_VISIBILITY _provinceLabelsVisibility = PROVINCE_LABELS_VISIBILITY.Automatic;

        public PROVINCE_LABELS_VISIBILITY provinceLabelsVisibility {
            get {
                return _provinceLabelsVisibility;
            }
            set {
                if (value != _provinceLabelsVisibility) {
                    _provinceLabelsVisibility = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float
            _provinceLabelsAbsoluteMinimumSize = 0.05f;

        public float provinceLabelsAbsoluteMinimumSize {
            get {
                return _provinceLabelsAbsoluteMinimumSize;
            }
            set {
                if (value != _provinceLabelsAbsoluteMinimumSize) {
                    _provinceLabelsAbsoluteMinimumSize = value;
                    isDirty = true;
                    if (_showProvinceNames)
                        RedrawProvinceLabels(countryProvincesLabelsShown);
                }
            }
        }


        [SerializeField]
        float
            _provinceLabelsSize = 0.2f;

        public float provinceLabelsSize {
            get {
                return _provinceLabelsSize;
            }
            set {
                if (value != _provinceLabelsSize) {
                    _provinceLabelsSize = value;
                    isDirty = true;
                    if (_showProvinceNames) {
                        RedrawProvinceLabels(countryProvincesLabelsShown);
                    }
                }
            }
        }



        [SerializeField]
        bool _provinceLabelsEnableAutomaticFade = true;

        /// <summary>
        /// Automatic fading of province labels depending on camera distance and label screen size
        /// </summary>
        public bool provinceLabelsEnableAutomaticFade {
            get { return _provinceLabelsEnableAutomaticFade; }
            set {
                if (_provinceLabelsEnableAutomaticFade != value) {
                    _provinceLabelsEnableAutomaticFade = value;
                    isDirty = true;
                    if (_showProvinceNames) {
                        RedrawProvinceLabels(countryProvincesLabelsShown);
                    }
                }
            }
        }

        [SerializeField]
        float
            _provinceLabelsAutoFadeMaxHeight = 0.2f;

        /// <summary>
        /// Max height of a label relative to screen height (0..1) at which fade out starts
        /// </summary>
        public float provinceLabelsAutoFadeMaxHeight {
            get {
                return _provinceLabelsAutoFadeMaxHeight;
            }
            set {
                if (value != _provinceLabelsAutoFadeMaxHeight) {
                    _provinceLabelsAutoFadeMaxHeight = value;
                    _provinceLabelsAutoFadeMinHeight = Mathf.Min(_provinceLabelsAutoFadeMaxHeight, _provinceLabelsAutoFadeMinHeight);
                    isDirty = true;
                    FadeProvinceLabels();
                }
            }
        }

        [SerializeField]
        float
            _provinceLabelsAutoFadeMaxHeightFallOff = 0.5f;

        /// <summary>
        /// Fall off for fade labels when height is greater than min height
        /// </summary>
        public float provinceLabelsAutoFadeMaxHeightFallOff {
            get {
                return _provinceLabelsAutoFadeMaxHeightFallOff;
            }
            set {
                if (value != _provinceLabelsAutoFadeMaxHeightFallOff) {
                    _provinceLabelsAutoFadeMaxHeightFallOff = value;
                    isDirty = true;
                    FadeProvinceLabels();
                }
            }
        }


        [SerializeField]
        float
            _provinceLabelsAutoFadeMinHeight = 0.018f;

        /// <summary>
        /// Min height of a label relative to screen height (0..1) at which fade out starts
        /// </summary>
        public float provinceLabelsAutoFadeMinHeight {
            get {
                return _provinceLabelsAutoFadeMinHeight;
            }
            set {
                if (value != _provinceLabelsAutoFadeMinHeight) {
                    _provinceLabelsAutoFadeMinHeight = value;
                    _provinceLabelsAutoFadeMaxHeight = Mathf.Max(_provinceLabelsAutoFadeMaxHeight, _provinceLabelsAutoFadeMinHeight);
                    isDirty = true;
                    FadeProvinceLabels();
                }
            }
        }

        [SerializeField]
        float
            _provinceLabelsAutoFadeMinHeightFallOff = 0.005f;

        /// <summary>
        /// Fall off for fade labels when height is less than min height
        /// </summary>
        public float provinceLabelsAutoFadeMinHeightFallOff {
            get {
                return _provinceLabelsAutoFadeMinHeightFallOff;
            }
            set {
                if (value != _provinceLabelsAutoFadeMinHeightFallOff) {
                    _provinceLabelsAutoFadeMinHeightFallOff = value;
                    isDirty = true;
                    FadeProvinceLabels();
                }
            }
        }


        [SerializeField]
        bool
            _showProvinceLabelsShadow = true;

        /// <summary>
        /// Draws a shadow under province labels. Specify the color using labelsShadowColor.
        /// </summary>
        /// <value><c>true</c> if show labels shadow; otherwise, <c>false</c>.</value>
        public bool showProvinceLabelsShadow {
            get {
                return _showProvinceLabelsShadow;
            }
            set {
                if (value != _showProvinceLabelsShadow) {
                    _showProvinceLabelsShadow = value;
                    isDirty = true;
                    if (gameObject.activeInHierarchy) {
                        RedrawProvinceLabels(countryProvincesLabelsShown);
                    }
                }
            }
        }

        [SerializeField]
        Color
            _provinceLabelsColor = Color.cyan;

        /// <summary>
        /// Color for province labels.
        /// </summary>
        public Color provinceLabelsColor {
            get {
                return _provinceLabelsColor;
            }
            set {
                if (value != _provinceLabelsColor) {
                    _provinceLabelsColor = value;
                    isDirty = true;
                    if (gameObject.activeInHierarchy && provinceLabelsFont != null) {
                        _provinceLabelsFont.material.color = _provinceLabelsColor;
                    }
                }
            }
        }

        [SerializeField]
        Color
            _provinceLabelsShadowColor = new Color(0, 0, 0, 0.5f);

        /// <summary>
        /// Color for province labels.
        /// </summary>
        public Color provinceLabelsShadowColor {
            get {
                return _provinceLabelsShadowColor;
            }
            set {
                if (value != _provinceLabelsShadowColor) {
                    _provinceLabelsShadowColor = value;
                    isDirty = true;
                    if (gameObject.activeInHierarchy) {
                        provLabelsShadowMaterial.color = _provinceLabelsShadowColor;
                    }
                }
            }
        }

        [SerializeField]
        Vector2 _provinceLabelsShadowOffset = new Vector2(0.4f, -0.4f);

        /// <summary>
        /// Shadow offset for province labels
        /// </summary>
        public Vector2 provinceLabelsShadowOffset {
            get {
                return _provinceLabelsShadowOffset;
            }
            set {
                if (value != _provinceLabelsShadowOffset) {
                    _provinceLabelsShadowOffset = value;
                    isDirty = true;
                    DrawMapLabels();
                }
            }
        }



        [SerializeField]
        Font _provinceLabelsFont;

        /// <summary>
        /// Gets or sets the default font for province labels
        /// </summary>
        public Font provinceLabelsFont {
            get {
                return _provinceLabelsFont;
            }
            set {
                if (value != _provinceLabelsFont) {
                    _provinceLabelsFont = value;
                    isDirty = true;
                    ReloadProvinceFont();
                    RedrawProvinceLabels(countryProvincesLabelsShown);
                }
            }
        }

        #endregion

        #region Public API area

        /// <summary>
        /// Draws the borders of the provinces/states a country by its id. Returns true is country is found, false otherwise.
        /// </summary>
        public bool DrawProvinces(int countryIndex, bool includeNeighbours, bool forceRefresh, bool justComputeBorders) {
            if (countryIndex >= 0) {
                return mDrawProvinces(countryIndex, includeNeighbours, forceRefresh, justComputeBorders);
            }
            return false;
        }

        /// <summary>
        /// Hides the borders of all provinces/states.
        /// </summary>
        public void HideProvinces() {
            if (provincesObj != null) {
                DestroyImmediate(provincesObj);
            }
            countryProvincesDrawnIndex = -1;
            HideProvinceRegionHighlight();
        }


        /// <summary>
        /// Returns true if the provinceIndex is valid (ie. within province array range)
        /// </summary>
        public bool ValidProvinceIndex(int provinceIndex) {
            return provinceIndex >= 0 && provinces != null && provinceIndex < _provinces.Length;
        }

        /// <summary>
        /// Returns true if the countryIndex and regionIndex are valie (ie. within province and province regions array range)
        /// </summary>
        public bool ValidProvinceRegionIndex(int provinceIndex, int regionIndex) {
            return provinceIndex >= 0 && regionIndex >= 0 && provinces != null && provinceIndex < _provinces.Length && _provinces[provinceIndex].regions != null && regionIndex < _provinces[provinceIndex].regions.Count;
        }

        /// <summary>
        /// Returns the index of a province in the provinces array by its reference.
        /// </summary>
        public int GetProvinceIndex(Province province) {
            int provinceIndex;
            if (provinceLookup.TryGetValue(province, out provinceIndex))
                return provinceIndex;
            else
                return -1;
        }

        /// <summary>
        /// Returns the index of a province in the global provinces array.
        /// </summary>
        public int GetProvinceIndex(string countryName, string provinceName) {
            int countryIndex = GetCountryIndex(countryName);
            return GetProvinceIndex(countryIndex, provinceName);
        }


        /// <summary>
        /// Returns the index of a province in the global provinces array.
        /// </summary>
        public int GetProvinceIndex(int countryIndex, string provinceName) {
            if (countryIndex < 0 || countryIndex >= _countries.Length)
                return -1;
            Country country = _countries[countryIndex];
            if (provinces == null || country.provinces == null) return -1;
            for (int k = 0; k < country.provinces.Length; k++) {
                if (country.provinces[k].name.Equals(provinceName)) {
                    return GetProvinceIndex(country.provinces[k]);
                }
            }
            // if province doesn't exist in the country province list, do a full search on the provinces array
            int provincesCount = _provinces.Length;
            for (int k = 0; k < provincesCount; k++) {
                Province prov = _provinces[k];
                if (prov.countryIndex == countryIndex && prov.name.Equals(provinceName)) {
                    return k;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the index of the province that contains the provided map coordinates. This will ignore hidden countries.
        /// </summary>
        /// <returns>The province index.</returns>
        /// <param name="localPosition">Map coordinates in the range of (-0.5 .. 0.5)</param>
        public int GetProvinceIndex(Vector2 localPosition) {
            // verify if hitPos is inside any country polygon
            int provinceIndex, provinceRegionIndex;
            if (GetProvinceRegionIndex(localPosition, -1, out provinceIndex, out provinceRegionIndex)) {
                return provinceIndex;
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of the province that contains the provided map coordinates and belongs to given country index. This will ignore hidden countries, and it's faster since countryIndex is passed to reduce candidate provinces.
        /// </summary>
        /// <returns>The province index.</returns>
        /// <param name="localPosition">Map coordinates in the range of (-0.5 .. 0.5)</param>
        public int GetProvinceIndex(Vector2 localPosition, int countryIndex) {
            // verify if hitPos is inside any country polygon
            int provinceIndex, provinceRegionIndex;
            if (GetProvinceRegionIndex(localPosition, countryIndex, out provinceIndex, out provinceRegionIndex)) {
                return provinceIndex;
            }
            return -1;
        }

        /// <summary>
        /// Gets the region of the province that contains the provided map coordinates.
        /// </summary>
        /// <returns>The province region.</returns>
        /// <param name="localPosition">Map coordinates in the range of (-0.5 .. 0.5)</param>
        public Region GetProvinceRegion(Vector2 localPosition) {
            // verify if hitPos is inside any country polygon
            int provinceIndex, provinceRegionIndex;
            if (GetProvinceRegionIndex(localPosition, -1, out provinceIndex, out provinceRegionIndex)) {
                return provinces[provinceIndex].regions[provinceRegionIndex];
            }
            return null;
        }


        /// <summary>
        /// Gets the region index of the province that contains the provided map coordinates.
        /// </summary>
        /// <returns>The Region index or -1 if no region found.</returns>
        /// <param name="localPosition">Map coordinates in the range of (-0.5 .. 0.5)</param>
        public int GetProvinceRegionIndex(Vector2 localPosition) {
            Region region = GetProvinceRegion(localPosition);
            if (region == null) return -1;
            return region.regionIndex;
        }



        /// <summary>
        /// Gets the index of the province region.
        /// </summary>
        /// <returns>The province region index.</returns>
        /// <param name="provinceIndex">Province index.</param>
        /// <param name="region">Region.</param>
        public int GetProvinceRegionIndex(int provinceIndex, Region region) {
            if (provinceIndex < 0 || provinceIndex >= provinces.Length)
                return -1;
            Province province = _provinces[provinceIndex];
            EnsureProvinceDataIsLoaded(province);
            if (province.regions == null)
                return -1;
            int rc = province.regions.Count;
            for (int k = 0; k < rc; k++) {
                if (province.regions[k] == region) {
                    return k;
                }
            }
            return -1;
        }



        /// <summary>
        /// Gets the index of the province and region that contains the provided map coordinates. This will ignore hidden countries.
        /// </summary>
        public bool GetProvinceRegionIndex(Vector2 localPosition, int countryIndex, out int provinceIndex, out int provinceRegionIndex) {
            provinceIndex = -1;
            provinceRegionIndex = -1;

            if (_enableEnclaves) {
                int candidateProvinceIndex = -1;
                int candidateProvinceRegionIndex = -1;
                float candidateRegionSize = float.MaxValue;
                int cc0, cc1;
                if (countryIndex < 0) {
                    cc0 = 0;
                    cc1 = countriesOrderedBySize.Count;
                } else {
                    cc0 = countryIndex;
                    cc1 = countryIndex + 1;
                }
                for (int c = cc0; c < cc1; c++) {
                    Country country = _countries[_countriesOrderedBySize[c]];
                    if (country.hidden)
                        continue;
                    if (country.regionsRect2D.Contains(localPosition)) {
                        if (country.provinces == null)
                            continue;
                        int pp = country.provinces.Length;
                        for (int p = 0; p < pp; p++) {
                            Province prov = country.provinces[p];
                            if (prov.regions == null)
                                ReadProvincePackedString(prov);
                            int rr = prov.regions.Count;
                            for (int r = 0; r < rr; r++) {
                                Region reg = prov.regions[r];
                                if (reg.rect2DArea < candidateRegionSize && reg.Contains(localPosition)) {
                                    candidateRegionSize = reg.rect2DArea;
                                    candidateProvinceIndex = GetProvinceIndex(prov);
                                    candidateProvinceRegionIndex = r;
                                }
                            }
                        }
                    }
                }
                provinceIndex = candidateProvinceIndex;
                provinceRegionIndex = candidateProvinceRegionIndex;
            } else {
                if (countryIndex < 0) {
                    countryIndex = GetCountryIndex(localPosition);
                }
                if (countryIndex >= 0) {
                    Country country = _countries[countryIndex];
                    if (country.provinces == null)
                        return false;
                    int pp = country.provinces.Length;
                    for (int p = 0; p < pp; p++) {
                        Province province = country.provinces[p];
                        EnsureProvinceDataIsLoaded(province);
                        if (!province.regionsRect2D.Contains(localPosition))
                            continue;
                        int rr = province.regions.Count;
                        for (int pr = 0; pr < rr; pr++) {
                            if (province.regions[pr].Contains(localPosition)) {
                                provinceIndex = GetProvinceIndex(province);
                                provinceRegionIndex = pr;
                                return true;
                            }
                        }
                    }
                }
            }

            if (provinceRegionIndex < 0 && countryIndex < 0) {
                // try checking all provinces in case they are not backed by country land (sea provinces for example)
                int provincesCount = provinces.Length;
                for (int p = 0; p < provincesCount; p++) {
                    Province province = _provinces[p];
                    EnsureProvinceDataIsLoaded(province);
                    if (!province.regionsRect2D.Contains(localPosition)) continue;
                    int rr = province.regions.Count;
                    for (int pr = 0; pr < rr; pr++) {
                        if (province.regions[pr].Contains(localPosition)) {
                            provinceIndex = GetProvinceIndex(province);
                            provinceRegionIndex = pr;
                            return true;
                        }
                    }
                }
            }

            return provinceRegionIndex >= 0;
        }


        /// <summary>
        /// Returns the province index by screen position.
        /// </summary>
        public bool GetProvinceIndex(Ray ray, out int provinceIndex, out int regionIndex) {
            // obtain country
            int hitCount = Physics.RaycastNonAlloc(ray, tempHits, 500, layerMask);
            if (hitCount > 0) {
                for (int k = 0; k < hitCount; k++) {
                    if (tempHits[k].collider.gameObject == gameObject) {
                        Vector2 localHit = transform.InverseTransformPoint(tempHits[k].point);
                        return GetProvinceRegionIndex(localHit, -1, out provinceIndex, out regionIndex);
                    }
                }
            }
            provinceIndex = -1;
            regionIndex = -1;
            return false;
        }


        /// <summary>
        /// Returns the province object by its name.
        /// </summary>
        public Province GetProvince(string provinceName, string countryName) {
            int countryIndex = GetCountryIndex(countryName);
            if (countryIndex < 0)
                return null;
            int provinceIndex = GetProvinceIndex(countryIndex, provinceName);
            if (provinceIndex >= 0) {
                Province prov = provinces[provinceIndex];
                if (prov.regions == null)
                    ReadProvincePackedString(prov);
                return prov;
            }
            return null;
        }

        /// <summary>
        /// Gets the province object in the provinces array by its index. Equals to map.provinces[provinceIndex] as provinces array is public.
        /// </summary>
        public Province GetProvince(int provinceIndex) {
            if (provinces == null || provinceIndex < 0 || provinceIndex >= provinces.Length)
                return null;
            return provinces[provinceIndex];
        }

        /// <summary>
        /// Gets the province object in the provinces array by its position.
        /// </summary>
        public Province GetProvince(Vector2 mapPosition) {
            int provinceIndex = GetProvinceIndex(mapPosition);
            if (provinceIndex >= 0)
                return provinces[provinceIndex];
            return null;
        }

        /// <summary>
        /// Gets the province index with that unique Id.
        /// </summary>
        public int GetProvinceIndex(int uniqueId) {
            int provinceCount = provinces.Length;
            for (int k = 0; k < provinceCount; k++) {
                if (_provinces[k].uniqueId == uniqueId)
                    return k;
            }
            return -1;
        }


        /// <summary>
        /// Returns all neighbour provinces
        /// </summary>
        public List<Province> ProvinceNeighbours(int provinceIndex) {
            List<Province> provinceNeighbours = new List<Province>();
            if (provinceIndex < 0 || provinceIndex >= provinces.Length)
                return provinceNeighbours;

            // Get country object
            Province province = provinces[provinceIndex];
            EnsureProvinceDataIsLoaded(province);
            if (province.regions != null) {
                // Iterate for all regions (a country can have several separated regions)
                for (int provinceRegionIndex = 0; provinceRegionIndex < province.regions.Count; provinceRegionIndex++) {
                    Region provinceRegion = province.regions[provinceRegionIndex];
                    List<Province> neighbours = ProvinceNeighboursOfRegion(provinceRegion);
                    neighbours.ForEach(p => {
                        if (!provinceNeighbours.Contains(p))
                            provinceNeighbours.Add(p);
                    });
                }
            }
            return provinceNeighbours;
        }


        /// <summary>
        /// Get neighbours of the main region of a province
        /// </summary>
        public List<Province> ProvinceNeighboursOfMainRegion(int provinceIndex) {
            // Get main region
            Province province = provinces[provinceIndex];
            Region provinceRegion = province.regions[province.mainRegionIndex];
            return ProvinceNeighboursOfRegion(provinceRegion);
        }

        /// <summary>
        /// Get neighbours of the currently selected region
        /// </summary>
        public List<Province> ProvinceNeighboursOfCurrentRegion() {
            Region selectedRegion = provinceRegionHighlighted;
            return ProvinceNeighboursOfRegion(selectedRegion);
        }

        /// <summary>
        /// Get neighbours of a given province region
        /// </summary>
        public List<Province> ProvinceNeighboursOfRegion(Region provinceRegion) {
            List<Province> provinceNeighbours = new List<Province>();

            // Get main region
            if (provinceRegion == null)
                return provinceNeighbours;

            // Check if neighbours have been computed
            if (!provinceNeighboursComputed && !_drawAllProvinces) {
                bool prevShowProvinces = _showProvinces;
                bool prevDrawAllProvinces = _drawAllProvinces;
                _showProvinces = true;
                _drawAllProvinces = true;
                DrawAllProvinceBorders(true, true);
                _drawAllProvinces = prevDrawAllProvinces;
                _showProvinces = prevShowProvinces;
            }

            // Get the neighbours for this region
            for (int neighbourIndex = 0; neighbourIndex < provinceRegion.neighbours.Count; neighbourIndex++) {
                Region neighbour = provinceRegion.neighbours[neighbourIndex];
                Province neighbourProvince = (Province)neighbour.entity;
                if (!provinceNeighbours.Contains(neighbourProvince)) {
                    provinceNeighbours.Add(neighbourProvince);
                }
            }

            // Find neighbours due to enclaves
            if (_enableEnclaves) {
                Province province = (Province)provinceRegion.entity;
                Country country = countries[province.countryIndex];
                for (int p = 0; p < provinces.Length; p++) {
                    Province p2 = provinces[p];
                    if (p2 == province || p2.regions == null)
                        continue;
                    // skip if country is not enclave of province country
                    Country c2 = countries[p2.countryIndex];
                    if (c2 != country && !c2.regionsRect2D.Overlaps(provinceRegion.rect2D) && !country.regionsRect2D.Overlaps(c2.regionsRect2D)) continue;
                    if (provinceNeighbours.Contains(p2))
                        continue;
                    int prc = p2.regions.Count;
                    for (int pr = 0; pr < prc; pr++) {
                        Region pregion = p2.regions[pr];
                        if (provinceRegion.Contains(pregion) || pregion.Contains(provinceRegion)) {
                            provinceNeighbours.Add(p2);
                            break;
                        }
                    }
                }
            }
            return provinceNeighbours;
        }

        /// <summary>
        /// Renames the province. Name must be unique, different from current and one letter minimum.
        /// </summary>
        /// <returns><c>true</c> if country was renamed, <c>false</c> otherwise.</returns>
        public bool ProvinceRename(int countryIndex, string oldName, string newName) {
            if (newName == null || newName.Length == 0)
                return false;
            int provinceIndex = GetProvinceIndex(countryIndex, oldName);
            int newProvinceIndex = GetProvinceIndex(countryIndex, newName);
            if (provinceIndex < 0 || newProvinceIndex >= 0)
                return false;
            provinces[provinceIndex].name = newName;

            // Update cities
            List<City> cities = GetCities(provinces[provinceIndex]);
            int cityCount = cities.Count;
            if (cityCount > 0) {
                for (int k = 0; k < cityCount; k++) {
                    if (cities[k].province != newName) {
                        cities[k].province = newName;
                    }
                }
            }

            lastProvinceLookupCount = -1;
            return true;

        }


        /// <summary>
        /// Creates a new province.
        /// </summary>
        /// <returns>The create.</returns>
        /// <param name="name">Name must be unique!</param>
        /// <param name="countryIndex">Country index.</param>
        public int ProvinceCreate(string name, int countryIndex) {
            Province newProvince = new Province(name, countryIndex, GetUniqueId(new List<IExtendableAttribute>(provinces)));
            newProvince.regions = new List<Region>();
            return ProvinceAdd(newProvince);
        }

        /// <summary>
        /// Adds a new province which has been properly initialized. Used by the Map Editor. Name must be unique.
        /// </summary>
        /// <returns>Index of new province.</returns>
        public int ProvinceAdd(Province province) {
            if (province.countryIndex < 0 || province.countryIndex >= _countries.Length)
                return -1;
            Province[] newProvinces = new Province[provinces.Length + 1];
            for (int k = 0; k < provinces.Length; k++) {
                newProvinces[k] = provinces[k];
            }
            int provinceIndex = newProvinces.Length - 1;
            newProvinces[provinceIndex] = province;
            provinces = newProvinces;
            lastProvinceLookupCount = -1;
            // add the new province to the country internal list
            if (province.countryIndex >= 0 && province.countryIndex < countries.Length) {
                Country country = _countries[province.countryIndex];
                if (country.provinces == null)
                    country.provinces = new Province[0];
                List<Province> newCountryProvinces = new List<Province>(country.provinces);
                newCountryProvinces.Add(province);
                newCountryProvinces.Sort(ProvinceSizeComparer);
                country.provinces = newCountryProvinces.ToArray();
            }

            RefreshProvinceGeometry(provinceIndex);

            return provinceIndex;
        }


        /// <summary>
        /// Creates a new country from an existing province. Existing province will be extracted from previous sovereign. Returns the index of the new country.
        /// </summary>
        /// <returns>The new country index or -1 if failed.</returns>
        public int ProvinceToCountry(Province province, string newCountryName, bool redraw = true) {

            if (province == null || province.countryIndex < 0 || string.IsNullOrEmpty(newCountryName))
                return -1;

            EnsureProvinceDataIsLoaded(province);
            if (province.regions == null) return -1;

            // Checks if newCountryName already exists
            int countryIndex = GetCountryIndex(newCountryName);
            if (countryIndex >= 0)
                return -1;

            // Add new country
            string continent = GetCountry(province.countryIndex).continent;
            Country newCountry = new Country(newCountryName, continent, GetUniqueId(new List<IExtendableAttribute>(_countries)));

            // Create dummy region
            newCountry.regions.Add(new Region(newCountry, 0));
            newCountry.mainRegionIndex = 0;
            newCountry.provinces = new Province[0];
            int newCountryIndex = CountryAdd(newCountry);

            // Transfer province
            if (!CountryTransferProvinceRegion(newCountryIndex, province.regions[province.mainRegionIndex], false)) {
                CountryDelete(newCountryIndex, false, false);
                return -1;
            }

            // Remove dummy region
            newCountry.regions.RemoveAt(0);

            if (redraw)
                Redraw();

            return newCountryIndex;
        }

        /// <summary>
        /// Creates a new country from a list of existing provinces. Existing provinces will be extracted from previous sovereign. Returns the index of the new country.
        /// </summary>
        /// <returns>The new country index or -1 if failed.</returns>
        public int ProvincesToCountry(List<Province> provinces, string newCountryName, bool redraw = true) {
            if (provinces == null || provinces.Count == 0) return -1;
            int countryIndex = provinces[0].countryIndex;
            string continent = "";
            if (countryIndex >= 0) {
                continent = countries[provinces[0].countryIndex].continent;
            }
            return ProvincesToCountry(provinces, newCountryName, continent, redraw);
        }


        /// <summary>
        /// Creates a new country from a list of existing provinces. Existing provinces will be extracted from previous sovereign. Returns the index of the new country.
        /// </summary>
        /// <returns>The new country index or -1 if failed.</returns>
        public int ProvincesToCountry(List<Province> provinces, string newCountryName, string continent, bool redraw = true) {

            if (provinces == null || provinces.Count == 0)
                return -1;

            // Checks if newCountryName already exists
            int countryIndex = GetCountryIndex(newCountryName);
            if (countryIndex >= 0)
                return -1;

            // From which countries?
            List<int> sourceCountries = new List<int>();
            foreach (Province province in provinces) {
                if (province.countryIndex >= 0) {
                    if (!sourceCountries.Contains(province.countryIndex)) sourceCountries.Add(province.countryIndex);
                }
            }

            // Remove provinces from each country
            List<Province> countryProvincesToRemove = new List<Province>();
            foreach (int sourceCountryIndex in sourceCountries) {
                countryProvincesToRemove.Clear();
                foreach (Province province in provinces) {
                    if (province.countryIndex == sourceCountryIndex) {
                        countryProvincesToRemove.Add(province);
                    }
                }
                CountryRemoveProvinces(sourceCountryIndex, countryProvincesToRemove);
            }

            // Create the new country
            Country newCountry = new Country(newCountryName, continent, GetUniqueId(new List<IExtendableAttribute>(_countries)));
            int newCountryIndex = CountryAdd(newCountry);
            CountrySetProvinces(newCountryIndex, provinces);

            if (redraw) {
                Redraw(true);
            }

            return newCountryIndex;
        }


        /// <summary>
        /// Flashes specified province by index in the global province array.
        /// </summary>
        public void BlinkProvince(int provinceIndex, Color color1, Color color2, float duration, float blinkingSpeed, bool smoothBlink = true) {
            if (provinceIndex < 0 || provinces == null || provinceIndex >= provinces.Length)
                return;
            int mainRegionIndex = provinces[provinceIndex].mainRegionIndex;
            BlinkProvince(provinceIndex, mainRegionIndex, color1, color2, duration, blinkingSpeed, smoothBlink);
        }

        /// <summary>
        /// Flashes specified province's region.
        /// </summary>
        public void BlinkProvince(int provinceIndex, int regionIndex, Color color1, Color color2, float duration, float blinkingSpeed, bool smoothBlink = true) {
            if (!ValidProvinceRegionIndex(provinceIndex, regionIndex)) return;
            Region region = provinces[provinceIndex].regions[regionIndex];
            GameObject surf = region.surface;
            if (surf == null) {
                surf = GenerateProvinceRegionSurface(provinceIndex, regionIndex, hudMatProvince);
                if (surf == null)
                    return;
            }
            SurfaceBlinker sb = surf.AddComponent<SurfaceBlinker>();
            sb.blinkMaterial = hudMatCountry;
            sb.color1 = color1;
            sb.color2 = color2;
            sb.duration = duration;
            sb.speed = blinkingSpeed;
            sb.smoothBlink = smoothBlink;
            sb.customizableSurface = provinces[provinceIndex].regions[regionIndex];
            surf.SetActive(true);
        }


        /// <summary>
        /// Starts navigation to target province/state. Returns false if not found.
        /// </summary>
        public bool FlyToProvince(string countryName, string provinceName) {
            int provinceIndex = GetProvinceIndex(countryName, provinceName);
            return FlyToProvince(provinceIndex);
        }

        /// <summary>
        /// Starts navigation to target province/state by index in the provinces collection. Returns false if not found.
        /// </summary>
        public bool FlyToProvince(int provinceIndex) {
            return FlyToProvince(provinceIndex, _navigationTime, GetZoomLevel());
        }

        /// <summary>
        /// Starts navigation to target province/state by name in the provinces collection with duration and zoom level options. Returns false if not found.
        /// </summary>
        public bool FlyToProvince(string countryName, string provinceName, float duration, float zoomLevel) {
            int provinceIndex = GetProvinceIndex(countryName, provinceName);
            return FlyToProvince(provinceIndex, duration, zoomLevel);
        }


        /// <summary>
        /// Starts navigation to target province/state by index in the provinces collection with duration and zoom level options. Returns false if not found.
        /// </summary>
        public bool FlyToProvince(int provinceIndex, float duration, float zoomLevel) {
            if (provinceIndex < 0 || provinceIndex >= provinces.Length)
                return false;
            Province province = provinces[provinceIndex];
            EnsureProvinceDataIsLoaded(province);
            SetDestination(provinces[provinceIndex].center, duration, zoomLevel);
            return true;
        }


        /// <summary>
        /// Colorize all regions of specified province/state by index in the global provinces collection.
        /// </summary>
        public void ToggleProvinceSurface(int provinceIndex, bool visible, Color color = default(Color)) {
            ToggleProvinceSurface(provinceIndex, visible, color, null, Misc.Vector2one, Misc.Vector2zero, 0, false);
        }

        /// <summary>
        /// Colorize all regions of specified province/state by index in the global provinces collection.
        /// </summary>
        public void ToggleProvinceSurface(Province province, bool visible, Color color = default(Color)) {
            int provinceIndex = GetProvinceIndex(province);
            if (provinceIndex < 0) return;
            ToggleProvinceSurface(provinceIndex, visible, color);
        }

        /// <summary>
        /// Colorize all regions of specified province and assings a texture to main region with options.
        /// </summary>
        public void ToggleProvinceSurface(Province province, bool visible, Texture2D texture, bool applyTextureToAllRegions = false) {
            int provinceIndex = GetProvinceIndex(province);
            if (provinceIndex < 0) return;
            ToggleProvinceSurface(provinceIndex, visible, Misc.ColorWhite, texture, applyTextureToAllRegions);
        }

        /// <summary>
        /// Colorize all regions of specified province and assings a color and texture to main region with options.
        /// </summary>
        public void ToggleProvinceSurface(Province province, bool visible, Color color, Texture2D texture, bool applyTextureToAllRegions = false) {
            int provinceIndex = GetProvinceIndex(province);
            if (provinceIndex < 0) return;
            ToggleProvinceSurface(provinceIndex, visible, color, texture, applyTextureToAllRegions);
        }

        /// <summary>
        /// Colorize all regions of specified province and assings a texture to main region with options.
        /// </summary>
        public void ToggleProvinceSurface(int provinceIndex, bool visible, Color color, Texture2D texture, bool applyTextureToAllRegions = false) {
            ToggleProvinceSurface(provinceIndex, visible, color, texture, Misc.Vector2one, Misc.Vector2zero, 0, applyTextureToAllRegions);
        }

        /// <summary>
        /// Colorize all regions of specified province and assings a texture to main region with options.
        /// </summary>
        public void ToggleProvinceSurface(int provinceIndex, bool visible, Color color, Texture2D texture, Vector2 textureScale, Vector2 textureOffset, float textureRotation, bool applyTextureToAllRegions) {
            if (!ValidProvinceIndex(provinceIndex)) return;
            if (!visible) {
                HideProvinceSurfaces(provinceIndex);
                return;
            }
            Province province = _provinces[provinceIndex];
            EnsureProvinceDataIsLoaded(province);
            int rCount = province.regions.Count;
            for (int r = 0; r < rCount; r++) {
                if (applyTextureToAllRegions || r == province.mainRegionIndex) {
                    ToggleProvinceRegionSurface(provinceIndex, r, visible, color, texture, textureScale, textureOffset, textureRotation);
                } else {
                    ToggleProvinceRegionSurface(provinceIndex, r, visible, color);
                }
            }
        }


        /// <summary>
        /// Colorize main region of specified province and assings a texture to main region with options.
        /// </summary>
        public GameObject ToggleProvinceMainRegionSurface(int provinceIndex, bool visible, Color color) {
            if (!ValidProvinceIndex(provinceIndex)) return null;
            Province province = _provinces[provinceIndex];
            EnsureProvinceDataIsLoaded(province);
            return ToggleProvinceRegionSurface(provinceIndex, province.mainRegionIndex, visible, color);
        }


        /// <summary>
        /// Highlights the province region specified.
        /// Internally used by the Editor component, but you can use it as well to temporarily mark a province region.
        /// </summary>
        /// <param name="refreshGeometry">Pass true only if you're sure you want to force refresh the geometry of the highlight (for instance, if the frontiers data has changed). If you're unsure, pass false.</param>
        public GameObject ToggleProvinceRegionSurfaceHighlight(int provinceIndex, int regionIndex, Color color) {
            if (!ValidProvinceRegionIndex(provinceIndex, regionIndex)) return null;
            Material mat = Instantiate(hudMatProvince);
            if (disposalManager != null)
                disposalManager.MarkForDisposal(mat);
            mat.color = color;
            mat.renderQueue--;
            Region region = provinces[provinceIndex].regions[regionIndex];
            GameObject surf = region.surface;
            if (surf != null) {
                surf.SetActive(true);
                surf.GetComponent<Renderer>().sharedMaterial = mat;
            } else {
                surf = GenerateProvinceRegionSurface(provinceIndex, regionIndex, mat);
            }
            return surf;
        }

        /// <summary>
        /// Disables all province regions highlights. This doesn't destroy custom materials.
        /// </summary>
        public void HideProvinceRegionHighlights(bool destroyCachedSurfaces) {
            HideProvinceRegionHighlight();
            if (provinces == null)
                return;
            int provincesLength = provinces.Length;
            for (int c = 0; c < provincesLength; c++) {
                Province province = provinces[c];
                if (province == null || province.regions == null)
                    continue;
                int regionsCount = province.regions.Count;
                for (int cr = 0; cr < regionsCount; cr++) {
                    Region region = province.regions[cr];
                    GameObject surf = region.surface;
                    if (surf != null) {
                        if (destroyCachedSurfaces) {
                            DestroyImmediate(surf);
                        } else {
                            if (region.customMaterial == null) {
                                surf.SetActive(false);
                            } else {
                                ApplyMaterialToSurface(surf, region.customMaterial);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Colorize a region of specified province/state by index in the provinces collection.
        /// </summary>
        public GameObject ToggleProvinceRegionSurface(int provinceIndex, int regionIndex, bool visible, Color color) {
            return ToggleProvinceRegionSurface(provinceIndex, regionIndex, visible, color, null, Misc.Vector2one, Misc.Vector2zero, 0);
        }

        /// <summary>
        /// Colorize specified region of a country by indexes.
        /// </summary>
        public GameObject ToggleProvinceRegionSurface(int provinceIndex, int regionIndex, bool visible, Color color, Texture2D texture, Vector2 textureScale, Vector2 textureOffset, float textureRotation) {

            if (!ValidProvinceRegionIndex(provinceIndex, regionIndex)) return null;

            if (!visible) {
                HideProvinceRegionSurface(provinceIndex, regionIndex);
                return null;
            }

            Province province = provinces[provinceIndex];
            if (province.regions == null) {
                ReadProvincePackedString(province);
                if (province.regions == null)
                    return null;
            }
            Region region = provinces[provinceIndex].regions[regionIndex];
            int cacheIndex = GetCacheIndexForProvinceRegion(provinceIndex, regionIndex);

            // Checks if current cached surface contains a material with a texture, if it exists but it has not texture, destroy it to recreate with uv mappings
            GameObject surf = region.surface;
            if (region.surfaceIsDirty) {
                DestroySafe(surf);
            }

            // Should the surface be recreated?
            Material surfMaterial;
            if (surf != null) {
                surfMaterial = surf.GetComponent<Renderer>().sharedMaterial;
                if (texture != null && (textureScale != region.customTextureScale || textureOffset != region.customTextureOffset || textureRotation != region.customTextureRotation || surfMaterial.name.Equals(coloredMat.name))) {
                    DestroyImmediate(surf);
                    surf = null;
                }
            }
            // If it exists, activate and check proper material, if not create surface
            bool isHighlighted = provinceHighlightedIndex == provinceIndex && (provinceRegionHighlightedIndex == regionIndex || _highlightAllProvinceRegions) && _enableProvinceHighlight && province.allowHighlight && _countries[province.countryIndex].allowProvincesHighlight;
            Material goodMaterial = GetColoredTexturedMaterial(color, texture, true, 1);
            if (goodMaterial.renderQueue < 2006) {
                // ensure it renders on top of country highlight
                goodMaterial.renderQueue = 2006;
            }
            if (surf != null) {
                if (!surf.activeSelf)
                    surf.SetActive(true);
                // Check if material is ok
                region.customMaterial = goodMaterial;
                surfMaterial = surf.GetComponent<Renderer>().sharedMaterial;
                if ((texture == null && !surfMaterial.name.Equals(coloredMat.name)) || (texture != null && !surfMaterial.name.Equals(texturizedMat.name))
                    || (surfMaterial.color != color && !isHighlighted) || (texture != null && region.customMaterial.mainTexture != texture)) {
                    ApplyMaterialToSurface(surf, goodMaterial);
                }
            } else {
                surf = GenerateProvinceRegionSurface(provinceIndex, regionIndex, goodMaterial, textureScale, textureOffset, textureRotation);
                region.customMaterial = goodMaterial;
                region.customTextureOffset = textureOffset;
                region.customTextureRotation = textureRotation;
                region.customTextureScale = textureScale;
            }
            // If it was highlighted, highlight it again
            if (region.customMaterial != null && isHighlighted && region.customMaterial.color != hudMatProvince.color) {
                Material clonedMat = Instantiate(region.customMaterial);
                if (disposalManager != null)
                    disposalManager.MarkForDisposal(clonedMat); //.hideFlags = HideFlags.DontSave;
                clonedMat.name = region.customMaterial.name;
                clonedMat.color = hudMatProvince.color;
                ApplyMaterialToSurface(surf, clonedMat);
                provinceRegionHighlightedObj = surf;
            }
            return surf;
        }

        /// <summary>
        /// Hides all colorized regions of all provinces/states.
        /// </summary>
        public void HideProvinceSurfaces() {
            if (provinces == null)
                return;
            for (int p = 0; p < provinces.Length; p++) {
                HideProvinceSurfaces(p);
            }
        }


        /// <summary>
        /// Hides all colorized regions of one province/state.
        /// </summary>
        public void HideProvinceSurfaces(int provinceIndex) {
            if (provinces[provinceIndex].regions == null)
                return;
            int rCount = provinces[provinceIndex].regions.Count;
            for (int r = 0; r < rCount; r++) {
                HideProvinceRegionSurface(provinceIndex, r);
            }
        }

        /// <summary>
        /// Hides all regions of one province.
        /// </summary>
        public void HideProvinceRegionSurface(int provinceIndex, int regionIndex) {
            if (!ValidProvinceRegionIndex(provinceIndex, regionIndex)) return;
            GameObject obj = provinces[provinceIndex].regions[regionIndex].surface;
            if (obj != null)
                obj.SetActive(false);
            provinces[provinceIndex].regions[regionIndex].customMaterial = null;
        }

        /// <summary>
        /// Returns an array of province names. The returning list can be grouped by country.
        /// </summary>
        public string[] GetProvinceNames(bool groupByCountry, bool addProvinceIndex = true) {
            List<string> c = new List<string>(provinces.Length + _countries.Length);
            if (provinces == null)
                return c.ToArray();
            bool[] countriesAdded = new bool[_countries.Length];
            for (int k = 0; k < provinces.Length; k++) {
                Province province = provinces[k];
                if (province != null) { // could be null if country doesn't exist in this level of quality
                    if (groupByCountry) {
                        if (!countriesAdded[province.countryIndex]) {
                            countriesAdded[province.countryIndex] = true;
                            c.Add(_countries[province.countryIndex].name);
                        }
                        if (addProvinceIndex) {
                            c.Add(_countries[province.countryIndex].name + "|" + province.name + " (" + k + ")");
                        } else {
                            c.Add(_countries[province.countryIndex].name + "|" + province.name);
                        }
                    } else {
                        if (addProvinceIndex) {
                            c.Add(province.name + " (" + k + ")");
                        } else {
                            c.Add(province.name);
                        }

                    }
                }
            }
            c.Sort();

            if (groupByCountry) {
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
        /// Returns an array of province names for the specified country.
        /// </summary>
        public string[] GetProvinceNames(int countryIndex, bool addProvinceIndex = true) {
            List<string> c = new List<string>(100);
            if (provinces == null || countryIndex < 0 || countryIndex >= _countries.Length)
                return c.ToArray();
            for (int k = 0; k < _provinces.Length; k++) {
                Province province = _provinces[k];
                if (province.countryIndex == countryIndex) {
                    if (addProvinceIndex) {
                        c.Add(province.name + " (" + k + ")");
                    } else {
                        c.Add(province.name);
                    }
                }
            }
            c.Sort();
            return c.ToArray();
        }

        /// <summary>
        /// Returns an array of province objects for the specified country.
        /// </summary>
        public Province[] GetProvinces(Country country) {
            if (country == null || provinces == null) {
                return null;
            }
            return country.provinces;
        }


        /// <summary>
        /// Returns an array of province objects for the specified country.
        /// </summary>
        public Province[] GetProvinces(int countryIndex) {
            if (countryIndex < 0 || countryIndex >= countries.Length) {
                return null;
            }
            return GetProvinces(_countries[countryIndex]);
        }


        /// <summary>
        /// Returns a list of provinces whose center is contained in a given region
        /// </summary>
        public List<Province> GetProvinces(Region region) {
            int provCount = provinces.Length;
            List<Province> cc = new List<Province>();
            for (int k = 0; k < provCount; k++) {
                if (region.Contains(_provinces[k].center))
                    cc.Add(_provinces[k]);
            }
            return cc;
        }

        /// <summary>
        /// Gets a list of provinces that overlap with a given region
        /// </summary>
        public List<Province> GetProvincesOverlap(Region region) {
            List<Province> rr = new List<Province>();
            int provinceCount = provinces.Length;
            for (int k = 0; k < provinceCount; k++) {
                Province province = _provinces[k];
                EnsureProvinceDataIsLoaded(province);
                if (province.regions == null || rr.Contains(province))
                    continue;
                int rCount = province.regions.Count;
                for (int r = 0; r < rCount; r++) {
                    Region otherRegion = province.regions[r];
                    if (region.Intersects(otherRegion)) {
                        rr.Add(province);
                        break;
                    }
                }
            }
            return rr;
        }



        /// <summary>
        /// Gets a list of provinces regions that overlap with a given region
        /// </summary>
        public List<Region> GetProvinceRegionsOverlap(Region region) {
            List<Region> rr = new List<Region>();
            int count = provinces.Length;
            for (int k = 0; k < count; k++) {
                Province province = _provinces[k];
                if (!province.regionsRect2D.Overlaps(region.rect2D))
                    continue;
                if (province.regions == null)
                    continue;
                int rCount = province.regions.Count;
                for (int r = 0; r < rCount; r++) {
                    Region otherRegion = province.regions[r];
                    if (otherRegion.points.Length > 0 && region.Intersects(otherRegion)) {
                        rr.Add(otherRegion);
                    }
                }
            }
            return rr;
        }



        /// <summary>
        /// Delete all provinces from specified continent. This operation does not include dependencies.
        /// </summary>
        public void ProvincesDeleteOfSameContinent(string continentName) {
            HideProvinceRegionHighlights(true);
            if (provinces == null)
                return;
            int numProvinces = _provinces.Length;
            List<Province> newProvinces = new List<Province>(numProvinces);
            for (int k = 0; k < numProvinces; k++) {
                if (_provinces[k] != null) {
                    int c = _provinces[k].countryIndex;
                    if (!_countries[c].continent.Equals(continentName)) {
                        newProvinces.Add(_provinces[k]);
                    }
                }
            }
            provinces = newProvinces.ToArray();
        }


        /// <summary>
        /// Returns a list of provinces whose attributes matches predicate
        /// </summary>
        public List<Province> GetProvinces(AttribPredicate predicate) {
            List<Province> selectedProvinces = new List<Province>();
            int provinceCount = provinces.Length;
            for (int k = 0; k < provinceCount; k++) {
                Province province = _provinces[k];
                if (predicate(province.attrib))
                    selectedProvinces.Add(province);
            }
            return selectedProvinces;
        }

        /// <summary>
        /// Returns a list of provinces that are visible in the Game View
        /// </summary>
        public List<Province> GetVisibleProvinces() {
            Camera cam = Application.isPlaying ? currentCamera : Camera.current;
            return GetVisibleProvinces(cam);
        }

        /// <summary>
        /// Returns a list of provinces that are visible (front facing camera)
        /// </summary>
        public List<Province> GetVisibleProvinces(Camera camera) {
            if (camera == null)
                return null;
            if (provinces == null)
                return null;
            List<Country> vc = GetVisibleCountries();
            List<Province> vp = new List<Province>(30);
            for (int k = 0; k < vc.Count; k++) {
                Country country = vc[k];
                if (country.provinces == null)
                    continue;
                for (int p = 0; p < country.provinces.Length; p++) {
                    Province prov = country.provinces[p];
                    Vector3 center = transform.TransformPoint(prov.center);
                    Vector3 vpos = camera.WorldToViewportPoint(center);
                    if (vpos.x >= 0 && vpos.x <= 1 && vpos.y >= 0 && vpos.y <= 1) {
                        vp.Add(prov);
                    }
                }
            }
            return vp;
        }

        /// <summary>
        /// Returns a list of provinces that are visible inside the window rectangle (constraint rect)
        /// </summary>
        public List<Province> GetVisibleProvincesInWindowRect() {
            if (provinces == null)
                return null;
            List<Country> vc = GetVisibleCountriesInWindowRect();
            List<Province> vp = new List<Province>(30);
            for (int k = 0; k < vc.Count; k++) {
                Country country = vc[k];
                if (country.provinces == null)
                    continue;
                for (int p = 0; p < country.provinces.Length; p++) {
                    Province prov = country.provinces[p];
                    if (_windowRect.Contains(prov.center)) {
                        vp.Add(prov);
                    }
                }
            }
            return vp;
        }

        /// <summary>
        /// Returns the center of the province
        /// </summary>
        /// <param name="provinceIndex"></param>
        /// <returns></returns>
        public Vector2 GetProvinceCenter(int provinceIndex) {
            if (!CheckProvinceIndex(provinceIndex)) return Misc.Vector2zero;
            Province province = provinces[provinceIndex];
            EnsureProvinceDataIsLoaded(province);
            return province.center;
        }


        /// <summary>
        /// Returns the centroid of the province
        /// </summary>
        public Vector2 GetProvinceCentroid(int provinceIndex) {
            if (!CheckProvinceIndex(provinceIndex)) return Misc.Vector2zero;
            Province province = provinces[provinceIndex];
            EnsureProvinceDataIsLoaded(province);
            return province.centroid;
        }


        /// <summary>
        /// Returns the centroid of the province
        /// </summary>
        public Vector2 GetProvinceCentroid(Province province) {
            if (province == null) return Misc.Vector2zero;
            EnsureProvinceDataIsLoaded(province);
            return province.centroid;
        }


        /// <summary>
        /// Returns the list of costal positions of a given province
        /// </summary>
        public List<Vector2> GetProvinceCoastalPoints(int provinceIndex, float minDistance = 0.005f) {
            List<Vector2> coastalPoints = new List<Vector2>();
            minDistance *= minDistance;
            if (provinces[provinceIndex].regions == null)
                ReadProvincePackedString(provinces[provinceIndex]);
            for (int r = 0; r < provinces[provinceIndex].regions.Count; r++) {
                Region region = provinces[provinceIndex].regions[r];
                for (int p = 0; p < region.points.Length; p++) {
                    Vector2 position = region.points[p];
                    Vector2 dummy;
                    if (ContainsWater(position, 4, out dummy)) {
                        bool valid = true;
                        for (int s = coastalPoints.Count - 1; s >= 0; s--) {
                            float sqrDist = FastVector.SqrDistanceByValue(coastalPoints[s], position);
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
            }
            return coastalPoints;
        }


        /// <summary>
        /// Returns a list of common border points between two provinces.
        /// <param name="provinceIndex1">Index of the first province</param>
        /// <param name="provinceIndex2">Index of the second province</param>
        /// <param name="extraWidth">Use extraWidth to widen the points, useful when using the result to block pass in pathfinding</param>
        /// <param name="pointIndices">Optional list in which the index of the points in the polygon vertex list are returned. You can use these indices to detect if points are sequential or if they are separated or belong to different segments.</param>
        /// </summary>
        public List<Vector2> GetProvinceBorderPoints(int provinceIndex1, int provinceIndex2, int extraWidth = 0, List<int> pointIndices = null) {

            if (provinceIndex1 < 0 || provinceIndex1 >= provinces.Length || provinceIndex2 < 0 || provinceIndex2 >= provinces.Length)
                return null;

            Province province1 = provinces[provinceIndex1];
            if (province1.regions == null) {
                ReadProvincePackedString(province1);
            }
            Province province2 = provinces[provinceIndex2];
            if (province2.regions == null) {
                ReadProvincePackedString(province2);
            }

            List<Vector2> samePoints = new List<Vector2>();
            bool requireIndices = pointIndices != null;
            if (requireIndices) {
                pointIndices.Clear();
            }

            int regionsCount = province1.regions.Count;
            for (int cr = 0; cr < regionsCount; cr++) {
                Region region1 = province1.regions[cr];
                int region1PointsCount = region1.points.Length;
                int neighboursCount = region1.neighbours.Count;
                for (int n = 0; n < neighboursCount; n++) {
                    Region otherRegion = region1.neighbours[n];
                    if (province2.regions.Contains(otherRegion)) {
                        int otherRegionPointsCount = otherRegion.points.Length;
                        for (int p = 0; p < region1PointsCount; p++) {
                            for (int o = 0; o < otherRegionPointsCount; o++) {
                                if (region1.points[p] == otherRegion.points[o]) {
                                    samePoints.Add(region1.points[p]);
                                    if (requireIndices) {
                                        pointIndices.Add(p);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Adds optional width to the line
            if (extraWidth > 0) {
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
            }

            return samePoints;
        }


        /// <summary>
        /// Returns the points for the given province region. Optionally in world space coordinates (normal map, not viewport).
        /// </summary>
        public Vector3[] GetProvinceBorderPoints(int provinceIndex, bool worldSpace) {
            if (provinceIndex < 0 || provinceIndex >= provinces.Length)
                return null;
            return GetProvinceBorderPoints(provinceIndex, provinces[provinceIndex].mainRegionIndex, worldSpace);
        }

        /// <summary>
        /// Returns the points for the given province region. Optionally in world space coordinates (normal map, not viewport).
        /// </summary>
        public Vector3[] GetProvinceBorderPoints(int provinceIndex, int regionIndex, bool worldSpace) {
            if (provinceIndex < 0 || provinceIndex >= provinces.Length)
                return null;
            Province province = provinces[provinceIndex];
            EnsureProvinceDataIsLoaded(province);
            if (regionIndex < 0 || regionIndex >= province.regions.Count)
                return null;

            Region region = provinces[provinceIndex].regions[regionIndex];
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
        /// Returns the zoom level required to show the entire province region on screen
        /// </summary>
        /// <returns>The province zoom level of -1 if error.</returns>
        /// <param name="provinceIndex">Province index.</param>
        public float GetProvinceRegionZoomExtents(int provinceIndex) {
            if (provinceIndex < 0 || provinces == null || provinceIndex >= provinces.Length)
                return -1;
            return GetProvinceRegionZoomExtents(provinceIndex, provinces[provinceIndex].mainRegionIndex);
        }

        /// <summary>
        /// Returns the zoom level required to show the entire province region on screen
        /// </summary>
        /// <returns>The province zoom level of -1 if error.</returns>
        /// <param name="provinceIndex">Country index.</param>
        /// <param name="regionIndex">Region index of the country.</param>
        public float GetProvinceRegionZoomExtents(int provinceIndex, int regionIndex) {
            if (provinceIndex < 0 || provinces == null || provinceIndex >= provinces.Length)
                return -1;
            Province province = provinces[provinceIndex];
            EnsureProvinceDataIsLoaded(province);
            if (regionIndex < 0 || regionIndex >= province.regions.Count)
                return -1;
            Region region = province.regions[regionIndex];
            return GetFrustumZoomLevel(region.rect2D.width * mapWidth, region.rect2D.height * mapHeight);
        }

        /// <summary>
        /// Returns the zoom level required to show the entire province (including all regions) on screen
        /// </summary>
        /// <returns>The province zoom level of -1 if error.</returns>
        /// <param name="provinceIndex">Province index.</param>
        public float GetProvinceZoomExtents(int provinceIndex) {
            if (provinceIndex < 0 || provinces == null || provinceIndex >= provinces.Length)
                return -1;
            Province province = provinces[provinceIndex];
            EnsureProvinceDataIsLoaded(province);
            return GetFrustumZoomLevel(province.regionsRect2D.width * mapWidth, province.regionsRect2D.height * mapHeight);
        }

        /// <summary>
        /// Checks quality of province's polygon points. Useful before using polygon clipping operations.
        /// </summary>
        /// <returns><c>true</c>, if province was changed, <c>false</c> otherwise.</returns>
        public bool ProvinceSanitize(int provinceIndex, int minimumPoints = 3, bool refresh = true) {
            if (provinceIndex < 0 || provinceIndex >= provinces.Length)
                return false;

            Province province = provinces[provinceIndex];
            bool changes = false;
            if (province.regions != null) {
                for (int k = 0; k < province.regions.Count; k++) {
                    Region region = province.regions[k];
                    if (RegionSanitize(region)) {
                        changes = true;
                    }
                    if (region.points.Length < minimumPoints) {
                        province.regions.Remove(region);
                        if (province.regions == null) {
                            return true;
                        }
                        k--;
                        changes = true;
                    }
                }
            }
            if (changes) {
                province.mainRegionIndex = 0;
                if (refresh) {
                    RefreshProvinceGeometry(provinceIndex);
                }
            }
            return changes;
        }

        /// <summary>
        /// Makes provinceIndex absorb another province providing any of its regions. All regions are transfered to target province.
        /// This function is quite slow with high definition frontiers.
        /// </summary>
        /// <param name="provinceIndex">Province index of the conquering province.</param>
        /// <param name="sourceRegion">Source region of the loosing province.</param>
        /// <param name="redraw">If set to true, map will be redrawn after operation finishes.</param>
        public bool ProvinceTransferProvinceRegion(int provinceIndex, Region sourceProvinceRegion, bool redraw) {
            if (sourceProvinceRegion == null) return false;
            Province sourceProvince = (Province)sourceProvinceRegion.entity;
            int sourceProvinceIndex = GetProvinceIndex(sourceProvince);
            if (provinceIndex < 0 || sourceProvinceIndex < 0 || provinceIndex == sourceProvinceIndex)
                return false;

            // Transfer cities
            Province targetProvince = provinces[provinceIndex];
            if (sourceProvince.countryIndex != targetProvince.countryIndex) {
                // Transfer source province to target country province
                return CountryTransferProvinceRegion(targetProvince.countryIndex, sourceProvinceRegion, redraw);
            }

            sourceProvince.DestroySurfaces();
            targetProvince.DestroySurfaces();

            int cityCount = cities.Length;
            for (int k = 0; k < cityCount; k++) {
                if (_cities[k].countryIndex == sourceProvince.countryIndex && _cities[k].province.Equals(sourceProvince.name))
                    _cities[k].province = targetProvince.name;
            }

            // Transfer mount points
            int mountPointCount = mountPoints.Count;
            for (int k = 0; k < mountPointCount; k++) {
                if (mountPoints[k].provinceIndex == sourceProvinceIndex)
                    mountPoints[k].provinceIndex = provinceIndex;
            }

            // Transfer regions
            int sprCount = sourceProvince.regions.Count;
            if (sprCount > 0) {
                List<Region> targetRegions = new List<Region>(targetProvince.regions);
                for (int k = 0; k < sprCount; k++) {
                    targetRegions.Add(sourceProvince.regions[k]);
                }
                targetProvince.regions = targetRegions;
            }

            // Fusion any adjacent regions that results from merge operation
            MergeAdjacentRegions(targetProvince);
            RegionSanitize(targetProvince.regions);
            targetProvince.mainRegionIndex = 0; // will be updated on RefreshProvinceDefinition

            // Finish operation
            ProvinceDelete(sourceProvinceIndex);
            if (provinceIndex > sourceProvinceIndex)
                provinceIndex--;
            RefreshProvinceDefinition(provinceIndex, true);
            ResortCountryProvinces(provinces[provinceIndex].countryIndex);
            if (redraw)
                Redraw(true);
            return true;
        }


        /// <summary>
        /// Makes provinceIndex absorb an hexagonal portion of the map. If that portion belong to another province, it will be substracted from that province as well.
        /// This function is quite slow with high definition frontiers.
        /// </summary>
        /// <param name="provinceIndex">Province index of the conquering province.</param>
        /// <param name="cellIndex">Index of the cell to add to the province.</param>
        public bool ProvinceTransferCell(int provinceIndex, int cellIndex, bool redraw = true) {
            if (provinceIndex < 0 || cellIndex < 0 || cells == null || cellIndex >= cells.Length)
                return false;

            // Start process
            Province province = provinces[provinceIndex];
            Cell cell = cells[cellIndex];
            int countryIndex = province.countryIndex;

            // Create a region for the cell
            Region sourceRegion = new Region(province, province.regions.Count);
            sourceRegion.UpdatePointsAndRect(cell.points, true);

            // Transfer cities
            List<City> citiesInCell = GetCities(sourceRegion);
            int cityCount = citiesInCell.Count;
            for (int k = 0; k < cityCount; k++) {
                City city = citiesInCell[k];
                if (city.countryIndex != countryIndex) {
                    city.countryIndex = countryIndex;
                    city.province = province.name;
                }
            }

            // Transfer mount points
            List<MountPoint> mountPointsInCell = GetMountPoints(sourceRegion);
            int mountPointCount = mountPointsInCell.Count;
            for (int k = 0; k < mountPointCount; k++) {
                MountPoint mp = mountPointsInCell[k];
                if (mp.countryIndex != countryIndex) {
                    mp.countryIndex = countryIndex;
                    mp.provinceIndex = provinceIndex;
                }
            }

            // Add region to target country's polygon - only if the country is touching or crossing target country frontier
            Region targetRegion = province.mainRegion;
            if (targetRegion != null && sourceRegion.Intersects(targetRegion)) {
                RegionMagnet(sourceRegion, targetRegion);
                Clipper clipper = new Clipper();
                clipper.AddPath(targetRegion, PolyType.ptSubject);
                clipper.AddPath(sourceRegion, PolyType.ptClip);
                clipper.Execute(ClipType.ctUnion);
            } else {
                // Add new region to country
                sourceRegion.entity = province;
                sourceRegion.regionIndex = province.regions.Count;
                province.regions.Add(sourceRegion);
            }

            // Fusion any adjacent regions that results from merge operation
            MergeAdjacentRegions(province);
            RegionSanitize(province.regions);

            // Finish operation with the country
            RefreshProvinceGeometry(provinceIndex);

            // Substract cell region from any other country
            List<Province> otherProvinces = GetProvincesOverlap(sourceRegion);
            int orCount = otherProvinces.Count;
            for (int k = 0; k < orCount; k++) {
                Province otherProvince = otherProvinces[k];
                if (otherProvince == province)
                    continue;
                Clipper clipper = new Clipper();
                clipper.AddPaths(otherProvince.regions, PolyType.ptSubject);
                clipper.AddPath(sourceRegion, PolyType.ptClip);
                clipper.Execute(ClipType.ctDifference, otherProvince);
                int otherProvinceIndex = GetProvinceIndex(otherProvince);
                if (otherProvince.regions.Count == 0) {
                    ProvinceDelete(otherProvinceIndex);
                } else {
                    RegionSanitize(otherProvince.regions);
                    RefreshProvinceDefinition(otherProvinceIndex, true);
                }
            }

            if (redraw)
                Redraw();
            return true;
        }

        /// <summary>
        /// Removes a cell from a province.
        /// </summary>
        /// <param name="provinceIndex">Province index.</param>
        /// <param name="cellIndex">Index of the cell to remove from the province.</param>
        public bool ProvinceRemoveCell(int provinceIndex, int cellIndex, bool redraw = true) {
            if (provinceIndex < 0 || provinceIndex >= provinces.Length || cellIndex < 0 || cells == null || cellIndex >= cells.Length)
                return false;

            Province province = provinces[provinceIndex];
            Cell cell = cells[cellIndex];
            Region sourceRegion = new Region(province, province.regions.Count);
            sourceRegion.UpdatePointsAndRect(cell.points, true);

            Clipper clipper = new Clipper();
            clipper.AddPaths(province.regions, PolyType.ptSubject);
            clipper.AddPath(sourceRegion, PolyType.ptClip);
            clipper.Execute(ClipType.ctDifference, province);
            if (province.regions.Count == 0) {
                ProvinceDelete(provinceIndex);
            } else {
                RegionSanitize(province.regions);
                RefreshProvinceGeometry(provinceIndex);
            }

            OptimizeFrontiers();

            if (redraw)
                Redraw();
            return true;
        }


        /// <summary>
        /// Returns the colored surface (game object) of a province. If it has not been colored yet, it will generate it.
        /// </summary>
        public GameObject GetProvinceRegionSurfaceGameObject(int provinceIndex, int regionIndex, bool forceCreation = false) {
            if (!ValidProvinceRegionIndex(provinceIndex, regionIndex)) return null;
            GameObject surf = provinces[provinceIndex].regions[regionIndex].surface;
            if (surf == null && forceCreation) {
                surf = ToggleProvinceRegionSurface(provinceIndex, regionIndex, true, Misc.ColorClear);
            }
            return surf;
        }


        /// <summary>
        /// Draws the labels for the provinces of a given country
        /// </summary>
        /// <param name="country">Country.</param>
        public void DrawProvinceLabels(int countryIndex, Color color = default(Color)) {
            if (countryIndex < 0 || countryIndex >= countries.Length)
                return;
            DrawProvinceLabels(_countries[countryIndex], color);
        }

        /// <summary>
        /// Draws the labels for the provinces of a given country
        /// </summary>
        /// <param name="country">Country.</param>
        /// <param name="color">Color override for the province labels</param>
        public void DrawProvinceLabels(Country country, Color color = default(Color)) {
            DrawProvinceLabelsInt(country, color);
        }

        /// <summary>
        /// Draws the labels for the provinces of a given country
        /// </summary>
        /// <param name="countryName">Name of country.</param>
        /// <param name="color">Color override for the province labels</param>
        public void DrawProvinceLabels(string countryName, Color color = default(Color)) {
            int countryIndex = GetCountryIndex(countryName);
            DrawProvinceLabels(countryIndex, color);
        }


        /// <summary>
        /// Hides all province labels
        /// </summary>
        public void HideProvinceLabels() {
            DestroyProvinceLabels();
        }

        #endregion

        #region IO functions area

        /// <summary>
        /// Exports the geographic data in packed string format.
        /// </summary>
        public string GetProvinceGeoData() {

            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < provinces.Length; k++) {
                Province province = provinces[k];
                int countryIndex = province.countryIndex;
                if (countryIndex < 0 || countryIndex >= countries.Length)
                    continue;
                string countryName = countries[countryIndex].name;
                if (k > 0)
                    sb.Append("|");
                sb.Append(province.name);
                sb.Append("$");
                sb.Append(countryName);
                sb.Append("$");
                EnsureProvinceDataIsLoaded(province);
                if (province.regions != null) {
                    int provinceRegionsCount = province.regions.Count;
                    for (int r = 0; r < provinceRegionsCount; r++) {
                        if (r > 0)
                            sb.Append("*");
                        Region region = province.regions[r];
                        for (int p = 0; p < region.points.Length; p++) {
                            if (p > 0)
                                sb.Append(";");
                            int x = (int)(region.points[p].x * WMSK.MAP_PRECISION);
                            int y = (int)(region.points[p].y * WMSK.MAP_PRECISION);
                            sb.Append(x.ToString(Misc.InvariantCulture));
                            sb.Append(",");
                            sb.Append(y.ToString(Misc.InvariantCulture));
                        }
                    }
                }
                sb.Append("$");
                sb.Append(province.uniqueId);
            }
            return sb.ToString();
        }


        public void SetProvincesGeoData(string s) {
            if (_countries == null) return;
            string[] provincesPackedStringData = s.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            int provinceCount = provincesPackedStringData.Length;
            List<Province> newProvinces = new List<Province>(provinceCount);
            List<Province>[] countryProvinces = new List<Province>[_countries.Length];
            char[] separatorProvinces = new char[] { '$' };
            for (int k = 0; k < provinceCount; k++) {
                string[] provinceInfo = provincesPackedStringData[k].Split(separatorProvinces);
                if (provinceInfo.Length <= 2)
                    continue;
                string name = provinceInfo[0];
                string countryName = provinceInfo[1];
                int countryIndex = GetCountryIndex(countryName);
                if (countryIndex >= 0) {
                    int uniqueId;
                    if (provinceInfo.Length >= 4) {
                        uniqueId = int.Parse(provinceInfo[3]);
                    } else {
                        uniqueId = GetUniqueId(new List<IExtendableAttribute>(newProvinces.ToArray()));
                    }
                    Province province = new Province(name, countryIndex, uniqueId);
                    province.packedRegions = provinceInfo[2];
                    newProvinces.Add(province);
                    if (countryProvinces[countryIndex] == null) {
                        countryProvinces[countryIndex] = new List<Province>(50);
                    }
                    countryProvinces[countryIndex].Add(province);
                }
            }
            provinces = newProvinces.ToArray();
            for (int k = 0; k < _countries.Length; k++) {
                if (countryProvinces[k] != null) {
                    countryProvinces[k].Sort(ProvinceSizeComparer);
                    _countries[k].provinces = countryProvinces[k].ToArray();
                }
            }
        }

        /// <summary>
        /// Gets XML attributes of all provinces in jSON format.
        /// </summary>
        public string GetProvincesAttributes(bool prettyPrint = true) {
            return GetProvincesAttributes(new List<Province>(provinces), prettyPrint);
        }

        /// <summary>
        /// Gets XML attributes of provided provinces in jSON format.
        /// </summary>
        public string GetProvincesAttributes(List<Province> provinces, bool prettyPrint = true) {
            JSONObject composed = new JSONObject();
            int provinceCount = provinces.Count;
            for (int k = 0; k < provinceCount; k++) {
                Province province = _provinces[k];
                if (province.attrib.keys != null)
                    composed.AddField(province.uniqueId.ToString(), province.attrib);
            }
            return composed.Print(prettyPrint);
        }

        /// <summary>
        /// Sets provinces attributes from a jSON formatted string.
        /// </summary>
        public void SetProvincesAttributes(string jSON) {
            JSONObject composed = new JSONObject(jSON);
            if (composed.keys == null)
                return;
            int keyCount = composed.keys.Count;
            for (int k = 0; k < keyCount; k++) {
                int uniqueId = int.Parse(composed.keys[k]);
                int provinceIndex = GetProvinceIndex(uniqueId);
                if (provinceIndex >= 0) {
                    provinces[provinceIndex].attrib = composed[k];
                }
            }
        }


        #endregion

        #region Province Color Map Methods

        /// <summary>
        /// Analizes and generate new countries based on a color map
        /// </summary>
        /// <returns>The provinces color map.</returns>
        /// <param name="tex">Tex.</param>
        public int ImportProvincesColorMap(string filename) {
            if (!File.Exists(filename))
                return 0;

            // Load texture data
            byte[] fileData = File.ReadAllBytes(filename);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(fileData);

            Color[] texColors = tex.GetPixels();
            int width = tex.width;
            int height = tex.height;
            int provCount = provinces.Length;
            int countryCount = 0;
            // Map provinces to new countries
            Dictionary<Color, List<Province>> colorsAndProvinces = new Dictionary<Color, List<Province>>();
            for (int prov = 0; prov < provCount; prov++) {
                // gets the color of this province in the texture
                Province province = _provinces[prov];
                EnsureProvinceDataIsLoaded(province);
                Vector2 localPos = province.center;
                bool pointInside = false;
                Color countryColor = Misc.ColorBlack;
                for (int t = 0; t < 100; t++) {
                    if (province.mainRegion.Contains(localPos)) {
                        Vector2 texCoords = Conversion.ConvertToTextureCoordinates(localPos, width, height);
                        int colorIndex = (int)(texCoords.y * width + texCoords.x);
                        countryColor = texColors[colorIndex];
                        if (countryColor.a > 0.5f) {
                            pointInside = true;
                            break;
                        }
                    }
                    localPos.x = UnityEngine.Random.Range(province.mainRegion.rect2D.xMin, province.mainRegion.rect2D.xMax);
                    localPos.y = UnityEngine.Random.Range(province.mainRegion.rect2D.yMin, province.mainRegion.rect2D.yMax);
                }
                if (!pointInside)
                    continue;

                List<Province> countryProvinces;
                if (!colorsAndProvinces.TryGetValue(countryColor, out countryProvinces)) {
                    countryProvinces = new List<Province>();
                    colorsAndProvinces[countryColor] = countryProvinces;
                    countryCount++;
                }
                countryProvinces.Add(province);
            }

            // Generate new countries
            Country[] newCountries = new Country[countryCount];
            int countryIndex = 0;
            foreach (KeyValuePair<Color, List<Province>> kvp in colorsAndProvinces) {
                Color color = kvp.Key;
                List<Province> provinces = kvp.Value;

                // Try to get the continent from first province
                Province province = provinces[0];
                string continent = "Continent";
                Country country = GetCountry(province.center);
                if (country != null) {
                    continent = country.continent;
                }
                Country newCountry = new Country("Country " + countryIndex, continent, GetUniqueId(new List<IExtendableAttribute>(newCountries)));
                newCountries[countryIndex] = newCountry;
                int pCount = provinces.Count;
                for (int p = 0; p < pCount; p++) {
                    province = provinces[p];
                    province.countryIndex = countryIndex;
                    newCountry.provinces = provinces.ToArray();
                    int regCount = province.regions.Count;
                    for (int pr = 0; pr < regCount; pr++) {
                        Region reg = province.regions[pr].Clone();
                        newCountry.regions.Add(reg);
                    }
                }
                countryIndex++;
            }

            // Assign new countries
            countries = newCountries;

            // Merge adjacent regions
            for (int k = 0; k < _countries.Length; k++) {
                MergeAdjacentRegions(_countries[k]);
                CountrySanitize(k);
                RefreshCountryGeometry(_countries[k]);
            }

            // Remap cities & mount points
            int citiesCount = cities.Length;
            List<City> newCities = new List<City>(citiesCount);
            for (int c = 0; c < citiesCount; c++) {
                int pi = GetProvinceIndex(_cities[c].unity2DLocation);
                if (pi >= 0) {
                    _cities[c].province = _provinces[pi].name;
                    _cities[c].countryIndex = _provinces[pi].countryIndex;
                    newCities.Add(_cities[c]);
                } else {
                    int ci = GetCountryIndex(_cities[c].unity2DLocation);
                    if (ci >= 0) {
                        _cities[c].countryIndex = ci;
                        _cities[c].province = "";
                        newCities.Add(_cities[c]);
                    }
                }
            }
            cities = newCities.ToArray();

            int mountpointsCount = mountPoints.Count;
            List<MountPoint> newMountpoints = new List<MountPoint>(mountpointsCount);
            for (int c = 0; c < mountpointsCount; c++) {
                int ci = GetCountryIndex(_cities[c].unity2DLocation);
                if (ci >= 0) {
                    mountPoints[c].countryIndex = ci;
                    newMountpoints.Add(mountPoints[c]);
                }
            }
            mountPoints = newMountpoints;

            Redraw(true);

            return countryIndex;
        }

        /// <summary>
        /// Returns province data (geodata and any attributes) in jSON format.
        /// </summary>
        public string GetProvincesDataJSON(bool prettyPrint = true) {
            ProvincesJSONData exported = new ProvincesJSONData();
            for (int k = 0; k < provinces.Length; k++) {
                Province province = provinces[k];
                ProvinceJSON pjson = new ProvinceJSON();
                pjson.name = province.name;
                pjson.countryName = countries[province.countryIndex].name;
                if (province.regions != null) {
                    pjson.regions = new List<RegionJSON>();
                    foreach (Region region in province.regions) {
                        RegionJSON regionJSON = new RegionJSON();
                        regionJSON.points = region.points;
                        pjson.regions.Add(regionJSON);
                    }
                }
                pjson.uniqueId = province.uniqueId;
                pjson.attrib = province.attrib;
                exported.provinces.Add(pjson);
            }
            return JsonUtility.ToJson(exported, prettyPrint);
        }

        /// <summary>
        /// Sets province data (geodata and attributes) from a jSON string.
        /// </summary>
        public void SetProvincesDataJSON(string json) {
            ProvincesJSONData imported = JsonUtility.FromJson<ProvincesJSONData>(json);
            int provinceCount = imported.provinces.Count;
            List<Province> newProvinces = new List<Province>(provinceCount);
            List<Province>[] countryProvinces = new List<Province>[_countries.Length];

            for (int k = 0; k < provinceCount; k++) {
                ProvinceJSON pjson = imported.provinces[k];
                string name = pjson.name;
                string countryName = pjson.countryName;
                int countryIndex = GetCountryIndex(countryName);
                if (countryIndex >= 0) {
                    int uniqueId = pjson.uniqueId;
                    Province province = new Province(pjson.name, countryIndex, uniqueId);

                    List<RegionJSON> regions = pjson.regions;
                    int regionCount = regions.Count;
                    province.regions = new List<Region>(regionCount);
                    float maxVol = float.MinValue;
                    Vector2 minProvince = Misc.Vector2one * 10;
                    Vector2 maxProvince = -minProvince;
                    for (int r = 0; r < regionCount; r++) {
                        Vector2[] coordinates = regions[r].points;
                        int coorCount = coordinates.Length;
                        Vector2 min = Misc.Vector2one * 10;
                        Vector2 max = -min;
                        Region provinceRegion = new Region(province, province.regions.Count);
                        provinceRegion.points = new Vector2[coorCount];
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
                            provinceRegion.points[c].x = x;
                            provinceRegion.points[c].y = y;
                        }
                        FastVector.Average(ref min, ref max, ref provinceRegion.center);
                        provinceRegion.sanitized = true;
                        province.regions.Add(provinceRegion);

                        // Calculate province bounding rect
                        if (min.x < minProvince.x)
                            minProvince.x = min.x;
                        if (min.y < minProvince.y)
                            minProvince.y = min.y;
                        if (max.x > maxProvince.x)
                            maxProvince.x = max.x;
                        if (max.y > maxProvince.y)
                            maxProvince.y = max.y;
                        provinceRegion.rect2D = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
                        provinceRegion.rect2DArea = provinceRegion.rect2D.width * provinceRegion.rect2D.height;
                        float vol = FastVector.SqrDistance(ref min, ref max); // (max - min).sqrMagnitude;
                        if (vol > maxVol) {
                            maxVol = vol;
                            province.mainRegionIndex = r;
                            province.center = provinceRegion.center;
                        }
                    }
                    province.regionsRect2D = new Rect(minProvince.x, minProvince.y, Math.Abs(maxProvince.x - minProvince.x), Mathf.Abs(maxProvince.y - minProvince.y));
                    newProvinces.Add(province);
                    if (countryProvinces[countryIndex] == null) {
                        countryProvinces[countryIndex] = new List<Province>(50);
                    }
                    countryProvinces[countryIndex].Add(province);
                }
            }

            provinces = newProvinces.ToArray();
            for (int k = 0; k < _countries.Length; k++) {
                if (countryProvinces[k] != null) {
                    countryProvinces[k].Sort(ProvinceSizeComparer);
                    _countries[k].provinces = countryProvinces[k].ToArray();
                }
            }
        }

        #endregion

    }

}