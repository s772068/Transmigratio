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

    public void Painting(int maxValue, int[] paramiters) {
        // int regionIndex = map.data.MaxMapParamIndexes[paramiterIndex].Value;
        // int detailIndex = map.data.Regions[regionIndex].Paramiters[paramiterIndex].MaxValueIndex;
        // float maxValue = map.data.Regions[regionIndex].Paramiters[paramiterIndex].Value[detailIndex];

        if (maxValue == 0) return;

        Color color;
        
        for (int i = 0; i < paramiters.Length; ++i) {
            color = Color.HSVToRGB(paramiters[i] / maxValue * 7f / 9f, 1f, 1f);
            map.data.Regions[i].Color = color;
            wmsk.RegionPainting(i, color);
        }
    }
}
