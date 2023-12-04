﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

	public struct CellSegment {
		
		public Vector2 start, end;

		// true if this segment is already used by another hex
		public bool isRepeated;


		public CellSegment(Vector2 start, Vector2 end, bool isRepeated = false) {
			this.start = start;
			this.end = end;
			this.isRepeated = isRepeated;
		}

		public override string ToString() {
			return string.Format("start:" + start.ToString("F5") + ", end:" + end.ToString("F5"));
		}

		public CellSegment swapped {
			get { 
				CellSegment n = new CellSegment(end, start, true); 
				return n;
			} 
		}


	}

}
	