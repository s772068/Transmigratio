using System.Runtime.InteropServices;
using UnityEngine;

namespace WorldMapStrategyKit {


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial class Cell : IFader {

        public ushort row, column;

        /// <summary>
        /// Center of this cell in local space coordinates (-0.5..0.5)
        /// </summary>
        public Vector2 center;

        JSONObject _attrib;

        /// <summary>
        /// Use this property to add/retrieve custom attributes for this country
        /// </summary>
        public JSONObject attrib {
            get {
                if (_attrib == null) {
                    _attrib = new JSONObject();
                }
                return _attrib;
            }
            set {
                _attrib = value;
            }
        }

        Vector2[] _points;

        /// <summary>
        /// List of vertices of this cell.
        /// </summary>
        public Vector2[] points {
            get {
                if (_points != null)
                    return _points;
                int pointCount = segments.Length;
                _points = new Vector2[pointCount];
                for (int k = 0; k < pointCount; k++) {
                    _points[k] = segments[k].start;
                }
                return _points;
            }
        }

        /// <summary>
        /// Segments of this cell. Internal use.
        /// </summary>
        public CellSegment[] segments;
        public Rect rect2D;

        public Renderer renderer;
        public Material customMaterial { get; set; }
        public Vector2 customTextureScale, customTextureOffset;
        public float customTextureRotation;

        //public bool isFading { get; set; }
        //public bool isWrapped;
        //public bool usedTemporary;

        byte flags;

        /// <summary>
        /// If this cell is being animated
        /// </summary>
        public bool isFading {
            get { return (flags & 1) != 0; }
            set {
                if (value) {
                    flags = (byte)(flags | 1);
                } else {
                    flags = (byte)(flags & 0xFE);
                }
            }
        }

        /// <summary>
        /// If this cell is part of the wrapped edge
        /// </summary>
        public bool isWrapped {
            get { return (flags & 2) != 0; }
            set {
                if (value) {
                    flags = (byte)(flags | 2);
                } else {
                    flags = (byte)(flags & 0xFD);
                }
            }
        }

        /// <summary>
        /// Internal use
        /// </summary>
        public bool usedTemporary {
            get { return (flags & 4) != 0; }
            set {
                if (value) {
                    flags = (byte)(flags | 4);
                } else {
                    flags = (byte)(flags & 0xFB);
                }
            }
        }

        public Cell(int row, int column, Vector2 center) {
            this.row = (ushort)row;
            this.column = (ushort)column;
            this.center = center;
            this.segments = new CellSegment[6];
        }

        public bool Contains(Vector2 position) {
            if (!rect2D.Contains(position))
                return false;
            int numPoints = points.Length;
            int j = numPoints - 1;
            bool inside = false;
            float x = position.x;
            float y = position.y;
            for (int i = 0; i < numPoints; j = i++) {
                if (((_points[i].y <= y && y < _points[j].y) || (_points[j].y <= y && y < _points[i].y)) &&
                                (x < (_points[j].x - _points[i].x) * (y - _points[i].y) / (_points[j].y - _points[i].y) + _points[i].x))
                    inside = !inside;
            }
            return inside;
        }

        public bool Contains(Region region) {
            if (region == null || region.points == null)
                return false;
            for (int k = 0; k < region.points.Length; k++) {
                if (!Contains(region.points[k])) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Intersection test between two rects
        /// </summary>
        /// <param name="otherRect">Other rect.</param>
        public bool Intersects(Rect otherRect) {

            if (otherRect.xMin > rect2D.xMax)
                return false;
            if (otherRect.xMax < rect2D.xMin)
                return false;
            if (otherRect.yMin > rect2D.yMax)
                return false;
            if (otherRect.yMax < rect2D.yMin)
                return false;

            return true;
        }


        /// <summary>
        /// Returns true if one rect crosses other rect but does not contain it (edge crossing)
        /// </summary>
        public bool IntersectsEdgesOnly(Rect otherRect) {

            if (otherRect.xMin > rect2D.xMax)
                return false;
            if (otherRect.xMax < rect2D.xMin)
                return false;
            if (otherRect.yMin > rect2D.yMax)
                return false;
            if (otherRect.yMax < rect2D.yMin)
                return false;

            if (otherRect.xMin < rect2D.xMin)
                return true;
            if (otherRect.xMax > rect2D.xMax)
                return true;
            if (otherRect.yMin < rect2D.yMin)
                return true;
            if (otherRect.yMax > rect2D.yMax)
                return true;

            return false;
        }

    }
}

