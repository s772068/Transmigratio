using UnityEngine;

public class HungerEvent : BaseEvent {
    private protected MigrationController migration;
    private protected ResourcesController resources;
    private protected MapController map;
    private int migrateCountryIndex;
    private int migratePopulation;
    private int costIntervention;

    public override S_Event Data(int localization) =>
        localization switch {
            0 => new S_Event {
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
                        Name = "Migration",
                        Description = "Hunger migration"
                    },
                    new() {
                        Name = "Intervene",
                        Description = "Hunger intervene"
                    }
                }
            },
            1 => new S_Event {
                Name = "Голод",
                Description = "Люди голодают",
                MarkerIndex = 1,
                IconIndex = 1,
                Results = new S_EventResult[] {
                    new() {
                        Name = "Не вмешиваться",
                        Description = "Пускай люди поголодают"
                    },
                    new() {
                        Name = "Мигрировать",
                        Description = "Пора уже искать еду в других местах"
                    },
                    new() {
                        Name = "Вмешаться",
                        Description = "Накормить людей на 50 лет вперёд"
                    }
                }
            },
            _ => default
        };

    public HungerEvent(GameController gameController, int migratePopulation, int costIntervention) {
        this.migratePopulation = migratePopulation;
        this.costIntervention = costIntervention;
        migration = gameController.Get<MigrationController>();
        resources = gameController.Get<ResourcesController>();
        map = gameController.Get<MapController>();
    }

    public override bool CheckBuild(int countryIndex, int resultIndex) =>
        resultIndex switch {
            0 => true,
            1 => CheckMigration(countryIndex),
            2 => resources.intervention >= costIntervention,
            _ => false
        };

    public override void Use(int countryIndex, int index) {
        switch (index) {
            case 0: Nothing(); break;
            case 1: Migration(countryIndex); break;
            case 2: Intervene(); break;
        }
    }

    private bool CheckMigration(int countryIndex) {
        for(int i = 0; i < map.countries[countryIndex].Neighbours.Length; ++i) {
            if (map.countries[map.countries[countryIndex].Neighbours[i]].Population == 0) {
                migrateCountryIndex = map.countries[countryIndex].Neighbours[i];
                return true;
            }
        }
        return false;
    }

    private void Nothing() {
        Debug.Log("Nothing");
    }

    private void Migration(int from) {
        Debug.Log("Migration");
        S_Migration migrationData = new S_Migration {
            From = from,
            To = migrateCountryIndex,
            MaxPopulation = migratePopulation
        };
        migration.StartMigration(migrationData);
    }

    private void Intervene() {
        resources.intervention -= costIntervention;
        Debug.Log("Intervene");
    }
}
