using System.Collections;
using UnityEngine;

public class Timeline : MonoBehaviour {
    [SerializeField] private Vector2 timeDelayLimit;
    private float timeDelay;
    private float timer;
    private bool isPlay;
    
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
                GameEvents.onTick();
            }
        }
    }
}
