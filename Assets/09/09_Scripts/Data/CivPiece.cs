using System;


/// <summary>
/// Ёкземпл€р CivPiece - это один "кусочек" цивилизации в конкретном регионе. 
/// ≈сли цивилизаци€ есть в трЄх регионах, это значит, что она состоит из трЄх объектов CivPiece
/// </summary>
[System.Serializable]
public class CivPiece {
    //public int regionId;                //wmsk_id того региона, где живЄт этот объект
    public int regionResidenceIndex;   //может лучше сам регион, а не айди?
    
    //public Civilization civBelonging;      // если мы обращаемс€ к CivPiece только из списка civPiecesList, эта штука нам не нужна
    public int civIndex;

    public Population population;
    public float populationGrow;
    public float givenFood;
    public float requestFood;
    public float reserveFood;
    public float takenFood;

    public TM_Region GetRegion(int index) => Transmigratio.Instance.GetRegion(index);
    public Civilization GetCiv(int index) => Transmigratio.Instance.GetCiv(index);

    /// <summary>
    /// »нициализаци€ при по€влении в области после миграции или при старте игры
    /// </summary>
    public void Init(int regionIndex, int startPop, int index, float reserve) {
        regionResidenceIndex = regionIndex;
        population = new Population();
        population.value = startPop;
        civIndex = index;
        reserveFood = reserve;  //изначальное количество еды у кусочка
        GameEvents.onTick += DeltaPop;
    }

    /// <summary>
    /// »зменение населени€ кусочка за тик
    /// </summary>
    public void DeltaPop() {
        Civilization civBelonging = GetCiv(civIndex);
        float faunaKr = (float)(Math.Pow(GetRegion(regionResidenceIndex).fauna.GetMax().Value, 0.58d) / 10);
        takenFood = population.value / 100 * faunaKr * civBelonging.prodModeK;
        requestFood = population.Value / 150f;
        if (reserveFood > requestFood) { givenFood = requestFood; }
        else { givenFood = reserveFood; }
        reserveFood += takenFood - givenFood;
        populationGrow = population.Value * civBelonging.governmentCorruption * givenFood / requestFood - population.Value / 3;

        //return (int)(populationGrow);
        population.value += (int)populationGrow;
    }
}
