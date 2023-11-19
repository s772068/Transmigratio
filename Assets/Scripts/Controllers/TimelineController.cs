using System.Collections;
using UnityEngine;

public class TimelineController : BaseController {
    private float interval;
    private ResourcesController resources;
    private MigrationController migration;
    private EventsController events;
    private MapController map;

    private bool isActive;

    public override GameController GameController {
        set {
            resources = value.Get<ResourcesController>();
            migration = value.Get<MigrationController>();
            events = value.Get<EventsController>();
            map = value.Get<MapController>();
        }
    }

    public float Interval {
        set {
            if (value == 0) {
                isActive = false;
            } else if (interval == 0 && value > 0) {
                isActive = true;
                StartCoroutine(UpdateActive());
            }
            interval = value;
        }
    }

    private void Start() {
        Interval = 1;
    }

    private IEnumerator UpdateActive() {
        while (isActive) {
            // events.Call();
            // migration.UpdateMigration();
            map.UpdateParams();
            resources.UpdateResources();
            yield return new WaitForSeconds(interval);
        }
    }
}
