using System.Collections.Generic;
using Gameplay.Scenarios;
using System;

/// <summary>
/// Ёкземпл€р CivPiece - это один "кусочек" цивилизации в конкретном регионе. 
/// ≈сли цивилизаци€ есть в трЄх регионах, это значит, что она состоит из трЄх объектов CivPiece
/// </summary>
[Serializable]
public class CivPiece {
    public static readonly int MinPiecePopulation = 50;
    public Population Population;
    public float PopulationGrow;
    public float GivenFood;
    public float RequestFood;
    public float ReserveFood;
    public float TakenFood;
    public float PrevPopulationGrow;

    public int EventsCount => events.Count;
    public int MigrationCount {
        get {
            int value = 0;
            foreach (Events.Controllers.Base e in events)
                if (e.GetType() == typeof(Events.Controllers.Global.Migration))
                    value++;
            return value;
        }
    }
    public int RegionID;
    public string CivName;

    public Action Destroy;

    private List<Events.Controllers.Base> events = new();

    public TM_Region Region => TMDB.map.AllRegions[RegionID];
    public Civilization Civilization => TMDB.humanity.Civilizations[CivName];
    private TMDB TMDB => Transmigratio.Instance.TMDB;
    
    /// <summary>
    /// »нициализаци€ при по€влении в области после миграции или при старте игры
    /// </summary>
    public void Init(int region, string civilization, int startPopulation, float _prodModeK, bool isFarmers) {
        RegionID = region;
        Population = new Population();
        Population.value = startPopulation;
        CivName = civilization;
        // ReserveFood = reserve;  //изначальное количество еды у кусочка
        RequestFood = Population.Value / Demography.data.val4;
        GivenFood = ReserveFood > RequestFood ? RequestFood : ReserveFood;
        float _floraKr = (float) (Math.Pow(Region.Flora["Flora"].Value, Demography.data.val9) / Demography.data.val10);
        float _faunaKr = (float) (Math.Pow(Region.Fauna["Fauna"].Value, Demography.data.val11) / Demography.data.val12);

        TakenFood = isFarmers ?
            Population.Value / Demography.data.val13 * _prodModeK * _floraKr :
            Population.Value / Demography.data.val14 * _prodModeK * _faunaKr;
    }

    public void AddEvent(Events.Controllers.Base e) => events.Add(e);
    public void RemoveEvent(Events.Controllers.Base e) => events.Remove(e);
    public void Play() => Gameplay.Controller.GamePlay(this);
}
