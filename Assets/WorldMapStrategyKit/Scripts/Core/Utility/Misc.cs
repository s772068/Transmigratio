using UnityEngine;
using System.Collections;
using System.Globalization;

namespace WorldMapStrategyKit {
	
	public static class Misc {
		
		public static Vector4 Vector4back = new Vector4(0f, 0f, -1f, 0f);
        public static Vector4 Vector4one = new Vector4(1f, 1f, 1f, 1f);

		public static Vector3 Vector3one = new Vector3(1f, 1f, 1f);
		public static Vector3 Vector3zero = new Vector3(0f, 0f, 0f);
		public static Vector3 Vector3forward = new Vector3(0f, 0f, 1f);
		public static Vector3 Vector3back = new Vector3(0f, 0f, -1f);
		public static Vector3 Vector3right = new Vector3(1f, 0f, 0f);
		public static Vector3 Vector3left = new Vector3(-1f, 0f, 0f);
		public static Vector3 Vector3up = new Vector3(0f, 1f, 0f);
		public static Vector3 Vector3down = new Vector3(0f, -1f, 0f);
		public static Vector3 Vector3max = Vector3.one * float.MaxValue;
		public static Vector3 Vector3min = Vector3.one * float.MinValue;

		public static Vector2 Vector2one = new Vector2(1f, 1f);
		public static Vector2 Vector2zero = new Vector2(0f, 0f);
		public static Vector2 Vector2left = new Vector2(-1f, 0f);
		public static Vector2 Vector2right = new Vector2(1f, 0f);
		public static Vector2 Vector2down = new Vector2(0f, -1f);
		public static Vector2 Vector2up = new Vector2(0f, 1f);
		public static Vector2 Vector2max = new Vector2(float.MaxValue, float.MaxValue);
		public static Vector2 Vector2min = new Vector2(float.MinValue, float.MinValue);

		public static Vector2 ViewportCenter = new Vector2(0.5f, 0.5f);

		public static Color ColorWhite = new Color(1, 1, 1, 1);
		public static Color ColorBlack = new Color(0, 0, 0, 1);
		public static Color ColorClear = new Color(0, 0, 0, 0);

		public static Quaternion QuaternionZero = Quaternion.Euler(Misc.Vector3zero);
		public static Quaternion QuaternionX90 =  Quaternion.Euler (90, 0, 0);

		public static float DistanceToLine(this Vector2 p, Vector2 a, Vector2 b) {
			Vector2 ab = b - a;
			float l2 = ab.sqrMagnitude;
			float u = Vector2.Dot(p - a, ab) / l2;
			Vector2 proj = a + u * ab;
			return Vector2.Distance(proj, p);
		}

		public static float DistanceToLineSqr(this Vector2 p, Vector2 a, Vector2 b) {
			Vector2 ab = b - a;
			float l2 = ab.sqrMagnitude;
			float u = Vector2.Dot(p - a, ab) / l2;
			Vector2 proj = a + u * ab;
			return Vector2.Dot(proj, p);
		}

		public static float DistanceToSegment(this Vector2 p, Vector2 a, Vector2 b) {
			Vector2 ab = b - a;
			float l2 = ab.sqrMagnitude;
			float u = Mathf.Clamp01(Vector2.Dot(p - a, ab) / l2);
			Vector2 proj = a + u * ab;
			return Vector2.Distance(proj, p);
		}

		public static float DistanceToSegmentSqr(this Vector2 p, Vector2 a, Vector2 b) {
			Vector2 ab = b - a;
			float l2 = ab.sqrMagnitude;
			float u = Mathf.Clamp01(Vector2.Dot(p - a, ab) / l2);
			Vector2 proj = a + u * ab;
			return Vector2.Dot(proj, p);
		}

		public static CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

	}

}