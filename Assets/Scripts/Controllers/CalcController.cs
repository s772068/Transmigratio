public class CalcController : BaseController {
    private BaseCalculator[] calculators = {
            new AddCalc(),
            new MultyCalc()
    };

    private SaveController save;

    public override GameController GameController {
        set {
            save = value.Get<SaveController>();
        }
    }

    public void UpdateParams() {
        for (int i = 0; i < calculators.Length; ++i) {
            calculators[i].Update(ref save.countries);
            // Update percents
            for (int j = 0; j < save.countries.Length; ++j) {
                calculators[i].Calc(ref save.countries[j]);
            }
        }
    }
}
