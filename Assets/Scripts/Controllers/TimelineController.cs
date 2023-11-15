using System.Collections;
using UnityEngine;

public class TimelineController : BaseController {
    private float interval;
    private MigrationController migrationController;
    private EventsController eventsController;
    private MapController mapController;

    private bool isActive;

    public override GameController GameController {
        set {
            migrationController = value.Get<MigrationController>();
            eventsController = value.Get<EventsController>();
            mapController = value.Get<MapController>();
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
            // mapController.UpdateParams();
            // eventsController.Call();
            // migrationController.UpdateMigration();
            yield return new WaitForSeconds(interval);
        }
    }
}
