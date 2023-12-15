using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class TimelineController : MonoBehaviour, IGameConnecter {
    [SerializeField] private Text yearTxt;
    [SerializeField, Min(1)] private float accelerator;
    [SerializeField, Min(0)] private float tick;
    [SerializeField, Min(0)] private float actionInterval;
    [SerializeField, Min(0)] private float updateDataInterval;
    [SerializeField, Min(0)] private float updateMigrationInterval;
    [SerializeField, Min(0)] private float updateYearInterval;
    [SerializeField, Min(0)] private int yearInterval;
    [SerializeField] private int startYear;
    [SerializeField, Range(0, 100)] private int eventOrMigration;
    
    private int year;
    private float interval;
    private bool isTick;
    private float timeToUpdateData = 0;
    private float timeToAction = 0;
    private float timeToUpdateMigration = 0;
    private float timeToUpdateYear = 0;

    public Action<int> OnSelectRegion;
    public Action OnUpdateData;
    public Action OnUpdateMigration;
    public Action OnCreateMigration;
    public Action OnCreateEvent;
    public Action OnUpdateYear;
    
    public float Interval {
        set {
            if (value == 0) {
                isTick = false;
            } else if (interval == 0 && value > 0) {
                isTick = true;
                // OnCreateMigration();
                StartCoroutine(UpdateActive());
            }
            interval = value;
        }
    }

    public GameController GameController { set { } }

    public void Pouse() => Interval = 0;
    public void Play() => Interval = tick;
    public void Forward() => Interval = tick * accelerator;

    private IEnumerator UpdateActive() {
        while (isTick) {
            /// TODO: Представить в виде for(structs)
            /// Struct { Action<>, [Min(0)] float interval, private float time }
            /// перенести логику в структуру
            /// инициализацию Action представить в Init
            
            if (timeToAction > timeToAction % actionInterval) {
                timeToAction %= actionInterval;
                ChoiseAction();
            }
            timeToAction += interval;
            print("Wait: " + (actionInterval - timeToAction));

            if (timeToUpdateYear > timeToUpdateYear % updateYearInterval) {
                timeToUpdateYear %= updateYearInterval;
                UpdateYear();
            }
            timeToUpdateYear += interval;

            if (timeToUpdateData > timeToUpdateData % updateDataInterval) {
                timeToUpdateData %= updateDataInterval;
                OnUpdateData?.Invoke();
            }
            timeToUpdateData += interval;

            if (timeToUpdateMigration > timeToUpdateMigration % updateMigrationInterval) {
                timeToUpdateMigration %= updateDataInterval;
                OnUpdateMigration?.Invoke();
            }
            timeToUpdateData += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    private void ChoiseAction() {
        if (Randomizer.Random(100) < eventOrMigration) {
            OnCreateMigration?.Invoke();
        } else {
            OnCreateEvent?.Invoke();
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
        tick = Mathf.Min(tick, updateYearInterval, updateDataInterval, actionInterval);
        
        year = startYear;
        PrintYear();
    }
}
