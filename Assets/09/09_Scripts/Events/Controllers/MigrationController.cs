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
    
    public int Population => migrations.Sum(x => x.Value.FullPopulations - x.Value.CurPopulations);

    private Map Map => Transmigratio.Instance.tmdb.map;
    private WMSK WMSK => Transmigratio.Instance.tmdb.map.WMSK;

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
        if (migrations.ContainsKey(civPice.Region.Id)) return;

        TM_Region curRegion = civPice.Region;
        List<Country> neighbourRegions = WMSK.CountryNeighbours(curRegion.Id);
        if (neighbourRegions.Count == 0) return;

        TM_Region targetRegion = null;
        List<TM_Region> targetList = new();
        List<TM_Region> allNeighbourRegionsList = new();

        for(int i = 0; i < neighbourRegions.Count; ++i) {
            int regionID = WMSK.GetCountryIndex(neighbourRegions[i].name);
            if (migrations.ContainsKey(regionID)) continue;
            
            TM_Region neighbourRegion = Map.GetRegionBywmskId(regionID);
            if (neighbourRegion.Fauna["Fauna"].Value > 0) {
                allNeighbourRegionsList.Add(neighbourRegion);
                if (neighbourRegion.Fauna["Fauna"].Value > curRegion.Fauna["Fauna"].Value) {
                    targetList.Add(neighbourRegion);
                }
            }
        }
        
        if (targetList.Count > 0) {
            targetRegion = GetMax(targetList, (TM_Region region) => region.Fauna["Fauna"].Value);
        } else if (allNeighbourRegionsList.Count > 0) {
            System.Random r = new System.Random();
            targetRegion = allNeighbourRegionsList[r.Next(0, allNeighbourRegionsList.Count)];
        } else return;
        
        Add(civPice.Region, targetRegion, civPice.Civilization);
    }

    private void Add(TM_Region from, TM_Region to, Civilization civ) {
        MigrationData newMigration = new();

        Vector2 start = WMSK.countries[from.Id].center;
        Vector2 end = WMSK.countries[to.Id].center;

        newMigration.From = from;
        newMigration.To = to;
        newMigration.Civilization = civ;
        newMigration.Line = CreateLine(start, end);
        newMigration.Marker = CreateMarker(from.Id, start, end);
        newMigration.FullPopulations = (int) (civ.Pieces[from.Id].Population.value / 100f * percentToMigration);
        newMigration.StepPopulations = (int) (newMigration.FullPopulations / 100f * stepPercent);

        civ.Pieces[from.Id].Population.value -= newMigration.FullPopulations;
        migrations[from.Id] = newMigration;

        Debug.Log($"Migration: From: {from.Id} | To: {to.Id} | Solution: {aiSolutionIndex}");

        if (!to.CivsList.Contains(civ.Name)) {
            newMigration.CurPopulations += newMigration.StepPopulations;
            civ.AddPiece(to.Id, newMigration.StepPopulations, 10);
            to.AddCivilization(civ.Name);
        }

        if (aiSolutionIndex == -1) {
            OpenPanel(from.Id);
            Timeline.Instance.Pause();
        } else ai[aiSolutionIndex]?.Invoke(newMigration.From.Id);
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
            if (!migration.Civilization.Pieces.ContainsKey(migration.To.Id)) continue;
            int curID = migrations.Keys.ElementAt(i);
            if(migration.TimerToStart < startTime) {
                ++migration.TimerToStart;
            } else {
                //Интервал
                if(migration.TimerInterval < interval) {
                    ++migration.TimerInterval;
                    continue;
                } else {
                    migration.TimerInterval = 0;
                }
                //Перенос людей
                if (migration.FullPopulations > migration.CurPopulations) {
                    if (migration.FullPopulations - migration.CurPopulations >= migration.StepPopulations) {
                        migration.CurPopulations += migration.StepPopulations;
                        migration.Civilization.Pieces[migration.To.Id].Population.value += migration.StepPopulations;
                    } else {
                        migration.CurPopulations = migration.FullPopulations;
                        migration.Civilization.Pieces[migration.To.Id].Population.value += migration.FullPopulations - migration.CurPopulations;
                    }
                }
                // Удаление миграции
                if (migration.CurPopulations == migration.FullPopulations) {
                    if(curID == selectedID) panel.Close();
                    Remove(migration.From.Id);
                }
            }
        }
    }

    private void Remove(CivPiece civPiece) {
        if (migrations.ContainsKey(civPiece.Region.Id)) {
            Remove(civPiece.Region.Id);
            return;
        }
        foreach(var pair in migrations) {
            if (pair.Value.To.Id == civPiece.Region.Id) {
                Remove(pair.Value.From.Id);
                break;
            }
        }
    }

    private void Remove(int index) {
        Debug.Log($"Remove migration {index}");
        migrations[index].Line.Destroy();
        migrations[index].Marker.Destroy();
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
        data.Civilization.Pieces[data.From.Id].Population.value += data.FullPopulations - data.CurPopulations;
        Remove(fromID);
    }

    private void ClickSpeedUp(int fromID) {
        Debug.Log(panel.IsOpenPanel);
        aiSolutionIndex = panel.IsOpenPanel ? -1 : 2;
        migrations[fromID].StepPopulations *= 2;
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
