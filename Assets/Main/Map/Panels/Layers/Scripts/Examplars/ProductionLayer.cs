public struct ProductionLayer : ILayer {
    public void Show(SettingsController settings, WmskController wmsk, MapController map, int layerIndex) {
        UnityEngine.Color color;
        int paramiterIndex = layerIndex - settings.Theme.CountEcologyParamiters - 1;
        for (int i = 0; i < map.data.CountRegions; ++i) {
            if (map.data.GetRegion(i).GetCountCivilizations() == 0) return;
            color = settings.Theme.GetCivilizationColor(paramiterIndex, map.data.GetRegion(i).GetCivilizationMaxIndex(paramiterIndex));
            map.data.GetRegion(i).SetColor(color);
            wmsk.RegionPainting(i, color);

        }
    }
}
