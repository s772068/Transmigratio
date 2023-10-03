// World Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WorldMapStrategyKit {

    public partial class WMSK : MonoBehaviour {

        #region Public properties

        [SerializeField]
        bool
            _showCursor = true;

        /// <summary>
        /// Toggle cursor lines visibility.
        /// </summary>
        public bool showCursor {
            get {
                return _showCursor;
            }
            set {
                if (value != _showCursor) {
                    _showCursor = value;
                    isDirty = true;

                    if (cursorLayerVLine != null) {
                        cursorLayerVLine.SetActive(_showCursor);
                    }
                    if (cursorLayerHLine != null) {
                        cursorLayerHLine.SetActive(_showCursor);
                    }
                }
            }
        }

        /// <summary>
        /// Cursor lines color.
        /// </summary>
        [SerializeField]
        Color
            _cursorColor = new Color(0.56f, 0.47f, 0.68f);

        public Color cursorColor {
            get {
                if (cursorMatH != null) {
                    return cursorMatH.color;
                } else {
                    return _cursorColor;
                }
            }
            set {
                if (value != _cursorColor) {
                    _cursorColor = value;
                    isDirty = true;

                    if (cursorMatH != null && _cursorColor != cursorMatH.color) {
                        cursorMatH.color = _cursorColor;
                    }
                    if (cursorMatV != null && _cursorColor != cursorMatV.color) {
                        cursorMatV.color = _cursorColor;
                    }
                }
            }
        }

        [SerializeField]
        bool
            _cursorFollowMouse = true;

        /// <summary>
        /// Makes the cursor follow the mouse when it's over the World.
        /// </summary>
        public bool cursorFollowMouse {
            get {
                return _cursorFollowMouse;
            }
            set {
                if (value != _cursorFollowMouse) {
                    _cursorFollowMouse = value;
                    isDirty = true;
                }
            }
        }

        Vector3
            _cursorLocation;

        public Vector3 cursorLocation {
            get {
                return _cursorLocation;
            }
            set {
                if (_cursorLocation.x != value.x || _cursorLocation.z != value.z || _cursorLocation.y != value.y) {
                    _cursorLocation = value;
                    if (cursorLayerVLine != null) {
                        Vector3 pos = cursorLayerVLine.transform.localPosition;
                        cursorLayerVLine.transform.localPosition = new Vector3(_cursorLocation.x, 0, pos.z);
                    }
                    if (cursorLayerHLine != null) {
                        Vector3 pos = cursorLayerHLine.transform.localPosition;
                        cursorLayerHLine.transform.localPosition = new Vector3(0, _cursorLocation.y, pos.z);
                    }
                }
            }
        }


        /// <summary>
        /// If set to false, cursor will be hidden when mouse if not over the map.
        /// </summary>
        [SerializeField]
        bool
            _cursorAllwaysVisible = true;

        public bool cursorAlwaysVisible {
            get {
                return _cursorAllwaysVisible;
            }
            set {
                if (value != _cursorAllwaysVisible) {
                    _cursorAllwaysVisible = value;
                    isDirty = true;
                    CheckCursorVisibility();
                }
            }
        }


        [SerializeField]
        bool
            _showLatitudeLines = true;

        /// <summary>
        /// Toggle latitude lines visibility.
        /// </summary>
        public bool showLatitudeLines {
            get {
                return _showLatitudeLines;
            }
            set {
                if (value != _showLatitudeLines) {
                    _showLatitudeLines = value;
                    isDirty = true;

                    if (latitudeLayer != null) {
                        latitudeLayer.SetActive(_showLatitudeLines);
                    } else if (_showLatitudeLines) {
                        DrawLatitudeLines();
                    }
                    if (_showLatitudeLines) {
                        showGrid = false;
                    }
                }
            }
        }

        [SerializeField]
        [Range(5.0f, 45.0f)]
        int
            _latitudeStepping = 15;

        /// <summary>
        /// Specify latitude lines separation.
        /// </summary>
        public int latitudeStepping {
            get {
                return _latitudeStepping;
            }
            set {
                if (value != _latitudeStepping) {
                    _latitudeStepping = value;
                    isDirty = true;

                    if (gameObject.activeInHierarchy)
                        DrawLatitudeLines();
                }
            }
        }

        [SerializeField]
        bool
            _showLongitudeLines = true;

        /// <summary>
        /// Toggle longitude lines visibility.
        /// </summary>
        public bool showLongitudeLines {
            get {
                return _showLongitudeLines;
            }
            set {
                if (value != _showLongitudeLines) {
                    _showLongitudeLines = value;
                    isDirty = true;

                    if (longitudeLayer != null) {
                        longitudeLayer.SetActive(_showLongitudeLines);
                    } else if (_showLongitudeLines) {
                        DrawLongitudeLines();
                    }
                    if (_showLongitudeLines) {
                        showGrid = false;
                    }
                }
            }
        }

        [SerializeField]
        [Range(5.0f, 45.0f)]
        int
            _longitudeStepping = 15;

        /// <summary>
        /// Specify longitude lines separation.
        /// </summary>
        public int longitudeStepping {
            get {
                return _longitudeStepping;
            }
            set {
                if (value != _longitudeStepping) {
                    _longitudeStepping = value;
                    isDirty = true;

                    if (gameObject.activeInHierarchy)
                        DrawLongitudeLines();
                }
            }
        }

        /// <summary>
        /// Color for imaginary lines (longitude and latitude).
        /// </summary>
        [SerializeField]
        Color
            _imaginaryLinesColor = new Color(0.16f, 0.33f, 0.498f);

        public Color imaginaryLinesColor {
            get {
                if (imaginaryLinesMat != null) {
                    return imaginaryLinesMat.color;
                } else {
                    return _imaginaryLinesColor;
                }
            }
            set {
                if (value != _imaginaryLinesColor) {
                    _imaginaryLinesColor = value;
                    isDirty = true;

                    if (imaginaryLinesMat != null && _imaginaryLinesColor != imaginaryLinesMat.color) {
                        imaginaryLinesMat.color = _imaginaryLinesColor;
                    }
                }
            }
        }

        #endregion

        #region Public API area

        /// <summary>
        /// Adds a custom marker (sprite) to the map on specified location and with custom scale.
        /// </summary>
        /// <param name="sprite">Sprite gameObject.</param>
        /// <param name="planeLocation">Plane location.</param>
        /// <param name="scale">Scale.</param>
        /// <param name="enableEvents">If set to <c>true</c>, a MarkerClickHandler script will be attached to the sprite gameObject. You can use the OnMarkerClick field to hook your mouse click handler.</param>
        /// <param name="autoScale">If set to <c>true></c>, the scale of this marker will be modified according to zoom distance to preserve screen size.</param>
        public void AddMarker2DSprite(GameObject sprite, Vector3 planeLocation, float scale, bool enableEvents = false, bool autoScale = false) {
            AddMarker2DSprite(sprite, planeLocation, new Vector2(scale, scale * mapWidth / mapHeight), enableEvents, autoScale);
        }

        /// <summary>
        /// Adds a custom marker (sprite) to the map on specified location and with custom scale.
        /// </summary>
        /// <param name="sprite">Sprite.</param>
        /// <param name="planeLocation">Plane location.</param>
        /// <param name="scale">Scale for x and y axis.</param>
        /// <param name="enableEvents">If set to <c>true</c>, a MarkerClickHandler script will be attached to the sprite gameObject. You can use the OnMarkerClick field to hook your mouse click handler.</param>
        /// <param name="autoScale">If set to <c>true></c>, the scale of this marker will be modified according to zoom distance to preserve screen size.</param>
        public void AddMarker2DSprite(GameObject sprite, Vector3 planeLocation, Vector2 scale, bool enableEvents = false, bool autoScale = false) {

            if (sprite == null)
                return;

            CheckMarkersLayer();

            sprite.transform.SetParent(markersLayer.transform, false);
            sprite.transform.localPosition = planeLocation + Misc.Vector3back * 0.01f;
            sprite.transform.localRotation = Quaternion.Euler(0, 0, 0);
            sprite.layer = gameObject.layer;
            sprite.transform.localScale = new Vector3(scale.x, scale.y, 1f);

            if (renderViewportIsEnabled) {
                SetGameObjectLayer(sprite);
            }

            if (enableEvents) {
                if (GetComponent<MarkerClickHandler>() == null) {
                    sprite.AddComponent<MarkerClickHandler>().map = this;
                }
            }

            if (autoScale) {
                if (GetComponent<MarkerAutoScale>() == null) {
                    sprite.AddComponent<MarkerAutoScale>();
                }
            }
        }

        void SetGameObjectLayer(GameObject o) {
            Renderer[] rr = o.GetComponentsInChildren<Renderer>(true);
            for (int k = 0; k < rr.Length; k++) {
                rr[k].gameObject.layer = gameObject.layer;
            }
        }


        /// <summary>
        /// Adds a custom text to the map on specified location and with custom scale.
        /// </summary>
        public TextMesh AddMarker2DText(string text, Vector3 planeLocation) {
            CheckMarkersLayer();

            GameObject textObj = new GameObject(text);
            textObj.transform.SetParent(markersLayer.transform, false);
            textObj.transform.localPosition = planeLocation + Misc.Vector3back * 0.01f;
            textObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            textObj.transform.localScale = new Vector3(0.01f / mapWidth, 0.01f / mapHeight, 1f);
            textObj.layer = gameObject.layer;
            if (renderViewportIsEnabled) {
                textObj.layer = gameObject.layer;
            }
            TextMesh tm = textObj.AddComponent<TextMesh>();
            tm.text = text;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.fontSize = (int)mapHeight * 10;
            tm.alignment = TextAlignment.Center;
            return tm;
        }

        /// <summary>
        /// Adds a custom marker (gameobject) to the map on specified location and with custom scale multiplier.
        /// </summary>
        public void AddMarker3DObject(GameObject marker, Vector3 planeLocation, float scale = 1f) {
            AddMarker3DObject(marker, planeLocation, marker.transform.localScale * scale);
        }

        /// <summary>
        /// Adds a custom marker (gameobject) to the map on specified location and with custom scale.
        /// </summary>
        public void AddMarker3DObject(GameObject marker, Vector3 planeLocation, Vector3 scale, float pivotY = 0.5f) {
            // Try to get the height of the object
            float height = 0;
            if (marker.GetComponent<MeshFilter>() != null)
                height = marker.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
            else if (marker.GetComponent<Collider>() != null)
                height = marker.GetComponent<Collider>().bounds.size.y;

            float h = height * scale.y; // lift the marker so it appears on the surface of the map

            CheckMarkersLayer();
            SetGameObjectLayer(marker);

            marker.transform.rotation = transform.rotation * Quaternion.Euler(-90, 0, 0) * marker.transform.localRotation;
            marker.transform.localScale = scale;

            marker.transform.SetParent(markersLayer.transform, true);
            marker.transform.localPosition = planeLocation + Misc.Vector3back * h * pivotY;
        }


        /// <summary>
        /// Updates a custom marker (gameobject) position preserving scale and height. Can be used after calling AddMarker3DObject to move units over the 2D map.
        /// </summary>
        public void UpdateMarker3DObjectPosition(GameObject marker, Vector3 planeLocation) {
            marker.transform.localPosition = new Vector3(planeLocation.x, planeLocation.y, marker.transform.localPosition.z);
        }


        /// <summary>
        /// Adds a line to the 2D map with options (returns the line gameobject).
        /// </summary>
        /// <param name="start">starting location on the plane (-0.5, -0.5)-(0.5,0.5)</param>
        /// <param name="end">end location on the plane (-0.5, -0.5)-(0.5,0.5)</param>
        /// <param name="arcElevation">arc elevation (-0.5 .. 0.5)</param>
        public LineMarkerAnimator AddLine(Vector2 start, Vector2 end, Color color, float arcElevation, float lineWidth) {
            Vector2[] path = new Vector2[] { start, end };
            LineMarkerAnimator lma = AddLine(path, markerLineMat, arcElevation, lineWidth);
            lma.color = color;
            return lma;
        }

        /// <summary>
        /// Adds a line to the 2D map with options (returns the line gameobject).
        /// </summary>
        /// <param name="points">Sequence of points for the line</param>
        /// <param name="Color">line color</param>
        /// <param name="arcElevation">arc elevation (-0.5 .. 0.5)</param>
        public LineMarkerAnimator AddLine(Vector2[] points, Color color, float arcElevation, float lineWidth) {
            LineMarkerAnimator lma = AddLine(points, markerLineMat, arcElevation, lineWidth);
            lma.color = color;
            return lma;
        }


        /// <summary>
        /// Adds a line to the 2D map with options (returns the line gameobject).
        /// </summary>
        /// <param name="points">Sequence of points for the line</param>
        /// <param name="Color">line color</param>
        /// <param name="arcElevation">arc elevation (-0.5 .. 0.5)</param>
        public LineMarkerAnimator AddLine(List<Vector2> points, Color color, float arcElevation, float lineWidth) {
            return AddLine(points.ToArray(), color, arcElevation, lineWidth);
        }


        /// <summary>
        /// Adds a line to the 2D map over a Cell edge with options (returns the line gameobject).
        /// </summary>
        /// <param name="points">Sequence of points for the line</param>
        /// <param name="side">the side of the hexagonal cell</param>
        /// <param name="color">line color</param>
        /// <param name="lineWidth">line width</param>
        public LineMarkerAnimator AddLine(int cellIndex, CELL_SIDE side, Color color, float lineWidth) {
            LineMarkerAnimator lma = AddLine(cellIndex, side, markerLineMat, lineWidth);
            if (lma != null)
                lma.color = color;
            return lma;

        }


        /// <summary>
        /// Adds a line to the 2D map over a Cell edge with options (returns the line gameobject).
        /// </summary>
        /// <param name="points">Sequence of points for the line</param>
        /// <param name="side">the side of the hexagonal cell</param>
        /// <param name="material">line material</param>
        /// <param name="lineWidth">line width</param>
        public LineMarkerAnimator AddLine(int cellIndex, CELL_SIDE side, Material material, float lineWidth) {
            if (cells == null || cellIndex < 0 || cellIndex >= cells.Length)
                return null;
            Vector2[] points = new Vector2[2];
            Cell cell = cells[cellIndex];
            switch (side) {
                case CELL_SIDE.TopLeft:
                    points[0] = cell.points[0];
                    points[1] = cell.points[1];
                    break;
                case CELL_SIDE.Top:
                    points[0] = cell.points[1];
                    points[1] = cell.points[2];
                    break;
                case CELL_SIDE.TopRight:
                    points[0] = cell.points[2];
                    points[1] = cell.points[3];
                    break;
                case CELL_SIDE.BottomRight:
                    points[0] = cell.points[3];
                    points[1] = cell.points[4];
                    break;
                case CELL_SIDE.Bottom:
                    points[0] = cell.points[4];
                    points[1] = cell.points[5];
                    break;
                case CELL_SIDE.BottomLeft:
                    points[0] = cell.points[5];
                    points[1] = cell.points[0];
                    break;
            }
            LineMarkerAnimator lma = AddLine(points, material, 0, lineWidth);
            lma.numPoints = 2;
            return lma;
        }

        /// <summary>
        /// Adds a line to the 2D map from the center of one given cell to another (returns the line gameobject).
        /// </summary>
        /// <param name="points">Sequence of points for the line</param>
        /// <param name="side">the side of the hexagonal cell</param>
        /// <param name="color">line color</param>
        /// <param name="lineWidth">line width</param>
        public LineMarkerAnimator AddLine(int cellIndex1, int cellIndex2, Color color, float lineWidth) {
            LineMarkerAnimator lma = AddLine(cellIndex1, cellIndex2, markerLineMat, lineWidth);
            if (lma != null)
                lma.color = color;
            return lma;
        }


        /// <summary>
        /// Adds a line to the 2D map from the center of one given cell to another (returns the line gameobject).
        /// </summary>
        /// <param name="points">Sequence of points for the line</param>
        /// <param name="side">the side of the hexagonal cell</param>
        /// <param name="material">line material</param>
        /// <param name="lineWidth">line width</param>
        public LineMarkerAnimator AddLine(int cellIndex1, int cellIndex2, Material material, float lineWidth) {
            if (cells == null || cellIndex1 < 0 || cellIndex1 >= cells.Length || cellIndex2 < 0 || cellIndex2 >= cells.Length)
                return null;
            Vector2[] points = new Vector2[2];
            points[0] = cells[cellIndex1].center;
            points[1] = cells[cellIndex2].center;
            LineMarkerAnimator lma = AddLine(points, material, 0, lineWidth);
            lma.numPoints = 2;
            return lma;
        }


        /// <summary>
        /// Adds a line to the 2D map with options (returns the line gameobject).
        /// </summary>
        /// <param name="start">starting location on the plane (-0.5, -0.5)-(0.5,0.5)</param>
        /// <param name="end">end location on the plane (-0.5, -0.5)-(0.5,0.5)</param>
        /// <param name="lineMaterial">line material</param>
        /// <param name="arcElevation">arc elevation (-0.5 .. 0.5)</param>
        public LineMarkerAnimator AddLine(Vector2 start, Vector2 end, Material lineMaterial, float arcElevation, float lineWidth) {
            Vector2[] path = new Vector2[] { start, end };
            return AddLine(path, lineMaterial, arcElevation, lineWidth);
        }

        /// <summary>
        /// Adds a line to the 2D map with options (returns the line gameobject).
        /// </summary>
        /// <param name="points">Sequence of points for the line</param>
        /// <param name="lineMaterial">line material</param>
        /// <param name="arcElevation">arc elevation (-0.5 .. 0.5)</param>
        public LineMarkerAnimator AddLine(Vector2[] points, Material lineMaterial, float arcElevation, float lineWidth) {
            CheckMarkersLayer();
            GameObject newLine = new GameObject("MarkerLine");
            newLine.layer = gameObject.layer;
            bool usesRenderViewport = renderViewportIsEnabled && arcElevation > 0;
            if (!usesRenderViewport) {
                newLine.transform.SetParent(markersLayer.transform, false);
            }
            LineMarkerAnimator lma = newLine.AddComponent<LineMarkerAnimator>();
            lma.map = this;
            lma.path = points;
            lma.color = Color.white;
            lma.arcElevation = arcElevation;
            lma.lineWidth = lineWidth;
            lma.lineMaterial = lineMaterial;
            return lma;
        }

        /// <summary>
        /// Adds a custom marker (polygon) to the map on specified location and with custom size in km.
        /// </summary>
        /// <param name="position">Position for the center of the circle.</param>
        /// <param name="kmRadius">Radius in KM.</param>
        /// <param name="color">Color</param>
        /// <param name="overdraw">If this circle can draw over other shapes</param>
        /// <param name="renderOrder">Order for rendering this circle</param>
        public GameObject AddCircle(Vector2 position, float kmRadius, Color color, bool overdraw = true, int renderOrder = 1) {
            return AddCircle(position, kmRadius, 0, 1, color, overdraw, renderOrder);
        }


        /// <summary>
        /// Adds a custom marker (circle) to the map on specified location and with custom size in km.
        /// </summary>
        /// <param name="kmRadius">Radius in KM.</param>
        /// <param name="ringWidthStart">Ring inner limit (0..1). Pass 0 to draw a full circle.</param>
        /// <param name="ringWidthEnd">Ring outer limit (0..1). Pass 1 to draw a full circle.</param>
        /// <param name="color">Color</param>
        /// <param name="overdraw">If this circle can draw over other shapes</param>
        /// <param name="renderOrder">Order for rendering this circle</param>
        public GameObject AddCircle(float latitude, float longitude, float kmRadius, float ringWidthStart, float ringWidthEnd, Color color, bool overdraw = true, int renderOrder = 1) {
            return AddCircle(Conversion.GetLocalPositionFromLatLon(latitude, longitude), kmRadius, ringWidthStart, ringWidthEnd, color, overdraw, renderOrder);
        }


        /// <summary>
        /// Adds a custom marker (circle) to the map on specified location and with custom size in km.
        /// </summary>
        /// <param name="position">Position for the center of the circle.</param>
        /// <param name="kmRadius">Radius in KM.</param>
        /// <param name="ringWidthStart">Ring inner limit (0..1). Pass 0 to draw a full circle.</param>
        /// <param name="ringWidthEnd">Ring outer limit (0..1). Pass 1 to draw a full circle.</param>
        /// <param name="color">Color</param>
        /// <param name="overdraw">If this circle can draw over other shapes</param>
        /// <param name="renderOrder">Order for rendering this circle</param>
        public GameObject AddCircle(Vector2 position, float kmRadius, float ringWidthStart, float ringWidthEnd, Color color, bool overdraw = true, int renderOrder = 1) {
            CheckMarkersLayer();
            float rw = 2.0f * Mathf.PI * Conversion.EARTH_RADIUS_KM;
            float w = kmRadius / rw;
            float h = w * 2f;
            Material mat = GetColoredMarkerMaterial(color);
            if (!overdraw || renderOrder != 0) {
                mat = Instantiate(mat);
                if (!overdraw) mat.renderQueue++;
                mat.renderQueue += renderOrder * 2;
                mat.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.NotEqual);
                disposalManager.MarkForDisposal(mat);
            }
            GameObject marker = Drawing.DrawCircle("MarkerCircle", position, w, h, 0, Mathf.PI * 2.0f, ringWidthStart, ringWidthEnd, 32, mat);
            if (marker != null) {
                marker.transform.SetParent(markersLayer.transform, false);
                marker.transform.localPosition = new Vector3(position.x, position.y, -0.01f);
                marker.layer = markersLayer.layer;
            }
            return marker;
        }


        /// <summary>
        /// Adds a custom marker (circle) to the map on specified latitude/longitude and with custom size in km. Circle segments will follow latitude/longitude with accuracy so it can look distorted on a 2D map.
        /// </summary>
        /// <param name="kmRadius">Radius in KM.</param>
        /// <param name="ringWidthStart">Ring inner limit (0..1). Pass 0 to draw a full circle.</param>
        /// <param name="ringWidthEnd">Ring outer limit (0..1). Pass 1 to draw a full circle.</param>
        /// <param name="color">Color</param>
        /// <param name="overdraw">If this circle can draw over other shapes</param>
        /// <param name="renderOrder">Order for rendering this circle</param>
        public GameObject AddCircleOnSphere(Vector2 latlon, float kmRadius, float ringWidthStart, float ringWidthEnd, Color color, bool overdraw = true, int renderOrder = 1) {
            return AddCircleOnSphere(latlon.x, latlon.y, kmRadius, ringWidthStart, ringWidthEnd, color, overdraw, renderOrder);
        }


        /// <summary>
        /// Adds a custom marker (circle) to the map on specified latitude/longitude and with custom size in km. Circle segments will follow latitude/longitude with accuracy so it can look distorted on a 2D map.
        /// </summary>
        /// <param name="kmRadius">Radius in KM.</param>
        /// <param name="ringWidthStart">Ring inner limit (0..1). Pass 0 to draw a full circle.</param>
        /// <param name="ringWidthEnd">Ring outer limit (0..1). Pass 1 to draw a full circle.</param>
        /// <param name="color">Color</param>
        /// <param name="overdraw">If this circle can draw over other shapes</param>
        /// <param name="renderOrder">Order for rendering this circle</param>
        public GameObject AddCircleOnSphere(float latitude, float longitude, float kmRadius, float ringWidthStart, float ringWidthEnd, Color color, bool overdraw = true, int renderOrder = 1) {
            CheckMarkersLayer();
            Material mat = GetColoredMarkerMaterial(color);
            if (!overdraw || renderOrder != 0) {
                mat = Instantiate(mat);
                if (!overdraw) mat.renderQueue++;
                mat.renderQueue += renderOrder * 2;
                mat.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.NotEqual);
                disposalManager.MarkForDisposal(mat);
            }
            GameObject marker = Drawing.DrawCircleOnSphere("MarkerCircle", latitude, longitude, kmRadius, 0, Mathf.PI * 2.0f, ringWidthStart, ringWidthEnd, 32, mat);
            if (marker != null) {
                marker.transform.SetParent(markersLayer.transform, false);
                marker.layer = markersLayer.layer;
            }
            return marker;
        }

        /// <summary>
        /// Deletes all custom markers and lines
        /// </summary>
        public void ClearMarkers() {
            if (markersLayer == null)
                return;
            Destroy(markersLayer);
        }


        /// <summary>
        /// Removes all marker lines.
        /// </summary>
        public void ClearLineMarkers() {
            if (markersLayer == null)
                return;
            LineRenderer[] t = markersLayer.transform.GetComponentsInChildren<LineRenderer>();
            for (int k = 0; k < t.Length; k++)
                Destroy(t[k].gameObject);
        }



        List<Transform> ttmp;

        /// <summary>
        /// Returns a list of all added markers game objects
        /// </summary>
        /// <returns>The markers.</returns>
        public void GetMarkers(List<Transform> results) {
            if (results == null)
                return;
            results.Clear();
            if (markersLayer == null)
                return;
            markersLayer.transform.GetComponentsInChildren<Transform>(results);
            results.RemoveAt(0); // removes parent
        }



        /// <summary>
        /// Returns a list of all added markers game objects inside a given country
        /// </summary>
        /// <returns>The markers.</returns>
        public void GetMarkers(Country country, List<Transform> results) {
            if (results == null || country == null || country.regions == null)
                return;
            GetMarkers(results);
            int cc = results.Count;
            if (ttmp == null) {
                ttmp = new List<Transform>(cc);
            } else {
                ttmp.Clear();
            }
            int countryRegionsCount = country.regions.Count;
            for (int k = 0; k < cc; k++) {
                Vector2 pos = markersLayer.transform.InverseTransformPoint(results[k].position);
                for (int r = 0; r < countryRegionsCount; r++) {
                    if (country.regions[r].Contains(pos)) {
                        ttmp.Add(results[k]);
                        break;
                    }
                }
            }
            results.Clear();
            results.AddRange(ttmp);
        }


        /// <summary>
        /// Returns a list of all added markers game objects inside a given province
        /// </summary>
        /// <returns>The markers.</returns>
        public void GetMarkers(Province province, List<Transform> results) {
            if (results == null || province == null || province.regions == null)
                return;
            GetMarkers(results);
            int cc = results.Count;
            if (ttmp == null) {
                ttmp = new List<Transform>(cc);
            } else {
                ttmp.Clear();
            }
            int provinceRegionsCount = province.regions.Count;
            for (int k = 0; k < cc; k++) {
                Vector2 pos = markersLayer.transform.InverseTransformPoint(results[k].position);
                for (int r = 0; r < provinceRegionsCount; r++) {
                    if (province.regions[r].Contains(pos)) {
                        ttmp.Add(results[k]);
                        break;
                    }
                }
            }
            results.Clear();
            results.AddRange(ttmp);
        }

        /// <summary>
        /// Returns a list of all added markers game objects inside a given cell
        /// </summary>
        /// <returns>The markers.</returns>
        public void GetMarkers(Cell cell, List<Transform> results) {
            if (results == null || cell == null)
                return;
            GetMarkers(results);
            int cc = results.Count;
            if (ttmp == null) {
                ttmp = new List<Transform>(cc);
            } else {
                ttmp.Clear();
            }
            for (int k = 0; k < cc; k++) {
                Vector2 pos = markersLayer.transform.InverseTransformPoint(results[k].position);
                if (cell.Contains(pos)) {
                    ttmp.Add(results[k]);
                    break;
                }
            }
            results.Clear();
            results.AddRange(ttmp);
        }


        /// <summary>
        /// Returns a list of all added markers game objects inside a given region
        /// </summary>
        /// <returns>The markers.</returns>
        public void GetMarkers(Region region, List<Transform> results) {
            if (results == null)
                return;
            GetMarkers(results);
            int cc = results.Count;
            if (ttmp == null) {
                ttmp = new List<Transform>(cc);
            } else {
                ttmp.Clear();
            }
            for (int k = 0; k < cc; k++) {
                Vector2 pos = markersLayer.transform.InverseTransformPoint(results[k].position);
                if (region.Contains(pos)) {
                    ttmp.Add(results[k]);
                    break;
                }
            }
            results.Clear();
            results.AddRange(ttmp);
        }



        #endregion

    }

}