﻿using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

	public partial class WMSK_Editor : MonoBehaviour {

		public int GUIMountPointIndex;
		public string GUIMountPointName = "";
		public string GUIMountPointNewName = "";
		public string GUIMountPointNewType = "";
		public string GUIMountPointNewTagKey = "";
		public string GUIMountPointNewTagValue = "";
		public int GUIMountPointMassPopulationMin = 1;
		public int GUIMountPointMassPopulationMax = 3;
		public float GUIMountPointMassPopulationSeparation = 10;
		public string GUIMountPointMassPopulationType = "";
		public int GUIMountPointMassPopulationTypeInteger { get { int tt = 0; int.TryParse(GUIMountPointMassPopulationType, out tt); return tt; } }
		public int mountPointIndex = -1;
		public bool mountPointChanges;  // if there's any pending change to be saved
		public Dictionary<string, string>mountPointTags;

		// private fields
		int lastMountPointCount = -1;
		string[] _mountPointNames;
				    

		public string[] mountPointNames {
			get {
				if (map.mountPoints!=null && lastMountPointCount != map.mountPoints.Count) {
					mountPointIndex =-1;
					ReloadMountPointNames ();
				}
				return _mountPointNames;
			}
		}

		
		#region Editor functionality

		
		public void ClearMountPointSelection() {
			map.HideMountPointHighlights();
			mountPointIndex = -1;
			GUIMountPointIndex = -1;
			GUIMountPointName = "";
			GUIMountPointNewName = "";
			GUIMountPointNewTagKey = "";
			GUIMountPointNewTagValue = "";
			GUIMountPointNewType = "";
		}


		/// <summary>
		/// Adds a new mount point to current country.
		/// </summary>
		public void MountPointCreate(Vector3 newPoint) {
			if (countryIndex<0) return;
			GUIMountPointName = "New Mount Point " + (map.mountPoints.Count+1);
			map.MountPointAdd(newPoint, GUIMountPointName, countryIndex, provinceIndex, 0);
			map.DrawMountPoints();
			lastMountPointCount = -1;
			ReloadMountPointNames();
			mountPointChanges = true;
		}

		public bool MountPointUpdateType () {
			if (mountPointIndex<0) return false;
			int type =map.mountPoints[mountPointIndex].type;
			int.TryParse(GUIMountPointNewType, out type);
			if (map.mountPoints[mountPointIndex].type!=type) {
				map.mountPoints[mountPointIndex].type = type;
				mountPointChanges = true;
				return true;
			}
			return false;
		}


		public bool MountPointRename () {
			if (mountPointIndex<0) return false;
			string prevName = map.mountPoints[mountPointIndex].name;
			GUIMountPointNewName = GUIMountPointNewName.Trim ();
			if (prevName.Equals(GUIMountPointNewName)) return false;
			map.mountPoints[mountPointIndex].name = GUIMountPointNewName;
			GUIMountPointName = GUIMountPointNewName;
			lastMountPointCount = -1;
			ReloadMountPointNames();
			map.DrawMountPoints();
			mountPointChanges = true;
			return true;
		}

		public void MountPointMove(Vector2 destination) {
			if (mountPointIndex<0) return;
			map.mountPoints[mountPointIndex].unity2DLocation = destination;
			Transform t = map.transform.Find("Mount Points/" + mountPointIndex.ToString());
			if (t!=null) t.localPosition = destination * 1.001f;
			mountPointChanges = true;
		}

		public void MountPointSelectByCombo (int selection) {
			GUIMountPointIndex = selection;
			GUIMountPointName = "";
			GetMountPointIndexByGUISelection();
			MountPointSelect ();
		}

		bool GetMountPointIndexByGUISelection() {
			if (GUIMountPointIndex<0 || GUIMountPointIndex>=mountPointNames.Length) return false;
			string[] s = mountPointNames [GUIMountPointIndex].Split (new char[] {
				'(',
				')'
			}, System.StringSplitOptions.RemoveEmptyEntries);
			if (s.Length >= 2) {
				GUIMountPointName = s [0].Trim ();
				if (int.TryParse (s [1], out mountPointIndex)) {
					return true;
				}
			}
			return false;
		}

		public void MountPointSelect() {
			if (mountPointIndex < 0 || mountPointIndex > map.mountPoints.Count)
				return;

			// If no country is selected (the mount point could be at sea) select it
			MountPoint mp = map.mountPoints[mountPointIndex];
			int mpCountryIndex = mp.countryIndex;
			if (mpCountryIndex<0) {
				SetInfoMsg("Country not found in this country file.");
			}

			if (countryIndex!=mpCountryIndex && mpCountryIndex>=0) {
				ClearSelection();
				countryIndex = mpCountryIndex;
				countryRegionIndex = map.countries[countryIndex].mainRegionIndex;
				CountryRegionSelect();
			} 

			// Just in case makes GUICountryIndex selects appropiate value in the combobox
			GUIMountPointName = mp.name;
			SyncGUIMountPointSelection();
			if (mountPointIndex>=0) {
				GUIMountPointNewName = mp.name;
				GUIMountPointNewType = mp.type.ToString();
				MountPointHighlightSelection();
			}
		}

		public bool MountPointSelectByScreenClick(Ray ray) {
			int targetMountPointIndex;
			if (map.GetMountPointIndex (ray, out targetMountPointIndex)) {
				mountPointIndex = targetMountPointIndex;
				MountPointSelect();
				return true;
			}
			return false;
		}

		void MountPointHighlightSelection() {

			if (mountPointIndex<0 || mountPointIndex>=map.mountPoints.Count) return;

			// Colorize mount point
			map.HideMountPointHighlights();
			map.ToggleMountPointHighlight(mountPointIndex, Color.green, true);
	    }

		
		public void ReloadMountPointNames () {
			if (map == null || map.mountPoints == null) {
				lastMountPointCount = -1;
				return;
			}
			lastMountPointCount = map.mountPoints.Count; // check this size, and not result from GetCityNames because it could return additional rows (separators and so)
			_mountPointNames = map.GetMountPointNames(countryIndex, -1);
			SyncGUIMountPointSelection();
			MountPointSelect(); // refresh selection
		}

		void SyncGUIMountPointSelection() {
			// recover GUI mount point index selection
			if (GUIMountPointName.Length>0) {
				for (int k=0; k<mountPointNames.Length; k++) { 
					if (_mountPointNames [k].TrimStart ().StartsWith (GUIMountPointName)) {
						GUIMountPointIndex = k;
						mountPointIndex = map.GetMountPointIndex(countryIndex, provinceIndex, GUIMountPointName);
						if (mountPointIndex<0) mountPointIndex = map.GetMountPointIndex(countryIndex, provinceIndex, GUIMountPointName);
						if (mountPointIndex<0) mountPointIndex = map.GetMountPointIndex(countryIndex, -1, GUIMountPointName);
						return;
					}
				}
				SetInfoMsg("Mount point " + GUIMountPointName + " not found in database.");
			}
			GUIMountPointIndex = -1;
			GUIMountPointName = "";
		}

		/// <summary>
		/// Deletes current mount point
		/// </summary>
		public void DeleteMountPoint() {
			if (map.mountPoints==null || mountPointIndex<0 || mountPointIndex>=map.mountPoints.Count) return;

			map.HideMountPointHighlights();
			map.mountPoints.RemoveAt(mountPointIndex);
			mountPointIndex = -1;
			GUIMountPointName = "";
			SyncGUIMountPointSelection();
			map.DrawMountPoints();
			mountPointChanges = true;
		}

		/// <summary>
		/// Deletes all mount points of current selected country
		/// </summary>
		public void DeleteCountryMountPoints() {
			if (countryIndex<0) return;
			
			map.HideMountPointHighlights();
			if (map.mountPoints != null) {
				int k=-1;
				while(++k<map.mountPoints.Count) {
					if (map.mountPoints[k].countryIndex == countryIndex) {
						map.mountPoints.RemoveAt(k);
						k--;
					}
				}
			}
			mountPointIndex = -1;
			GUIMountPointName = "";
			SyncGUIMountPointSelection();
			map.DrawMountPoints();
			mountPointChanges = true;
		}
	
		#endregion

		#region IO stuff

		/// <summary>
		/// Returns the file name corresponding to the current mount point data file
		/// </summary>
		public string GetMountPointGeoDataFileName() {
			return "mountPoints.json";
		}
		
	
		/// <summary>
		/// Automatically adds new mount points to all countries of a continent.
		/// </summary>
		public void MountPointPopulateContinentCountries() {
			if (countryIndex<0) return;

			string continent = map.countries[countryIndex].continent;
			for (int k=0;k<map.countries.Length;k++) {
				if (map.countries[k].continent.Equals(continent)) {
					m_MountPointPopulateCountry(k);
				}
			}
			PopulateEnds();
		}

		/// <summary>
		/// Automatically adds new mount points to all provinces of a continent.
		/// </summary>
		public void MountPointPopulateContinentProvinces() {
			if (countryIndex<0) return;
			
			string continent = map.countries[countryIndex].continent;
			for (int k=0;k<map.countries.Length;k++) {
				Country country = map.countries[k];
				if (country.continent.Equals(continent)) {
					for (int p=0;p<country.provinces.Length;p++) {
						int provinceIndex = map.GetProvinceIndex(country.provinces[p]);
						m_MountPointPopulateProvince(provinceIndex);
					}
				}
			}
			PopulateEnds();
		}


		/// <summary>
		/// Automatically adds new mount points to current country.
		/// </summary>
		public void MountPointPopulateCountry() {
			MountPointPopulateCountry(countryIndex);
		}

		void m_MountPointPopulateCountry(int countryIndex) {
			if (countryIndex<0) return;
			int mainRegionIndex = map.countries[countryIndex].mainRegionIndex;
			Region region = map.countries[countryIndex].regions[mainRegionIndex];
			PopulateRegion(countryIndex, -1, region);
		}


		void MountPointPopulateCountry(int countryIndex) {
			m_MountPointPopulateCountry(countryIndex);
			PopulateEnds();
		}

		/// <summary>
		/// Automatically adds new mount points to all provinces of current country.
		/// </summary>
		public void MountPointPopulateProvinces() {
			if (countryIndex<0) return;
			for (int k=0;k<map.provinces.Length;k++) {
				if (map.provinces[k].countryIndex == countryIndex) {
					m_MountPointPopulateProvince(k);
				}
			}
			PopulateEnds();
		}


		/// <summary>
		/// Automatically adds new mount points to current province.
		/// </summary>
		public void MountPointPopulateProvince() {
			MountPointPopulateProvince(provinceIndex);
		}

		void m_MountPointPopulateProvince(int provinceIndex) {
			if (provinceIndex<0) return;
			if (map.provinces[provinceIndex].regions == null) map.ReadProvincePackedString(map.provinces[provinceIndex]);
			int mainRegionIndex = map.provinces[provinceIndex].mainRegionIndex;
			Region region = map.provinces[provinceIndex].regions[mainRegionIndex];
			PopulateRegion(countryIndex, provinceIndex, region);
		}

		void MountPointPopulateProvince(int provinceIndex) {
			m_MountPointPopulateProvince(provinceIndex);
			PopulateEnds();
		}

		void PopulateRegion(int countryIndex, int provinceIndex, Region region) {

			int mountPoints = UnityEngine.Random.Range(GUIMountPointMassPopulationMin, GUIMountPointMassPopulationMax+1);
			while(mountPoints>0) {

				// Get a random position inside region
				Vector2 point;;
				for (int k=0;k<100;k++) {
					float rx = UnityEngine.Random.value * region.rect2D.width;
					float ry = UnityEngine.Random.value * region.rect2D.height;
					point = new Vector2(region.rect2D.xMin + rx, region.rect2D.yMin + ry);
					float r = Mathf.Min (rx,ry) * GUIMountPointMassPopulationSeparation * 0.01f;
					if (region.Contains(point) && map.GetMountPointNearPoint(point, r)<0) {
						map.MountPointAdd(point, map.mountPoints.Count.ToString(), countryIndex, provinceIndex, GUIMountPointMassPopulationTypeInteger);
						break;
					}
				}
				mountPoints--;
			}
		}

		void PopulateEnds() {
			map.DrawMountPoints();
			lastMountPointCount = -1;
			ReloadMountPointNames();
			mountPointChanges = true;
		}

		#endregion

	}
}
