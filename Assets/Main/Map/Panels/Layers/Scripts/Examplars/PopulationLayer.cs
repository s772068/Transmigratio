using UnityEngine;

public struct PopulationLayer : ILayer {
    public void Show(SettingsController settings, WmskController wmsk, MapController map, int layerIndex) {
        Color color;
        layerIndex -= settings.Theme.CountEcologyParamiters;
        int count = settings.Theme.CountCivilizationDetails(layerIndex);
        float max = map.data.GetPopulations();
        float percent;
        for (int i = 0, detailIndex = 0; i < map.data.CountRegions; ++i) {
            if (map.data.GetRegion(i).GetCountCivilizations() == 0) continue;
            percent = map.data.GetRegion(i).GetAllPopulations() * 100 / max;
            if (percent < 25) {
                detailIndex = 0;
            } else if (25 <= percent && percent <= 75) {
                detailIndex = 1;
            } else if (75 < percent) {
                detailIndex = 2;
            }
            Debug.Log($"Percent:{percent} | DetailIndex: {detailIndex}");
            color = settings.Theme.GetCivilizationColor(0, detailIndex);
            color.a = percent / 100;
            Debug.Log($"Color: {color}");
            map.data.GetRegion(i).Color = color;
            wmsk.RegionPainting(i, color);
        }
    }
}
