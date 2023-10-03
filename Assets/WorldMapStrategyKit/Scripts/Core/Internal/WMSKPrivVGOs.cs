// World Map Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

    public partial class WMSK : MonoBehaviour {

        // viewport game objects
        Dictionary<int, GameObjectAnimator> vgosDict;
        GameObjectAnimator[] vgos;
        int vgosCount;
        bool vgosArrayIsDirty;

        // Water effects
        float buoyancyCurrentAngle;

        void SetupVGOs() {
            if (vgos == null) {
                vgosDict = new Dictionary<int, GameObjectAnimator>();
            }
            if (vgos == null || vgos.Length < vgosCount) {
                vgos = new GameObjectAnimator[vgosCount > 100 ? vgosCount : 100];
            }
        }


        void CheckVGOsArrayDirty() {
            if (vgos == null || vgos.Length < vgosCount) {
                vgos = new GameObjectAnimator[vgosCount];
            }
            if (!vgosArrayIsDirty) return;
            for (int k = 0; k < vgosCount; k++) {
                if (vgos[k] == null) {
                    for (int j = k + 1; j < vgosCount; j++) {
                        vgos[j - 1] = vgos[j];
                    }
                    vgosCount--;
                    k--;
                }
            }
            vgosArrayIsDirty = false;
        }

        void UpdateViewportObjectsLoop() {
            // Update animators
            CheckVGOsArrayDirty();
            for (int k = 0; k < vgosCount; k++) {
                GameObjectAnimator vgo = vgos[k];
                if (vgo.isMoving || vgo.mouseIsOver || (vgo.lastKnownPosIsOnWater && vgo.enableBuoyancyEffect)) {
                    vgo.PerformUpdateLoop();
                }
            }
        }


        void UpdateViewportObjectsTransformAndVisibility() {
            // Update animators
            CheckVGOsArrayDirty();
            for (int k = 0; k < vgosCount; k++) {
                GameObjectAnimator vgo = vgos[k];
                vgo.UpdateTransformAndVisibility();
            }
        }

        void UpdateViewportObjectsVisibility() {
            // Update animators
            CheckVGOsArrayDirty();
            for (int k = 0; k < vgosCount; k++) {
                GameObjectAnimator vgo = vgos[k];
                vgo.UpdateVisibility(false);
            }
        }

        void RepositionViewportObjects() {
            if (renderViewportIsEnabled) {
                for (int k = 0; k < vgosCount; k++) {
                    GameObjectAnimator go = vgos[k];
                    go.transform.SetParent(null, true);
                    go.UpdateTransformAndVisibility(true);
                }
            } else {
                for (int k = 0; k < vgosCount; k++) {
                    GameObjectAnimator go = vgos[k];
                    go.transform.localScale = go.originalScale;
                    go.UpdateTransformAndVisibility(true);
                }
            }
        }

        void UpdateViewportObjectsBuoyancy() {
            buoyancyCurrentAngle = Mathf.Sin(time) * VGOBuoyancyAmplitude * Mathf.Rad2Deg;
        }



    }
}
