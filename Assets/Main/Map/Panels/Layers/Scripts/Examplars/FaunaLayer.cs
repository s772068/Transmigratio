public struct FaunaLayer : ILayer {
    public void Show(SettingsController settings, WmskController wmsk, MapController map, int layerIndex) {
        UnityEngine.Color color;
        int count = settings.Theme.CountEcologyDetails(layerIndex);
        for (int i = 0, detailIndex = 0, percent; i < map.data.CountRegions; ++i) {
            percent = map.data.GetRegion(i).GetEcologyParamiter(layerIndex).MaxDetail;
            if (percent < 25) {
                detailIndex = 0;
            } else if (25 <= percent && percent <= 75) {
                detailIndex = 1;
            } else if (75 < percent) {
                detailIndex = 2;
            }
            color = settings.Theme.GetEcologyColor(layerIndex, detailIndex);
            color.a = percent / 100f;
            map.data.GetRegion(i).SetColor(color);
            wmsk.RegionPainting(i, color);
        }
    }
}
