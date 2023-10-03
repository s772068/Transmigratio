// World Map Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK


using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WorldMapStrategyKit
{

	public partial class WMSK : MonoBehaviour
	{

		const float MOUNT_POINT_HIT_PRECISION = 0.0015f;

		#region Internal variables

		// resources
		Material mountPointsMat;
		GameObject mountPointSpot, mountPointsLayer;

		#endregion



		#region System initialization

		bool CheckMountPointIndex(int mountPointIndex) {
			return mountPointIndex >= 0 && mountPointIndex < mountPoints.Count;
        }

		void ReadMountPointsPackedString ()
		{
			string mountPointsCatalogFileName = _geodataResourcesPath + "/mountPoints";
			TextAsset ta = Resources.Load<TextAsset> (mountPointsCatalogFileName);
			if (ta != null) {
				string s = ta.text;
				SetMountPointsGeoData (s);
			} else {
				mountPoints = new List<MountPoint> ();
			}
		}

		void ReadMountPointsXML (string s)
		{
			JSONObject json = new JSONObject (s);
			int mountPointsCount = json.list.Count;
			mountPoints = new List<MountPoint> (mountPointsCount);
			for (int k = 0; k < mountPointsCount; k++) {
				JSONObject mpJSON = json [k];
				string name = mpJSON ["Name"];
				int countryUniqueId = mpJSON ["Country"];
				int countryIndex = GetCountryIndex (countryUniqueId);
				int provinceUniqueId = mpJSON ["Province"];
				int provinceIndex = GetProvinceIndex (provinceUniqueId);
				float x = mpJSON ["X"];
				float y = mpJSON ["Y"];
				if (x == 0 && y == 0) {
					// workaround for string data: fixes old issue, no longer needed but kept for compatibility
					float.TryParse (mpJSON ["X"], System.Globalization.NumberStyles.Float, Misc.InvariantCulture, out x);
					float.TryParse (mpJSON ["Y"], System.Globalization.NumberStyles.Float, Misc.InvariantCulture, out y);
				}
				// Try to locate country and provinces in case data does not match
				Vector2 location = new Vector2 (x, y);
				if (countryIndex < 0 && countryUniqueId>0) {
					countryIndex = GetCountryIndex (location);
				}
				if (provinceIndex < 0 && provinceUniqueId>0) {
					provinceIndex = GetProvinceIndex (location);
				}
				int uniqueId = mpJSON ["Id"];
				int type = mpJSON ["Type"];
				MountPoint mp = new MountPoint (name, countryIndex, provinceIndex, location, uniqueId, type);
				mp.attrib = mpJSON ["Attrib"];
				mountPoints.Add (mp);
			}
		}

		#endregion

		#region Drawing stuff

		/// <summary>
		/// Redraws the mounts points but only in editor time. This is automatically called by Redraw(). Used internally by the Map Editor. You should not need to call this method directly.
		/// </summary>
		public void DrawMountPoints ()
		{
			// Create mount points layer
			Transform t = transform.Find ("Mount Points");
			if (t != null)
				DestroyImmediate (t.gameObject);
			if (Application.isPlaying || mountPoints == null)
				return;

			mountPointsLayer = new GameObject ("Mount Points");
			mountPointsLayer.transform.SetParent (transform, false);

			// Draw mount points marks
			for (int k = 0; k < mountPoints.Count; k++) {
				MountPoint mp = mountPoints [k];
				GameObject mpObj = Instantiate (mountPointSpot); 
				mpObj.name = k.ToString ();
				mpObj.transform.position = transform.TransformPoint (mp.unity2DLocation);
				if (disposalManager != null) {
					disposalManager.MarkForDisposal (mpObj);
				}
				mpObj.hideFlags |= HideFlags.HideInHierarchy;
				mpObj.transform.SetParent (mountPointsLayer.transform, true);
			}

			MountPointScaler mpScaler = mountPointsLayer.GetComponent<MountPointScaler> () ?? mountPointsLayer.AddComponent<MountPointScaler> ();
			mpScaler.map = this;
			mpScaler.ScaleMountPoints ();
		}


		#endregion

		#region Internal Mount Points API

		/// <summary>
		/// Updates the mount points scale.
		/// </summary>
		public void ScaleMountPoints ()
		{
			if (mountPointsLayer != null) {
				MountPointScaler scaler = mountPointsLayer.GetComponent<MountPointScaler> ();
				if (scaler != null)
					scaler.ScaleMountPoints ();
			}
		}

		#endregion
	}

}