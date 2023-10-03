using UnityEngine;
using System.Collections;

namespace WorldMapStrategyKit {

	public class MarkerBlinker : MonoBehaviour {

		public float duration = 4.0f;
		public float speed = 0.25f;
		public bool destroyWhenFinished;
		public float stopBlinkAfter = 0;
		WMSK map;
		Renderer markerRenderer;

		/// <summary>
		/// Adds a blinker to the given marker
		/// </summary>
		/// <param name="marker">Marker.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="speed">Blinking interval.</param>
		/// <param name="stopBlinkAfter">Stop blinking after x seconds (pass 0 to blink for the entire duration).</param>
		/// <param name="destroyWhenFinised">If set to <c>true</c> destroy when finised.</param>
		public static void AddTo (GameObject marker, float duration, float speed, float stopBlinkAfter = 0, bool destroyWhenFinised = false) {
			MarkerBlinker mb = marker.AddComponent<MarkerBlinker> ();
			mb.duration = duration;
			mb.speed = speed;
			mb.destroyWhenFinished = destroyWhenFinised;
			mb.stopBlinkAfter = stopBlinkAfter;
		}

		float startTime, lapTime;
		bool originalState;
		bool phase;

		void Start () {
			map = WMSK.GetInstance (transform);
			startTime = map.time;
			lapTime = startTime - speed;
			if (stopBlinkAfter <= 0) {
				stopBlinkAfter = float.MaxValue;
			}
			markerRenderer = GetComponentInChildren<Renderer>();
			if (markerRenderer == null) {
				Destroy(this);
				return;
            }
			originalState = markerRenderer.enabled;
		}


		// Update is called once per frame
		void Update () {
			float elapsed = map.time - startTime;
			if (elapsed > duration) {
				// Restores material
				markerRenderer.enabled = originalState;
				if (destroyWhenFinished) {
					Destroy (gameObject);
				} else {
					Destroy (this);
				}
				return;
			}
			if (map.time - lapTime > speed && markerRenderer != null) {
				lapTime = Time.time;
				phase = !phase;
				if (phase && elapsed < stopBlinkAfter) {
					markerRenderer.enabled = false;
				} else {
					markerRenderer.enabled = originalState;
				}
			}
		}
	}
}