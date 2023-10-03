using UnityEngine;
using System;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

    public partial class Region : IFader {
        /// <summary>
        /// Region border
        /// </summary>
        public Vector2[] points;

        /// <summary>
        /// Center of the enclosing rectangle of the region
        /// </summary>
        public Vector2 center;


        Vector2 _centroid;
        bool _centroidCalculated;
        public Vector2 centroid {
            get {
                if (!_centroidCalculated) {
                    ComputeCentroid();
                }
                return _centroid;
            }
        }

        /// <summary>
        /// 2D rect enclosing all points
        /// </summary>
        public Rect rect2D;

        /// <summary>
        /// Equals to rect2D.width * rect2D.height - precomputed for performance purposes in comparison functions
        /// </summary>
        public float rect2DArea;

        public Material customMaterial { get; set; }

        public Vector2 customTextureScale, customTextureOffset;
        public float customTextureRotation;

        public List<Region> neighbours { get; set; }

        /// <summary>
        /// Used by the extrusion APIs
        /// </summary>
        public float extrusionAmount;

        /// <summary>
        /// Country or province whose this region belongs to
        /// </summary>
        /// <value>The entity.</value>
        public IAdminEntity entity { get; set; }

        public int regionIndex { get; set; }

        public bool isFading { get; set; }

        /// <summary>
        /// Some operations require to sanitize the region point list. This flag determines if the point list changed and should pass a sanitize call.
        /// </summary>
        public bool sanitized;

        /// <summary>
        /// Internal cache for path finding engine.
        /// </summary>
        public List<int> pathFindingPositions;

        public CustomBorder customBorder;

        public struct CurvedLabelInfo {
            public Vector2 axisStart;
            public Vector2 axisEnd;
            public float axisAngle;
            public Vector2 axisAveragedThickness;
            public Vector2 axisMidDisplacement;
            public Vector2 p0, p1;
            public bool isDirty;
        }

        public CurvedLabelInfo curvedLabelInfo;

        GameObject _surface;

        public GameObject surface {
            get { return _surface; }
            set {
                _surface = value;
                surfaceIsDirty = false;
            }
        }
        public bool surfaceIsDirty; // true when polygon data changes

        public bool surfaceisMerged; // if the surface is merged with other surfaces for optimization purposes

        public bool surfaceIsCombinedRegions;  // if this surface is the result of a merge of all country regions

        public Region(IAdminEntity entity, int regionIndex) {
            this.entity = entity;
            this.regionIndex = regionIndex;
            this.sanitized = true;
            neighbours = new List<Region>();
            customBorder.Init();
        }

        public Region Clone() {
            Region c = new Region(entity, regionIndex);
            c.center = this.center;
            c.rect2D = this.rect2D;
            c.rect2DArea = this.rect2DArea;
            c.customMaterial = this.customMaterial;
            c.customTextureScale = this.customTextureScale;
            c.customTextureOffset = this.customTextureOffset;
            c.customTextureRotation = this.customTextureRotation;
            c.points = new Vector2[points.Length];
            Array.Copy(points, c.points, points.Length);
            c.customBorder.texture = this.customBorder.texture;
            c.customBorder.width = this.customBorder.width;
            c.customBorder.textureTiling = this.customBorder.textureTiling;
            c.customBorder.tintColor = this.customBorder.tintColor;
            c.extrusionAmount = this.extrusionAmount;
            c._centroid = this._centroid;
            c._centroidCalculated = this._centroidCalculated;
            c.neighbours.AddRange(neighbours);
            return c;
        }

        public void ResetCentroid() {
            _centroidCalculated = false;
        }

        public bool Contains(Vector2 p) {

            if (!rect2D.Contains(p))
                return false;

            int numPoints = points.Length;
            int j = numPoints - 1;
            bool inside = false;
            for (int i = 0; i < numPoints; j = i++) {
                if (((points[i].y <= p.y && p.y < points[j].y) || (points[j].y <= p.y && p.y < points[i].y)) &&
                    (p.x < (points[j].x - points[i].x) * (p.y - points[i].y) / (points[j].y - points[i].y) + points[i].x))
                    inside = !inside;
            }
            return inside;
        }


        public bool Contains(Region other) {

            if (other == null || !rect2D.Overlaps(other.rect2D) || other.points == null)
                return false;

            int numPoints = other.points.Length;
            for (int i = 0; i < numPoints; i++) {
                if (!Contains(other.points[i]))
                    return false;
            }
            return true;
        }


        public bool Intersects(Region other) {

            if (points == null || other == null || other.points == null)
                return false;

            Rect otherRect = other.rect2D;

            if (otherRect.xMin > rect2D.xMax)
                return false;
            if (otherRect.xMax < rect2D.xMin)
                return false;
            if (otherRect.yMin > rect2D.yMax)
                return false;
            if (otherRect.yMax < rect2D.yMin)
                return false;

            int pointCount = points.Length;
            int otherPointCount = other.points.Length;

            for (int k = 0; k < otherPointCount; k++) {
                int j = pointCount - 1;
                bool inside = false;
                Vector2 p = other.points[k];
                for (int i = 0; i < pointCount; j = i++) {
                    if (((points[i].y <= p.y && p.y < points[j].y) || (points[j].y <= p.y && p.y < points[i].y)) &&
                        (p.x < (points[j].x - points[i].x) * (p.y - points[i].y) / (points[j].y - points[i].y) + points[i].x))
                        inside = !inside;
                }
                if (inside)
                    return true;
            }

            for (int k = 0; k < pointCount; k++) {
                int j = otherPointCount - 1;
                bool inside = false;
                Vector2 p = points[k];
                for (int i = 0; i < otherPointCount; j = i++) {
                    if (((other.points[i].y <= p.y && p.y < other.points[j].y) || (other.points[j].y <= p.y && p.y < other.points[i].y)) &&
                        (p.x < (other.points[j].x - other.points[i].x) * (p.y - other.points[i].y) / (other.points[j].y - other.points[i].y) + other.points[i].x))
                        inside = !inside;
                }
                if (inside)
                    return true;
            }

            return false;
        }

        // Clears all point data and reset region info
        public void Clear() {
            points = new Vector2[0];
            rect2D = new Rect(0, 0, 0, 0);
            rect2DArea = 0;
            neighbours.Clear();
            curvedLabelInfo.isDirty = true;
            _centroidCalculated = false;
            DestroySurface();
        }

        public void MakeSurfaceDirty() {
            if (_surface != null) surfaceIsDirty = true;
        }

        public void DestroySurface() {
            surfaceisMerged = false;
            surfaceIsCombinedRegions = false;
            surfaceIsDirty = false;
            if (_surface == null) return;
            GameObject.DestroyImmediate(_surface);
            _surface = null;
        }


        /// <summary>
        /// Updates the region rect2D. Needed if points is updated manually.
        /// </summary>
        public void UpdatePointsAndRect(List<Vector2> newPoints) {
            sanitized = false;
            int pointCount = newPoints.Count;
            if (points == null || points.Length != pointCount) {
                points = newPoints.ToArray();
            } else {
                for (int k = 0; k < pointCount; k++) {
                    points[k] = newPoints[k];
                }
            }
            curvedLabelInfo.isDirty = true;
            ComputeBounds();
            _centroidCalculated = false;
            MakeSurfaceDirty();
        }

        /// <summary>
        /// Just sets the new points
        /// </summary>
        /// <param name="newPoints"></param>
        public void UpdatePoints(Vector2[] newPoints) {
            points = newPoints;
            MakeSurfaceDirty();
        }


        /// <summary>
        /// Updates the region rect2D. Needed if points is updated manually.
        /// </summary>
        /// <param name="newPoints">New points.</param>
        /// <param name="inflate">If set to <c>true</c> points will be slightly displaced to prevent polygon clipping floating point issues.</param>
        public void UpdatePointsAndRect(Vector2[] newPoints, bool inflate = false) {
            sanitized = false;
            points = newPoints;
            curvedLabelInfo.isDirty = true;
            _centroidCalculated = false;
            MakeSurfaceDirty();

            ComputeBounds();
            if (inflate) {
                Vector2 tmp = Misc.Vector2zero;
                for (int k = 0; k < points.Length; k++) {
                    FastVector.NormalizedDirection(ref center, ref points[k], ref tmp);
                    FastVector.Add(ref points[k], ref tmp, 0.00001f);
                }
            }
        }


        /// <summary>
        /// Updates the region rect2D. Needed if points is updated manually.
        /// </summary>
        public void UpdatePointsAndRect(Region fromRegion) {
            sanitized = fromRegion.sanitized;
            points = fromRegion.points;
            rect2D = fromRegion.rect2D;
            rect2DArea = fromRegion.rect2DArea;
            center = fromRegion.center;
            curvedLabelInfo.isDirty = true;
            _centroidCalculated = false;
            MakeSurfaceDirty();
        }


        void ComputeBounds() {
            Vector2 min = Misc.Vector2max;
            Vector2 max = Misc.Vector2min;
            int pointsLength = points.Length;
            for (int k = 0; k < pointsLength; k++) {
                float x = points[k].x;
                float y = points[k].y;
                if (x < min.x) {
                    min.x = x;
                }
                if (x > max.x) {
                    max.x = x;
                }
                if (y < min.y) {
                    min.y = y;
                }
                if (y > max.y) {
                    max.y = y;
                }
            }
            rect2D = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
            rect2DArea = rect2D.width * rect2D.height;
            FastVector.Average(ref min, ref max, ref center); // center = (min + max) * 0.5f;
        }

        public Vector2 GetNearestPoint(Vector2 p) {
            float minDist = float.MaxValue;
            Vector2 nearest = p;
            int pointsLength = points.Length;
            for (int k = 0; k < pointsLength; k++) {
                float dx = points[k].x - p.x;
                float dy = points[k].y - p.y;
                float dist = dx * dx + dy * dy;
                if (dist < minDist) {
                    nearest = points[k];
                    minDist = dist;
                }
            }
            return nearest;
        }


        /// <summary>
        /// Checks if this region rect overlaps a circle given by a point and radius
        /// </summary>
        /// <returns></returns>
        public bool Rect2DOverlapsCircle(Vector2 center, float radius) {
            float radiusSqr = radius * radius;
            float deltaX = center.x - Mathf.Max(rect2D.xMin, Mathf.Min(center.x, rect2D.xMax));
            float deltaY = center.y - Mathf.Max(rect2D.yMin, Mathf.Min(center.y, rect2D.yMax));
            bool rectOverlapsCircle = (deltaX * deltaX + deltaY * deltaY) < radiusSqr;
            return rectOverlapsCircle;
        }

        /// <summary>
        /// Checks if this region overlaps a circle given by a point and radius
        /// </summary>
        /// <returns></returns>
        public bool OverlapsCircle(Vector2 center, float radius) {
            float radiusSqr = radius * radius;
            float deltaX = center.x - Mathf.Max(rect2D.xMin, Mathf.Min(center.x, rect2D.xMax));
            float deltaY = center.y - Mathf.Max(rect2D.yMin, Mathf.Min(center.y, rect2D.yMax));
            bool rectOverlapsCircle = (deltaX * deltaX + deltaY * deltaY) < radiusSqr;
            if (!rectOverlapsCircle) return false;

            int pointsLength = points.Length;
            for (int k = 0; k < pointsLength; k++) {
                float dx = points[k].x - center.x;
                float dy = points[k].y - center.y;
                if (dx * dx + dy * dy < radiusSqr) return true;
            }
            return false;
        }

        /// <summary>
        /// Insert a point within two nearest points if distance is less than a certain max distance
        /// </summary>
        /// <param name="point">Point to insert</param>
        /// <param name="maxDistanceSqr">Point to insert</param>
        public void InjectPoint(Vector2 point, float maxDistanceSqr = float.MaxValue) {
            int pointsLength = points.Length;
            float minDist = maxDistanceSqr;
            int nearest = -1;
            for (int k = 0; k < pointsLength; k++) {
                float dx = point.x - points[k].x;
                float dy = point.y - points[k].y;
                float dist = dx * dx + dy * dy;
                if (dist < minDist) {
                    if (dist < 0.00000000001f) return;
                    minDist = dist;
                    nearest = k;
                }
            }
            if (nearest < 0) return;

            // Check if should add after or before this point
            int prev = nearest - 1;
            if (prev < 0) prev = pointsLength - 1;
            float prevDist = (points[prev] - point).sqrMagnitude;
            int next = nearest + 1;
            if (next >= pointsLength) next = 0;
            float nextDist = (points[next] - point).sqrMagnitude;

            int pos = prevDist < nextDist ? nearest : next;
            Array.Resize(ref points, pointsLength + 1);
            for (int k = pointsLength - 1; k > pos; k--) {
                points[k] = points[k - 1];
            }
            points[pos] = point;

            _centroidCalculated = false;
        }

        /// <summary>
        /// Computes the center of the polygon so it falls inside it
        /// </summary>
        void ComputeCentroid() {

            Vector2 c = Misc.Vector2zero;
            float area = 0f;

            int pointCount = points.Length;
            for (int i = 0; i < pointCount; ++i) {
                Vector2 p1 = points[i];
                Vector2 p2 = i + 1 < pointCount ? points[i + 1] : points[0];

                float d = p1.x * p2.y - p1.y * p2.x;
                float triangleArea = 0.5f * d;
                area += triangleArea;

                c.x += triangleArea * (p1.x + p2.x) / 3f;
                c.y += triangleArea * (p1.y + p2.y) / 3f;
            }

            if (area != 0) {
                c.x /= area;
                c.y /= area;
            }
            _centroid = c;
            _centroidCalculated = true;
        }


        /// <summary>
        /// Returns a random coordinate that's inside this region
        /// </summary>
        /// <returns></returns>
        public Vector2 GetRandomPointInside() {
            Vector2 pos;
            for (int k = 0; k < 100; k++) {  // try up to 100 times
                pos.x = rect2D.xMin + UnityEngine.Random.value * rect2D.width;
                pos.y = rect2D.yMin + UnityEngine.Random.value * rect2D.height;
                if (Contains(pos)) return pos;
            }
            return Misc.Vector2zero;
        }

        /// <summary>
        /// Returns true if both regions have the same shape
        /// </summary>
        public bool HasSameShapeThan(Region otherRegion) {
            if (otherRegion == null) return false;
            if (rect2DArea != otherRegion.rect2DArea) return false;
            if (rect2D != otherRegion.rect2D) return false;
            if (points == null || otherRegion.points == null || points.Length != otherRegion.points.Length) return false;
            int pointLength = points.Length;
            for (int k = 0; k < pointLength; k++) {
                if (points[k] != otherRegion.points[k]) return false;
            }
            return true;
        }
    }

    [Serializable]
    public struct RegionJSON {
        public Vector2[] points;
    }


}