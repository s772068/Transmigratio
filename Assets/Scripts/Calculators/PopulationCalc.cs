using UnityEngine;

public class PopulationCalc {
    private static int minPopulation = 1;
    private static int add = 1;
    public static int MinPopulation { set => minPopulation = value; }
    public static int Add { set => add = value; }
    public static void Calc(ref S_Country country) {
        //if (country.Population >= minPopulation && country.Population < country.MaxPopulation) {
        //    country.Population = Mathf.Clamp(country.Population + add, minPopulation, country.MaxPopulation);
        //}
    }
}
