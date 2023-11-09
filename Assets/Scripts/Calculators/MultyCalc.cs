public class MultyCalc : BaseCalculator {
    public override void Update(ref S_Country[] countries) {
    }
    public override void Calc(ref S_Country country) {
        country.population = country.population * 1;
    }
}
