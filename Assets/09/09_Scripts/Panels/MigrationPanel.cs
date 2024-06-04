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

    public bool IsOpenPanel => !isOpenPanel.isOn;

    public MigrationData Data { set {
            data = value;
            slider.value = value.curPopulations * 100 / value.fullPopulations;

            // ToDo: вставить строку в виде "from: {0} | to: {1}"
            // fromTo.text = string.Format(StringLoader.Load(""), data.from.name, data.to.name);
        }
    }

    public Action onNothing;
    public Action<int> onBreak;
    public Action<int> onSpeedUp;

    //ToDo: выставить ключ ОВ
    //public void Awake() {
    //    breakPointTxt.text = $"{10} {StringLoader.Load("IP")}";
    //    breakPointTxt.text = $"{0} {StringLoader.Load("IP")}";
    //    breakPointTxt.text = $"{5} {StringLoader.Load("IP")}";
    //}

    public void Open() {
        Timeline.Instance.Pause();
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
        Timeline.Instance.Play();
        gameObject.SetActive(false);
    }
}
