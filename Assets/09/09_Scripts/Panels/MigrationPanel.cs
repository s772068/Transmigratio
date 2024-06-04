using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class MigrationPanel : MonoBehaviour {
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text fromTo;
    [SerializeField] private TMP_Text BreakPointTxt;
    [SerializeField] private TMP_Text NothingPointTxt;
    [SerializeField] private TMP_Text SpeedUpTxt;

    public MigrationData Data { private get; set; }

    public Action<int> onBreak;
    public Action<int> onSpeedUp;

    public float Value { set {

        }
    }

    public void OpenPanel() {
        Timeline.Instance.Pause();
        gameObject.SetActive(true);
    }

    public void ClosePanel() {
        Timeline.Instance.Play();
        gameObject.SetActive(false);
    }

    public void Break() {
        onBreak.Invoke(Data.from.id);
        ClosePanel();
    }
    public void SpeedUp() {
        onSpeedUp?.Invoke(Data.from.id);
        ClosePanel();
    }
}
