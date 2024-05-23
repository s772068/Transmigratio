using System.Collections;
using UnityEngine;

public class Timeline : PersistentSingleton<Timeline> {
    [SerializeField] private Vector2 timeDelayLimit;
    private float timeDelay;
    private float timer;
    private bool isPlay;
    private int tick;

    public int Tick => tick;
    
    public void Rewind() {
        timeDelay = timeDelayLimit.x;
        if (!isPlay) StartCoroutine(TickPlay());
        Debug.Log("Rewind pressed");
    }
    
    public void Play() {
        timeDelay = timeDelayLimit.y;
        if (!isPlay) StartCoroutine(TickPlay());
        Debug.Log("Play pressed");
    }

    public void Pause() {
        isPlay = false;
        Debug.Log("Pause pressed");
    }

    private IEnumerator TickPlay() {
        isPlay = true;
        while (isPlay) {
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
            if(timer >= timeDelay) {
                timer = 0;
                ++tick;
                GameEvents.onTickLogic();
                GameEvents.onTickShow();
            }
        }
    }
}
