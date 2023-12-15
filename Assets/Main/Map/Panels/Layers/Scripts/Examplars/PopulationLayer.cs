public struct PopulationLayer : ILayer {
    public void Show(SettingsController settings, WmskController wmsk, MapController map, int layerIndex) {
        UnityEngine.Color color;
        layerIndex -= settings.Theme.CountEcologyParamiters;
        int count = settings.Theme.CountCivilizationDetails(layerIndex);
        int max = map.data.GetPopulations();
        for (int i = 0, detailIndex = 0, percent; i < map.data.CountRegions; ++i) {
            if (map.data.GetRegion(i).GetCountCivilizations() == 0) return;
            percent = map.data.GetRegion(i).GetAllPopulations() * 100 / max;
            if (percent < 25) {
                detailIndex = 0;
            } else if (25 <= percent && percent <= 75) {
                detailIndex = 1;
            } else if (75 < percent) {
                detailIndex = 2;
            }
            color = settings.Theme.GetEcologyColor(layerIndex, detailIndex);
            color.a = percent / 100;
            map.data.GetRegion(i).SetColor(color);
            wmsk.RegionPainting(i, color);
        }
    }
}
