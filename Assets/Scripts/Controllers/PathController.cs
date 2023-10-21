using WorldMapStrategyKit;
using UnityEngine;
using Zenject;

public class PathController : MonoBehaviour {
    private WMSK map;

    [Inject] private MapHolder mapHolder;

    private void Awake() {
        map = WMSK.instance;
    }
}
