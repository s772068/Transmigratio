using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit
{
	/// <summary>
	/// Mount Point scaler (similar to City Scaler). Checks the mount point icons' size is always appropiate
	/// </summary>
	public class MountPointScaler : MonoBehaviour
	{

		const float MOUNTPOINT_SIZE_ON_SCREEN = 10.0f;
		Vector3 lastCamPos, lastPos;
		float lastIconSize;
		float lastCustomSize;
		float lastOrtographicSize;

		[NonSerialized]
		public WMSK map;

		void Start ()
		{
			ScaleMountPoints ();
		}
	
		// Update is called once per frame
		void Update ()
		{
			if (lastPos != transform.position || lastCamPos != map.currentCamera.transform.position || lastIconSize != map.cityIconSize ||
			             map.currentCamera.orthographic && map.currentCamera.orthographicSize != lastOrtographicSize) {
				ScaleMountPoints ();
			}
		}

		public void ScaleMountPoints ()
		{
			if (map == null || map.currentCamera == null || map.currentCamera.pixelWidth == 0)
				return; // Camera pending setup

			// annotate current values 
			lastPos = transform.position;
			lastCamPos = map.currentCamera.transform.position;
			lastIconSize = map.cityIconSize;
			lastOrtographicSize = map.currentCamera.orthographicSize;

			Plane plane = new Plane(transform.forward, transform.position);
			float dist = plane.GetDistanceToPoint(lastCamPos);
			Vector3 centerPos = lastCamPos - transform.forward * dist;
			Vector3 a = map.currentCamera.WorldToScreenPoint(centerPos);
			Vector3 b = new Vector3(a.x, a.y + MOUNTPOINT_SIZE_ON_SCREEN, a.z);
			if (map.currentCamera.pixelWidth == 0) return; // Camera pending setup
			Vector3 aa = map.currentCamera.ScreenToWorldPoint(a);
			Vector3 bb = map.currentCamera.ScreenToWorldPoint(b);
			float scale = (aa - bb).magnitude * map.cityIconSize;
			if (map.currentCamera.orthographic) {
				scale /= 1 + (map.currentCamera.orthographicSize * map.currentCamera.orthographicSize) * (0.1f / map.transform.localScale.x);
			} else {
				scale /= 1 + dist * dist * (0.1f / map.transform.localScale.x);
			}
			Vector3 newScale = new Vector3 (scale / WMSK.mapWidth, scale / WMSK.mapHeight, 1.0f);
			newScale *= 2.0f;
			foreach (Transform t in transform)
				t.localScale = newScale;
		}

		public void ScaleMountPoints (float customSize)
		{
			if (customSize == lastCustomSize)
				return;
			lastCustomSize = customSize;
			Vector3 newScale = new Vector3 (customSize / WMSK.mapWidth, customSize / WMSK.mapHeight, 1);
			newScale *= 2.0f;
			foreach (Transform t in transform)
				t.localScale = newScale;
		}
	}

}