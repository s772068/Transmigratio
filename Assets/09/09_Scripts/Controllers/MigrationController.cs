using System.Collections.Generic;
using WorldMapStrategyKit;
using System.Linq;
using UnityEngine;
using System;
using AYellowpaper.SerializedCollections;

public class MigrationController : Singleton<MigrationController> {
    [SerializeField] private MigrationPanel panel;
    [SerializeField] private IconMarker markerPrefab;
    [SerializeField] private Sprite markerSprite;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private GameObject startLine;
    [SerializeField] private GameObject endLine;
    
    [Min(0)]
    [SerializeField] private int startTime;
    [Min(0)]
    [SerializeField] private int interval;
    
    [Range(0, 100)]
    [SerializeField] private int percentToMigration;
    [Range(0, 100)]
    [SerializeField] private int stepPercent;

    private int selectedID;
    private int aiSolutionIndex = -1;
    private List<Action<int>> ai = new();
    [SerializeField] private SerializedDictionary<int, MigrationData> migrations = new();
    
    public int Population => migrations.Sum(x => x.Value.fullPopulations - x.Value.curPopulations);

    private Map Map => Transmigratio.Instance.tmdb.map;
    private WMSK WMSK => Transmigratio.Instance.tmdb.map.wmsk;

    private void Start() {
        ai = new() { (int i) => { }, ClickBreak, ClickSpeedUp };

        GameEvents.onTickLogic += OnTickLogic;

        GameEvents.onTickShow += UpdatePercent;

        panel.onBreak = ClickBreak;
        panel.onNothing = ClickNothing;
        panel.onSpeedUp = ClickSpeedUp;
    }

    public void TryMigration(CivPiece civPice) {
        if (migrations.ContainsKey(civPice.region.id)) return;

        TM_Region region = civPice.region;
        List<Country> countries = WMSK.CountryNeighbours(region.id);
        if (countries.Count == 0) return;

        TM_Region targetRegion = null;
        int nextFauna;

        int fauna = Map.GetRegionBywmskId(region.id).fauna["Fauna"].value;

        List<TM_Region> list = new();
        int res = 0;

        for (int i = 0; i < countries.Count; ++i) {
            region = Map.GetRegionBywmskId(WMSK.GetCountryIndex(countries[i].name));
            if (Transmigratio.Instance.GetRegion(WMSK.GetCountryIndex(countries[i])).Population == 0) {
                list.Add(region);
            }
        }

        if (list.Count == 0) {
            nextFauna = region.fauna["Fauna"].value;
            if (fauna < nextFauna) {
                fauna = nextFauna;
                targetRegion = region;
            }
        } else if(list.Count == 1) {
            targetRegion = list[0];
        } else if (list.Count > 1) {
            for (int i = 0; i < list.Count; ++i) {
                nextFauna = region.fauna["Fauna"].value;
                if (fauna < nextFauna) {
                    fauna = nextFauna;
                    targetRegion = list[i];
                }

            }
        }

        if (targetRegion == null) return;

        Add(civPice.region, targetRegion, civPice.civilization);
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
        lma.startCap = startLine;
        lma.endCap = endLine;
        return lma;
    }

    private IconMarker CreateMarker(int from, Vector2 start, Vector2 end) {
        Vector2 position = (start + end) / 2;
        IconMarker marker = Instantiate(markerPrefab);
        marker.Sprite = markerSprite;
        marker.onClick += OpenPanel;
        marker.Index = from;

        MarkerClickHandler handler = WMSK.AddMarker2DSprite(marker.gameObject, position, 0.03f, true, true);
        handler.allowDrag = false;

        return marker;
    }

    private void OnTickLogic() {
        for(int i = 0; i < migrations.Count; ++i) {
            // Этап перед началом миграции
            MigrationData migration = migrations.Values.ElementAt(i);
            int curID = migrations.Keys.ElementAt(i);
            if(migration.timerToStart < startTime) {
                ++migration.timerToStart;
            } else {
                // Создание цивилизации в целевом регионе
                if (!migration.to.civsList.Contains(migration.civilization)) {
                    migration.curPopulations += migration.stepPopulations;
                    migration.civilization.AddPiece(migration.to, migration.stepPopulations, 10);
                    migration.to.AddCivilization(migration.civilization);
                }
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
}
