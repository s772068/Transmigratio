using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class MigrationPanel : MonoBehaviour {
    [SerializeField] private Slider slider;
    [SerializeField] private Toggle dontShowAgain;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text territory;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text dontShowAgainTxt;

    [SerializeField] private TMP_Text breakPointsTxt;
    [SerializeField] private TMP_Text nothingPointsTxt;
    [SerializeField] private TMP_Text speedUpPointsTxt;

    [Header("Colors")]
    [SerializeField] private Color regionColor;
    [SerializeField] private Color civColor;

    private MigrationData data;

    public Action onNothing;
    public Action<int> onBreak;
    public Action<int> onSpeedUp;

    public bool IsOpenPanel => !dontShowAgain.isOn;

    private void Awake() {
        description.text = StringLoader.Load("Migration", "Description");
        breakPointsTxt.text = $"10 {StringLoader.Load("Events", "Points")}";
        nothingPointsTxt.text = $"0 {StringLoader.Load("Events", "Points")}";
        speedUpPointsTxt.text = $"5 {StringLoader.Load("Events", "Points")}";
    }

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
            territory.text = StringLoader.Load("Migration", "Territory1") + " " +
                             $"<color=#{regionColor.ToHexString()}>" +
                             value.from.name + "</color> " +
                             StringLoader.Load("Migration", "Territory2") + " " +
                             $"<color=#{regionColor.ToHexString()}>" +
                             value.to.name + "</color>";
        }
    }

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
