using UnityEngine;

public class GUIE_NumParamiter : GUIE_BaseParamiter {
    [SerializeField] private GUI_ProgressBar progressBar;

    public void SetValue(float value, bool useProgressBar) {
        val.text = value.ToString();
        if(useProgressBar) progressBar.Value = value;
        else progressBar.Value = 100;
    }

    public void Build(Paramiter paramiter, bool isShowLabel, bool useProgressBar) {
        Build(paramiter, isShowLabel);
        Detail detail = paramiter.MaxDetail;
        SetValue(detail.Value, useProgressBar);
    }
}
