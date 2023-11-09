using WorldMapStrategyKit;

public class SaveController : BaseController {
    public int intervention;
    public S_Country[] countries;

    public void Load() {
        IOHelper.LoadFromJson(out S_Save data);
        intervention = data.intervention;
        countries = data.countries;
    }

    public void Save() {
        IOHelper.SaveToJson(new S_Save() {
            intervention = intervention,
            countries = countries
        });
    }

    public override void Init() {
        WMSK wmsk = WMSK.instance;
        for (int i = 0; i < countries.Length; ++i) {
            for (int j = 0; j < wmsk.countries[i].neighbours.Length; ++j) {
                countries[i].neighbours.Add(wmsk.countries[i].neighbours[j]);
            }
        }
    }
}
