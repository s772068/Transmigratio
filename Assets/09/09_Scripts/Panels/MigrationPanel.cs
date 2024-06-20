using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class MigrationPanel : MonoBehaviour {
    [SerializeField] private Slider slider;
    [SerializeField] private Toggle isOpenPanel;
    [SerializeField] private TMP_Text fromTo;
    [SerializeField] private TMP_Text breakPointTxt;
    [SerializeField] private TMP_Text nothingPointTxt;
    [SerializeField] private TMP_Text speedUpTxt;

    private MigrationData data;

    public Action onNothing;
    public Action<int> onBreak;
    public Action<int> onSpeedUp;

    public bool IsOpenPanel => !isOpenPanel.isOn;

    public void UpdatePercents() {
        if (data != null) {
            slider.value = data.curPopulations * 100 / data.fullPopulations;
        } else {
            data = null;
            Close();
        }
    }

    public MigrationData Data { set {
            data = value;

            // ToDo: вставить строку в виде "from: {0} | to: {1}"
            // fromTo.text = string.Format(StringLoader.Load(""), data.from.name, data.to.name);
        }
    }

    //ToDo: выставить ключ ОВ
    //public void Awake() {
    //    breakPointTxt.text = $"{10} {StringLoader.Load("Points")}";
    //    breakPointTxt.text = $"{0} {StringLoader.Load("Points")}";
    //    breakPointTxt.text = $"{5} {StringLoader.Load("Points")}";
    //}

    public void Open() {
        gameObject.SetActive(true);
    }

    public void Nothing() {
        onNothing?.Invoke();
        Close();
    }

    public void Break() {
        onBreak.Invoke(data.from.id);
        Close();
    }
    public void SpeedUp() {
        onSpeedUp?.Invoke(data.from.id);
        Close();
    }

    public void Close() {
        gameObject.SetActive(false);
    }
}
