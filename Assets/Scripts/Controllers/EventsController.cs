using System.Collections.Generic;
using UnityEngine;

public class EventsController : BaseController {
    [SerializeField] private float[] chances;

    private List<BaseEvent> events = new();
    private List<int> logMessages = new();
    
    private WmskController wmsk;
    private SaveController save;
    private MigrationController migration;

    public override GameController GameController {
        set {
            wmsk = value.Get<WmskController>();
            save = value.Get<SaveController>();
            migration = value.Get<MigrationController>();
        }
    }

    public void Call() {
        for (int i = 0; i < save.countries.Length; ++i) {
            if (save.countries[i].events.Count > 0 &&
                Random.Range(0, 1f) <= chances[save.countries[i].eventChanceIndex]) {
                save.countries[i].eventChanceIndex = 0;
                int countryEvent = Random.Range(0, save.countries[i].events.Count - 1);
                int index = save.countries[i].events[countryEvent];
                // print(save.countries[i].names[1] + " : " + events[index].Data.name);
                wmsk.CreateEventMarker(events[index].Data, index, i);
                save.countries[i].events.RemoveAt(countryEvent);
                return;
                // if (save.countries[i].usedEvents.Count < events.Count) {
                // 
                // }
            } else {
                ++save.countries[i].eventChanceIndex;
            }
        }
    }

    public void ClickResult(int countryIndex, int eventIndex, int resultIndex) {
        events[eventIndex].Use(ref save.countries[countryIndex], resultIndex);
    }

    public void AddEvnts(int index) {
        for(int i = 0; i < events.Count; ++i) {
            save.countries[index].events.Add(i);
        }
    }

    public override void Init() {
        events.Add(new VolcanoEvent(migration.StartMigration));
        events.Add(new HungerEvent(migration.StartMigration));
    }
}
