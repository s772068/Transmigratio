using UnityEngine;
using System.Collections;

namespace WorldMapStrategyKit {
	public class RectangleSelection : MonoBehaviour {
		public WMSK map;
		public OnRectangleSelection callback;
		public float lineWidth = 0.02f;
		public Color lineColor = Color.green;

		bool prevAllowDrag, dragging;
		Vector2 startPos, endPos;
		LineMarkerAnimator lines;

		void Start () {
			prevAllowDrag = map.allowUserDrag;
			map.allowUserDrag = false;
			map.OnMouseDown += ClickHandler;
			map.OnMouseMove += DragHandler;
			map.OnMouseRelease += ReleaseHandler;
		}

		void OnDestroy () {
			if (map != null) {
				map.OnMouseDown -= ClickHandler;
				map.OnMouseMove -= DragHandler;
				map.OnMouseRelease -= ReleaseHandler;
				map.allowUserDrag = prevAllowDrag;
			}
			if (lines != null) {
				DestroyImmediate (lines.gameObject);
			}
		}

		void ClickHandler (float x, float y, int buttonIndex) {
			if (dragging)
				return;
			InitiateSelection (x, y, buttonIndex);
		}

		public void InitiateSelection (float x, float y, int buttonIndex) {
			startPos = new Vector2 (x, y);
			dragging = true;
			endPos = startPos;
			UpdateRectangle (false);
		}

		void DragHandler (float x, float y) {
			if (!map.input.GetMouseButton (0))
				return;
			endPos = new Vector2 (x, y);
			UpdateRectangle (false);
		}

		void ReleaseHandler (float x, float y, int buttonIndex) {
			UpdateRectangle (true);
			Destroy (gameObject);
		}


		void UpdateRectangle (bool finishSelection) {
			if (map == null)
				return;

			Vector2 center = (startPos + endPos) * 0.5f;
			Vector2 scale = new Vector2 (Mathf.Abs (endPos.x - startPos.x), Mathf.Abs (endPos.y - startPos.y));
			map.AddMarker2DSprite (gameObject, center, scale);
			Vector2[] points = new Vector2[5];
			points [0] = center - scale * 0.5f;
			points [1] = points [0] + Misc.Vector2right * scale.x;
			points [2] = points [1] + Misc.Vector2up * scale.y;
			points [3] = points [2] - Misc.Vector2right * scale.x;
			points [4] = points [3] - Misc.Vector2up * scale.y;
			if (lines != null) {
				DestroyImmediate (lines.gameObject);
			}
			lines = map.AddLine (points, lineColor, 0f, lineWidth);
			lines.dashInterval = 0.001f;
			lines.dashAnimationDuration = 0.2f;
			if (callback != null) {
				callback (new Rect (center - scale * 0.5f, scale), finishSelection);
			}
		}
	}
}