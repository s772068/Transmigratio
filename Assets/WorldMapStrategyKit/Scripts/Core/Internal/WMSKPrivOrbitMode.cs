// World Map Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;

namespace WorldMapStrategyKit {


    public partial class WMSK : MonoBehaviour {

        Camera cameraPivot;
        Transform pitchTransform;
        const string CAMERA_PIVOT = "Camera Pivot";
        const string CAMERA_PITCH = "Pitch";
        float yawAngle, currentYawAngle;
        Vector3[] viewportCorners;
        Vector2[] viewportNDC;
        Region viewportNDCRegion;
        float fittedViewCameraDistance;
        float fittedPreviousCameraDistance;

        /// <summary>
        /// The current main camera. It's the camera pivot when camera tilt is enabled.
        /// </summary>
        public Camera viewCamera {
            get {
                if (isMiniMap && instance != this) return instance.viewCamera;

                if (_enableCameraTilt && cameraPivot != null) return cameraPivot;
                return cameraMain;
            }
        }

        bool renderViewportAllowsTilt => viewportMode == ViewportMode.None || viewportMode == ViewportMode.Viewport3D || viewportMode == ViewportMode.Terrain;

        void SetupCameraTiltMode() {

            if (isMiniMap) return;

            Camera cam = cameraMain;
            CameraPivot pivot = cam.transform.root.GetComponentInChildren<CameraPivot>();
            if (pivot != null) cameraPivot = pivot.GetComponent<Camera>();
            if (_enableCameraTilt) {
                if (!renderViewportAllowsTilt) {
                    Debug.LogWarning("Current render viewport mode does not allow camera tilt.");
                    _enableCameraTilt = false;
                    return;
                }
                if (cameraPivot == null) {
                    GameObject pitchGO = new GameObject(CAMERA_PITCH);
                    pitchTransform = pitchGO.transform;
                    pitchTransform.SetParent(cam.transform, false);

                    GameObject cameraPivotGO = new GameObject(CAMERA_PIVOT, typeof(Camera), typeof(CameraPivot));
                    cameraPivot = cameraPivotGO.GetComponent<Camera>();
                    cameraPivot.CopyFrom(cam);
                    Transform pivotTransform = cameraPivot.transform;
                    pivotTransform.SetParent(pitchTransform);
                    pivotTransform.localRotation = Misc.QuaternionZero;
                    pivotTransform.localPosition = Misc.Vector3zero;
                    cameraMain.enabled = false;
                } else {
                    pitchTransform = cameraPivot.transform.parent;
                }
                WMSKMiniMap.Reposition();
            } else {
                WMSKMiniMap.Reposition();
                if (cameraPivot != null) {
                    Transform parent = cameraPivot.transform.parent;
                    if (parent != null && CAMERA_PITCH.Equals(parent.name)) {
                        DestroyImmediate(parent.gameObject);
                    } else {
                        DestroyImmediate(cameraPivot.gameObject);
                    }
                    cam.enabled = true;
                }
            }

            if (viewportCorners == null || viewportCorners.Length != 4) {
                viewportCorners = new Vector3[4];
            }
            if (viewportNDC == null || viewportNDC.Length != 4) {
                viewportNDC = new Vector2[4];
            }
            if (viewportNDCRegion == null) {
                viewportNDCRegion = new Region(null, 0);
            }
            fittedViewCameraDistance = 0;
        }

        /// <summary>
        /// Better than Transform.RotateAround
        /// </summary>
        void RotateAround(Transform transform, Vector3 center, Vector3 axis, float angle) {
            Vector3 pos = transform.position;
            Quaternion rot = Quaternion.AngleAxis(angle, axis); // get the desired rotation
            Vector3 dir = pos - center;                         // find current direction relative to center
            dir = rot * dir;                                    // rotate the direction
            transform.position = center + dir;                  // define new position
                                                                // rotate object to keep looking at the center:
            transform.rotation = rot * transform.rotation;
        }


