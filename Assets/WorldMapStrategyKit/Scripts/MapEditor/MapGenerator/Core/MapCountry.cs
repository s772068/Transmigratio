using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

	public partial class MapCountry: MapEntity {

		public MapRegion region { get; set; }
		readonly public List<MapCell> cells = new List<MapCell>();
		readonly public List<MapProvince> provinces = new List<MapProvince>();
        public Color color = Color.gray;
		public Vector2 capitalCenter;

	}

}