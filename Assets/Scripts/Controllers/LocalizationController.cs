
public class LocalizationController : BaseController, ISave {
    public Language localization;
    public SO_Localization[] array;

    public SO_Localization Localization => array[(int) localization];
    public int LocalIndex => (int) localization;

    public void Save() {
        IOHelper.SaveToJson(new S_Localization() {
            localization = (int) localization
        });
    }

    public void Load() {
        IOHelper.LoadFromJson(out S_Localization data);
        localization = (Language) data.localization;
    }

    public enum Language {
        Eng,
        Ru
    }
}
