using UnityEngine;
using System.Collections;

namespace WorldMapStrategyKit {
    public class PointerTrigger : MonoBehaviour {

        public WMSK map;

        void Awake() {
            if (GetComponent<TerrainCollider>())
                return;
            if (GetComponent<MeshCollider>() == null)
                gameObject.AddComponent<MeshCollider>();
        }

        void OnMouseEnter() {
            if (map == null)
                return;
            map.OnMouseEnter();
        }

        void OnMouseExit() {
            if (map == null)
                return;
            map.OnMouseExit();
        }

        // Support for NGUI
        void OnHover(bool isOver) {
            if (map == null)
                return;
            if (isOver) {
                map.OnMouseEnter();
            } else {
                map.OnMouseExit();
            }
        }


        void OnPress(bool isPressed) {
            if (map == null)
                return;

            if (isPressed) {
                map.DoOnMouseClick();
            } else {
                map.DoOnMouseRelease();
            }
        }


    }
}
