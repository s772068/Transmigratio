using System.Collections.Generic;
using Events.Controllers.Local;
using UnityEngine;
using System;

using GlobalEvents = Events.Controllers.Global;

/// <summary>
/// Ёкземпл€р CivPiece - это один "кусочек" цивилизации в конкретном регионе. 
/// ≈сли цивилизаци€ есть в трЄх регионах, это значит, что она состоит из трЄх объектов CivPiece
/// </summary>
[Serializable]
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

    private List<Events.Controllers.Base> events = new();
    private float _prevPopulationGrow;

    public TM_Region Region => TMDB.map.AllRegions[RegionID];
    public Civilization Civilization => TMDB.humanity.Civilizations[CivName];
    private TMDB TMDB => Transmigratio.Instance.TMDB;
    
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

    public void AddEvent(Events.Controllers.Base e) {
        events.Add(e);
        if (events.Count == 1) e.CreateMarker();
    }

    public void RemoveEvent(Events.Controllers.Base e) {
        events.Remove(e);
        if (events.Count == 0) Region.Marker.Destroy();
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
        _prevPopulationGrow = PopulationGrow;
        PopulationGrow = Population.Value * Civilization.GovernmentCorruption * GivenFood / RequestFood - Population.Value / 3f;

        Population.value += (int)PopulationGrow;
        if (Population.value <= 50) { Destroy?.Invoke(); return; }

        if (_prevPopulationGrow >= 0 && PopulationGrow < 0) {
            Hunger.onActivate?.Invoke(this);
            GlobalEvents.Migration.OnMigration(this);
        } else if(_prevPopulationGrow < 0 && PopulationGrow >= 0) {
            Hunger.onDeactivate?.Invoke(this);
        }
    }
}
