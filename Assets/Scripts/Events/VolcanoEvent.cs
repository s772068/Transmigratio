using UnityEngine;
using System;

public class VolcanoEvent : BaseEvent {
    public Action<S_Migration> OnMigration;

    public override int CountRes => 3;

    public override S_Event Data => new S_Event {
        Name = "Volcano",
        Description = "Volcano description",
        Results = new S_EventResult[] {
            new() {
                Name = "Migration",
                Description = "Volcano migration"
            },
            new() {
                Name = "Nothing",
                Description = "Volcano nothing"
            },
            new() {
                Name = "Intervene",
                Description = "Volcano intervene"
            }
        }
    };

    public VolcanoEvent(Action<S_Migration> onMigration) {
        OnMigration = onMigration;
    }

    public override void Use(ref S_Country country, int index) {
        switch (index) {
            case 0:
                Migration(ref country);
                break;
            case 1:
                Nothing(ref country);
                break;
            case 2:
                Intervene(ref country);
                break;
        }
    }

    public void Migration(ref S_Country country) {
        Debug.Log("Migration");
        OnMigration?.Invoke(default);
    }

    public void Nothing(ref S_Country country) {
        Debug.Log("Nothing");
    }

    public void Intervene(ref S_Country country) {
        Debug.Log("Intervene");
    }
}
