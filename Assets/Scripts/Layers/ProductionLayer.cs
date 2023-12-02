public struct ProductionLayer : ILayer {
    public void Show(SettingsController settings, WmskController wmsk, MapController map, int layerIndex) {
        UnityEngine.Color color;
        int paramiterIndex = layerIndex - settings.Theme.CountEcologyParamiters - 1;
        for (int i = 0; i < map.data.Regions.Length; ++i) {
            color = settings.Theme.GetCivilizationColor(paramiterIndex, map.data.Regions[i].MaxCivilizationIndex(paramiterIndex));
            map.data.Regions[i].Color = color;
            wmsk.RegionPainting(i, color);

        }
    }
}
