﻿using UnityEngine;
using System.Collections;

namespace WorldMapStrategyKit {
	
	public class ExtrudedRegionInteraction : MonoBehaviour {

		public WMSK map;
		public Region region;
		public Color highlightColor;
		public Material topMaterial, sideMaterial;
		Color bandColor;

		void Start() {
			// Get a reference to the World Map API:
			bandColor = sideMaterial.color;

			map.OnRegionEnter += (region) => {
				if (region == this.region) {
					ChangeColor(highlightColor);
				}
			};
			map.OnRegionExit += (region) => {
				if (region == this.region) {
					RemoveColor();
				}
			};

		}


		void ChangeColor(Color color) {
			topMaterial.color = color;
			sideMaterial.color = color;
		}

		void RemoveColor() {
			topMaterial.color = Color.white;
			sideMaterial.color = bandColor;
		}


	}
}