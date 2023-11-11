using System.Collections.Generic;
using UnityEditor.Localization.Editor;
using UnityEngine;

public class MigrationController : BaseController {
    [SerializeField] private GUI_MigrationPanel panel;
    [SerializeField] private int populationsPerTick;

    private List<S_Migration> migrations = new();
    private S_Migration migration;

    private LocalizationController localization;
    private WmskController wmsk;
    private SaveController save;

    public override GameController GameController {
        set {
            localization = value.Get<LocalizationController>();
            wmsk = value.Get<WmskController>();
            save = value.Get<SaveController>();
        }
    }

    public bool OpenPanel(S_Migration data, int index) {
        if (panel.gameObject.activeSelf) return false;
        panel.index = index;
        panel.gameObject.SetActive(true);
        panel.Localization(localization.array[(int) save.localization].migration);
        panel.Init(data,
            save.countries[data.From].names[(int) save.localization],
            save.countries[data.To].names[(int) save.localization]);
        panel.Population = migrations[panel.index].Population;
        panel.OnClose = () => panel.gameObject.SetActive(false);
        return true;
    }

    public void StartMigration(S_Migration migration) {
        wmsk.StartMigration(migration, migrations.Count);
        migrations.Add(migration);
    }

    public void UpdateMigration() {
        for(int i = 0; i < migrations.Count; ++i) {
            migration = migrations[i];
            if (populationsPerTick < migration.MaxPopulation - migration.Population) {
                migration.Population += populationsPerTick;
                save.countries[migration.From].population -= populationsPerTick;
                save.countries[migration.To].population += populationsPerTick;
                migrations[i] = migration;
            } else {
                save.countries[migration.From].population -= migration.MaxPopulation - migration.Population;
                save.countries[migration.To].population += migration.MaxPopulation - migration.Population;
                panel.Close();
                EndMigration(i);
            }
        }
        if (panel.gameObject.activeSelf) panel.UpdatePanel(migrations[panel.index].Population);
    }

    public void EndMigration(int index) {
        wmsk.EndMigration(index);
        migrations.RemoveAt(index);
    }
}
