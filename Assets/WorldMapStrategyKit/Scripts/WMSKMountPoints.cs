// World Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK
// ***************************************************************************
// This is the public API file - every property or public method belongs here
// ***************************************************************************

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace WorldMapStrategyKit {

	/* Public WPM Class */
	public partial class WMSK : MonoBehaviour {

		/// <summary>
		/// Complete list of mount points.
		/// </summary>
		[NonSerialized]
		public List<MountPoint>	mountPoints;


		#region Public API area

		/// <summary>
        /// Returns the position of a mount point
        /// </summary>
        /// <param name="mountPointIndex"></param>
        /// <returns></returns>
		public Vector2 GetMountPointPosition(int mountPointIndex) {
			if (!CheckMountPointIndex(mountPointIndex)) return Misc.Vector2zero;
			return mountPoints[mountPointIndex].unity2DLocation;
        }

		/// <summary>
		/// Clears any mount point highlighted (color changed) and resets them to default city color (used from Editor)
		/// </summary>
		public void HideMountPointHighlights() {
			if (mountPointsLayer == null)
				return;
			Renderer[] rr = mountPointsLayer.GetComponentsInChildren<Renderer>(true);
			for (int k = 0; k < rr.Length; k++)
				rr[k].sharedMaterial = mountPointsMat;
		}

		/// <summary>
		/// Toggles the mount point highlight.
		/// </summary>
		/// <param name="mountPointIndex">Moint point index in the mount points collection.</param>
		/// <param name="color">Color.</param>
		/// <param name="highlighted">If set to <c>true</c> the color of the mount point will be changed. If set to <c>false</c> the color of the mount point will be reseted to default color</param>
		public void ToggleMountPointHighlight(int mountPointIndex, Color color, bool highlighted) {
			if (mountPointsLayer == null)
				return;
			Transform t = mountPointsLayer.transform.Find(mountPointIndex.ToString());
			if (t == null)
				return;
			Renderer rr = t.gameObject.GetComponent<Renderer>();
			if (rr == null)
				return;
			Material mat;
			if (highlighted) {
				mat = Instantiate(rr.sharedMaterial);
				mat.name = rr.sharedMaterial.name;
				if (disposalManager!=null) disposalManager.MarkForDisposal(mat); //mat.hideFlags = HideFlags.DontSave;
				mat.color = color;
				rr.sharedMaterial = mat;
			} else {
				rr.sharedMaterial = mountPointsMat;
			}
		}

		
		/// <summary>
		/// Returns an array with the mount points names.
		/// </summary>
		public string[] GetMountPointNames() {
			return GetMountPointNames(-1, -1);
		}

		/// <summary>
		/// Returns an array with the mount points names.
		/// </summary>
		public string[] GetMountPointNames(int countryIndex) {
			return GetMountPointNames(countryIndex, -1);
		}

		
		/// <summary>
		/// Returns an array with the mount points names.
		/// </summary>
		public string[] GetMountPointNames(int countryIndex, int provinceIndex) {
			List<string> c = new List<string>(20);
			for (int k = 0; k < mountPoints.Count; k++) {
				if ((mountPoints[k].countryIndex == countryIndex || countryIndex == -1) &&
				                (mountPoints[k].provinceIndex == provinceIndex || provinceIndex == -1)) {
					c.Add(mountPoints[k].name + " (" + k + ")");
				}
			}
			c.Sort();
			return c.ToArray();
		}

		/// <summary>
		/// Gets the mount point index with that unique Id.
		/// </summary>
		public int GetMountPointIndex(int uniqueId) {
			if (mountPoints == null)
				return -1;
			for (int k = 0; k < mountPoints.Count; k++) {
				if (mountPoints[k].uniqueId == uniqueId)
					return k;
			}
			return -1;
		}

		/// <summary>
		/// Returns the index of a mount point in the global mount points collection. Note that country (and optionally province) index can be supplied due to repeated mount point names.
		/// Pass -1 to countryIndex or provinceIndex to ignore filters.
		/// </summary>
		public int GetMountPointIndex(int countryIndex, int provinceIndex, string mountPointName) {
			if (mountPoints == null)
				return -1;
			for (int k = 0; k < mountPoints.Count; k++) {
				if ((mountPoints[k].countryIndex == countryIndex || countryIndex == -1) &&
				                (mountPoints[k].provinceIndex == provinceIndex || provinceIndex == -1) &&
				                mountPoints[k].name.Equals(mountPointName)) {
					return k;
				}
			}
			return -1;
		}

		
		/// <summary>
		/// Returns the mount point index by screen position.
		/// </summary>
		public bool GetMountPointIndex(Ray ray, out int mountPointIndex) {
			int hitCount = Physics.RaycastNonAlloc(ray, tempHits, 5000, layerMask);
			if (hitCount > 0) {
				for (int k = 0; k < hitCount; k++) {
					if (tempHits[k].collider.gameObject == gameObject) {
						Vector3 localHit = transform.InverseTransformPoint(tempHits[k].point);
						int c = GetMountPointNearPoint(localHit);
						if (c >= 0) {
							mountPointIndex = c;
							return true;
						}
					}
				}
			}
			mountPointIndex = -1;
			return false;
		}


		
		/// <summary>
		/// Deletes all mount points of current selected country's continent
		/// </summary>
		public void MountPointsDeleteFromSameContinent(string continentName) {
			HideMountPointHighlights();
			int k = -1;
			while (++k < mountPoints.Count) {
				int cindex = mountPoints[k].countryIndex;
				if (cindex >= 0) {
					string mpContinent = _countries[cindex].continent;
					if (mpContinent.Equals(continentName)) {
						mountPoints.RemoveAt(k);
						k--;
					}
				}
			}
		}


		public void MountPointAdd(Vector2 location, string name, int countryIndex, int provinceIndex, int type) {
			if (mountPoints == null)
				mountPoints = new List<MountPoint>();
			int uniqueId = GetUniqueId(new List<IExtendableAttribute>(mountPoints.ToArray()));
			MountPoint newMountPoint = new MountPoint(name, countryIndex, provinceIndex, location, uniqueId, type);
			mountPoints.Add(newMountPoint);
		}


		
		/// <summary>
		/// Returns a list of mount points whose attributes matches predicate
		/// </summary>
		public List<MountPoint> GetMountPoints(AttribPredicate predicate) {
			List <MountPoint> selectedMountPoints = new List<MountPoint>();
			int mountPointCount = mountPoints.Count;
			for (int k = 0; k < mountPointCount; k++) {
				MountPoint mountPoint = mountPoints[k];
				if (predicate(mountPoint.attrib))
					selectedMountPoints.Add(mountPoint);
			}
			return selectedMountPoints;
		}


		/// <summary>
		/// Returns a list of mount points contained in a given region
		/// </summary>
		public List<MountPoint> GetMountPoints(Region region) {
			int mpCount = mountPoints.Count;
			List<MountPoint> cc = new List<MountPoint>();
			for (int k = 0; k < mpCount; k++) {
				if (region.Contains(mountPoints[k].unity2DLocation))
					cc.Add(mountPoints[k]);
			}
			return cc;
		}


		/// <summary>
		/// Returns any mousnep point near the point specified in local coordinates.
		/// </summary>
		public int GetMountPointNearPoint(Vector2 localPoint) {
			return GetMountPointNearPoint(localPoint, MOUNT_POINT_HIT_PRECISION);
		}

		/// <summary>
		/// Returns any mount point near the point specified in local coordinates.
		/// </summary>
		/// <param name="separation">Distance threshold (minimum should be MOUNT_POINT_HIT_PRECISION constant).</param>
		public int GetMountPointNearPoint(Vector2 localPoint, float separation) {
			if (mountPoints == null)
				return -1;
			if (separation < MOUNT_POINT_HIT_PRECISION)
				separation = MOUNT_POINT_HIT_PRECISION;
			float separationSqr = separation * separation;
			int count = mountPoints.Count;
			for (int c = 0; c < count; c++) {
				Vector2 mpLoc = mountPoints[c].unity2DLocation;
				float distSqr = FastVector.SqrDistance(ref mpLoc, ref localPoint); // (mpLoc - localPoint).sqrMagnitude;
				if (distSqr < separationSqr) {
					return c;
				}
			}
			return -1;
		}


		/// <summary>
		/// Returns mount points belonging to a provided country.
		/// </summary>
		List<MountPoint> GetMountPoints(int countryIndex) {
			List<MountPoint> results = new List<MountPoint>(20);
			for (int c = 0; c < mountPoints.Count; c++) {
				if (mountPoints[c].countryIndex == countryIndex)
					results.Add(mountPoints[c]);
			}
			return results;
		}

		/// <summary>
		/// Returns mount points belonging to a provided country and province.
		/// </summary>
		List<MountPoint> GetMountPoints(int countryIndex, int provinceIndex) {
			List<MountPoint> results = new List<MountPoint>(20);
			for (int c = 0; c < mountPoints.Count; c++) {
				if (mountPoints[c].countryIndex == countryIndex && mountPoints[c].provinceIndex == provinceIndex)
					results.Add(mountPoints[c]);
			}
			return results;
		}

		#endregion

		#region IO functions area

		/// <summary>
		/// Exports the geographic data of mount points in packed string format.
		/// </summary>
		public string GetMountPointsGeoData() {
			JSONObject json = new JSONObject();
			for (int k = 0; k < mountPoints.Count; k++) {
				MountPoint mp = mountPoints[k];
				JSONObject mpJSON = new JSONObject();
				mpJSON.AddField("Name", DataEscape(mp.name));
				int provinceUniqueID = -1;
				if (mp.provinceIndex >= 0 && mp.provinceIndex < provinces.Length)
					provinceUniqueID = provinces[mp.provinceIndex].uniqueId;
				mpJSON.AddField("Province", provinceUniqueID);
				int countryUniqueID = -1;
				if (mp.countryIndex >= 0 && mp.countryIndex < countries.Length)
					countryUniqueID = countries[mp.countryIndex].uniqueId;
				mpJSON.AddField("Country", countryUniqueID);
				mpJSON.AddField("Type", mp.type);
				mpJSON.AddField("X", mp.unity2DLocation.x);
				mpJSON.AddField("Y", mp.unity2DLocation.y);
				mpJSON.AddField("Id", mp.uniqueId);
				json.Add(mpJSON);
			}
			return json.Print(true);
		}

		/// <summary>
		/// Reads the mount points geo data from a packed string.
		/// </summary>
		public void SetMountPointsGeoData(string s) {
			if (s.IndexOf('{') >= 0) {
				ReadMountPointsXML(s);
				return;
			}
			string[] mountPointsList = s.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			int mountPointsCount = mountPointsList.Length;
			mountPoints = new List<MountPoint>(mountPointsCount);
			
			for (int k = 0; k < mountPointsCount; k++) {
				string[] mountPointInfo = mountPointsList[k].Split(new char[] { '$' });
				if (mountPointInfo.Length < 6)
					continue;
				string name = mountPointInfo[0];
				string country = mountPointInfo[2];
				int countryIndex = GetCountryIndex(country);
				if (countryIndex >= 0) {
					string province = mountPointInfo[1];
					int provinceIndex = GetProvinceIndex(countryIndex, province);
					int type = int.Parse(mountPointInfo[3], Misc.InvariantCulture);
					float x = float.Parse(mountPointInfo[4], Misc.InvariantCulture);
					float y = float.Parse(mountPointInfo[5], Misc.InvariantCulture);
					int uniqueId;
					if (mountPointInfo.Length >= 7) {
						uniqueId = int.Parse(mountPointInfo[6], Misc.InvariantCulture);
					} else {
						uniqueId = GetUniqueId(new List<IExtendableAttribute>(_countries));
					}
					MountPoint mountPoint = new MountPoint(name, countryIndex, provinceIndex, new Vector2(x, y), type, uniqueId);
					mountPoints.Add(mountPoint);
				}
			}
		}

		/// <summary>
		/// Get attributes of all mount points in jSON format.
		/// </summary>
		public string GetMountPointsAttributes(bool prettyPrint = true) {
			return GetMountPointsAttributes(new List<MountPoint>(mountPoints), prettyPrint);
		}

		/// <summary>
		/// Gets attributes of provided mount points in jSON format.
		/// </summary>
		public string GetMountPointsAttributes(List<MountPoint> mountPoints, bool prettyPrint = true) {
			JSONObject composed = new JSONObject();
			int mountPointCount = mountPoints.Count;
			for (int k = 0; k < mountPointCount; k++) {
				MountPoint mountPoint = mountPoints[k];
				composed.AddField(mountPoint.uniqueId.ToString(), mountPoint.attrib);
			}
			return composed.Print(prettyPrint);
		}

		/// <summary>
		/// Sets mount points attributes from a jSON formatted string.
		/// </summary>
		public void SetMountPointsAttributes(string jSON) {
			JSONObject composed = new JSONObject(jSON);
			if (composed.keys == null)
				return;
			int keyCount = composed.keys.Count;
			for (int k = 0; k < keyCount; k++) {
				int uniqueId = int.Parse(composed.keys[k]);
				int mountPointIndex = GetMountPointIndex(uniqueId);
				if (mountPointIndex >= 0) {
					mountPoints[mountPointIndex].attrib = composed[k];
				}
			}
		}


        /// <summary>
        /// Exports all mount points data (geo and attributes) in jSON format.
        /// </summary>
        public string GetMountPointsDataJSON(bool prettyPrint = true) {
            MountPointsJSONData exported = new MountPointsJSONData();
			int mpCount = mountPoints.Count;
            for (int k = 0; k < mpCount; k++) {
                MountPoint mp = mountPoints[k];
                MountPointJSON mpjson = new MountPointJSON();
                mpjson.name = mp.name;
                mpjson.type = mp.type;
                mpjson.province = GetProvince(mp.provinceIndex).name;
				mpjson.country = GetCountry(mp.countryIndex).name;
                mpjson.unity2DLocation = mp.unity2DLocation;
				mpjson.uniqueId = mp.uniqueId;
				mpjson.attrib = mp.attrib;
                exported.mountPoints.Add(mpjson);
            }
            return JsonUtility.ToJson(exported, prettyPrint);
        }

        /// <summary>
        /// Loads mount points data (geo and attributes) from a jSON string
        /// </summary>
        public void SetMountPointsDataJSON(string s) {
            MountPointsJSONData imported = JsonUtility.FromJson<MountPointsJSONData>(s);
            int mpCount = imported.mountPoints.Count;
			mountPoints.Clear();
            for (int k = 0; k < mpCount; k++) {
                MountPointJSON mpjson = imported.mountPoints[k];
                string name = mpjson.name;
				string country = mpjson.country;
                int countryIndex = GetCountryIndex(country);
                if (countryIndex >= 0) {
                    string province = mpjson.province;
                    int provinceIndex = GetProvinceIndex(countryIndex, province);
					int type = mpjson.type;
					Vector2 location = mpjson.unity2DLocation;
					int uniqueId = mpjson.uniqueId;
                    MountPoint mountPoint = new MountPoint(name, countryIndex, provinceIndex, location, type, uniqueId);
					mountPoint.attrib = mpjson.attrib;
                    mountPoints.Add(mountPoint);
                }
            }
        }


        #endregion



    }

}