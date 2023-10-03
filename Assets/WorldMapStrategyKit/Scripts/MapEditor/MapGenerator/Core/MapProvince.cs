using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

	public partial class MapProvince: MapEntity {

		public MapRegion region { get; set; }
		public int countryIndex;
		public Vector2 capitalCenter;
		readonly public List<MapCell> cells = new List<MapCell>();
		public Color color = Color.gray;
		public bool hasCapital { get; set; }

	}

}