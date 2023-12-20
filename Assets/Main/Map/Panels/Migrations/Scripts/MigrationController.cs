using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MigrationController : MonoBehaviour, IGameConnecter {
    [SerializeField] GameObject startLine;
    [SerializeField] IconMarker endLine;
    [SerializeField] Material lineMaterial;
    [SerializeField] Sprite markerSprite;
    [SerializeField] int minPopulationToMigration; 
    [SerializeField, Range(0, 100)] private int percentToMigration;
    [SerializeField, Range(0, 100)] private int percentPerTick;

    private Dictionary<int, MigrationData> migrations = new();

    private TimelineController timeline;
    private WmskController wmsk;
    private MapController map;

    public Action<int, int, int> OnOpenPanel;
    public Action<int, int> OnUpdatePanel;
    public Action<int> OnClosePanel;

    public GameController GameController {
        set {
            value.Get(out timeline);
            value.Get(out wmsk);
            value.Get(out map);
        }
    }

    public void CreateMigration() {
        float[] civArr = map.data.GetArrayCivilizationsID();
        
        int from = 0;
        int to = 0;
        float civID = 0;

        Dictionary<int, float> civs = new();
        Dictionary<int, List<int>> neighbours = new();

        for(int i = 0; i < civArr.Length; ++i) {
            civID = civArr[i];
            for(int j = 0; j < map.data.GetCountRegionsInCivilization(civArr[i]); ++j) {
                from = map.data.GetRegionIndexFromCivilization(civArr[i], j);
                if (migrations.ContainsKey(from)) continue;
                for(int k = 0; k < map.data.GetCountNeighbours(from); ++k) {
                    to = map.data.GetNeighbour(from, k);
                    if(map.data.GetEcologyDetail(from, 3, 0) < map.data.GetEcologyDetail(to, 3, 0) &&
                        map.data.GetPopulations(from) >= minPopulationToMigration &&
                        map.data.GetPopulations(to) <= minPopulationToMigration &&
                        !migrations.ContainsKey(to)) {
                        if(!civs      .ContainsKey(from)) civs.Add(from, civID);
                        if (!neighbours.ContainsKey(from)) neighbours.Add(from, new());
                        neighbours[from].Add(to);
                    }
                }
            }
        }

        if (civs.Count == 0) return;
        from = civs.Keys.ToArray()[Randomizer.Random(civs.Count)];
        if (migrations.ContainsKey(from)) return;
        civID = civs[from];
        to = neighbours[from][Randomizer.Random(neighbours[from].Count)];
        if (migrations.ContainsKey(to)) return;
        CreateMigration(from, to, civID);
    }

    public void CreateMigration(int from, int to, float fromCivID) {
        if (!map.data.HasCivilization(to, fromCivID)) {
            map.data.CopyCivilization(from, to, fromCivID);
            //map.data.SetPopulation(to, civID, 0);
        }

        float toCivID = fromCivID < 1 ? (to + 1) / 100f : fromCivID;
        float fromPop = map.data.GetPopulation(from, fromCivID);
        float toPop   = map.data.GetPopulation(to, toCivID);
        // print($"From: {from} | To: {to}");
        // print($"FromPop: {fromPop} | ToPop: {toPop}");
        int step    = (int) GetParamStep(fromPop, toPop);

        // print($"Step: {step}");

        MigrationData newMigration = new MigrationData {
            FromCivID = fromCivID,
            ToCivID = toCivID,
            Step = step,
            From = from,
            To = to
        };

        migrations.Add(from, newMigration);

        CreateLine(newMigration);
    }

    private void UpdateMigration() {
        for (int i = 0; migrations.Count > 0 && i < migrations.Count; ++i) {
            MigrationData migration = migrations.Values.ToArray()[i];

            if (migration.Percent + percentPerTick > 100 ||
                !map.data.HasCivilization(migration.From, migration.FromCivID) ||
                !map.data.HasCivilization(migration.To, migration.ToCivID)) {
                DestroyMigration(migration.From);
                continue;
            }
            else migration.Percent += percentPerTick;
            OnUpdatePanel?.Invoke(migration.From, migration.Percent);

            float populationFrom = map.data.GetPopulation(migration.From, migration.FromCivID);
            float populationTo = map.data.GetPopulation(migration.To, migration.ToCivID);

            map.data.SetPopulation(migration.From, migration.FromCivID, populationFrom - migration.Step);
            map.data.SetPopulation(migration.To,   migration.ToCivID, populationTo   + migration.Step);
        }
    }

    public bool HasMigration(int regionIndex) => migrations.ContainsKey(regionIndex);

    public void DestroyMigration(int from) {
        OnClosePanel?.Invoke(from);
        migrations[from].Line.FadeOut(0);
        migrations[from].Marker.DestroyGO();
        migrations.Remove(from);
    }

    public void AmplifyMigration(int from) {
        // 100 - percent
        // step = 500 = 15%
        // end = step / stepPercent * (100 - percent)

        // print($"Migration From: {migrations[from].Step} / {percentPerTick} * (100 - {migrations[from].Percent}) = {migrations[from].Step / percentPerTick * (100 - migrations[from].Percent)}");
        // print($"Migration: {migrations[from].Step} / {percentPerTick} * (100 - {migrations[from].Percent}) = {migrations[from].Step / percentPerTick * (100 - migrations[from].Percent)}");

        map.data.SetPopulation(migrations[from].From, migrations[from].FromCivID,
            map.data.GetPopulation(migrations[from].From, migrations[from].FromCivID) -
            (migrations[from].Step / percentPerTick * (100 - migrations[from].Percent)));
        
        map.data.SetPopulation(migrations[from].To, migrations[from].ToCivID,
            map.data.GetPopulation(migrations[from].To, migrations[from].ToCivID) +
            (migrations[from].Step / percentPerTick * (100 - migrations[from].Percent)));
        DestroyMigration(from);
    }

    private bool CreateMarker(MigrationData data) {
        if (!wmsk.GetRegionPosition(data.From, out Vector2 position)) return false;
        wmsk.GetRegionPosition(data.From, out Vector2 startPosition);
        wmsk.GetRegionPosition(data.To, out Vector2 endPosition);
        Vector2 center = (startPosition + endPosition) / 2;
        data.Marker = wmsk.CreateMarker(center, -1, markerSprite, (IconMarker owner) => {
            OnOpenPanel?.Invoke(data.From, data.To, data.Percent);
        },
        () => { });
        return true;
    }

    public void CreateLine(MigrationData data) {
        wmsk.GetRegionPosition(data.From, out Vector2 start);
        wmsk.GetRegionPosition(data.To, out Vector2 end);

        Color color = Color.red;
        Vector3 startCapScale = new Vector3(0.5f, 0.5f, 1f);
        Vector3 endCapScale = new Vector3(0.5f, 0.5f, 1f);
        
        CreateMarker(data);
        
        data.Line = wmsk.CreateLine(start, end, color, lineMaterial, startLine, endLine.gameObject, startCapScale, endCapScale, lineWidth: 3);
    }

    private float GetParamStep(float from, float to) {
        float different = to - from;
        if (different == 0) return 0;
        float orientation = different / Mathf.Abs(different);
        // print($"Orientation: {different} / {Mathf.Abs(different)} = {orientation}");

        float path = different * (percentToMigration / 100f) * orientation;
        // print($"Path: {different} * ({percentToMigration} / 100) * {orientation} = {path}");
        // print($"Step: {path} / {percentPerTick} = {path / percentPerTick}");

        return path / percentPerTick;
    }

    public void Init() {
        timeline.OnUpdateData += CreateMigration;
        timeline.OnUpdateData += UpdateMigration;
    }

    private void OnDestroy() {
        timeline.OnUpdateData -= CreateMigration;
        timeline.OnUpdateData -= UpdateMigration;
    }
}
