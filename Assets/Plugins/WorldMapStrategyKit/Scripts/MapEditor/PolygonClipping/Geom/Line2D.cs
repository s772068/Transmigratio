using UnityEngine;
using System.Collections;
using WorldMapStrategyKit.Poly2Tri;

namespace WorldMapStrategyKit.PolygonClipping {
	public class Line2D {
		
		public Vector2 P1;
		public Vector2 P2;
		public int P1Index, P2Index;

		double X1;
		double X2;
		double Y1;
		double Y2;
		double slopeX, slopeY;
		public float sqrMagnitude;


		public Line2D (Vector2 p1, Vector2 p2, int index1, int index2) {
			P1 = p1; 
			P2 = p2;
			X1 = p1.x;
			X2 = p2.x;
			Y1 = p1.y;
			Y2 = p2.y;
			P1Index = index1;
			P2Index = index2;
			slopeX = X2 - X1;
			slopeY = Y2 - Y1;
            sqrMagnitude = FastVector.SqrDistance(ref P1, ref P2);
        }

        public void Set (Vector2 p1, Vector2 p2, int index1, int index2) {
			P1 = p1; 
			P2 = p2;
			X1 = p1.x;
			X2 = p2.x;
			Y1 = p1.y;
			Y2 = p2.y;
			P1Index = index1;
			P2Index = index2;
			slopeX = X2 - X1;
			slopeY = Y2 - Y1;
            sqrMagnitude = FastVector.SqrDistance(ref P1, ref P2);
        }

        public bool IntersectsLine (Line2D comparedLine) {
			if (X2 == comparedLine.X1 && Y2 == comparedLine.Y1) {
				return false;
			}
		
			if (X1 == comparedLine.X2 && Y1 == comparedLine.Y2) {
				return false;
			}

			double s, t, w;
			w = slopeX * comparedLine.slopeY - comparedLine.slopeX * slopeY;
			s = (slopeX * (Y1 - comparedLine.Y1) - slopeY * (X1 - comparedLine.X1)) / w;
			t = (comparedLine.slopeX * (Y1 - comparedLine.Y1) - comparedLine.slopeY * (X1 - comparedLine.X1)) / w;
		
			if (s >= 0 && s <= 1 && t >= 0 && t <= 1) {
				return true;
			}
			return false; // No collision
		}


		public override int GetHashCode () {
			return (X1 * 1000 + X2 * 1000 + Y1 * 1000 + Y2 * 1000).GetHashCode ();
		}

		public override bool Equals (object obj) {
			return (obj.GetHashCode () == this.GetHashCode ());
		}
	
	}
}