using System;


/// <summary>
/// Ёкземпл€р CivPiece - это один "кусочек" цивилизации в конкретном регионе. 
/// ≈сли цивилизаци€ есть в трЄх регионах, это значит, что она состоит из трЄх объектов CivPiece
/// </summary>
[System.Serializable]
public class CivPiece {
    public Population Population;
    public float PopulationGrow;
    public float GivenFood;
    public float RequestFood;
    public float ReserveFood;
    public float TakenFood;
    
    public int RegionID;
    public string CivName;

    public Action Destroy;

    public TM_Region Region => TMDB.map.AllRegions[RegionID];
    public Civilization Civilization => TMDB.humanity.Civilizations[CivName];
    private TMDB TMDB => Transmigratio.Instance.tmdb;

    /// <summary>
    /// »нициализаци€ при по€влении в области после миграции или при старте игры
    /// </summary>
    public void Init(int region, string civilization, int startPopulation, float reserve) {
        RegionID = region;
        Population = new Population();
        Population.value = startPopulation;
        CivName = civilization;
        ReserveFood = reserve;  //изначальное количество еды у кусочка
    }

    /// <summary>
    /// »зменение населени€ кусочка за тик
    /// </summary>
    public void DeltaPop() {
        float faunaKr = (float)(Math.Pow(Region.Fauna.GetMaxQuantity().value, 0.58d) / 10f);
        TakenFood = Population.value / 100f * faunaKr * Civilization.ProdModeK;
        RequestFood = Population.Value / 150f;
        if (ReserveFood > RequestFood) GivenFood = RequestFood;
        else                           GivenFood = ReserveFood;
        ReserveFood += TakenFood - GivenFood;
        PopulationGrow = Population.Value * Civilization.GovernmentCorruption * GivenFood / RequestFood - Population.Value / 3f;

        Population.value += (int)PopulationGrow;
        if (Population.value <= 50) { Destroy?.Invoke(); return; }

        if (PopulationGrow < 0) {
            GameEvents.ActivateHunger?.Invoke(this);
            MigrationController.Instance.TryMigration(this);
        } else {
            GameEvents.DeactivateHunger?.Invoke(this);
        }
    }
}
