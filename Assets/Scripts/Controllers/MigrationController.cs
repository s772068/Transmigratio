using System.Collections.Generic;
using UnityEngine;

public class MigrationController : BaseController {
    [SerializeField] private GUI_MigrationPanel panel;
    [SerializeField] private int populationsPerTick;

    private List<S_Migration> migrations = new();
    private S_Migration migration;

    private SettingsController settings;
    private WmskController wmsk;
    private MapController map;

    public override GameController GameController {
        set {
            settings = value.Get<SettingsController>();
            wmsk = value.Get<WmskController>();
            map = value.Get<MapController>();
        }
    }

    public bool OpenPanel(S_Migration data, int index) {
        if (panel.gameObject.activeSelf) return false;
        panel.index = index;
        panel.gameObject.SetActive(true);
        panel.Localization(settings.Localization.Migration,
                           settings.Localization.System);
        panel.Init(data, settings.Localization.Map.Countries[data.From],
                         settings.Localization.Map.Countries[data.To]);
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
//                map.data.Regions[migration.From].Population -= populationsPerTick;
//                map.data.Regions[migration.To].Population += populationsPerTick;
                migrations[i] = migration;
            } else {
//                map.data.Regions[migration.From].Population -= migration.MaxPopulation - migration.Population;
//                map.data.Regions[migration.To].Population += migration.MaxPopulation - migration.Population;
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