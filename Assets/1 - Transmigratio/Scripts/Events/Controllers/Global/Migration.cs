using System.Collections.Generic;
using WorldMapStrategyKit;
using System.Linq;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace Events.Controllers.Global {
    public sealed class Migration : Base {
        [Header("Line")]
        [SerializeField] private Material lineMaterial;
        [SerializeField] private GameObject endLine;
        [Header("Timers")]
        [SerializeField, Min(0)] private int startTime;
        [SerializeField, Min(0)] private int interval;
        [Header("Percents")]
        [SerializeField, Range(0, 100)] private int percentToMigration;
        [SerializeField, Range(0, 100)] private int stepPercent;
        [Header("Points")]
        [SerializeField, Min(0)] private int breakPoints;
        [SerializeField, Min(0)] private int speedUpPoints;

        private CivPiece _fromPiece;
        private CivPiece _toPiece;
        private Dictionary<int, MigrationData> _migrations = new();

        public static Action<CivPiece> OnMigration;
        public static Func<int> GetPopulation;

        private protected override string Name => "Migration";
        private protected override string Territory => Local("Territory1") + " " +
                              $"<color=#{regionColor.ToHexString()}>" +
                              _fromPiece.Region.Name + "</color> " +
                              Local("Territory2") + " " +
                              $"<color=#{regionColor.ToHexString()}>" +
                              _toPiece.Region.Name + "</color> ";

        private protected override void ActivateEvents() {
            Timeline.TickLogic += OnTickLogic;
            Civilization.RemoveCivPiece += RemoveMigration;

            OnMigration = TryMigration;
            GetPopulation = () => _migrations.Sum(x => x.Value.FullPopulations - x.Value.CurPopulations);
        }

        private protected override void DeactivateEvents() {
            Timeline.TickLogic -= OnTickLogic;
            Civilization.RemoveCivPiece -= RemoveMigration;

            OnMigration = default;
            GetPopulation = default;
        }

        private protected override void OpenPanel()
        {
            PanelFabric.CreateEvent(HUD.Instance.Events, _desidionPrefab, panel, this, panelSprite, Local("Title"),
                                    Territory, Local("Description"), _desidions);
        }

        private protected override void InitDesidions() {
            AddDesidion(Break, Local("Break"), () => breakPoints);
            AddDesidion(default, Local("Nothing"), () => 0);
            AddDesidion(SpeedUp, Local("SpeedUp"), () => speedUpPoints);
        }

        public void TryMigration(CivPiece civPiece) {
            if (_migrations.ContainsKey(civPiece.Region.Id)) return;

            TM_Region curRegion = civPiece.Region;
            List<Country> neighbourRegions = WMSK.CountryNeighbours(curRegion.Id);
            if (neighbourRegions.Count == 0) return;

            TM_Region targetRegion = null;
            List<TM_Region> targetList = new();
            List<TM_Region> allNeighbourRegionsList = new();

            for (int i = 0; i < neighbourRegions.Count; ++i) {
                int regionID = WMSK.GetCountryIndex(neighbourRegions[i].name);
                if (_migrations.ContainsKey(regionID)) continue;

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

            if (!civPiece.Civilization.Pieces.ContainsKey(civPiece.Region.Id))
                return;

            AddMigration(civPiece.Region, targetRegion, civPiece.Civilization);
        }

        private void AddMigration(TM_Region from, TM_Region to, Civilization civ) {
            MigrationData newMigration = new();

            Vector2 start = WMSK.countries[from.Id].center;
            Vector2 end = WMSK.countries[to.Id].center;

            newMigration.From = from;
            newMigration.To = to;
            newMigration.Civilization = civ;
            newMigration.Line = CreateLine(start, end);
            newMigration.Marker = CreateMarker(start, end);
            newMigration.FullPopulations = (int) (civ.Pieces[from.Id].Population.value / 100f * percentToMigration);
            newMigration.StepPopulations = (int) (newMigration.FullPopulations / 100f * stepPercent);

            civ.Pieces[from.Id].Population.value -= newMigration.FullPopulations;
            _migrations[from.Id] = newMigration;

            if (!to.CivsList.Contains(civ.Name)) {
                newMigration.CurPopulations += newMigration.StepPopulations;
                civ.AddPiece(to.Id, newMigration.StepPopulations, 10);
                to.AddCivilization(civ.Name);
            }
            
            _fromPiece = civ.Pieces[from.Id];
            _toPiece = civ.Pieces[to.Id];

            if (!AutoChoice) {
                _fromPiece.AddEvent(this);
                _toPiece.AddEvent(this);
                OpenPanel();
            } else _activeDesidion.ActionClick?.Invoke();
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

        private IconMarker CreateMarker(Vector2 start, Vector2 end) {
            Vector3 position = (start + end) / 2;
            IconMarker marker = Instantiate(markerPrefab);
            marker.Sprite = markerSprite;
            marker.onClick += OpenPanel;
            //marker.Index = from;
            position.z = -0.1f;

            MarkerClickHandler handler = WMSK.AddMarker2DSprite(marker.gameObject, position, 0.03f, true, true);
            handler.allowDrag = false;

            return marker;
        }

        private void OnTickLogic() {
            for (int i = 0; i < _migrations.Count; ++i) {
                // Этап перед началом миграции
                MigrationData migration = _migrations.Values.ElementAt(i);
                if (!migration.Civilization.Pieces.ContainsKey(migration.To.Id)) continue;
                int curID = _migrations.Keys.ElementAt(i);
                if (migration.TimerToStart < startTime) {
                    ++migration.TimerToStart;
                } else {
                    //Интервал
                    if (migration.TimerInterval < interval) {
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
                        RemoveMigration(curID);
                    }
                }
            }
        }

        private void RemoveMigration(CivPiece civPiece) {
            if (_migrations.ContainsKey(civPiece.Region.Id)) {
                RemoveMigration(civPiece.Region.Id);
                return;
            }
            foreach (var pair in _migrations) {
                if (pair.Value.To.Id == civPiece.Region.Id) {
                    RemoveMigration(pair.Value.From.Id);
                    break;
                }
            }
            civPiece.RemoveEvent(this);
        }

        private void RemoveMigration(int index) {
            Debug.Log("RemoveMigration");
            _migrations[index].Line.Destroy();
            _migrations[index].Marker.Destroy();
            _fromPiece.RemoveEvent(this);
            _toPiece.RemoveEvent(this);
            _migrations.Remove(index);
        }

        private protected override void CreateMarker(CivPiece piece = null) {
            if (!_migrations.ContainsKey(_fromPiece.RegionID)) return;
            Vector2 start = WMSK.countries[_fromPiece.RegionID].center;
            Vector2 end = WMSK.countries[_toPiece.RegionID].center;
            CreateMarker(start, end);
        }

        private void Break() {
            int fromID = _fromPiece.Region.Id;
            MigrationData data = _migrations[_fromPiece.Region.Id];
            _fromPiece.Population.value += data.FullPopulations - data.CurPopulations;
            RemoveMigration(fromID);
        }

        private void SpeedUp() {
            int fromID = _fromPiece.Region.Id;
            _migrations[fromID].StepPopulations *= 2;
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
}
