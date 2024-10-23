using System.Collections.Generic;
using Gameplay.Scenarios;
using System.Linq;
using System;

using Events = Gameplay.Scenarios.Events;

/// <summary>
/// ��������� CivPiece - ��� ���� "�������" ����������� � ���������� �������. 
/// ���� ����������� ���� � ��� ��������, ��� ������, ��� ��� ������� �� ��� �������� CivPiece
/// </summary>
[Serializable]
public class CivPiece {
    public static readonly int MinPiecePopulation = 50;

    public string CivName;
    public int RegionID;
    public int Category;
    public float gold;

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
            foreach (Events.Base e in events)
                if (e.GetType() == typeof(Events.Global.Migration))
                    value++;
            return value;
        }
    }

    public Action Destroy;

    private List<Events.Base> events = new();

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
                EcoCulture["СattleBreeders"],
                EcoCulture["Townsman"]
            };
            return list.IndexOf(list.Max()) == 0;
        }
    }

    /// <summary>
    /// ������������� ��� ��������� � ������� ����� �������� ��� ��� ������ ����
    /// </summary>
    public void Init(int region, string civilization, int startPopulation) {
        EcoCulture.Init(("Hunters", GameSettings.StartHunters));
        EcoCulture.Init("Farmers", "СattleBreeders", "Townsman");

        ProdMode.Init(("PrimitiveCommunism", GameSettings.StartPrimitiveCommunism));
        ProdMode.Init("Slavery", "Feodalism", "Capitalism", "Socialism", "Communism");

        Government.Init(("Leaderism", GameSettings.StartLeaderism));
        Government.Init("Monarchy", "CityState", "Empire", "Federation", "NationalState", "Anarchy");

        Category = 3;
        RegionID = region;
        CivName = civilization;
        Population = new(startPopulation);

        float _floraKr = (float)(Math.Pow(Region.Flora["Flora"], Demography.data.krDegree) / Demography.data.krModifier);
        float _faunaKr = (float)(Math.Pow(Region.Fauna["Fauna"], Demography.data.krDegree) / Demography.data.krModifier);
        TakenFood = new(IsFarmers ?
            Population.Value / Demography.data.takenFoodModifier * ProdModeK * _floraKr :
            Population.Value / Demography.data.takenFoodModifier * ProdModeK * _faunaKr);

        RequestFood = new(Population.Value / Demography.data.peopleForFood);
        ReserveFood = new(RequestFood.value * 2);
        GivenFood = new(ReserveFood.value > RequestFood.value ? RequestFood.value : ReserveFood.value);
    }
    //�������� CivPiece ��� ��� ���������� (�������� ��������� ������ �����������)
    public void Init(CivPiece piece, string civilization, int category)
    {
        EcoCulture.Init(piece.EcoCulture.QuantitiesDictionary);
        ProdMode.Init(piece.ProdMode.QuantitiesDictionary);
        Government.Init(piece.Government.QuantitiesDictionary);

        Category = category;
        RegionID = piece.RegionID;
        CivName = civilization;
        Population = new(piece.Population.Value);

        float _floraKr = (float)(Math.Pow(Region.Flora["Flora"], Demography.data.krDegree) / Demography.data.krModifier);
        float _faunaKr = (float)(Math.Pow(Region.Fauna["Fauna"], Demography.data.krDegree) / Demography.data.krModifier);

        TakenFood = new(IsFarmers ?
            Population.Value / Demography.data.takenFoodModifier * ProdModeK * _floraKr :
            Population.Value / Demography.data.takenFoodModifier * ProdModeK * _faunaKr);
        RequestFood = new(Population.Value / Demography.data.peopleForFood);
        ReserveFood = new(piece.ReserveFood.value);
        GivenFood = new(ReserveFood.value > RequestFood.value ? RequestFood.value : ReserveFood.value);
    }

    public void AddEvent(Events.Base e) => events.Add(e);
    public void RemoveEvent(Events.Base e) => events.Remove(e);
    public void Play() => Gameplay.Controller.GamePlay(this);
}
