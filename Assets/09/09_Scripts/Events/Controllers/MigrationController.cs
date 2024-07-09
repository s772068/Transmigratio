using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using WorldMapStrategyKit;
using System.Linq;
using UnityEngine;
using System;

public class MigrationController : Singleton<MigrationController> {
    [SerializeField] private MigrationPanel panel;
    [Header("Marker")]
    [SerializeField] private IconMarker markerPrefab;
    [SerializeField] private Sprite markerSprite;
    [Header("Line")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private GameObject endLine;
    [Header("Timers")]
    [SerializeField, Min(0)] private int startTime;
    [SerializeField, Min(0)] private int interval;
    [Header("Percents")]
    [SerializeField, Range(0, 100)] private int percentToMigration;
    [SerializeField, Range(0, 100)] private int stepPercent;
    [Header("Colors")]
    [SerializeField] private Color regionColor;
    [SerializeField] private Color civColor;

    private int selectedID;
    private int aiSolutionIndex = -1;
    private List<Action<int>> ai = new();
    [SerializeField] private SerializedDictionary<int, MigrationData> migrations = new();
    
    public int Population => migrations.Sum(x => x.Value.fullPopulations - x.Value.curPopulations);

    private Map Map => Transmigratio.Instance.tmdb.map;
    private WMSK WMSK => Transmigratio.Instance.tmdb.map.wmsk;

    private void OnEnable() {
        ai = new() { (int i) => { }, ClickBreak, ClickSpeedUp };

        GameEvents.onTickLogic += OnTickLogic;
        GameEvents.onTickShow += UpdatePercent;
        GameEvents.onRemoveCivPiece += Remove;

        panel.onBreak = ClickBreak;
        panel.onNothing = ClickNothing;
        panel.onSpeedUp = ClickSpeedUp;
    }

    private void OnDisable() {
        ai.Clear();
        GameEvents.onTickLogic -= OnTickLogic;
        GameEvents.onTickShow -= UpdatePercent;
        GameEvents.onRemoveCivPiece -= Remove;
    }

    public void TryMigration(CivPiece civPice) {
        if (migrations.ContainsKey(civPice.Region.id)) return;

        TM_Region curRegion = civPice.Region;
        List<Country> neighbourRegions = WMSK.CountryNeighbours(curRegion.id);
        if (neighbourRegions.Count == 0) return;

        TM_Region targetRegion = null;
        List<TM_Region> targetList = new();
        List<TM_Region> allNeighbourRegionsList = new();

        for(int i = 0; i < neighbourRegions.Count; ++i) {
            int regionID = WMSK.GetCountryIndex(neighbourRegions[i].name);
            if (migrations.ContainsKey(regionID)) continue;
            
            TM_Region neighbourRegion = Map.GetRegionBywmskId(regionID);
            if (neighbourRegion.fauna["Fauna"].value > 0) {
                allNeighbourRegionsList.Add(neighbourRegion);
                if (neighbourRegion.fauna["Fauna"].value > curRegion.fauna["Fauna"].value) {
                    targetList.Add(neighbourRegion);
                }
            }
        }
        
        if (targetList.Count > 0) {
            targetRegion = GetMax(targetList, (TM_Region region) => region.fauna["Fauna"].value);
        } else if (allNeighbourRegionsList.Count > 0) {
            System.Random r = new System.Random();
            targetRegion = allNeighbourRegionsList[r.Next(0, allNeighbourRegionsList.Count)];
        } else return;
        
        Add(civPice.Region, targetRegion, civPice.Civilization);
    }

    private void Add(TM_Region from, TM_Region to, Civilization civ) {
        MigrationData newMigration = new();

        Vector2 start = WMSK.countries[from.id].center;
        Vector2 end = WMSK.countries[to.id].center;

        newMigration.from = from;
        newMigration.to = to;
        newMigration.civilization = civ;
        newMigration.line = CreateLine(start, end);
        newMigration.marker = CreateMarker(from.id, start, end);
        newMigration.fullPopulations = (int) (civ.pieces[from.id].population.value / 100f * percentToMigration);
        newMigration.stepPopulations = (int) (newMigration.fullPopulations / 100f * stepPercent);

        civ.pieces[from.id].population.value -= newMigration.fullPopulations;
        migrations[from.id] = newMigration;

        Debug.Log($"Migration: From: {from.id} | To: {to.id} | Solution: {aiSolutionIndex}");

        if (!to.civsList.Contains(civ.name)) {
            newMigration.curPopulations += newMigration.stepPopulations;
            civ.AddPiece(to.id, newMigration.stepPopulations, 10);
            to.AddCivilization(civ.name);
        }

        if (aiSolutionIndex == -1) {
            OpenPanel(from.id);
            Timeline.Instance.Pause();
        } else ai[aiSolutionIndex]?.Invoke(newMigration.from.id);
    }

    private LineMarkerAnimator CreateLine(Vector2 start, Vector2 end) {
        LineMarkerAnimator lma = WMSK.AddLine(start, end, Color.red, 0f, 4f);
        lma.lineMaterial = lineMaterial;
        lma.lineWidth = 2f;
        lma.drawingDuration = 1.5f;
        lma.dashInterval = 0.005f;
        lma.dashAnimationDuration = 0.8f;
        lma.endCap = endLine;
        return lma;
    }

    private IconMarker CreateMarker(int from, Vector2 start, Vector2 end) {
        Vector3 position = (start + end) / 2;
        IconMarker marker = Instantiate(markerPrefab);
        marker.Sprite = markerSprite;
        marker.onClick += OpenPanel;
        marker.Index = from;
        position.z = -0.1f;

        MarkerClickHandler handler = WMSK.AddMarker2DSprite(marker.gameObject, position, 0.03f, true, true);
        handler.allowDrag = false;

        return marker;
    }

    private void OnTickLogic() {
        for(int i = 0; i < migrations.Count; ++i) {
            // Этап перед началом миграции
            MigrationData migration = migrations.Values.ElementAt(i);
            if (!migration.civilization.pieces.ContainsKey(migration.to.id)) continue;
            int curID = migrations.Keys.ElementAt(i);
            if(migration.timerToStart < startTime) {
                ++migration.timerToStart;
            } else {
                //Интервал
                if(migration.timerInterval < interval) {
                    ++migration.timerInterval;
                    continue;
                } else {
                    migration.timerInterval = 0;
                }
                //Перенос людей
                if (migration.fullPopulations > migration.curPopulations) {
                    if (migration.fullPopulations - migration.curPopulations >= migration.stepPopulations) {
                        migration.curPopulations += migration.stepPopulations;
                        migration.civilization.pieces[migration.to.id].population.value += migration.stepPopulations;
                    } else {
                        migration.curPopulations = migration.fullPopulations;
                        migration.civilization.pieces[migration.to.id].population.value += migration.fullPopulations - migration.curPopulations;
                    }
                }
                // Удаление миграции
                if (migration.curPopulations == migration.fullPopulations) {
                    if(curID == selectedID) panel.Close();
                    Remove(migration.from.id);
                }
            }
        }
    }

    private void Remove(CivPiece civPiece) {
        if (migrations.ContainsKey(civPiece.Region.id)) {
            Remove(civPiece.Region.id);
            return;
        }
        foreach(var pair in migrations) {
            if (pair.Value.to.id == civPiece.Region.id) {
                Remove(pair.Value.from.id);
                break;
            }
        }
    }

    private void Remove(int index) {
        Debug.Log($"Remove migration {index}");
        migrations[index].line.Destroy();
        migrations[index].marker.Destroy();
        migrations.Remove(index);
    }

    private void OpenPanel(int fromID) {
        selectedID = fromID;
        panel.Data = migrations[fromID];
        panel.Open();
    }

    private void UpdatePercent() {
        panel.UpdatePercents();
    }

    private void ClickNothing() {
        Debug.Log(panel.IsOpenPanel);
        aiSolutionIndex = panel.IsOpenPanel ? -1 : 0;
    }

    private void ClickBreak(int fromID) {
        Debug.Log(panel.IsOpenPanel);
        aiSolutionIndex = panel.IsOpenPanel ? -1 : 1;
        MigrationData data = migrations[fromID];
        data.civilization.pieces[data.from.id].population.value += data.fullPopulations - data.curPopulations;
        Remove(fromID);
    }

    private void ClickSpeedUp(int fromID) {
        Debug.Log(panel.IsOpenPanel);
        aiSolutionIndex = panel.IsOpenPanel ? -1 : 2;
        migrations[fromID].stepPopulations *= 2;
    }

    private T GetMax<T>(List<T> list, Func<T, int> GetValue) {
        int res = 0;
        for (int i = 1; i < list.Count; ++i) {
            if (GetValue(list[res]) < GetValue(list[i])) {
                res = i;
            }
        }
        return list[res];
    }
}
