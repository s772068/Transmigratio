using System.Collections.Generic;
using Unity.VisualScripting;
using WorldMapStrategyKit;
using System.Linq;
using UnityEngine;
using System;
using Gameplay.Scenarios.Events.Data;

namespace Gameplay.Scenarios.Events.Global {
    [CreateAssetMenu(menuName = "ScriptableObjects/Scenarios/Events/Global/Migration", fileName = "Migration")]
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
        [Header("Parametrs")]
        [SerializeField] private int _minPopulation = 210;

        private CivPiece _fromPiece;
        private CivPiece _toPiece;
        private Dictionary<string, MigrationData> _migrations = new();

        public static Action<CivPiece> OnMigration;
        public static Func<int> GetPopulation;

        private protected override string Name => "Migration";
        private protected override string Territory(CivPiece piece) => Local("Territory1") + " " +
                              $"<color=#{regionColor.ToHexString()}>" +
                              piece.Region.Name + "</color> " +
                              Local("Territory2") + " " +
                              $"<color=#{regionColor.ToHexString()}>" +
                              _toPiece.Region.Name + "</color> ";

        private protected override void ActivateEvents() {
            Timeline.TickLogic += OnTickLogic;
            Civilization.onRemovePiece += RemoveMigration;

            OnMigration = TryMigration;
            GetPopulation = () => _migrations.Sum(x => x.Value.FullPopulations - x.Value.CurPopulations);
            Events.AutoChoice.NewEvent(this, _desidions);
        }

        private protected override void OpenPanel(CivPiece piece) {
            PanelFabric.CreateEvent(HUD.Instance.PanelsParent, _desidionPrefab, panel, this, panelSprite, Local("Title"),
                                    Territory(piece), Local("Description"), _desidions, piece);
        }

        private protected override void InitDesidions() {
            AddDesidion(Nothing, Local("Nothing"), (piece) => 0);
            AddDesidion(Break, Local("Break"), (piece) => GetDesidionCost(breakPoints));
            AddDesidion(SpeedUp, Local("SpeedUp"), (piece) => GetDesidionCost(speedUpPoints));
        }

        public void TryMigration(CivPiece civPiece) {
            if (_migrations.ContainsKey($"{civPiece.Civilization.Name}-{civPiece.Region.Id}"))
                return;

            if (civPiece.Population.Value < _minPopulation)
                return;

            TM_Region curRegion = civPiece.Region;
            List<Country> neighbourRegions = WMSK.CountryNeighbours(curRegion.Id);
            if (neighbourRegions.Count == 0) return;

            TM_Region targetRegion = null;
            List<TM_Region> targetList = new();
            List<TM_Region> allNeighbourRegionsList = new();

            for (int i = 0; i < neighbourRegions.Count; ++i) {
                int regionID = WMSK.GetCountryIndex(neighbourRegions[i].name);
                if (_migrations.ContainsKey($"{civPiece.Civilization.Name}-{regionID}"))
                    continue;

                TM_Region neighbourRegion = Map.GetRegionBywmskId(regionID);
                if (neighbourRegion.Fauna["Fauna"] > 0) {
                    allNeighbourRegionsList.Add(neighbourRegion);
                    if (neighbourRegion.Population == 0) {
                        targetList.Add(neighbourRegion);
                    }
                }
            }

            if (targetList.Count > 0) {
                targetRegion = GetMax(targetList, (TM_Region region) => region.Fauna["Fauna"]);
            } else if (allNeighbourRegionsList.Count > 0) {
                targetRegion = GetMax(allNeighbourRegionsList, (TM_Region region) => region.Fauna["Fauna"]);
            } else return;

            if (!civPiece.Civilization.Pieces.ContainsKey(civPiece.Region.Id))
                return;

            AddMigration(civPiece.Region, targetRegion, civPiece.Civilization);
        }

