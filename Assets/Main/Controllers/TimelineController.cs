using System.Collections;
using UnityEngine;
using System;

public class TimelineController : MonoBehaviour, IGameConnecter {
    [SerializeField, Min(0)] private float speedPlayGame;
    [SerializeField, Min(0)] private float speedFastPlayGame;
    
    private float interval;
    private float intervalEvents;
    private float intervalArrow;

    public Action<int> OnSelectRegion;
    public Action OnTick;
    public Action OnEvents;
    public Action OnArrows;

    private bool isTick;

    public float Interval {
        set {
            if (value == 0) {
                isTick = false;
            } else if (interval == 0 && value > 0) {
                isTick = true;
                StartCoroutine(UpdateActive());
            }
            interval = value;
        }
    }

    public float IntervalEvents {
        set {
            if (value == 0) {
                isTick = false;
            } else if (intervalEvents == 0 && value > 0) {
                isTick = true;
                StartCoroutine(UpdateEvents());
            }
            intervalEvents = value;
        }
    }

    public float IntervalArrow {
        set {
            if (value == 0) {
                isTick = false;
            } else if (intervalArrow == 0 && value > 0) {
                isTick = true;
                StartCoroutine(UpdateArrows());
            }
            intervalArrow = value;
        }
    }

    public GameController GameController { set { } }

    public void Pouse() => Interval = IntervalEvents = IntervalArrow = 0;
    public void Play() {
        Interval = 1 / speedPlayGame;
        IntervalEvents = 12 / speedPlayGame;
        IntervalArrow = 5 / speedPlayGame;

    }
    public void Forward() {
        Interval = 0.5f / speedFastPlayGame;
        IntervalEvents = 6 / speedFastPlayGame;
        IntervalArrow = 2.5f / speedFastPlayGame;
    }

    private IEnumerator UpdateActive() {
        while (isTick) {
            OnTick?.Invoke();
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator UpdateEvents() {
        while (isTick) {
            OnEvents?.Invoke();
            yield return new WaitForSeconds(intervalEvents);
        }
    }

    private IEnumerator UpdateArrows() {
        while (isTick) {
            OnArrows?.Invoke();
            yield return new WaitForSeconds(intervalArrow);
        }
    }

    public void Init() { }
}
