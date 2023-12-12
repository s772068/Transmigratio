using UnityEngine;

public class HungerEvent : BaseEvent {
    private MigrationController migration;
    private ResourcesController resources;
    private MapController map;
    private int migrateRegionIndex;
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

    public HungerEvent(GameController game, int migratePopulation, int costIntervention) {
        this.migratePopulation = migratePopulation;
        this.costIntervention = costIntervention;
        game.Get(out migration);
        game.Get(out resources);
        game.Get(out map);
    }

    public override bool CheckActivate(ref S_Region region) {
        if (region.GetCountCivilizations() == 0) return false;
        // int farmers = region.AllCivilizationVlaues(1,0);
        // int hunters = region.AllCivilizationVlaues(1,1);
        // int flora = region.Ecology[2][0];
        // int fauna = region.Ecology[3][0];
        // int eat = Proportion(farmers, hunters) > 50 ? flora : fauna;
        return false;
        // return eat <= 0;
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
            case 0: Nothing(); break;
            case 1: Migration(regionIndex); break;
            case 2: Intervene(); break;
        }
    }

    private bool CheckMigration(int regionIndex) {
        for(int i = 0; i < map.data.GetRegion(regionIndex).GetCountNeighbours(); ++i) {
            if (map.data.GetRegion(map.data.GetRegion(regionIndex).GetNeighbour(i)).GetCountCivilizations() == 0) {
                migrateRegionIndex = map.data.GetRegion(regionIndex).GetNeighbour(i);
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
            To = migrateRegionIndex,
            MaxPopulation = migratePopulation
        };
        migration.StartMigration(migrationData);
    }

    private void Intervene() {
        resources.intervention -= costIntervention;
        Debug.Log("Intervene");
    }
    private float Proportion(params float[] vals) {
        float all = 0;
        for (int i = 0; i < vals.Length; ++i) {
            all += vals[i];
        }
        return vals[0] / all;
    }
}
