using WorldMapStrategyKit;
using UnityEngine;
using Zenject;

public class MapController : MonoSingleton<MapController> {
    [Inject] private ColorsHolder colorsHolder;

    private int selectedCountryIndex;
    private WMSK map;

    private void ClickMap(float x, float y, int buttonIndex) {
        selectedCountryIndex = map.GetCountryIndex(new Vector2(x, y));
        GameEvents.ClickMap(selectedCountryIndex);
    }

    private void SelectCountry(int index) {
        map.ToggleCountrySurface(selectedCountryIndex, false, Color.clear);
        selectedCountryIndex = index;
        map.ToggleCountrySurface(selectedCountryIndex, true, colorsHolder.SelectCountry);
    }

    private void Awake() {
        map = WMSK.instance;
        map.OnClick += ClickMap;
        GameEvents.OnClickMap += SelectCountry;
    }
}
