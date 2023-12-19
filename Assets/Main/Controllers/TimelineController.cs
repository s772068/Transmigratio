using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class TimelineController : MonoBehaviour, IGameConnecter {
    [SerializeField] private Text yearTxt;
    [SerializeField, Min(1)] private float accelerator;
    [SerializeField, Min(0)] private float tick;
    [SerializeField, Min(0)] private float volcanoInterval;
    [SerializeField, Min(0)] private float updateDataInterval;
    [SerializeField, Min(0)] private float updateMigrationInterval;
    [SerializeField, Min(0)] private float updateYearInterval;
    //[SerializeField, Min(0)] private float intervalShowFactAboutEarth;
    [SerializeField, Min(0)] private float endGame;
    [SerializeField, Min(0)] private int yearInterval;
    [SerializeField] private int startYear;
    [SerializeField, Range(0, 100)] private int eventOrMigration;

    private InfoController info;

    private int year;
    private float interval;
    private bool isTick;
    private float timeToVolcano = 0;
    private float timeToUpdateYear = 0;
    private float timeToUpdateData = 0;
    private float timeToUpdateMigration = 0;
    //private float timeToShowFactAboutEarth = 0;
    private float timeToEndGame = 0;

    public Action<int> OnSelectRegion;
    public Action OnUpdateData;
    public Action OnCreateVolcano;
    public Action OnCreateEvent;
    public Action OnUpdateYear;

    private bool isForward;

    public float Interval {
        set {
            if (value == 0) {
                isTick = false;
            } else if (!isTick && value > 0) {
                isTick = true;
                StartCoroutine(UpdateActive());
            }
            interval = value;
        }
    }

    public GameController GameController {
        set {
            value.Get(out info);
        }
    }

    public void Pouse() => Interval = 0;
    public void Play() { isForward = false; Interval = tick; }
    public void Forward() { isForward = true; Interval = tick / accelerator; }
    public void TurnPlay() { if (isForward) Forward(); else Play(); }

    private IEnumerator UpdateActive() {
        while (isTick) {
            /// TODO: Представить в виде for(structs)
            /// Struct { Action<>, [Min(0)] float interval, private float time }
            /// перенести логику в структуру
            /// инициализацию Action представить в Init
            
            yield return new WaitForSeconds(interval);

            if (timeToVolcano > timeToVolcano % volcanoInterval) {
                timeToVolcano %= volcanoInterval;
                OnCreateVolcano?.Invoke();
            }
            timeToVolcano += tick;

            timeToUpdateYear += tick;
            if (timeToUpdateYear >= timeToUpdateYear % updateYearInterval) {
                timeToUpdateYear %= updateYearInterval;
                UpdateYear();
            }

            if (timeToUpdateData > timeToUpdateData % updateDataInterval) {
                timeToUpdateData %= updateDataInterval;
                OnUpdateData?.Invoke();
            }
            timeToUpdateData += tick;

            //if (timeToShowFactAboutEarth > timeToShowFactAboutEarth % intervalShowFactAboutEarth) {
            //    timeToShowFactAboutEarth %= intervalShowFactAboutEarth;
            //    info.FactAboutEarth();
            //}
            //timeToShowFactAboutEarth += interval;

            if (timeToEndGame > timeToEndGame % endGame) {
                timeToEndGame %= endGame;
                info.EndGame();
            }
            timeToEndGame += tick;
            // print("TimeToEndGame: " + (endGame - timeToEndGame));
        }
    }

    private void UpdateYear() {
        year += yearInterval;
        PrintYear();
    }

    private void PrintYear() {
        yearTxt.text = Mathf.Abs(year).ToString().Insert(2, " ") + (year < 0 ? " Д.Н.Э." : "Н.Э.");
    }

    public void Init() {
        tick = Mathf.Min(tick, updateYearInterval, updateDataInterval, volcanoInterval);
        
        year = startYear;        
        PrintYear();
    }
}
