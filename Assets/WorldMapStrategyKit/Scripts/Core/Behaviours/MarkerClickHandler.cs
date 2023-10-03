using UnityEngine;
using System;


namespace WorldMapStrategyKit {

    public delegate void OnMarkerPointerClickEvent(MarkerClickHandler marker, int buttonIndex);
    public delegate void OnMarkerEvent(MarkerClickHandler marker);

    public class MarkerClickHandler : MonoBehaviour {

        public OnMarkerPointerClickEvent OnMarkerMouseDown;
        public OnMarkerPointerClickEvent OnMarkerMouseUp;
        public OnMarkerEvent OnMarkerMouseEnter;
        public OnMarkerEvent OnMarkerMouseExit;
        public OnMarkerEvent OnMarkerDragStart;
        public OnMarkerEvent OnMarkerDragEnd;
        public WMSK map;
        public bool respectOtherUI;
        public bool captureClickEvents = true;
        public bool allowDrag = true;

        public bool isDragging => dragState == DragState.Dragging;

        [NonSerialized]
        public bool isMouseOver;

        [NonSerialized]
        public Vector3 dragStartClick;

        [NonSerialized]
        public Vector3 dragStartPosition;

        bool wasInside;

        enum DragState {
            None,
            CanStartDrag,
            Dragging
        }
        DragState dragState;
        public static GameObject draggingObject;
        Renderer markerRenderer;
        
        void Start() {
            // Get a reference to the World Map API:
            if (map == null) {
                map = WMSK.instance;
            }
            markerRenderer = GetComponentInChildren<Renderer>();
            wasInside = RectContainsPointer();
        }


        void Update() {
            bool leftButtonPressed = map.input.GetMouseButtonDown(0);
            bool rightButtonPressed = map.input.GetMouseButtonDown(1);
            bool leftButtonReleased = map.input.GetMouseButtonUp(0);
            bool rightButtonReleased = map.input.GetMouseButtonUp(1);
            bool checkEnterExit = OnMarkerMouseEnter != null || OnMarkerMouseExit != null;
            if (checkEnterExit || leftButtonPressed || rightButtonPressed || leftButtonReleased || rightButtonReleased) {
                // Check if cursor location is inside marker rect
                isMouseOver = RectContainsPointer();
                if (isMouseOver) {
                    if (leftButtonPressed) {
                        if (captureClickEvents) { map.FlyToCancel(); map.IgnoreClickEvents(); }
                        if (OnMarkerMouseDown != null) OnMarkerMouseDown(this, 0);
                        if (draggingObject==null) {
                            draggingObject = gameObject;
                            dragStartClick = map.cursorLocation;
                            dragState = DragState.CanStartDrag;
                        }
                    }
                    if (rightButtonPressed) {
                        if (captureClickEvents) { map.FlyToCancel(); map.IgnoreClickEvents(); }
                        if (OnMarkerMouseDown != null) OnMarkerMouseDown(this, 1);
                    }
                    if (rightButtonReleased) {
                        if (captureClickEvents) { map.FlyToCancel(); map.IgnoreClickEvents(); }
                        if (OnMarkerMouseUp != null) OnMarkerMouseUp(this, 1);
                    }
                    if (!wasInside) {
                        if (OnMarkerMouseEnter != null) OnMarkerMouseEnter(this);
                    }
                } else {
                    if (wasInside) {
                        if (OnMarkerMouseExit != null) OnMarkerMouseExit(this);
                    }
                }
                wasInside = isMouseOver;

                // release left button
                if (leftButtonReleased && (isMouseOver || dragState == DragState.Dragging)) { 
                    if (captureClickEvents) { map.FlyToCancel(); map.IgnoreClickEvents(); }
                    if (dragState == DragState.CanStartDrag) {
                        draggingObject = null;
                        dragState = DragState.None;
                    }
                    if (dragState != DragState.Dragging) {
                        if (OnMarkerMouseUp != null) OnMarkerMouseUp(this, 0);
                    } else {
                        EndDrag();
                    }
                }
            }

            if (allowDrag) {
                Vector3 delta = map.cursorLocation - dragStartClick;
                if (dragState == DragState.Dragging) {
                    transform.localPosition = dragStartPosition + delta;
                } else if (dragState == DragState.CanStartDrag && delta.sqrMagnitude > 0) {
                    StartDrag();
                }
            }
        }

        bool RectContainsPointer() {
            // Check if cursor location is inside marker rect
            if (map == null)
                return false;
            Vector3 cursorLocation;
            if (respectOtherUI) {
                cursorLocation = map.cursorLocation;
            } else {
                map.GetLocalHitFromScreenPos(map.input.mousePosition, out cursorLocation, false);
            }
            Vector3 size;
            if (markerRenderer != null) {
                size = markerRenderer.bounds.size;
                size.x /= map.transform.localScale.x;
                size.y /= map.transform.localScale.y;
            } else {
                size = transform.localScale;
            }

            Rect rect = new Rect(transform.localPosition - size * 0.5f, size);
            return rect.Contains(cursorLocation);
        }

        void StartDrag() {
            dragStartPosition = transform.localPosition;
            dragState = DragState.Dragging;
            if (OnMarkerDragStart != null) OnMarkerDragStart(this);
        }

        void EndDrag() {
            if (dragState == DragState.Dragging) {
                dragState = DragState.None;
                draggingObject = null;
                if (OnMarkerDragEnd != null) OnMarkerDragEnd(this);
            }
        }
    }

}

