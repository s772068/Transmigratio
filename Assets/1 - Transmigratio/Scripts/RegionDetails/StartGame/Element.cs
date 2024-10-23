namespace RegionDetails.StartGame {
    public class Element : Base.Element {
        public void Init(string paramiter, string label, float value) {
            Label = Localization.Load(paramiter, label);
            SetValue(value, true);
        }
    }
}
