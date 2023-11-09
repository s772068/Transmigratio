// World Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;

namespace WorldMapStrategyKit {

    public delegate void OnMouseClick(float x, float y, int buttonIndex);
    public delegate void OnMouseEvent(float x, float y);
    public delegate void OnRectangleSelection(Rect rectangle, bool finishedSelection);
    public delegate void OnSimpleMapEvent();

    public enum UNIT_SELECTION_MODE {
        Disabled = 0,
        Single = 10
    }

    public partial class WMSK : MonoBehaviour {
        #region Public properties

        /// <summary>
        /// Raised when user clicks on the map using left mouse button. Returns x/y local space coordinates (-0.5, 0.5)
        /// </summary>
        public event OnMouseClick OnClick;

        /// <summary>
        /// Occurs when mouse moves over the map.
        /// </summary>
        public event OnMouseEvent OnMouseMove;

        /// <summary>
        /// Occurs when mouse button is pressed on the map.
        /// </summary>
        public event OnMouseClick OnMouseDown;

        /// <summary>
        /// Occurs when mouse button is released on the map.
        /// </summary>
        public event OnMouseClick OnMouseRelease;

        /// <summary>
        /// Occurs when user start dragging on the map
        /// </summary>
        public event OnSimpleMapEvent OnDragStart;

        /// <summary>
        /// Occurs when user ends dragging on the map (release mouse button after a drag) 
        /// </summary>
        public event OnSimpleMapEvent OnDragEnd;

        /// <summary>
        /// Occurs when a FlyTo command is started
        /// </summary>
		public event OnSimpleMapEvent OnFlyStart;

        /// <summary>
        /// Occurs when a FlyTo command has reached destination
        /// </summary>
        public event OnSimpleMapEvent OnFlyEnd;

        /// <summary>
        /// Returns true is mouse has entered the Earth's collider.
        /// </summary>
        [NonSerialized]
        public bool
            mouseIsOver;

        /// <summary>
        /// Returns true is mouse is over an Unity UI element (button, label, ...)
        /// </summary>
        [NonSerialized]
        public bool
            mouseIsOverUIElement;


        /// <summary>
        /// The navigation time in seconds.
        /// </summary>
        [SerializeField]
        [Range(1.0f, 16.0f)]
        float
            _navigationTime = 4.0f;

        public float navigationTime {
            get {
                return _navigationTime;
            }
            set {
                if (_navigationTime != value) {
                    _navigationTime = value;
                    isDirty = true;
                }
            }
        }

        /// <summary>
        /// Returns whether a navigation is taking place at this moment.
        /// </summary>
        public bool isFlying { get { return flyToActive; } }


        [SerializeField]
        bool _allowInteractionWhileFlying;

