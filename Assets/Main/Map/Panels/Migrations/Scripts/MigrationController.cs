using System.Collections.Generic;
using UnityEngine;

public class MigrationController : MonoBehaviour, IGameConnecter {
    [SerializeField] private GUI_MigrationPanel panel;
    [SerializeField] private int populationsPerTick;

    private List<S_Migration> migrations = new();
    private S_Migration migration;

    private TimelineController timeline;
    private SettingsController settings;
    private WmskController wmsk;
    private MapController map;

    public GameController GameController {
        set {
            value.Get(out timeline);
            value.Get(out settings);
            value.Get(out wmsk);
            value.Get(out map);
        }
    }

    public bool OpenPanel(S_Migration data, int index) {
        if (panel.gameObject.activeSelf) return false;
        panel.index = index;
        panel.gameObject.SetActive(true);
        panel.Localization(settings.Localization.Migration,
                           settings.Localization.System);
        panel.Init(data, settings.Localization.Map.Countries.Value[data.From],
                         settings.Localization.Map.Countries.Value[data.To]);
        panel.Population = migrations[panel.index].Population;
        panel.OnClose = () => panel.gameObject.SetActive(false);
        return true;
    }

    public void StartMigration(S_Migration migration) {
        wmsk.StartMigration(migration, migrations.Count);
        migrations.Add(migration);
    }

    public void UpdateMigration() {
        int civIndex;
        for(int i = 0; i < migrations.Count; ++i) {
            migration = migrations[i];
            if (populationsPerTick < migration.MaxPopulation - migration.Population) {
                migration.Population += populationsPerTick;
                //civIndex = map.data.Regions[migration.From].StageToIndex(migration.Stage);
                //map.data.Regions[migration.From].Civilizations[civIndex].Population -= populationsPerTick;
                //if (Random.Range()/*Удаление населения с 5 - 30 % вероятностью*/) {
                //} else {
                //    civIndex = map.data.Regions[migration.To].StageToIndex(migration.Stage);
                //    map.data.Regions[migration.To].Civilizations[civIndex].Population += populationsPerTick;
                //}
                //migrations[i] = migration;
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

    public void Init() {
        timeline.OnTick += UpdateMigration;
    }

    private void OnDestroy() {
        timeline.OnTick -= UpdateMigration;
    }
}
