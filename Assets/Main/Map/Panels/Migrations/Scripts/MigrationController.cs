using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MigrationController : MonoBehaviour, IGameConnecter {
    [SerializeField] private GUI_MigrationPanel panel;
    [SerializeField, Range(0, 100)] private int percentToMigration;
    [SerializeField, Range(0, 100)] private int percentPerTick;

    private Dictionary<int, MigrationData> migrations = new();
    // private S_Migration migration;

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

    //    public bool OpenPanel(S_Migration data, int index) {
    //        if (panel.gameObject.activeSelf) return false;
    //        panel.index = index;
    //        panel.gameObject.SetActive(true);
    //        panel.Localization(settings.Localization.Migration,
    //                           settings.Localization.System);
    //        panel.Init(data, settings.Localization.Map.Countries.Value[data.From],
    //                         settings.Localization.Map.Countries.Value[data.To]);
    //        panel.Population = migrations[panel.index].Population;
    //        panel.OnClose = () => panel.gameObject.SetActive(false);
    //        return true;
    //    }

    private void CreateMigration() {
        int civID = map.data.GetCivilizationKeys()[Randomizer.Random(map.data.CountCivilizations)];
        int from = map.data.GetCivilization(civID)[Randomizer.Random(map.data.GetCivilization(civID).Length)];
        if (migrations.ContainsKey(from)) return;
        if (!map.data.GetRandomNeighbour(from, out int to)) return;
        if (migrations.ContainsKey(to)) return;
        CreateMigration(from, to, civID);
    }

    public void CreateMigration(int from, int to, int civID) {
        if (!map.data.HasCivilization(to, civID)) {
            map.data.CopyCivilization(to, civID, map.data.GetCivilization(from, civID));
            map.data.SetPopulation(to, civID, 0);
        }

        MigrationData newMigration = new MigrationData {
            Step = CreateStep(from, to, civID),
            CivID = civID,
            From = from,
            To = to
        };
        migrations.Add(from, newMigration);
    }

    private void UpdateMigration() {
        for (int i = 0; migrations.Count > 0 && i < migrations.Count; ++i) {
            MigrationData migration = migrations.Values.ToArray()[i];

            // Population
            // if(migration.MaxPercent < migration.Percent + migration.PercentPerTick)
            //    migration.PercentPerTick = migration.Percent + migration.PercentPerTick - migration.MaxPercent;
            // print(migration.Percent + " -> " + (migration.Percent + migration.PercentPerTick));
            
            if (migration.Percent + percentPerTick > 100) {
                migrations.Remove(migration.From);
                continue;
            }
            else migration.Percent += percentPerTick;
            print(migration.Percent);
            
            S_Civilization from = map.data.GetCivilization(migration.From, migration.CivID);
            S_Civilization to = map.data.GetCivilization(migration.To, migration.CivID);
            S_Civilization step = migration.Step;
            //// int different = to - from;
            //// int orientation = different / Mathf.Abs(different);
            //// int path = (int)(different * (migration.MaxPercent / 100f) * orientation);
            //// int step = path / migration.PercentPerTick;

            //from.SetPopulation(from.GetPopulation() - step.GetPopulation());
            //to.SetPopulation(to.GetPopulation() + step.GetPopulation());
            // print("From: " + migration.From + " To: " + migration.To);

            if (step.GetPopulation() != 0) {
                from.SetPopulation(from.GetPopulation() - step.GetPopulation());
                to.SetPopulation(to.GetPopulation() + step.GetPopulation());
            }

            if (step.GetTakenFood() != 0) {
                from.SetTakenFood(from.GetTakenFood() - step.GetTakenFood());
                to.SetTakenFood(to.GetTakenFood() + step.GetTakenFood());
            }

            if (step.GetGovernmentObstacle() != 0) {
                from.SetGovernmentObstacle(from.GetGovernmentObstacle() - step.GetGovernmentObstacle());
                to.SetGovernmentObstacle(to.GetGovernmentObstacle() + step.GetGovernmentObstacle());
            }

            for (int paramIndex = 0; paramIndex < step.GetCountParamiters(); ++paramIndex) {
                for (int detailIndex = 0; detailIndex < step.GetCountDetails(paramIndex); ++detailIndex) {
                    // print(civilization.GetDetail(paramIndex, detailIndex));
                    if (step.GetDetail(paramIndex, detailIndex) != 0) {
                        from.SetDetail(paramIndex, detailIndex,
                            from.GetDetail(paramIndex, detailIndex) -
                            step.GetDetail(paramIndex, detailIndex));

                        to.SetDetail(paramIndex, detailIndex,
                            to.GetDetail(paramIndex, detailIndex) +
                            step.GetDetail(paramIndex, detailIndex));
                    }
                }
            }

            // print("From: " + migration.From + " Val : " + map.data.GetPopulation(migration.From, migration.CivID) + "\n" +
            //   "To: " + migration.To + " Val : " + map.data.GetPopulation(migration.To, migration.CivID) + "\n");

            // print("From: "        + from        + "\n" +
            //       "To: "          + to          + "\n" +
            //       "Different: "   + different   + "\n" +
            //       "Orientation: " + orientation + "\n" +
            //       "Path: "        + path        + "\n" +
            //       "Step: "        + step);

            // map.data.SetPopulation(migration.From, migration.CivID, population);
            /*
            Population;
            TakenFood;
            GovernmentObstacle;
            Paramiters;
            */
        }
        //foreach(var val in migrations) {
        //    S_Migration migration = val.Value;
        //    if (migration.Percent > migration.MaxPercent)
        //        migrations.Remove(val.Key);
        //}
    }

    private float GetParamStep(float from, float to) {
        float different = to - from;
        //print("From: " + from + " To: " + to);
        //print("Different: " + different);

        if (different == 0) { /*print("Res: 0"); print("---");*/ return 0; }
        float orientation = different / Mathf.Abs(different);
        //print("Orientation: " + orientation);

        float path = different * (percentToMigration / 100f) * orientation;


        //print("---");
        return path / percentPerTick;
    }

    private S_Civilization CreateStep(int from, int to, int civID) {
        S_Civilization civilization = new();
        civilization.SetPopulation((int) GetParamStep(map.data.GetPopulation(from, civID), map.data.GetPopulation(to, civID)));
        civilization.SetTakenFood(GetParamStep(map.data.GetTakenFood(from, civID), map.data.GetTakenFood(to, civID)));
        civilization.SetGovernmentObstacle(GetParamStep(map.data.GetGovernmentObstacle(from, civID), map.data.GetGovernmentObstacle(to, civID)));
        for (int paramIndex = 0; paramIndex < map.data.GetCountCivilizationParamiters(from, civID); ++paramIndex) {
            S_Paramiter paramiter = new();
            for (int detailIndex = 0; detailIndex < map.data.GetCountCivilizationDetails(from, civID, paramIndex); ++detailIndex) {
                paramiter.AddDetail(GetParamStep(map.data.GetCivilizationDetail(from, civID, paramIndex, detailIndex), map.data.GetCivilizationDetail(to, civID, paramIndex, detailIndex)));
            }
            civilization.AddParamiter(paramiter);
        }

        // print(civilization.GetPopulation());
        // print(civilization.GetTakenFood());
        // print(civilization.GetGovernmentObstacle());
        // for (int paramIndex = 0; paramIndex < map.data.GetCountCivilizationParamiters(from, civID); ++paramIndex) {
        //     for (int detailIndex = 0; detailIndex < map.data.GetCountCivilizationDetails(from, civID, paramIndex); ++detailIndex) {
        //         print(civilization.GetDetail(paramIndex, detailIndex));
        //     }
        // }

        return civilization;
    }
    // {
    //     float different = to - from;
    //     float orientation = different / Mathf.Abs(different);
    //     float path = different * (percentToMigration / 100f) * orientation;
    //     return path / percentPerTick;
    // }

    //public void StartMigration(MigrationData migration) {
    //        wmsk.StartMigration(migration, migrations.Count);
    //        migrations.Add(migration);
    //}

    //    public void UpdateMigration() {
    //        int civIndex;
    //        for(int i = 0; i < migrations.Count; ++i) {
    //            migration = migrations[i];
    //            if (populationsPerTick < migration.MaxPopulation - migration.Population) {
    //                migration.Population += populationsPerTick;
    //                //civIndex = map.data.Regions[migration.From].StageToIndex(migration.Stage);
    //                //map.data.Regions[migration.From].Civilizations[civIndex].Population -= populationsPerTick;
    //                //if (Random.Range()/*Удаление населения с 5 - 30 % вероятностью*/) {
    //                //} else {
    //                //    civIndex = map.data.Regions[migration.To].StageToIndex(migration.Stage);
    //                //    map.data.Regions[migration.To].Civilizations[civIndex].Population += populationsPerTick;
    //                //}
    //                //migrations[i] = migration;
    //            } else {
    ////                map.data.Regions[migration.From].Population -= migration.MaxPopulation - migration.Population;
    ////                map.data.Regions[migration.To].Population += migration.MaxPopulation - migration.Population;
    //                panel.Close();
    //                EndMigration(i);
    //            }
    //        }
    //        if (panel.gameObject.activeSelf) panel.UpdatePanel(migrations[panel.index].Population);
    //    }

    //    public void EndMigration(int index) {
    //        wmsk.EndMigration(index);
    //        migrations.RemoveAt(index);
    //    }

    public void Init() {
        timeline.OnCreateMigration += CreateMigration;
        timeline.OnUpdateData += UpdateMigration;
    }

    private void OnDestroy() {
        timeline.OnCreateMigration -= CreateMigration;
        timeline.OnUpdateData -= UpdateMigration;
    }
}
