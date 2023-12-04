using System.Collections;
using UnityEngine;
using System;

public class TimelineController : MonoBehaviour, IGameConnecter {
    [SerializeField, Min(0)] private float speedPlayGame;
    [SerializeField, Min(0)] private float speedFastPlayGame;
    
    private float interval;

    public Action<int> OnSelectRegion;
    public Action OnTick;

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

    public GameController GameController { set { } }

    public void Pouse() => Interval = 0;
    public void Play() => Interval = 1 / speedPlayGame;
    public void Forward() => Interval = 1 / speedFastPlayGame;

    private IEnumerator UpdateActive() {
        while (isTick) {
            OnTick?.Invoke();
            yield return new WaitForSeconds(interval);
        }
    }

    public void Init() { }
}
