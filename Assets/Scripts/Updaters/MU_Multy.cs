public class MU_Multy : BaseMapUpdater {
    public override void Update(ref S_Map map) {
        map.Countries[0].Population *= 1;
    }
}
