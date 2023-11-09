using System.Collections.Generic;
using UnityEngine;

public class MigrationController : BaseController {
    [SerializeField] private int populationsPerTick;

    private List<S_Migration> migrations = new();
    private S_Migration migration;

    private WmskController wmsk;
    private SaveController save;
    private HUD hud;

    public override GameController GameController {
        set {
            wmsk = value.Get<WmskController>();
            save = value.Get<SaveController>();
            hud = value.Get<HUD>();
        }
    }

    public void StartMigration(S_Migration migration) {
        migrations.Add(migration);
        wmsk.StartMigration(migration);
    }

    public void UpdateMigration() {
        for(int i = 0; i < migrations.Count; ++i) {
            migration = migrations[i];
            if (populationsPerTick <= migration.MaxPopulation - migration.Population) {
                migration.Population += populationsPerTick;
                save.countries[migration.To].population += populationsPerTick;
            } else {
                save.countries[migration.To].population += migration.MaxPopulation - migration.Population;
                EndMigration(i);
            }
            migrations[i] = migration;
        }
    }

    public void EndMigration(int index) {
        wmsk.EndMigration(index);
        migrations.RemoveAt(index);
    }
}
