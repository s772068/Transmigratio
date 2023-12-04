using UnityEngine;
using System.Collections;

namespace WorldMapStrategyKit {
	public class TileAnimator : MonoBehaviour {

		public delegate void AnimationEvent (TileInfo ti);

		public event AnimationEvent OnAnimationEnd;

		public float duration;
		internal TileInfo ti;
		float startTime;
		bool playing;

		public void Play () {
			ti.SetAlpha (0);
			startTime = Time.time;
			playing = true;
		}

		void Update () {
			if (!playing)
				return;

			if (ti == null || ti.loadStatus != TILE_LOAD_STATUS.Loaded) {
				Destroy (this);
				return;
			}

			float t = (Time.time - startTime) / duration;
			if (t >= 1f)
				t = 1f;
			ti.SetAlpha (t);
			if (t >= 1) {
				ti.animationFinished = true;
				if (OnAnimationEnd != null)
					OnAnimationEnd (ti);
				Destroy (this); 
			}
		}
	
	}
}