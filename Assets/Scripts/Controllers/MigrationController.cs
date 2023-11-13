using System.Collections.Generic;
using UnityEngine;

public class MigrationController : BaseController {
    [SerializeField] private GUI_MigrationPanel panel;
    [SerializeField] private int populationsPerTick;

    private List<S_Migration> migrations = new();
    private S_Migration migration;

    private LocalizationController localization;
    private WmskController wmsk;
    private MapController map;

    public override GameController GameController {
        set {
            localization = value.Get<LocalizationController>();
            wmsk = value.Get<WmskController>();
            map = value.Get<MapController>();
        }
    }

    public bool OpenPanel(S_Migration data, int index) {
        if (panel.gameObject.activeSelf) return false;
        panel.index = index;
        panel.gameObject.SetActive(true);
        panel.Localization(localization.Localization.Migration,
                           localization.Localization.System);
        panel.Init(data, localization.Localization.Countries[data.From],
                         localization.Localization.Countries[data.To]);
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
                map.countries[migration.From].Population -= populationsPerTick;
                map.countries[migration.To].Population += populationsPerTick;
                migrations[i] = migration;
            } else {
                map.countries[migration.From].Population -= migration.MaxPopulation - migration.Population;
                map.countries[migration.To].Population += migration.MaxPopulation - migration.Population;
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