        private void AddMigration(TM_Region from, TM_Region to, Civilization civ) {
            MigrationData newMigration = new();

            Vector2 start = WMSK.countries[from.Id].centroid;
            Vector2 end = WMSK.countries[to.Id].centroid;

            string migrationId = $"{civ.Name}-{from.Id}";

            newMigration.From = from;
            newMigration.To = to;
            newMigration.Civilization = civ;
            newMigration.Line = CreateLine(start, end);
            newMigration.Marker = CreateMarker(start, end, civ.Pieces[from.Id]);
            newMigration.FullPopulations = (int) (civ.Pieces[from.Id].Population.Value / 100f * percentToMigration);
            newMigration.StepPopulations = (int) (newMigration.FullPopulations / 100f * stepPercent);

            civ.Pieces[from.Id].Population.Value -= newMigration.FullPopulations;
            _migrations[$"{civ.Name}-{from.Id}"] = newMigration;

            if (!to.CivsList.Contains(civ.Name)) {
                newMigration.CurPopulations += newMigration.StepPopulations < Demography.data.MinPiecePopulation * 2 ? Demography.data.MinPiecePopulation * 2 + 1 : newMigration.StepPopulations;
                civ.AddPiece(to.Id, newMigration.CurPopulations);
                to.AddCivilization(civ.Name);
            }

            newMigration.CivFrom = civ.Pieces[from.Id];
            newMigration.CivTo = civ.Pieces[to.Id];
            _fromPiece = newMigration.CivFrom;
            _toPiece = newMigration.CivTo;
            ChroniclesController.AddActive(Name, from.Id, OpenPanel,
                new Chronicles.Data.Panel.LocalVariablesChronicles { RegionFirst = newMigration.CivFrom.Region.Name, RegionSecond = newMigration.CivTo.Region.Name, Count = newMigration.CurPopulations });

            newMigration.CivFrom.AddEvent(this);
            newMigration.CivTo.AddEvent(this);

            if (!AutoChoice && _isAutoOpenPanel) {
                OpenPanel(newMigration.CivFrom);
            } 
            else if (AutoChoice) {
                foreach (var autochoice in Events.AutoChoice.Events[this])
                {
                    if (autochoice is DesidionPiece desP)
                    {
                        if (desP.CostFunc(newMigration.CivFrom) <= MaxAutoInterventionPoints)
                        {
                            if (desP.ActionClick.Invoke(newMigration.CivFrom, desP.CostFunc))
                                break;
                        }
                    }
                }
            }
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

        private IconMarker CreateMarker(Vector2 start, Vector2 end, CivPiece piece) {
            Vector3 position = (start + end) / 2;
            IconMarker marker = Instantiate(markerPrefab);
            marker.Sprite = markerSprite;
            marker.onClick += OpenPanel;
            marker.Piece = piece;
            marker.SetCount = 1;
            position.z = -0.1f;

            MarkerClickHandler handler = WMSK.AddMarker2DSprite(marker.gameObject, position, 0.024f, true, true);
            handler.allowDrag = false;

            return marker;
        }

        private void OnTickLogic() {
            for (int i = 0; i < _migrations.Count; ++i) {
                // ���� ����� ������� ��������
                MigrationData migration = _migrations.Values.ElementAt(i);
                if (!migration.Civilization.Pieces.ContainsKey(migration.To.Id)) {

                    continue;
                }
                string curID = _migrations.Keys.ElementAt(i);
                if (migration.TimerToStart < startTime) {
                    ++migration.TimerToStart;
                } else {
                    //��������
                    if (migration.TimerInterval < interval) {
                        ++migration.TimerInterval;
                        continue;
                    } else {
                        migration.TimerInterval = 0;
                    }

                    if (!migration.Civilization.Pieces.TryGetValue(migration.To.Id, out CivPiece civPiece)) {
                        Break(migration.CivFrom);
                        continue;
                    }

                    //������� �����
                    if (migration.FullPopulations > migration.CurPopulations) {
                        if (migration.FullPopulations - migration.CurPopulations >= migration.StepPopulations) {
                            migration.CurPopulations += migration.StepPopulations;
                            migration.Civilization.Pieces[migration.To.Id].Population.Value += migration.StepPopulations;
                        } else {
                            migration.CurPopulations = migration.FullPopulations;
                            migration.Civilization.Pieces[migration.To.Id].Population.Value += migration.FullPopulations - migration.CurPopulations;
                        }
                    }
                    // �������� ��������
                    if (migration.CurPopulations >= migration.FullPopulations) {
                        RemoveMigration(curID);
                    }
                }
            }
        }

        private void RemoveMigration(CivPiece civPiece) {
            if (!_migrations.ContainsKey($"{civPiece.Civilization.Name}-{civPiece.Region.Id}")) {
                foreach (var pair in _migrations) {
                    if (pair.Value.To.Id == civPiece.Region.Id) {
                        if (_migrations.ContainsKey($"{civPiece.Civilization.Name}-{pair.Value.From.Id}"))
                        {
                            RemoveMigration($"{civPiece.Civilization.Name}-{pair.Value.From.Id}");
                            return;
                        }
                    }
                }
                new Exception("Dont find migration");
            }

            DestroyMarker($"{civPiece.Civilization.Name}-{civPiece.Region.Id}");
            civPiece.RemoveEvent(this);
        }

        private void RemoveMigration(string index) {
            Debug.Log("RemoveMigration");
            _migrations[index].Line?.Destroy();
            DestroyMarker(index);
            _migrations[index].CivFrom?.RemoveEvent(this);
            _migrations[index].CivFrom?.RemoveEvent(this);

            _migrations.Remove(index);
        }

        private protected override void CreateMarker(CivPiece piece = null) {
            if (!_migrations.ContainsKey($"{piece.Civilization.Name}-{piece.Region.Id}")) return;
            Vector2 start = WMSK.countries[piece.RegionID].centroid;
            Vector2 end = WMSK.countries[_toPiece.RegionID].centroid;
            CreateMarker(start, end, piece);
        }

        private bool Break(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (AutoChoice && interventionPoints(piece) > MaxAutoInterventionPoints)
                return false;

            if (!_useIntervention(interventionPoints(piece)))
                return false;

            Break(piece);
            return true;
        }

        //������ �������� �� ���������� ������
        private void Break(CivPiece piece) {
            MigrationData data = _migrations[$"{piece.Civilization.Name}-{piece.Region.Id}"];
            piece.Population.Value += (data.FullPopulations - data.CurPopulations) >= 0 ? data.FullPopulations - data.CurPopulations : 0;
            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "Break",
                new Chronicles.Data.Panel.LocalVariablesChronicles { RegionFirst = data.From.Name, RegionSecond = data.To.Name, Count = data.FullPopulations - data.CurPopulations });
            RemoveMigration(piece);
        }

