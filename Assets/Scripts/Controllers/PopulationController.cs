public class PopulationController : MonoSingleton<PopulationController> {
    public int Calc(int population, int add) {
        return population > 0 ? population + add : 0;
    }
}
