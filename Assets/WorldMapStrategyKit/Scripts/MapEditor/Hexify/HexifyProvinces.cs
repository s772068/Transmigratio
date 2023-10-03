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

		/// <summary>
		/// Adjusts all provinces frontiers to match the hexagonal grid
		/// </summary>
		public IEnumerator HexifyProvinces (EditorOpContext context)
		{
			Cell[] cells = _map.cells;
			if (cells == null || _map.provinces == null)
				yield break;
			
			// Initialization
			cancelled = false;
			hexifyContext = context;

			if (procCells == null || procCells.Length < cells.Length) {
				procCells = new RegionCell[cells.Length];
			}
			for (int k = 0; k < cells.Length; k++) {
				procCells [k].entityIndex = -1;
			}

			// Compute area of a single cell; for optimization purposes we'll ignore all regions whose surface is smaller than a 20% the size of a cell
			float minArea = 0;
			Region templateCellRegion = null;
			for (int k = 0; k < cells.Length; k++) {
				if (cells [k] != null) {
					templateCellRegion = new Region (null, 0);
					templateCellRegion.UpdatePointsAndRect (cells [k].points, true);
					minArea = templateCellRegion.rect2DArea * 0.2f;
					break;
				}
			}

			if (templateCellRegion == null)
				yield break;

			if (hexagonPoints == null || hexagonPoints.Length != 6) {
				hexagonPoints = new Vector2[6];
			}
			for (int k = 0; k < 6; k++) {
				hexagonPoints [k] = templateCellRegion.points [k] - templateCellRegion.center;
			}

			// Ensure all province regions are loaded
			Province[] provinces = _map.provinces;
			for (int k = 0; k < provinces.Length; k++) {
				if (provinces [k].regions == null) {
					_map.ReadProvincePackedString (provinces [k]);
				}
			}

			// Pass 1: remove minor regions
			yield return RemoveSmallRegions (minArea, _map.provinces);

			// Pass 2: assign all region centers to each provincey from biggest province to smallest province
			if (!cancelled) {
				yield return AssignRegionCenters (_map.provinces);
			}

			// Pass 3: add cells to target provinces
			if (!cancelled) {
				yield return AddHexagons (_map.provinces);
			}

			// Pass 4: merge adjacent regions
			if (!cancelled) {
				yield return MergeAdjacentRegions (_map.provinces);
			}

			// Pass 5: remove cells from other provinces
			if (!cancelled) {
				yield return RemoveHexagons (_map.provinces);
			}

			// Pass 6: update geometry of resulting provinces
			if (!cancelled) {
				yield return UpdateProvinces ();
			}

			if (!cancelled) {
				_map.OptimizeFrontiers ();
				_map.Redraw (true);
			}

			hexifyContext.progress (1f,  hexifyContext.title, ""); // hide progress bar
			yield return null;

			if (hexifyContext.finish != null) {
				hexifyContext.finish (cancelled);
			}
		}

		IEnumerator UpdateProvinces ()
		{

			Province[] _provinces = _map.provinces;

			for (int k = 0; k < _provinces.Length; k++) {
				if (k % 10 == 0) {
					if (hexifyContext.progress != null) {
						if (hexifyContext.progress ((float)k / _provinces.Length, hexifyContext.title, "Pass 6/6: updating provinces...")) {
							cancelled = true;
							hexifyContext.finish (true);
							yield break;
						}
					}
					yield return null;
				}

				_map.ProvinceSanitize (k, 3, true);
				if (_provinces[k].regions.Count == 0) {
					if (_map.ProvinceDelete (k)) {
						_provinces = _map.provinces;
						k--;
					}
				}
			}

			// Update cities and mount points
			Province province;
			City[] cities = _map.cities;
			int cityCount = cities.Length;
			for (int k = 0; k < cityCount; k++) {
				City city = cities [k];
				int provinceIndex = _map.GetProvinceIndex (city.unity2DLocation);
				if (provinceIndex >= 0) {
					province = _map.provinces [provinceIndex];
					if (city.province != province.name) {
						city.province = province.name;
						city.countryIndex = province.countryIndex;
					}
				}
			}
			int mountPointCount = _map.mountPoints.Count;
			for (int k = 0; k < mountPointCount; k++) {
				MountPoint mp = _map.mountPoints [k];
				int provinceIndex = _map.GetProvinceIndex (mp.unity2DLocation);
				if (provinceIndex >= 0) {
					if (mp.provinceIndex != provinceIndex) {
						province = _map.provinces [provinceIndex];
						mp.countryIndex = province.countryIndex;
						mp.provinceIndex = provinceIndex;
					}
				}
			}
		}

	}
}