        void ApplyCameraTilt() {
            if (!_enableCameraTilt || cameraPivot == null || isMiniMap) return;

            float zoomLevel = GetZoomLevel();
            Camera cam = cameraMain;
            Vector3 camPos = cam.transform.position;
            Vector3 cursorPosWS = camPos;
            Transform pivot = cameraPivot.transform;
            float pivotDistance = lastDistanceFromCamera;
            Vector3 prevCameraPivotPos = pivot.position;

            switch (viewportMode) {
                case ViewportMode.None: {
                        cursorPosWS = camPos + transform.forward * lastDistanceFromCamera;
                        break;
                    }
                case ViewportMode.Viewport3D: {
                        pivotDistance = Vector3.Distance(renderViewport.transform.position, camPos);
                        cursorPosWS = renderViewport.transform.position;

                        // Alternate algorithm in case camera is not always centered on the viewport
                        //Transform viewportTransform = renderViewport.transform;
                        //Plane plane = new Plane(viewportTransform.forward, viewportTransform.position);
                        //float distanceToPlane = Mathf.Abs(plane.GetDistanceToPoint(camPos));
                        //cursorPosWS = camPos + viewportTransform.forward * distanceToPlane;
                        //pivotDistance = Vector3.Distance(cursorPosWS, camPos);
                        break;
                    }
                case ViewportMode.Terrain: {
                        // we assume terrain is horizontal and sits at y=0
                        pivotDistance = cam.transform.position.y;
                        cursorPosWS = new Vector3(camPos.x, 0, camPos.z);
                        break;
                    }
            }


            float pitchAngle = Mathf.Lerp(_cameraNearTiltAngle, _cameraFarTiltAngle, zoomLevel);
            pitchTransform.localRotation = Quaternion.Euler(pitchAngle, 0, 0);
            pivot.localRotation = Quaternion.identity;
            pivot.position = cursorPosWS - pivot.forward * pivotDistance;

            if (_enableCameraOrbit) {
                if (input.GetKey(_orbitLeftKeyName)) {
                    yawAngle += _orbitRotationSpeed;
                    yawAngle = (yawAngle + 360) % 360f;
                } else if (input.GetKey(_orbitRightKeyName)) {
                    yawAngle -= _orbitRotationSpeed;
                    yawAngle = (yawAngle + 360) % 360f;
                }

                if (zoomLevel > _orbitMaxZoomLevel) {
                    yawAngle = 0;
                }
                float destYawAngle;
                if (yawAngle - currentYawAngle > 180) {
                    destYawAngle = yawAngle - 360f;
                } else if (currentYawAngle - yawAngle > 180) {
                    destYawAngle = yawAngle + 360f;
                } else {
                    destYawAngle = yawAngle;
                }
                currentYawAngle = Mathf.Lerp(currentYawAngle, destYawAngle, Time.deltaTime * _orbitRotationDamping);
                if (currentYawAngle != 0) {
                    currentYawAngle = (currentYawAngle + 360) % 360f;
                    RotateAround(pivot, cursorPosWS, -cam.transform.forward, currentYawAngle);
                }
            }


            if (_fitViewportToScreen && renderViewportIs3DViewport) {

                Vector3 toViewCameraDir = cameraPivot.transform.forward;

                if (_orbitKeepZoomDistance && fittedPreviousCameraDistance == lastDistanceFromCamera) {
                    cameraPivot.transform.position = cursorPosWS - toViewCameraDir * fittedViewCameraDistance;
                } else {

                    float maxAllowedDistance = fittedViewCameraDistance;
                    if (fittedPreviousCameraDistance < lastDistanceFromCamera || fittedViewCameraDistance == 0) {
                        maxAllowedDistance = pivotDistance;
                    }
                    fittedPreviousCameraDistance = lastDistanceFromCamera;

                    Transform viewportTransform = renderViewport.transform;
                    viewportCorners[0] = viewportTransform.TransformPoint(-0.5f, -0.5f, 0);
                    viewportCorners[1] = viewportTransform.TransformPoint(-0.5f, 0.5f, 0);
                    viewportCorners[2] = viewportTransform.TransformPoint(0.5f, 0.5f, 0);
                    viewportCorners[3] = viewportTransform.TransformPoint(0.5f, -0.5f, 0);

                    float maxCameraViewDistance = maxAllowedDistance;
                    float minCameraViewDistance = _minCameraDistanceToViewport;

                    float prevDistance = float.MaxValue;
                    float fittedDistance = maxCameraViewDistance;
                    for (int k = 0; k < 64; k++) {
                        cameraPivot.transform.position = cursorPosWS - toViewCameraDir * fittedDistance;

                        for (int j = 0; j < 4; j++) {
                            viewportNDC[j] = GetViewportNDC(viewportCorners[j]);
                        }
                        viewportNDCRegion.UpdatePointsAndRect(viewportNDC);

                        if (viewportNDCRegion.Contains(Misc.Vector2zero) && viewportNDCRegion.Contains(Misc.Vector2up) && viewportNDCRegion.Contains(Misc.Vector2one) && viewportNDCRegion.Contains(Misc.Vector2right)) {
                            minCameraViewDistance = fittedDistance;
                        } else {
                            maxCameraViewDistance = fittedDistance;
                        }
                        if (Mathf.Abs(fittedDistance - prevDistance) < 1f) break;
                        prevDistance = fittedDistance;
                        fittedDistance = (maxCameraViewDistance + minCameraViewDistance) * 0.5f;
                    }
                    fittedViewCameraDistance = fittedDistance;
                }
            }


            Vector3 newViewCameraPosition = Vector3.Lerp(prevCameraPivotPos, cameraPivot.transform.position, Time.deltaTime * 60f * (1.01f - _cameraTiltSmoothing));
            cameraPivot.transform.position = newViewCameraPosition;
            cameraPivot.transform.LookAt(cursorPosWS, cameraPivot.transform.up);
        }

        Vector2 GetViewportNDC(Vector3 wpos) {
            Vector3 vpos = viewCamera.WorldToViewportPoint(wpos);
            if (vpos.z < 0) vpos *= -1f;
            return vpos;
        }




    }

}