/// <summary>
/// Several ancilliary functions to sanitize polygons
/// </summary>
using System.Threading.Tasks;
using UnityEngine;


namespace WorldMapStrategyKit.PolygonClipping {
	public class PolygonSanitizer {

        static Line2D[] lines;
        const float REMOVED = -999;

        /// <summary>
        /// Searches for segments that crosses themselves and removes the shorter until there're no one else
        /// </summary>
        public static bool RemoveCrossingSegments(ref Vector2[] points) {

            bool changes = false;
            while (points.Length > 5) {
                if (!DetectCrossingSegment(points)) {
                    break;
                }
                changes = true;
            }
            if (changes) {
                PackPointsArray(ref points);
            }
            return changes;
        }

        static bool DetectCrossingSegment(Vector2[] points) {
            int max = points.Length;
            if (lines == null || lines.Length < max) {
                lines = new Line2D[max];
                for (int k = 0; k < max - 1; k++) {
                    lines[k] = new Line2D(points[k], points[k + 1], k, k + 1);
                }
				lines [max - 1] = new Line2D (points [max - 1], points [0], max - 1, 0);
			} else {
                for (int k = 0; k < max - 1; k++) {
                    lines[k].Set(points[k], points[k + 1], k, k + 1);
                }
                lines[max - 1].Set(points[max - 1], points[0], max - 1, 0);
            }
            bool needsPacking = false;
            Parallel.For(0, max - 2, k => {
                Line2D line1 = lines[k];
                if (line1.P1.x <= REMOVED) return;
                for (int j = k + 2; j < max; j++) {
                    Line2D line2 = lines[j];
                    if (line2.P1.x > REMOVED && line2.IntersectsLine(line1)) {
                        if (line1.sqrMagnitude < line2.sqrMagnitude) {
                            points[line1.P1Index].x = REMOVED;
                            points[line1.P2Index].x = REMOVED;
                            needsPacking = true;
                            break;
                        } else {
                            points[line2.P1Index].x = REMOVED;
                            points[line2.P2Index].x = REMOVED;
                            needsPacking = true;
                        }
                    }
                }
            });

            return needsPacking;
        }

        static void PackPointsArray(ref Vector2[] points) {
            int max = points.Length;
            int validPoints = 0;
            for (int k = 0; k < max; k++) {
                if (points[k].x > REMOVED) {
                    validPoints++;
                }
            }
				Vector2[] newPoints = new Vector2[validPoints];
            for (int j = 0, k = 0; k < max; k++) {
                if (points[k].x > REMOVED) {
                    newPoints[j++] = points[k];
                }
            }
            points = newPoints;
        }
    }

}

