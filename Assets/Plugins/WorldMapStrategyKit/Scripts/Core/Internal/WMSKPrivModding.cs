// World Map Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WorldMapStrategyKit.Poly2Tri;


namespace WorldMapStrategyKit {

	public partial class WMSK : MonoBehaviour {

		#region Region related functions

		bool LoadProvinceMap(string path) {

						
			byte[] bytes = File.ReadAllBytes (path);

			Texture2D tex = new Texture2D (2, 2, TextureFormat.ARGB32, false, true);
			tex.LoadImage (bytes);



			return true;


		}

		#endregion

	}

}