        private bool Nothing(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            MigrationData data = _migrations[$"{piece.Civilization.Name}-{piece.Region.Id}"];

            ChroniclesController.AddPassive(Name, piece.RegionID, panelSprite, "Nothing",
                new Chronicles.Data.Panel.LocalVariablesChronicles { RegionFirst = data.From.Name, RegionSecond = data.To.Name, Count = _migrations[$"{piece.Civilization.Name}-{piece.Region.Id}"].FullPopulations });
            DestroyMarker($"{piece.Civilization.Name}-{piece.Region.Id}");
            return true;
        }

        private bool SpeedUp(CivPiece piece, Func<CivPiece, int> interventionPoints) {
            if (!_useIntervention(interventionPoints(piece)))
                return false;

            MigrationData data = _migrations[$"{piece.Civilization.Name}-{piece.Region.Id}"];
            data.StepPopulations *= 2;

            ChroniclesController.Deactivate(Name, piece.RegionID, panelSprite, "SpeedUp",
                new Chronicles.Data.Panel.LocalVariablesChronicles { RegionFirst = data.From.Name, RegionSecond = data.To.Name, Count = _migrations[$"{piece.Civilization.Name}-{piece.Region.Id}"].FullPopulations });

            DestroyMarker($"{piece.Civilization.Name}-{piece.Region.Id}");
            return true;
        }

        private void DestroyMarker(string index) {
            if (_migrations.TryGetValue(index, out MigrationData migration))
                if (migration.Marker != null)
                    migration.Marker.Destroy();
        }

        private T GetMax<T>(List<T> list, Func<T, float> GetValue) {
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
