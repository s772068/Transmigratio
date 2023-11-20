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
        int countryIndex = map.data.MaxMapParamIndexes[paramiterIndex].Value;
        int detailIndex = map.data.Countries[countryIndex].Paramiters[paramiterIndex].MaxValueIndex;
        float maxValue = map.data.Countries[countryIndex].Paramiters[paramiterIndex].Value[detailIndex];

        if (maxValue == 0) return;

        Color color;
        for (int i = 0; i < map.data.Countries.Length; ++i) {
            detailIndex = map.data.Countries[i].Paramiters[paramiterIndex].MaxValueIndex;
            float value = map.data.Countries[i].Paramiters[paramiterIndex].Value[detailIndex];
            color = Color.HSVToRGB(value / maxValue * 7f / 9f, 1f, 1f);
            map.data.Countries[i].Color = color;
            wmsk.CountryPainting(i, color);
        }
    }
}
