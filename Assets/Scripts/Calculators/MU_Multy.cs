public class MU_Multy : BaseMapUpdater {
    public override void Update(ref S_Country[] countries) {
    }
    public override void Calc(ref S_Country country) {
        country.Population *= 1;
    }
}
