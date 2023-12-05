using UnityEngine;

public class VolcanoEvent : BaseEvent {
    private MigrationController migration;
    private ResourcesController resources;
    private MapController map;
    private int migrateRegionIndex;
    private int migratePopulation;
    private int costIntervention;
    private float[] chances = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f};

    public override S_Event Data(int localization) =>
        localization switch {
            0 => new S_Event {
                Name = "Volcano",
                Description = "Volcano description",
                Results = new S_EventResult[] {
                    new() {
                        Name = "Nothing",
                        Description = "Volcano nothing"
                    },
                    new() {
                        Name = "Migration",
                        Description = "Volcano migration"
                    },
                    new() {
                        Name = "Intervene",
                        Description = "Volcano intervene"
                    }
                }
            },
            1 => new S_Event {
                Name = "Извержение вулкана",
                Description = "Вулкан извергается",
                Results = new S_EventResult[] {
                    new() {
                        Name = "Не вмешиваться",
                        Description = "Без вмешательства"
                    },
                    new() {
                        Name = "Миграция",
                        Description = "Миграция людей"
                    },
                    new() {
                        Name = "Утихомирить вулкан",
                        Description = "Утихомирить вулкан"
                    }
                }
            },
            _ => default
        };

    public VolcanoEvent(GameController game, int migratePopulation, int costIntervention) {
        this.migratePopulation = migratePopulation;
        this.costIntervention = costIntervention;
        game.Get(out migration);
        game.Get(out resources);
        game.Get(out map);
    }

    public override bool CheckActivate(ref S_Region region) {
        if(region.Civilizations.Length == 0) return false;
        if (region.EventChanceIndex > -1) {
            if (Random.Range(0, 1f) <= chances[region.EventChanceIndex]) {
                region.EventChanceIndex = -1;
                return true;
            } else {
                ++region.EventChanceIndex;
                return false;
            }
        } else return false;
    }

    public override bool CheckBuild(int regionIndex, int resultIndex) =>
        resultIndex switch {
            0 => true,
            1 => CheckMigration(regionIndex),
            2 => resources.intervention >= costIntervention,
            _ => false
        };

    public override void Use(int regionIndex, int index) {
        switch (index) {
            case 0: ActivateVolcano(); break;
            case 1: Migration(regionIndex); break;
            case 2: Intervene(); break;
        }
    }

    private bool CheckMigration(int regionIndex) {
        int[] neighbours = map.data.Regions[regionIndex].Neighbours;
        for (int i = 0; i < neighbours.Length; ++i) {
            // if(neighbours)
            if (map.data.Regions[map.data.Regions[regionIndex].Neighbours[i]].Civilizations.Length == 0) {
                migrateRegionIndex = map.data.Regions[regionIndex].Neighbours[i];
                return true;
            }
        }
        return false;
    }

    private void ActivateVolcano() {
        Debug.Log("ActivateVolcano");
    }

    private void Migration(int from) {
        Debug.Log("Migration");
        S_Migration migrationData = new S_Migration {
            From = from,
            To = migrateRegionIndex,
            MaxPopulation = migratePopulation
        };
        migration.StartMigration(migrationData);
    }

    private void Intervene() {
        resources.intervention -= costIntervention;
        Debug.Log("Intervene");
        // Realise volcanoes action
    }
}
