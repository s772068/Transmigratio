using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using WorldMapStrategyKit.ClipperLib;

namespace WorldMapStrategyKit
{

	public partial class WMSK_Editor : MonoBehaviour
	{

		RegionCell[] procCells;

		EditorOpContext hexifyContext;
		Vector2[] hexagonPoints;

		/// <summary>
		/// Adjusts all countries frontiers to match the hexagonal grid
		/// </summary>
		public IEnumerator HexifyAll (EditorOperationProgress progressOp, EditorOperationFinish finishOp) {
			EditorOpContext cc = new EditorOpContext { title = "Hexifying Countries...", progress = progressOp, finish = null };
			yield return HexifyCountries (cc);
			cc.title = "Hexifying Provinces...";
			cc.finish = finishOp;
			yield return HexifyProvinces (cc);
		}

		IEnumerator RemoveSmallRegions (float minArea, IAdminEntity[] _entities)
		{
			// Clear small regions
			for (int c = 0; c < _entities.Length; c++) {
				if (c % 10 == 0) {
					if (hexifyContext.progress != null) {
						if (hexifyContext.progress ((float)c / _entities.Length, hexifyContext.title, "Pass 1/6: removing small regions...")) {
							cancelled = true;
							hexifyContext.finish (true);
							yield break;
						}
					}
					yield return null;
				}

				IAdminEntity entity = _entities [c];
				int rCount = entity.regions.Count;
				bool recalc = false;
				for (int r = 0; r < rCount; r++) {
					if (entity.regions [r].rect2DArea < minArea) {
						entity.regions [r].Clear ();
						recalc = true;
					}
				}
				if (recalc) {
					if (entity is Country) {
						_map.RefreshCountryGeometry ((Country)entity);
					} else {
						_map.RefreshProvinceGeometry ((Province)entity);
					}
				}
			}
		}



		IEnumerator AssignRegionCenters (IAdminEntity[] entities)
		{
			List<Region> regions = new List<Region> ();
			for (int k = 0; k < entities.Length; k++) {
				IAdminEntity entity = entities [k];
				int rCount = entity.regions.Count;
				for (int r = 0; r < rCount; r++) {
					Region region = entity.regions [r];
					if (region.points.Length > 0) {
						regions.Add (region);
					}
				}
			}
			regions.Sort ((Region x, Region y) => {
				return y.rect2DArea.CompareTo (x.rect2DArea);
			});

			int regionsCount = regions.Count;
			for (int r = 0; r < regionsCount; r++) {
				Region region = regions [r];
				int cellIndex = _map.GetCellIndex (region.center);
				if (cellIndex >= 0 && procCells [cellIndex].entityIndex < 0) {
					IAdminEntity entity = region.entity;
					if (entity is Country) {
						procCells [cellIndex].entityIndex = _map.GetCountryIndex ((Country)region.entity);
					} else {
						procCells [cellIndex].entityIndex = _map.GetProvinceIndex ((Province)region.entity);
					}
					procCells [cellIndex].entityRegion = region;
				}
			}

			// Pass 2: iterate all frontier points
			for (int c = 0; c < entities.Length; c++) {
				if (c % 10 == 0) {
					if (hexifyContext.progress != null) {
						if (hexifyContext.progress ((float)c / entities.Length, hexifyContext.title, "Pass 2/6: assigning centers...")) {
							cancelled = true;
							hexifyContext.finish (true);
							yield break;
						}
					}
					yield return null;
				}
				IAdminEntity entity = entities [c];
				int rCount = entity.regions.Count;
				for (int cr = 0; cr < rCount; cr++) {
					Region region = entity.regions [cr];
					for (int p = 0; p < region.points.Length; p++) {
						int cellIndex = _map.GetCellIndex (region.points [p]);
						if (cellIndex >= 0 && procCells [cellIndex].entityIndex < 0) {
							procCells [cellIndex].entityIndex = c;
							procCells [cellIndex].entityRegion = region;
						}
					}
				}
			}
		}



		IEnumerator AddHexagons (IAdminEntity[] entities)
		{
			Cell[] cells = _map.cells;
			Clipper clipper = new Clipper ();
			for (int j = 0; j < cells.Length; j++) {
				if (j % 100 == 0) {
					if (hexifyContext.progress != null) {
						if (hexifyContext.progress ((float)j / cells.Length, hexifyContext.title, "Pass 3/6: adding hexagons to frontiers...")) {
							cancelled = true;
							hexifyContext.finish (true);
							yield break;
						}
					}
					yield return null;
				}

				int entityIndex = procCells [j].entityIndex;
				if (entityIndex < 0)
					continue;
				Cell cell = cells [j];

				// Create a region for the cell
				IAdminEntity entity = entities [entityIndex];
				Vector2[] newPoints = new Vector2[6];
				for (int k = 0; k < 6; k++) {
					newPoints [k].x = hexagonPoints [k].x + cell.center.x;
					newPoints [k].y = hexagonPoints [k].y + cell.center.y;
				}
				procCells [j].cellRegion = new Region (entity, entity.regions.Count);
				procCells [j].cellRegion.UpdatePointsAndRect (newPoints);

				// Add region to target entity's polygon - only if the entity is touching or crossing target entity frontier
				Region targetRegion = procCells [j].entityRegion;
				clipper.Clear ();
				clipper.AddPath (targetRegion, PolyType.ptSubject);
				clipper.AddPath (procCells [j].cellRegion, PolyType.ptClip);
				clipper.Execute (ClipType.ctUnion);
			}
		}



		IEnumerator MergeAdjacentRegions (IAdminEntity[] entities)
		{
			for (int k = 0; k < entities.Length; k++) {
				if (k % 10 == 0) {
					if (hexifyContext.progress != null) {
						if (hexifyContext.progress ((float)k / entities.Length, hexifyContext.title, "Pass 4/6: merging adjacent regions...")) {
							cancelled = true;
							hexifyContext.finish (true);
							yield break;
						}
					}
					yield return null;
				}
				_map.MergeAdjacentRegions (entities [k]);
			}
		}

		IEnumerator RemoveHexagons (IAdminEntity[] entities)
		{
			Clipper clipper = new Clipper ();
			Cell[] cells = _map.cells;
			for (int j = 0; j < cells.Length; j++) {
				if (j % 100 == 0) {
					if (hexifyContext.progress != null) {
						if (hexifyContext.progress ((float)j / cells.Length, hexifyContext.title, "Pass 5/6: removing cells from neighbours...")) {
							cancelled = true;
							hexifyContext.finish (true);
							yield break;
						}
					}
					yield return null;
				}

				int entityIndex = procCells [j].entityIndex;
				if (entityIndex < 0)
					continue;

				RegionCell regionCell = procCells [j];
				IAdminEntity entity = entities [entityIndex];

				// Substract cell region from any other entity
				List<Region> otherRegions;
				if (entity is Country) {
					otherRegions = _map.GetCountryRegionsOverlap (regionCell.cellRegion);
				} else {
					otherRegions = _map.GetProvinceRegionsOverlap (regionCell.cellRegion);
				}
				int orCount = otherRegions.Count;
				for (int o = 0; o < orCount; o++) {
					Region otherRegion = otherRegions [o];
					IAdminEntity otherEntity = otherRegion.entity;
					if (otherEntity == entity)
						continue;
					clipper.Clear ();
					clipper.AddPath (otherRegion, PolyType.ptSubject);
					clipper.AddPath (regionCell.cellRegion, PolyType.ptClip);
					clipper.Execute (ClipType.ctDifference, otherEntity);
				}
			}
		}


	}
}

