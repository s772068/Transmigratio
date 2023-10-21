using System.Collections;
using UnityEngine;
using Zenject;

public class TimelineController : MonoSingleton<TimelineController> {
    [SerializeField] private float interval;

    [Inject] private MapHolder holder;

    private bool isActive;

    public void Active(bool val) {
        isActive = val;
        if (val) StartCoroutine(Active(holder.map));
    }

    private IEnumerator Active(S_Map map) {
        while (isActive) {
            //GameEvents.Step();
            yield return new WaitForSeconds(interval);
        }
    }
}
