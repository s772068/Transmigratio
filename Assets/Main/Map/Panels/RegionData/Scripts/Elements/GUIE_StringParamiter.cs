using UnityEngine.Localization.Settings;

public class GUIE_StringParamiter : GUIE_BaseParamiter {
    public string Value {
        set {
            var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Details", value);
            if (op.IsDone) val.text = op.Result;
            else op.Completed += (op) => val.text = op.Result;
        }
    }

    public new void Build(Paramiter paramiter, bool isShowLabel) {
        base.Build(paramiter, isShowLabel);
        Detail detail = paramiter.MaxDetail;
        Value = detail.Name;
    }
}
