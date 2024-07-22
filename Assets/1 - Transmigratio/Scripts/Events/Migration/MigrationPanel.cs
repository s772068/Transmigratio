using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using Utilits;
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

    private Database.Data.Migration data;

    public Action onNothing;
    public Action<int> onBreak;
    public Action<int> onSpeedUp;

    public bool IsOpenPanel => !dontShowAgain.isOn;

    private void Awake() {
        description.text = Localization.Load("Migration", "Description");
        breakPointsTxt.text = $"10 {Localization.Load("Events", "Points")}";
        nothingPointsTxt.text = $"0 {Localization.Load("Events", "Points")}";
        speedUpPointsTxt.text = $"5 {Localization.Load("Events", "Points")}";
    }

    public void UpdatePercents() {
        if (data != null) {
            slider.value = data.CurPopulations * 100 / data.FullPopulations;
        } else {
            data = null;
            Close();
        }
    }

    public Database.Data.Migration Data { set {
            data = value;
            territory.text = Localization.Load("Migration", "Territory1") + " " +
                             $"<color=#{regionColor.ToHexString()}>" +
                             value.From.Name + "</color> " +
                             Localization.Load("Migration", "Territory2") + " " +
                             $"<color=#{regionColor.ToHexString()}>" +
                             value.To.Name + "</color>";
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
        onBreak.Invoke(data.From.Id);
        Close();
    }
    public void SpeedUp() {
        onSpeedUp?.Invoke(data.From.Id);
        Close();
    }

    public void Close() {
        gameObject.SetActive(false);
    }
}
