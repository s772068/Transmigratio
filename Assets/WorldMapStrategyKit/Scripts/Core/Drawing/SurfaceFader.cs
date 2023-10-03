using UnityEngine;
using System.Collections;

namespace WorldMapStrategyKit {

	public enum FADER_STYLE
	{
		FadeOut = 0,
		Blink = 1,
		Flash = 2
	}

	public class SurfaceFader : MonoBehaviour {

		public float duration;
		Material fadeMaterial;
		float highlightFadeStart;
		public IFader fadeEntity;
		FADER_STYLE style;
		Color color, initialColor;
		Renderer _renderer;
		WMSK map;

		public static void Animate (FADER_STYLE style, IFader fadeEntity, Renderer renderer, Color initialColor, Color color, float duration)
		{
			SurfaceFader fader = renderer.GetComponent<SurfaceFader> ();
			if (fader == null) {
				fader = renderer.gameObject.AddComponent<SurfaceFader>();
				fader.fadeMaterial = Instantiate(renderer.sharedMaterial);
				fader.fadeMaterial.hideFlags = HideFlags.DontSave;
				renderer.sharedMaterial = fader.fadeMaterial;
			}
			fader.duration = duration + 0.0001f;
			fader.color = color;
			fader.style = style;
			fader._renderer = renderer;
			fader.fadeEntity = fadeEntity;
			fadeEntity.isFading = true;
			fader.initialColor = initialColor;
			fader.map = WMSK.GetInstance(renderer.transform);
			fader.highlightFadeStart = fader.map.time;
			fader.Update();
		}

        void OnDestroy() {
			if (fadeMaterial != null) DestroyImmediate(fadeMaterial);
        }

        // Update is called once per frame
        public void Update () {
			if (this == null || map == null) return;
			float elapsed = map.time - highlightFadeStart;
			switch (style) {
			case FADER_STYLE.FadeOut:
				UpdateFadeOut (elapsed);
				break;
			case FADER_STYLE.Blink:
				UpdateBlink (elapsed);
				break;
			case FADER_STYLE.Flash:
				UpdateFlash (elapsed);
				break;
			}
		}


		void UpdateFadeOut(float elapsed) {
			SetFadeOutColor(elapsed / duration);
			if (elapsed > duration) {
				if (fadeEntity!=null) {
					fadeEntity.isFading = false;
					fadeEntity.customMaterial = null;
				}
				_renderer.enabled = false;
				Destroy (this);
			}
		}

		void SetFadeOutColor(float t) {
			Color newColor = Color.Lerp (color, Misc.ColorClear, t);
			fadeMaterial.color = newColor;
			if (_renderer.sharedMaterial != fadeMaterial) {
				fadeMaterial.mainTexture = _renderer.sharedMaterial.mainTexture;
				_renderer.sharedMaterial = fadeMaterial;
			}
		}

		#region Flash effect
		
		void UpdateFlash (float elapsed)
		{
			SetFlashColor (elapsed / duration);
			if (elapsed >= duration) {
				if (fadeEntity!=null) {
					fadeEntity.isFading = false;
					if (fadeEntity.customMaterial!=null) {
						_renderer.sharedMaterial = fadeEntity.customMaterial;
					} else {
						_renderer.enabled = false;
					}
				}
				Destroy (this);
			}
		}
		
		void SetFlashColor (float t)
		{
			Color newColor = Color.Lerp (color, initialColor, t);
			fadeMaterial.color = newColor;
			if (_renderer.sharedMaterial != fadeMaterial) {
				fadeMaterial.mainTexture = _renderer.sharedMaterial.mainTexture;
				_renderer.sharedMaterial = fadeMaterial;
			}
		}
		
		#endregion
		
		#region Blink effect
		
		void UpdateBlink (float elapsed)
		{
			SetFadeColor (elapsed / duration);
			if (elapsed >= duration) {
				SetFadeColor (0);
				if (fadeEntity!=null) {
					fadeEntity.isFading = false;
					if (fadeEntity.customMaterial!=null) {
						_renderer.sharedMaterial = fadeEntity.customMaterial;
					} else {
						_renderer.enabled = false;
					}
				}
				Destroy (this);
			}
		}
		
		void SetFadeColor (float t)
		{
			Color newColor;
			if (t < 0.5f) {
				newColor = Color.Lerp (initialColor, color, t * 2f);
			} else {
				newColor = Color.Lerp (color, initialColor, (t - 0.5f) * 2f);
			}
			fadeMaterial.color = newColor;
			if (_renderer.sharedMaterial != fadeMaterial) {
				fadeMaterial.mainTexture = _renderer.sharedMaterial.mainTexture;
				_renderer.sharedMaterial = fadeMaterial;
			}
		}
		
		#endregion

	}

}
