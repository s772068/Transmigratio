public struct MU_FoodAppropriated : IUpdater {
    int GetFlora(ref S_Region region) => region.Ecology[2].Details[0];
    int GetFauna(ref S_Region region) => region.Ecology[3].Details[0];
    void SetFlora(ref S_Region region, int value) => region.Ecology[2].Details[0] = value;
    void SetFauna(ref S_Region region, int value) => region.Ecology[3].Details[0] = value;
    int Population(ref S_Region region) => region.AllPopulations;
    int TakenFood(ref S_Region region) => region.TakenFood;
    int PC(ref S_Region region) => region.AllCivilizationVlaues(0, 0);
    int Slavery(ref S_Region region) => region.AllCivilizationVlaues(0, 1);
    int Farmers(ref S_Region region) => region.AllCivilizationVlaues(1, 0);
    int Hunters(ref S_Region region) => region.AllCivilizationVlaues(1, 1);
    public void Update(S_Map map) {
    }
}
