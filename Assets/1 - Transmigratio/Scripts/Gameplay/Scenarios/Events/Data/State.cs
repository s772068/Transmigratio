using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace Gameplay.Scenarios.Events.Data {
    [System.Serializable]
    public class State {        
        [SerializeField] private List<int> ticks = new();
        [SerializeField, Space(10)] private UnityEvent onEndTime;

        private int tick;
        private int tickDestination = -1;


        public void Start() {
            tick = 0;
            if (ticks.Count == 1) tickDestination = ticks[0];
            else if (ticks.Count == 2) tickDestination = Random.Range(ticks[0], ticks[1] + 1);
        }
        
        public bool Update() {
            ++tick;
            bool isEndTime = tick >= tickDestination;
            if (isEndTime) onEndTime?.Invoke();
            return isEndTime;
        }
    }
}
