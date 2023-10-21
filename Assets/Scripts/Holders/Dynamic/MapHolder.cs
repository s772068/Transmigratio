public class MapHolder : MonoSingleton<MapHolder> {
    public S_Map map;

    public void Load() => IOHelper.LoadFromJson(out map);
    public void Save() => IOHelper.SaveToJson(map);
}
