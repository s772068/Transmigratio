using UnityEngine;
using System.Collections;

namespace WorldMapStrategyKit {

	[ExecuteInEditMode]
	public class MiniMapTrigger : MonoBehaviour {

		WMSKMiniMap miniMap;

		void OnEnable() {
			if (miniMap == null) {
				miniMap = WMSKMiniMap.Show(new Vector4(0, 0, 1, 1));
				miniMap.UIParent = GetComponent<RectTransform>();
			}
		}

		void OnDisable() {
			if (miniMap != null) {
				DestroyImmediate(miniMap.gameObject);
				miniMap = null;
			}
		}
	}
}
