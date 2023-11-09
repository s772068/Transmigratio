using UnityEngine;
using System;

public class HungerEvent : BaseEvent {
    public Action<S_Migration> OnMigration;

    public override int CountRes => 3;

    public override S_Event Data => new S_Event {
        Name = "Hunger",
        Description = "Hunger description",
        MarkerIndex = 1,
        IconIndex = 1,
        Results = new S_EventResult[] {
            new() {
                Name = "Nothing",
                Description = "Hunger nothing"
            },
            new() {
                Name = "Intervene",
                Description = "Hunger intervene"
            },
            new() {
                Name = "Migration",
                Description = "Hunger migration"
            }
        }
    };

    public HungerEvent(Action<S_Migration> onMigration) {
        OnMigration = onMigration;
    }

    public override void Use(ref S_Country country, int index) {
        switch (index) {
            case 0:
                Intervene(ref country);
                break;
            case 1:
                Nothing(ref country);
                break;
            case 2:
                Migration(ref country);
                break;
        }
    }

    public void Intervene(ref S_Country country) {
        Debug.Log("Intervene");
    }

    public void Nothing(ref S_Country country) {
        Debug.Log("Nothing");
    }

    public void Migration(ref S_Country country) {
        Debug.Log("Migration");
        OnMigration?.Invoke(default);
    }
}
