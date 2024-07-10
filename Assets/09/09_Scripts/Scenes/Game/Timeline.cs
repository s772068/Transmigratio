using System.Collections;
using UnityEngine;

public class Timeline : PersistentSingleton<Timeline> {
    [SerializeField] private ButtonsRadioGroup buttonsGroup;
    [SerializeField] private Vector2 timeDelayLimit;
    private float timeDelay;
    private float timer;
    private bool isPlay;
    private int tick;

    public int Tick => tick;

    public void Pause() {
        Debug.Log("Pause");
        isPlay = false;
        buttonsGroup.Click(0);
    }

    public void Play() {
        Debug.Log("Play");
        timeDelay = timeDelayLimit.y;
        if (!isPlay) StartCoroutine(TickPlay());
        buttonsGroup.Click(1);
    }

    public void Rewind() {
        timeDelay = timeDelayLimit.x;
        if (!isPlay) StartCoroutine(TickPlay());
        Debug.Log("Rewind");
        buttonsGroup.Click(2);
    }

    private IEnumerator TickPlay() {
        isPlay = true;
        while (isPlay) {
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
            if(timer >= timeDelay) {
                timer = 0;
                ++tick;
                GameEvents.TickLogic();
                GameEvents.TickShow();
            }
        }
    }
}
