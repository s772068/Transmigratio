using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayersController : BaseController {
    private MapController map;
    private WmskController wmsk;

    public override GameController GameController {
        set {
            map = value.Get<MapController>();
            wmsk = value.Get<WmskController>();
        }
    }

    public void MapPainting(int paramiterIndex) {
        int countryIndex = map.maxMapParamIndexes[paramiterIndex].Value;
        int detailIndex = map.countries[countryIndex].Paramiters[paramiterIndex].MaxValueIndex;
        float maxValue = map.countries[countryIndex].Paramiters[paramiterIndex].Value[detailIndex];

        if (maxValue == 0) return;

        Color color;
        for (int i = 0; i < map.countries.Length; ++i) {
            detailIndex = map.countries[i].Paramiters[paramiterIndex].MaxValueIndex;
            float value = map.countries[i].Paramiters[paramiterIndex].Value[detailIndex];
            color = Color.HSVToRGB(value / maxValue * 7f / 9f, 1f, 1f);
            map.countries[i].Color = color;
            wmsk.CountryPainting(i, color);
        }
    }
}
