using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WorldMapStrategyKit.MapGenerator.Geom;

namespace WorldMapStrategyKit {
    public class MapRegion {

        public Polygon polygon;

        public Vector2[] points { get; set; }

        public List<Segment> segments;
        public List<MapRegion> neighbours;
        public MapEntity entity;
        public Rect rect2D;
        public float rect2DArea;

        public Material customMaterial { get; set; }

        public MapRegion(MapEntity entity) {
            this.entity = entity;
            neighbours = new List<MapRegion>(12);
            segments = new List<Segment>(50);
        }

        public MapRegion Clone() {
            MapRegion c = new MapRegion(entity);
            c.customMaterial = this.customMaterial;
            c.points = new Vector2[points.Length];
            System.Array.Copy(this.points, c.points, this.points.Length);
            c.polygon = polygon.Clone();
            c.segments = new List<Segment>(segments);
            return c;
        }

        public bool Intersects(MapRegion otherRegion) {
            return otherRegion.rect2D.Overlaps(otherRegion.rect2D);
        }

        public bool Contains(Vector2 p) {
            return Contains(p.x, p.y);
        }

        public bool Contains(float x, float y) {

            if (x < rect2D.xMin || x > rect2D.xMax || y < rect2D.yMin || y > rect2D.yMax) {
                return false;
            }

            if (points == null)
                return false;

            int numPoints = points.Length;
            int j = numPoints - 1;
            bool inside = false;
            for (int i = 0; i < numPoints; j = i++) {
                if (((points[i].y <= y && y < points[j].y) || (points[j].y <= y && y < points[i].y)) &&
                    (x < (points[j].x - points[i].x) * (y - points[i].y) / (points[j].y - points[i].y) + points[i].x))
                    inside = !inside;
            }
            return inside;
        }

        public bool Contains(MapRegion otherRegion) {
            if (!Intersects(otherRegion))
                return false;

            if (!Contains(otherRegion.rect2D.xMin, otherRegion.rect2D.yMin))
                return false;
            if (!Contains(otherRegion.rect2D.xMin, otherRegion.rect2D.yMax))
                return false;
            if (!Contains(otherRegion.rect2D.xMax, otherRegion.rect2D.yMin))
                return false;
            if (!Contains(otherRegion.rect2D.xMax, otherRegion.rect2D.yMax))
                return false;

            int opc = otherRegion.points.Length;
            for (int k = 0; k < opc; k++) {
                if (!Contains(otherRegion.points[k].x, otherRegion.points[k].y))
                    return false;
            }
            return true;
        }
    }
}

