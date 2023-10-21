using WorldMapStrategyKit;
using UnityEngine;
using System;

public class WmskController : MonoSingleton<WmskController> {
    private WMSK map;
    
    public Action<int> OnClick;

    public int[] GetNeighboursIndexes(int index) {
        System.Collections.Generic.List<Country> neighbours = map.CountryNeighbours(index);
        int[] res = new int[neighbours.Count];
        for (int i = 0; i < neighbours.Count; ++i) {
            res[i] = map.GetCountryIndex(neighbours[i].name);
        }
        return res;
    }

    private void Click(float x, float y, int buttonIndex) {
        OnClick?.Invoke(map.GetCountryIndex(new Vector2(x, y)));
    }

    private void Awake() {
        map = WMSK.instance;
        map.OnClick += Click;
    }
}