        /// <summary>
        /// Gets / sets flag to allow user interaction with map during a FlyTo operation
        /// </summary>
        public bool allowInteractionWhileFlying {
            get { return _allowInteractionWhileFlying; }
            set {
                if (_allowInteractionWhileFlying != value) {
                    _allowInteractionWhileFlying = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _staticInteractionVelocityThreshold = 0.1f;

        /// <summary>
        /// Gets / sets the maximum velocity for the map or camera to consider it static
        /// </summary>
        public float staticInteractionVelocityThreshold {
            get { return _staticInteractionVelocityThreshold; }
            set {
                if (_staticInteractionVelocityThreshold != value) {
                    _staticInteractionVelocityThreshold = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool
            _fitWindowWidth = true;

        /// <summary>
        /// Ensure the map is always visible and occupy the entire Window.
        /// </summary>
        public bool fitWindowWidth {
            get {
                return _fitWindowWidth;
            }
            set {
                if (value != _fitWindowWidth) {
                    _fitWindowWidth = value;
                    isDirty = true;
                    if (_fitWindowWidth) {
                        CenterMap();
                        _wrapHorizontally = false;
                    } else if (!_fitWindowHeight)
                        maxFrustumDistance = float.MaxValue;
                }
            }
        }

        [SerializeField]
        bool
            _fitWindowHeight = true;

        /// <summary>
        /// Ensure the map is always visible and occupy the entire Window.
        /// </summary>
        public bool fitWindowHeight {
            get {
                return _fitWindowHeight;
            }
            set {
                if (value != _fitWindowHeight) {
                    _fitWindowHeight = value;
                    isDirty = true;
                    if (_fitWindowHeight)
                        CenterMap();
                    else if (!_fitWindowWidth)
                        maxFrustumDistance = float.MaxValue;
                }
            }
        }


        public float fitWindowHeightLimitTop = 1f;
        public float fitWindowHeightLimitBottom = 0f;
        public float fitWindowWidthLimitRight = 1f;
        public float fitWindowWidthLimitLeft = 0f;


        [SerializeField]
        Vector2
        _flyToScreenCenter = Misc.ViewportCenter;

        /// <summary>
        /// Sets the position of the screen used by the FlyTo() operations
        /// </summary>
        public Vector2 flyToScreenCenter {
            get {
                return _flyToScreenCenter;
            }
            set {
                if (value != _flyToScreenCenter) {
                    value.x = Mathf.Clamp01(value.x);
                    value.y = Mathf.Clamp01(value.y);
                    _flyToScreenCenter = value;
                    SetDestinationAndDistance(_cursorLocation, 0, GetFrustumDistance());
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        bool _wrapHorizontally = false;

        /// <summary>
        /// Allows to scroll around horizontal edges.
        /// </summary>
        public bool wrapHorizontally {
            get {
                return _wrapHorizontally;
            }
            set {
                if (value != _wrapHorizontally) {
                    _wrapHorizontally = value;
                    isDirty = true;
                    _fitWindowWidth = !_wrapHorizontally;
                    SetupViewport();
                    if (_showGrid) {
                        GenerateGrid(); // need to refresh grid mesh
                    }
                    if (!_wrapHorizontally) {
                        CenterMap();
                    }
                }
            }
        }


        [SerializeField]
        Rect _windowRect = new Rect(-0.5f, -0.5f, 1, 1);

        /// <summary>
        /// The playable area. By default it's a Rect(-0.5, -0.5, 1f, 1f) where x,y are the -0.5, -0.5 is the left/bottom corner, and 1,1 is the width/height. Use renderViewportRect to get the current map viewable area in the window.
        /// </summary>
        /// <value>The window rect.</value>
        public Rect windowRect {
            get {
                return _windowRect;
            }
            set {
                if (value != _windowRect) {
                    _windowRect = value;
                    fitWindowHeight = true;
                    fitWindowWidth = true;
                    isDirty = true;
                    CenterMap();

                    
                }
            }
        }

        [SerializeField]
        bool
            _allowUserKeys = false;

        /// <summary>
        /// If user can use WASD keys to drag the map.
        /// </summary>
        public bool allowUserKeys {
            get { return _allowUserKeys; }
            set {
                if (value != _allowUserKeys) {
                    _allowUserKeys = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _dragKeySpeedMultiplier = 0.1f;

        public float dragKeySpeedMultiplier {
            get { return _dragKeySpeedMultiplier; }
            set {
                if (value != dragKeySpeedMultiplier) {
                    _dragKeySpeedMultiplier = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        string
            _keyUpName = "w";

        /// <summary>
        /// Keyboard mapping for up shift.
        /// </summary>
        public string keyUpName {
            get { return _keyUpName; }
            set {
                if (value != _keyUpName) {
                    _keyUpName = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        string
            _keyDownName = "s";

        /// <summary>
        /// Keyboard mapping for down shift.
        /// </summary>
        public string keyDownName {
            get { return _keyDownName; }
            set {
                if (value != _keyDownName) {
                    _keyDownName = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        string
            _keyLeftName = "a";

        /// <summary>
        /// Keyboard mapping for left shift.
        /// </summary>
        public string keyLeftName {
            get { return _keyLeftName; }
            set {
                if (value != _keyLeftName) {
                    _keyLeftName = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        string
            _keyRightName = "d";

        /// <summary>
        /// Keyboard mapping for right shift.
        /// </summary>
        public string keyRightName {
            get { return _keyRightName; }
            set {
                if (value != _keyRightName) {
                    _keyRightName = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool
            _dragFlipDirection;

        /// <summary>
        /// Whether the direction of the drag should be inverted.
        /// </summary>
        public bool dragFlipDirection {
            get { return _dragFlipDirection; }
            set {
                if (value != _dragFlipDirection) {
                    _dragFlipDirection = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        bool
            _dragConstantSpeed;

        /// <summary>
        /// Whether the drag should follow a constant movement, withouth acceleration.
        /// </summary>
        public bool dragConstantSpeed {
            get { return _dragConstantSpeed; }
            set {
                if (value != _dragConstantSpeed) {
                    _dragConstantSpeed = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _dragDampingDuration = 0.5f;

        public float dragDampingDuration {
            get { return _dragDampingDuration; }
            set {
                if (value != _dragDampingDuration) {
                    _dragDampingDuration = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool
            _allowUserDrag = true;

        public bool allowUserDrag {
            get { return _allowUserDrag; }
            set {
                if (value != _allowUserDrag) {
                    _allowUserDrag = value;
                    dragDirection = Misc.Vector3zero;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        bool
            _allowScrollOnScreenEdges;

        public bool allowScrollOnScreenEdges {
            get { return _allowScrollOnScreenEdges; }
            set {
                if (value != _allowScrollOnScreenEdges) {
                    _allowScrollOnScreenEdges = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        int
            _screenEdgeThickness = 2;

        public int screenEdgeThickness {
            get { return _screenEdgeThickness; }
            set {
                if (value != _screenEdgeThickness) {
                    _screenEdgeThickness = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        bool
            _centerOnRightClick = true;

        public bool centerOnRightClick {
            get { return _centerOnRightClick; }
            set {
                if (value != _centerOnRightClick) {
                    _centerOnRightClick = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        LayerMask
            _blockingMask = -1;

        /// <summary>
        /// Layer mask to determine which UI elements can block the interaction
        /// </summary>
        public LayerMask blockingMask {
            get { return _blockingMask; }
            set {
                if (value != _blockingMask) {
                    _blockingMask = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool
            _allowUserZoom = true;

        public bool allowUserZoom {
            get { return _allowUserZoom; }
            set {
                if (value != _allowUserZoom) {
                    _allowUserZoom = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _zoomMaxDistance = 10f;

        public float zoomMaxDistance {
            get { return _zoomMaxDistance; }
            set {
                if (value != _zoomMaxDistance) {
                    _zoomMaxDistance = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _zoomMinDistance = 0.01f;

        public float zoomMinDistance {
            get { return _zoomMinDistance; }
            set {
                if (value != _zoomMinDistance) {
                    _zoomMinDistance = Mathf.Clamp01(value);
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        bool
            _invertZoomDirection;

        public bool invertZoomDirection {
            get { return _invertZoomDirection; }
            set {
                if (value != _invertZoomDirection) {
                    _invertZoomDirection = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _zoomDampingDuration = 0.5f;

        public float zoomDampingDuration {
            get { return _zoomDampingDuration; }
            set {
                if (value != _zoomDampingDuration) {
                    _zoomDampingDuration = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        bool
            _respectOtherUI = true;

        /// <summary>
        /// When enabled, will prevent interaction with the map if pointer is over an UI element
        /// </summary>
        public bool respectOtherUI {
            get { return _respectOtherUI; }
            set {
                if (value != _respectOtherUI) {
                    _respectOtherUI = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        bool _enableFreeCamera;

        /// <summary>
        /// Allow camera to be freely moved/rotated when in terrain mode
        /// </summary>
        public bool enableFreeCamera {
            get { return _enableFreeCamera; }
            set {
                if (value != _enableFreeCamera) {
                    _enableFreeCamera = value;
                    if (!_enableFreeCamera && renderViewportIsTerrain) {
                        Vector3 mapPos = Misc.Vector3zero;
                        GetCurrentMapLocation(out mapPos);
                        FlyToLocation(mapPos, 0.8f);    // reset view
                    }
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        [Range(0.1f, 3)]
        float
            _mouseWheelSensitivity = 0.5f;

        public float mouseWheelSensitivity {
            get { return _mouseWheelSensitivity; }
            set {
                if (value != _mouseWheelSensitivity) {
                    _mouseWheelSensitivity = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        [Range(0.1f, 3)]
        float
            _mouseDragSensitivity = 0.5f;

        public float mouseDragSensitivity {
            get { return _mouseDragSensitivity; }
            set {
                if (value != _mouseDragSensitivity) {
                    _mouseDragSensitivity = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float
            _mouseDragThreshold;

        public float mouseDragThreshold {
            get { return _mouseDragThreshold; }
            set {
                if (_mouseDragThreshold != value) {
                    _mouseDragThreshold = value;
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        bool _enableCameraTilt;

        /// <summary>
        /// Tilts camera depending on zoom
        /// </summary>
        public bool enableCameraTilt {
            get {
                return _enableCameraTilt;
            }
            set {
                if (value != _enableCameraTilt) {
                    _enableCameraTilt = value;
                    SetupCameraTiltMode();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _cameraNearTiltAngle = -30f;

        /// <summary>
        /// Minimum tilt for camera
        /// </summary>
        public float cameraNearTiltAngle {
            get {
                return _cameraNearTiltAngle;
            }
            set {
                if (value != _cameraNearTiltAngle) {
                    _cameraNearTiltAngle = value;
                    SetupCameraTiltMode();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _cameraFarTiltAngle;

        /// <summary>
        /// Minimum tilt for camera
        /// </summary>
        public float cameraFarTiltAngle {
            get {
                return _cameraFarTiltAngle;
            }
            set {
                if (value != _cameraFarTiltAngle) {
                    _cameraFarTiltAngle = value;
                    SetupCameraTiltMode();
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        float _cameraTiltSmoothing = 0.2f;

        /// <summary>
        /// Smoothing for camera tilt movement
        /// </summary>
        public float cameraTiltSmoothing {
            get {
                return _cameraTiltSmoothing;
            }
            set {
                if (value != _cameraTiltSmoothing) {
                    _cameraTiltSmoothing = value;
                    isDirty = true;
                }
            }
        }


        

        [SerializeField]
        bool _fitViewportToScreen;

        /// <summary>
        /// Ensures viewport fills the screen
        /// </summary>
        public bool fitViewportToScreen {
            get {
                return _fitViewportToScreen;
            }
            set {
                if (value != _fitViewportToScreen) {
                    _fitViewportToScreen = value;
                    SetupCameraTiltMode();
                    isDirty = true;
                }
            }
        }




        [SerializeField]
        float _minCameraDistanceToViewport = 20f;

        /// <summary>
        /// The minimum distance of the camera to the viewport
        /// </summary>
        public float minCameraDistanceToViewport {
            get {
                return _minCameraDistanceToViewport;
            }
            set {
                if (value != _minCameraDistanceToViewport) {
                    _minCameraDistanceToViewport = value;
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        bool _enableCameraOrbit;

        /// <summary>
        /// Allow rotation of camera around target point when in tilted mode
        /// </summary>
        public bool enableCameraOrbit {
            get {
                return _enableCameraOrbit;
            }
            set {
                if (value != _enableCameraOrbit) {
                    _enableCameraOrbit = value;
                    if (!_enableCameraOrbit) yawAngle = 0;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        [Range(0, 1)]
        float _orbitMaxZoomLevel = 0.5f;

        /// <summary>
        /// Maximum zoom level allowed for orbiting
        /// </summary>
        public float orbitMaxZoomLevel {
            get {
                return _orbitMaxZoomLevel;
            }
            set {
                if (value != _orbitMaxZoomLevel) {
                    _orbitMaxZoomLevel = value;
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        string _orbitLeftKeyName = "q";

        /// <summary>
        /// Key used to orbit counter-clockwise
        /// </summary>
        public string orbitLeftKeyName {
            get {
                return _orbitLeftKeyName;
            }
            set {
                if (value != _orbitLeftKeyName) {
                    _orbitLeftKeyName = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        string _orbitRightKeyName = "e";

        /// <summary>
        /// Key used to orbit clockwise
        /// </summary>
        public string orbitRightKeyName {
            get {
                return _orbitRightKeyName;
            }
            set {
                if (value != _orbitRightKeyName) {
                    _orbitRightKeyName = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float
            _orbitRotationSpeed = 1f;

        public float orbitRotationSpeed {
            get { return _orbitRotationSpeed; }
            set {
                if (value != _orbitRotationSpeed) {
                    _orbitRotationSpeed = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float
            _orbitRotationDamping = 5f;

        public float orbitRotationDamping {
            get { return _orbitRotationDamping; }
            set {
                if (value != _orbitRotationDamping) {
                    _orbitRotationDamping = value;
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        bool _orbitKeepZoomDistance;

        /// <summary>
        /// Keeps current distance when rotating around target when fit viewport to screen is enabled. This prevents camera distance changes during rotation but can show borders of viewport at certain angles.
        /// </summary>
        public bool orbitKeepZoomDistance {
            get {
                return _orbitKeepZoomDistance;
            }
            set {
                if (value != _orbitKeepZoomDistance) {
                    _orbitKeepZoomDistance = value;
                    isDirty = true;
                }
            }
        }

        #endregion

        #region Public API area

        /// <summary>
        /// Moves the map in front of the camera so it fits the viewport.
        /// </summary>
        public void CenterMap() {

            if (isMiniMap)
                return;

            if (_renderViewport == null) {
                SetupViewport();
            }

            lastDistanceFromCamera = GetFrustumDistance();

            // move center to renderviewport then adjust its position from there. This avoids issues with GetCurrentMapLocation used in SetDestinationAndDistance
            currentCamera.transform.position = renderViewport.transform.position;

            SetDestinationAndDistance(Misc.Vector2zero, 0, lastDistanceFromCamera);

            CheckRectConstraints();
        }

        /// <summary>
        /// Returns the coordinates of the center of the map as it's shown on the screen
        /// </summary>
        public bool GetCurrentMapLocation(out Vector3 location, bool worldSpace = false) {
            Vector3 screenPos;
            if (renderViewportIsEnabled && !renderViewportIsTerrain) {
                screenPos = cameraMain.WorldToScreenPoint(_renderViewport.transform.position);
            } else {
                screenPos = new Vector3(cameraMain.pixelWidth / 2, cameraMain.pixelHeight / 2, 0f);
            }
            if (!GetLocalHitFromScreenPos(screenPos, out location, true)) return false;
            if (worldSpace) {
                location = Map2DToWorldPosition(location);
            }
            return true;
        }


        /// <summary>
        /// Sets the zoom level progressively
        /// </summary>
        /// <param name="zoomLevel">Value from 0 to 1 (close zoom, fit to window zoom)</param>
        /// <param name="duration">Duratin of the transition</param>
        public void SetZoomLevel(float zoomLevel, float duration) {
            if (duration == 0) {
                SetZoomLevel(zoomLevel);
            } else {
                Vector3 location;
                GetCurrentMapLocation(out location);
                FlyToLocation(location, duration, zoomLevel);
            }
        }

        /// <summary>
        /// Sets the zoom level
        /// </summary>
        /// <param name="zoomLevel">Value from 0 to 1</param>
        public void SetZoomLevel(float zoomLevel) {
            Camera cam = currentCamera;
            if (cam.orthographic) {
                float aspect = cam.aspect;
                float frustumDistanceH;
                if (_fitWindowWidth) {
                    frustumDistanceH = mapWidth * 0.5f / aspect;
                } else {
                    frustumDistanceH = mapHeight * 0.5f;
                }
                zoomLevel = Mathf.Clamp01(zoomLevel);
                cam.orthographicSize = Mathf.Max(frustumDistanceH * zoomLevel, 1);
            } else {
                // Takes the distance from the focus point and adjust it according to the zoom level
                Vector3 dest;
                if (GetLocalHitFromScreenPos(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f), out dest, true)) {
                    dest = transform.TransformPoint(dest);
                } else {
                    dest = transform.position;
                }
                float distance = GetZoomLevelDistance(zoomLevel);
                cam.transform.position = dest - (dest - cam.transform.position).normalized * distance;
                float minDistance = cam.nearClipPlane + 0.0001f;
                float camDistance = (dest - cam.transform.position).magnitude;
                // Last distance
                lastDistanceFromCamera = camDistance;
                if (camDistance < minDistance) {
                    cam.transform.position = dest - transform.forward * minDistance;
                }
            }
        }

        /// <summary>
        /// Gets the current zoom level (0..1)
        /// </summary>
        public float GetZoomLevel() {

            float distanceToCamera;
            Camera cam = currentCamera;
            if (cam == null)
                return 1;

            if (_enableFreeCamera) {
                Plane mapPlane = new Plane(transform.forward, transform.position);
                Ray ray = new Ray(cam.transform.position, cam.transform.forward);
                if (mapPlane.Raycast(ray, out distanceToCamera)) {
                    float h = transform.localScale.y;
                    return (distanceToCamera - _zoomMinDistance * h) / (_zoomMaxDistance * h - _zoomMinDistance * h);
                }
            }


            float frustumDistanceW, frustumDistanceH;
            float aspect = cam.aspect;
            if (cam.orthographic) {
                if (_fitWindowWidth) {
                    frustumDistanceH = mapWidth * 0.5f / aspect;
                } else {
                    frustumDistanceH = mapHeight * 0.5f;
                }
                return cam.orthographicSize / frustumDistanceH;
            }

            float fv = cam.fieldOfView;
            float radAngle = fv * Mathf.Deg2Rad;
            frustumDistanceH = mapHeight * 0.5f / Mathf.Tan(radAngle * 0.5f);
            frustumDistanceW = (mapWidth / aspect) * 0.5f / Mathf.Tan(radAngle * 0.5f);
            float distance;
            if (_fitWindowWidth && _fitWindowHeight) {
                distance = Mathf.Min(frustumDistanceH, frustumDistanceW);
            } else if (_fitWindowHeight) {
                distance = frustumDistanceH;
            } else {
                distance = frustumDistanceW;
            }
            // Takes the distance from the camera to the plane //focus point and adjust it according to the zoom level
            Plane plane = new Plane(transform.forward, transform.position);
            distanceToCamera = Mathf.Abs(plane.GetDistanceToPoint(cam.transform.position));
            lastKnownZoomLevel = distanceToCamera / distance;
            return lastKnownZoomLevel;
        }

        /// <summary>
        /// Starts navigation to target location in local 2D coordinates.
        /// </summary>
        public void FlyToLocation(Vector2 destination) {
            FlyToLocation(destination, _navigationTime, GetZoomLevel());
        }

        /// <summary>
        /// Starts navigation to target location in local 2D coordinates.
        /// </summary>
        public void FlyToLocation(Vector2 destination, float duration) {
            FlyToLocation(destination, duration, GetZoomLevel());
        }

        /// <summary>
        /// Starts navigation to target location in local 2D coordinates.
        /// </summary>
        public void FlyToLocation(float x, float y) {
            FlyToLocation(new Vector2(x, y), _navigationTime, GetZoomLevel());
        }

        /// <summary>
        /// Starts navigation to target location in local 2D coordinates.
        /// </summary>
        public void FlyToLocation(float x, float y, float duration) {
            SetDestination(new Vector2(x, y), duration, GetZoomLevel());
        }

        /// <summary>
        /// Starts navigation to target location in local 2D coordinates with target zoom level.
        /// </summary>
        public void FlyToLocation(Vector2 destination, float duration, float zoomLevel) {
            SetDestination(destination, duration, zoomLevel);
        }


        /// <summary>
        /// Starts navigation to target lat/lon.
        /// </summary>
        /// <param name="latlon">Latitude (x) and Longitude (y).</param>
        public void FlyToLatLon(Vector2 latlon, float duration, float zoomLevel) {
            FlyToLatLon(latlon.x, latlon.y, duration, zoomLevel);
        }

        /// <summary>
        /// Starts navigation to target lat/lon.
        /// </summary>
        /// <param name="latitude">Latitude.</param>
        /// <param name="longitude">Longitude.</param>
        public void FlyToLatLon(float latitude, float longitude, float duration, float zoomLevel) {
            Vector2 location = Conversion.GetLocalPositionFromLatLon(latitude, longitude);
            FlyToLocation(location, duration, zoomLevel);
        }

        /// <summary>
        /// Stops any navigation in progress
        /// </summary>
        public void FlyToCancel() {
            flyToActive = false;
        }

        /// <summary>
        /// Initiates a rectangle selection operation.
        /// </summary>
        /// <returns>The rectangle selection.</returns>
        public GameObject RectangleSelectionInitiate(OnRectangleSelection rectangleSelectionCallback, Color rectangleFillColor, Color rectangleColor, float lineWidth = 0.02f) {
            RectangleSelectionCancel();
            GameObject rectangle = GameObject.CreatePrimitive(PrimitiveType.Quad);
            if (rectangleSelectionMat == null) {
                rectangleSelectionMat = Instantiate(Resources.Load<Material>("WMSK/Materials/hudRectangleSelection")) as Material;
                if (disposalManager != null)
                    disposalManager.MarkForDisposal(rectangleSelectionMat);
            }
            rectangleSelectionMat.color = rectangleFillColor;
            rectangle.GetComponent<Renderer>().sharedMaterial = rectangleSelectionMat;
            AddMarker2DSprite(rectangle, _cursorLocation, 0f);
            RectangleSelection rs = rectangle.AddComponent<RectangleSelection>();
            currentRectangleSelection = rs;
            rs.map = this;
            rs.callback = rectangleSelectionCallback;
            rs.lineColor = rectangleColor;
            rs.lineWidth = lineWidth;
            if (input.GetMouseButton(0)) {
                rs.InitiateSelection(lastMouseMapLocalHitPos.x, lastMouseMapLocalHitPos.y, 0);
            }
            return rectangle;
        }

        /// <summary>
        /// Cancel any rectangle selection operation in progress
        /// </summary>
        public void RectangleSelectionCancel() {
            if (currentRectangleSelection != null) {
                DestroyImmediate(currentRectangleSelection.gameObject);
            }
        }

        /// <summary>
        /// Returns true if a rectangle selection is occuring
        /// </summary>
        public bool rectangleSelectionInProgress => currentRectangleSelection != null;


        /// <summary>
        /// Makes WMSK ignore any click event for a number of given frames (default = 1)
        /// </summary>
        public void IgnoreClickEvents(int numberOfFrames = 1) {
            ignoreClickEventFrame = Time.frameCount + numberOfFrames;
        }


        /// <summary>
        /// The actual input system. Check the documentation for details about how to change the input system. WMSK will use the legacy or new input system automatically but you can assign your own input manager here.
        /// </summary>
        public IInputProxy input;

        #endregion

    }

}