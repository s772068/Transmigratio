using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WorldMapStrategyKit.MapGenerator.Geom;

namespace WorldMapStrategyKit {
	
	public interface MapEntity {

		MapRegion region { get; set; }
	}

}
