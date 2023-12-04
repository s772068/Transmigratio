#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit.ClipperLib;


namespace WorldMapStrategyKit {

	public partial class WMSK : MonoBehaviour {

		// Use this for initialization
		public void ExecuteTests () {
//								ProvinceMigrationTests();
			ClearAll ();
			RegionMergeTest ();
		}


		void RegionMergeTest () {

			ClearAll ();


			Country country = new Country ("", "", 1);
			Region r1 = new Region (country, 0);
			showGrid = true;
			//								Vector2[] pp1 = new Vector2[] { new Vector2(2,1), new Vector2(3,1), new Vector2(4,2), new Vector2(1,2) };
			//								r1.UpdatePointsAndRect(pp1);
			Cell cell1 = GetCell (10, 10);
			r1.UpdatePointsAndRect (cell1.points);
			Region r2 = new Region (country, 1);
			//								Vector2[] pp2 = new Vector2[] { new Vector2(2,1), new Vector2(1,0), new Vector2(4,0), new Vector2(3,1) };
			//								r2.UpdatePointsAndRect(pp2);
			Cell cell2 = GetCell (10, 11);
			r2.UpdatePointsAndRect (cell2.points);

//												PolygonClipper pc = new PolygonClipper (r1, r2);
//												pc.Compute (PolygonOp.UNION, null);

			Clipper clipper = new Clipper ();
			clipper.AddPath (r1, PolyType.ptSubject);
			clipper.AddPath (r2, PolyType.ptClip);
			clipper.Execute (ClipType.ctUnion);

			country.regions.Add (r1);
			int countryIndex = CountryAdd (country);
			RefreshCountryDefinition (countryIndex, null);
			Redraw ();
			GenerateCountryRegionSurface (countryIndex, 0, GetColoredTexturedMaterial (Color.red, null));
			Debug.Log ("ok");
		}

	

	}
}

#endif