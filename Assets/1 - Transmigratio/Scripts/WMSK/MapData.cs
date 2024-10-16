using WorldMapStrategyKit;
using UnityEngine;
using System;

public static class MapData {
    private static WMSK _wmsk;
    private static Texture2D _borderTexture;
    private static int _regionID;
    private static bool _isClickableMarker = true;

    public static event Action<int> onClickRegion;

    public static int RegionID => _regionID;
    public static WMSK WMSK => _wmsk;

    public static void Init(Texture2D borderTexture) {
        _wmsk = WMSK.instance;

        _wmsk.OnCountryClick += OnClickRegion;
        _wmsk.OnCountryLongClick += OnClickRegion;

        _wmsk.OnMarkerMouseDown += OnMarkerMouseDown;
        _wmsk.OnMarkerMouseEnter += OnMarkerEnter;
        _wmsk.OnMarkerMouseExit += OnMarkerExit;

        _wmsk.OnCountryHighlight += OnCountryHighlight;

        onClickRegion += UpdateRegion;

        _borderTexture = borderTexture;

        foreach (TM_Region region in Transmigratio.Instance.TMDB.map.AllRegions) {
            _wmsk.ToggleCountrySurface(region.Id, true, Color.clear);
        }
    }
    public static void Clear()
    {
        _wmsk.OnCountryClick -= OnClickRegion;
        _wmsk.OnCountryLongClick -= OnClickRegion;

        _wmsk.OnMarkerMouseDown -= OnMarkerMouseDown;
        _wmsk.OnMarkerMouseEnter -= OnMarkerEnter;
        _wmsk.OnMarkerMouseExit -= OnMarkerExit;

        _wmsk.OnCountryHighlight -= OnCountryHighlight;

        onClickRegion -= UpdateRegion;
    }

    public static bool IsClickableMarker {
        get => _isClickableMarker;
        set {
            if (value && !_isClickableMarker) {
                _isClickableMarker = value;
                _wmsk.OnMarkerMouseDown += OnMarkerMouseDown;
            } else if (!value && _isClickableMarker) {
                _isClickableMarker = value;
                _wmsk.OnMarkerMouseDown -= OnMarkerMouseDown;
            }
        }
    }

    public static void UpdateRegion(int index) {
        UnselectRegion();
        _regionID = index;
        SelectRegion();
    }

    public static void SelectRegion() {
        _wmsk.ToggleCountrySurface(_regionID, true, new Color(1, 0.92f, 0.16f, 0.25f));
        _wmsk.ToggleCountryOutline(_regionID, true, _borderTexture, 2, new Color(0.46f, 0.78f, 1, 1));
    }

    public static void UnselectRegion() {
        _wmsk.ToggleCountrySurface(_regionID, true, Color.clear);
        _wmsk.ToggleCountryOutline(_regionID, true, borderWidth: 0.1f, tintColor: _wmsk.frontiersColor);
    }

    private static void OnClickRegion(int countryIndex, int regionIndex, int buttonIndex) {
        onClickRegion?.Invoke(countryIndex);
    }

    private static void OnMarkerEnter(MarkerClickHandler marker) {
        IsClickableMarker = true;
    }

    private static void OnMarkerExit(MarkerClickHandler marker) {
        IsClickableMarker = false;
    }

    private static void OnMarkerMouseDown(MarkerClickHandler marker, int buttonIndex) {
        marker.GetComponent<IconMarker>().Click();
    }

    private static void OnCountryHighlight(int countryIndex, int regionIndex, ref bool allowHighlight) {
        _wmsk.ToggleCountryOutline(countryIndex, true, borderWidth: 0.2f, tintColor: _wmsk.outlineColor);
    }
}
