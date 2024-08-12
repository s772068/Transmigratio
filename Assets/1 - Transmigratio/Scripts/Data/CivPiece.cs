using System.Collections.Generic;
using Gameplay.Scenarios;
using System;
using UnityEngine;
using System.Linq;

/// <summary>
/// Ёкземпл€р CivPiece - это один "кусочек" цивилизации в конкретном регионе. 
/// ≈сли цивилизаци€ есть в трЄх регионах, это значит, что она состоит из трЄх объектов CivPiece
/// </summary>
[Serializable]
public class CivPiece {
    public static readonly int MinPiecePopulation = 50;

    public Population Population;
    public ParamiterValue PopulationGrow = new();
    public ParamiterValue ReserveFood = new();
    public ParamiterValue GivenFood;
    public ParamiterValue RequestFood;
    public ParamiterValue TakenFood;

    public Paramiter EcoCulture = new(true);
    public Paramiter ProdMode = new(true);
    public Paramiter Government = new(true);

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

    public float ProdModeK {
        get {
            List<float> prodModeList = new() {
                ProdMode["PrimitiveCommunism"],
                ProdMode["Slavery"],
                ProdMode["Feodalism"]
            };
            return prodModeList.IndexOf(prodModeList.Max()) switch {
                0 => Demography.data.prodModeK_PC,
                1 => Demography.data.prodModeK_S,
                _ => 1
            };
        }
    }
    public bool IsFarmers {
        get {
            List<float> list = new() {
                EcoCulture["Farmers"],
                EcoCulture["Hunters"],
                EcoCulture["Nomads"]
            };
            return list.IndexOf(list.Max()) == 0;
        }
    }

    /// <summary>
    /// »нициализаци€ при по€влении в области после миграции или при старте игры
    /// </summary>
    public void Init(int region, string civilization, int startPopulation) {
        EcoCulture.Init(("Hunters", GameSettings.StartHunters));
        EcoCulture.Init("Farmers", "Nomads", "Mountain", "City");

        ProdMode.Init(("PrimitiveCommunism", GameSettings.StartPrimitiveCommunism));
        ProdMode.Init("Slavery", "Feodalism", "Capitalism", "Socialism", "Communism");

        Government.Init(("Leaderism", GameSettings.StartLeaderism));
        Government.Init("Monarchy", "CityState", "Imperium", "Federation", "NationalState", "Anarchy");

        RegionID = region;
        CivName = civilization;
        Population = new(startPopulation);
        RequestFood = new(Population.Value / Demography.data.val4);
        GivenFood = new(ReserveFood.Value > RequestFood.Value ? RequestFood.Value : ReserveFood.Value);
        float _floraKr = (float) (Math.Pow(Region.Flora["Flora"], Demography.data.val9) / Demography.data.val10);
        float _faunaKr = (float) (Math.Pow(Region.Fauna["Fauna"], Demography.data.val11) / Demography.data.val12);

        TakenFood = new(IsFarmers ?
            Population.Value / Demography.data.val13 * ProdModeK * _floraKr :
            Population.Value / Demography.data.val14 * ProdModeK * _faunaKr);
    }

    public void AddEvent(Events.Controllers.Base e) => events.Add(e);
    public void RemoveEvent(Events.Controllers.Base e) => events.Remove(e);
    public void Play() => Gameplay.Controller.GamePlay(this);
}
