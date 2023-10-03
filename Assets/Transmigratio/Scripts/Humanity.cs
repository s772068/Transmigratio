using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Humanity
{
    public List<Civilization> allCivs;
    public int accumulatedPopulation;



    public void Init()
    {
        allCivs = new List<Civilization>();
        allCivs.Clear();
        AddUncivilizedTribes();
        accumulatedPopulation = 0;
        Debug.Log("Humanity initialized");
    }
    public void AddCiv()
    {
        Civilization newCiv = new Civilization();
        newCiv.Init(allCivs.Count + 1);
        allCivs.Add(newCiv);
    }
    public void AddUncivilizedTribes()
    {
        Civilization newCiv = new Civilization();

        newCiv.Init(0);
        newCiv.name = "Uncivilized Tribes";

        allCivs.Add(newCiv);

    }
    public void CalcGlobalVars()
    {
        accumulatedPopulation = 0;
        foreach (var c in allCivs)
        {
            accumulatedPopulation += c.civPopulation;
        }
    }

    public void PopulateRegion(TM_Region region, int newComers, TM_Region from) //куда, сколько и откуда
    {
        region.population += newComers;
        region.civAttachment = from.civAttachment;
    }
    public void PopulateRegion(TM_Region region, int newComers)                 //куда и сколько - перегрузка для заселения первого региона в начале игры
    {
        region.population += newComers;
        GetCivById(0).AttachToRegion(region, newComers / region.population * 100);
        region.isPopulated = true;
        region.SetPopulatedDefault();
    }
    public void CivExtinct(Civilization civ)
    {

    }
    public Civilization GetCivById(int id)
    {
        foreach (var c in allCivs)
        {
            if (c.civId == id) return c;
        }
        return null;
    }
}
[System.Serializable]
public class Civilization
{
    public string name;
    public int civId;
    public List<int> idsOfRegionsOfPresence;
    public int civPopulation;
    //techs

    public void Init(int id)
    {
        civId = id;
        name = "Civilization_" + id;
        idsOfRegionsOfPresence = new List<int>();
        idsOfRegionsOfPresence.Clear();
        civPopulation = 0;
    }
    public void AttachToRegion(TM_Region region, float quantity)
    {
        idsOfRegionsOfPresence.Add(region.id);
        region.civAttachment.quantities.Add(name, quantity);
        civPopulation += region.population;
    }

}
