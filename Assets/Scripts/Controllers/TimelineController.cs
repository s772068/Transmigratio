using System.Collections;
using UnityEngine;

public class TimelineController : BaseController {
    private float interval;
    private MigrationController migrationController;
    private EventsController eventsController;
    private CalcController calcController;

    private bool isActive;

    public override GameController GameController {
        set {
            migrationController = value.Get<MigrationController>();
            eventsController = value.Get<EventsController>();
            calcController = value.Get<CalcController>();
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
        // S_Migration data = new S_Migration {
        //     MaxPopulation = 100,
        //     Population = 0,
        //     MarkerIndex = 0,
        //     IconIndex = 0,
        //     From = 0,
        //     To = 1
        // };
        // migrationController.StartMigration(data);
        while (isActive) {
            eventsController.Call();
            migrationController.UpdateMigration();
            yield return new WaitForSeconds(interval);
        }
    }
}
