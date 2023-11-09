using System.Collections;
using UnityEngine;

public class TimelineController : BaseController {
    [SerializeField] private float interval;

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

    private void Start() {
        Active = true;
    }

    public bool Active {
        set {
            isActive = value;
            if (value) StartCoroutine(UpdateActive());
        }
    }

    private IEnumerator UpdateActive() {
        while (isActive) {
            // eventsController.Call();
            migrationController.StartMigration(default);
            yield return new WaitForSeconds(interval);
        }
    }
}
