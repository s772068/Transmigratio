using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MigrationController : MonoBehaviour, IGameConnecter {
    [SerializeField] private GUIP_Migration panel;
    [SerializeField, Range(0, 100)] private int percentToMigration;
    [SerializeField, Range(0, 100)] private int percentPerTick;
    [SerializeField] GameObject startLine;
    [SerializeField] IconMarker endLine;
    [SerializeField] Material lineMaterial;
    [SerializeField] Sprite markerSprite;

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

    private void CreateMigration() {
        float civID = map.data.GetCivilizationKeys()[Randomizer.Random(map.data.CountCivilizations)];
        if (!map.data.GetRandomRegion(civID, out int from)) return;
        if (migrations.ContainsKey(from)) return;
        if (!map.data.GetRandomNeighbour(from, out int to)) return;
        if (migrations.ContainsKey(to)) return;
        CreateMigration(from, to, civID);
    }

    public void CreateMigration(int from, int to, float civID) {
        if (!map.data.HasCivilization(to, civID)) {
            map.data.CopyCivilization(to, from, civID);
            map.data.SetPopulation(to, civID, 0);
        }

        int fromPop = map.data.GetPopulation(from, civID);
        int toPop   = map.data.GetPopulation(to, civID);
        int step    = (int) GetParamStep(fromPop, toPop);

        MigrationData newMigration = new MigrationData {
            CivID = civID,
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

            if (migration.Percent + percentPerTick > 100) {
                OnClosePanel?.Invoke(migration.From);
                migration.Line.FadeOut(0);
                migration.Marker.DestroyGO();
                migrations.Remove(migration.From);
                continue;
            }
            else migration.Percent += percentPerTick;
            OnUpdatePanel?.Invoke(migration.From, migration.Percent);

            int populationFrom = map.data.GetPopulation(migration.From, migration.CivID);
            int populationTo = map.data.GetPopulation(migration.To, migration.CivID);

            map.data.SetPopulation(migration.From, migration.CivID, populationFrom - migration.Step);
            map.data.SetPopulation(migration.To,   migration.CivID, populationTo   + migration.Step);
        }
    }

    private bool CreateMarker(MigrationData data) {
        if (!wmsk.GetRegionPosition(data.From, out Vector2 position)) return false;
        wmsk.GetRegionPosition(data.From, out Vector2 startPosition);
        wmsk.GetRegionPosition(data.To, out Vector2 endPosition);
        Vector2 center = (startPosition + endPosition) / 2;
        data.Marker = wmsk.CreateMarker(center, -1, markerSprite, (IconMarker owner) => {
            OnOpenPanel?.Invoke(data.From, data.To, data.Percent);
        });
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

        float path = different * (percentToMigration / 100f) * orientation;

        return path / percentPerTick;
    }

    public void Init() {
        timeline.OnCreateMigration += CreateMigration;
        timeline.OnUpdateData += UpdateMigration;
    }

    private void OnDestroy() {
        timeline.OnCreateMigration -= CreateMigration;
        timeline.OnUpdateData -= UpdateMigration;
    }
}
