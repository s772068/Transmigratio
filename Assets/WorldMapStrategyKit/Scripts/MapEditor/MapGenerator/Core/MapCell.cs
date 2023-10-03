using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WorldMapStrategyKit.MapGenerator.Geom;

namespace WorldMapStrategyKit {


	public partial class MapCell: MapEntity {

		/// <summary>
		/// The country to which this province belongs to.
		/// </summary>
		public int countryIndex = -1;

        /// <summary>
        /// The province to which this province belongs to.
        /// </summary>
        public int provinceIndex = -1;

        /// <summary>
        /// Can be used to force this provice to be ignored by countries
        /// </summary>
        public bool ignoreTerritories;

		public MapRegion region { get; set; }

		public Vector2 center;

		public bool visible { get; set; }

		public MapCell (Vector2 center) {
			this.center = center;
			this.visible = true;
		}

	}
}

