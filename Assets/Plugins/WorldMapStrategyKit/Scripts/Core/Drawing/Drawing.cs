﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using WorldMapStrategyKit.Poly2Tri;

namespace WorldMapStrategyKit {

    public static class Drawing {

        public static bool hideInHierarchy = true;

        static Dictionary<Vector3, int> hit;
        static Vector2[] uv;
        static Vector2[] uv2;

        /// <summary>
        /// Rotates one point around another
        /// </summary>
        /// <param name="pointToRotate">The point to rotate.</param>
        /// <param name="centerPoint">The centre point of rotation.</param>
        /// <param name="angleInDegrees">The rotation angle in degrees.</param>
        /// <returns>Rotated point</returns>
        static Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, float angleInDegrees) {
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
            float cosTheta = Mathf.Cos(angleInRadians);
            float sinTheta = Mathf.Sin(angleInRadians);
            return new Vector2(cosTheta * (pointToRotate.x - centerPoint.x) - sinTheta * (pointToRotate.y - centerPoint.y) + centerPoint.x,
                sinTheta * (pointToRotate.x - centerPoint.x) + cosTheta * (pointToRotate.y - centerPoint.y) + centerPoint.y);
        }

        public static Renderer CreateSurface(string name, Vector3[] surfPoints, int[] indices, Material material, DisposalManager disposalManager) {
            Rect dummyRect = new Rect();
            return CreateSurface(name, surfPoints, indices, material, dummyRect, Misc.Vector2one, Misc.Vector2zero, 0, disposalManager);
        }

        public static Renderer CreateSurface(string name, Vector3[] points, int[] indices, Material material, Rect rect, Vector2 textureScale, Vector2 textureOffset, float textureRotation, DisposalManager disposalManager, bool addVerticesPositionsAsUV2 = false) {

            GameObject hexa = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
            if (disposalManager != null) {
                disposalManager.MarkForDisposal(hexa);
            }
#if UNITY_EDITOR
            if (hideInHierarchy) {
                hexa.hideFlags |= HideFlags.HideInHierarchy;
            }
#endif
            Mesh mesh = new Mesh();
            if (disposalManager != null) {
                disposalManager.MarkForDisposal(mesh);
            }
            mesh.vertices = points;
            mesh.triangles = indices;
            // uv mapping
            if (material.mainTexture != null) {
                if (uv == null || uv.Length != points.Length) {
                    uv = new Vector2[points.Length];
                }
                for (int k = 0; k < uv.Length; k++) {
                    Vector2 coor = points[k];
                    coor.x /= textureScale.x;
                    coor.y /= textureScale.y;
                    if (textureRotation != 0)
                        coor = RotatePoint(coor, Misc.Vector2zero, textureRotation);
                    coor += textureOffset;
                    Vector2 normCoor = new Vector2((coor.x - rect.xMin) / rect.width, (coor.y - rect.yMax) / rect.height);
                    uv[k] = normCoor;
                }
                mesh.uv = uv;
            }
            // pass vertices positions as option in uv2 (need them due to dynamic batching converting to world space positions)
            if (addVerticesPositionsAsUV2) {
                if (uv2 == null || uv2.Length != points.Length) {
                    uv2 = new Vector2[points.Length];
                }
                for (int k = 0; k < uv2.Length; k++) {
                    uv2[k].x = points[k].x + 0.5f;
                    uv2[k].y = points[k].y + 0.5f;
                }
                mesh.uv2 = uv2;
            }

            mesh.RecalculateNormals();


            MeshFilter meshFilter = hexa.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            Renderer renderer = hexa.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            return renderer;

        }


        static List<Vector3> newPoints;

        public static GameObject CreateSurface(string name, Polygon poly, Material material, Rect rect, Vector2 textureScale, Vector2 textureOffset, float textureRotation, DisposalManager disposalManager) {

            GameObject hexa = new GameObject(name, typeof(MeshRenderer), typeof(MeshFilter));

            int triCount = poly.Triangles.Count;
            if (newPoints == null) {
                newPoints = new List<Vector3>(triCount * 3);
            } else {
                newPoints.Clear();
            }
            int[] triNew = new int[triCount * 3];
            int newPointsCount = -1;
            if (hit == null) {
                hit = new Dictionary<Vector3, int>(20000);
            } else {
                hit.Clear();
            }
            Vector3 p;
            p.z = 0;
            for (int k = 0; k < triCount; k++) {
                DelaunayTriangle dt = poly.Triangles[k];
                int ptmp;
                p.x = dt.Points[0].Xf;
                p.y = dt.Points[0].Yf;
                if (hit.TryGetValue(p, out ptmp)) {
                    triNew[k * 3] = ptmp;
                } else {
                    newPoints.Add(p);
                    hit[p] = ++newPointsCount;
                    triNew[k * 3] = newPointsCount;
                }
                p.x = dt.Points[2].Xf;
                p.y = dt.Points[2].Yf;
                if (hit.TryGetValue(p, out ptmp)) {
                    triNew[k * 3 + 1] = ptmp;
                } else {
                    newPoints.Add(p);
                    hit[p] = ++newPointsCount;
                    triNew[k * 3 + 1] = newPointsCount;
                }
                p.x = dt.Points[1].Xf;
                p.y = dt.Points[1].Yf;
                if (hit.TryGetValue(p, out ptmp)) {
                    triNew[k * 3 + 2] = ptmp;
                } else {
                    newPoints.Add(p);
                    hit[p] = ++newPointsCount;
                    triNew[k * 3 + 2] = newPointsCount;
                }
            }

            Mesh mesh = new Mesh();

            if (disposalManager != null) {
                if (disposalManager != null)
                    disposalManager.MarkForDisposal(hexa);
                if (disposalManager != null)
                    disposalManager.MarkForDisposal(mesh);
            }
#if UNITY_EDITOR
            if (hideInHierarchy) {
                hexa.hideFlags |= HideFlags.HideInHierarchy;
            }
#endif

            mesh.SetVertices(newPoints);
            // uv mapping
            if (material.mainTexture != null) {
                Vector2[] uv = new Vector2[newPoints.Count];
                for (int k = 0; k < uv.Length; k++) {
                    Vector2 coor = newPoints[k];
                    coor.x /= textureScale.x;
                    coor.y /= textureScale.y;
                    if (textureRotation != 0)
                        coor = RotatePoint(coor, Misc.Vector2zero, textureRotation);
                    coor += textureOffset;
                    uv[k].x = (coor.x - rect.xMin) / rect.width;
                    uv[k].y = (coor.y - rect.yMax) / rect.height;
                }
                mesh.uv = uv;
            }
            mesh.SetTriangles(triNew, 0);
            mesh.RecalculateNormals();

            MeshFilter meshFilter = hexa.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer renderer = hexa.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            return hexa;

        }

        public static TextMesh CreateText(string text, GameObject parent, Vector2 center, Font labelFont, Color textColor, bool showShadow, Material shadowMaterial, Color shadowColor, Vector2 shadowOffset, TextAnchor anchor = TextAnchor.MiddleCenter) {
            TextMesh dummy;
            return CreateText(text, parent, center, labelFont, textColor, showShadow, shadowMaterial, shadowColor, shadowOffset, out dummy, anchor);
        }

        public static TextMesh CreateText(string text, GameObject parent, Vector2 center, Font labelFont, Color textColor, bool showShadow, Material shadowMaterial, Color shadowColor, Vector2 shadowOffset, out TextMesh shadowTextMesh, TextAnchor anchor = TextAnchor.MiddleCenter) {
            // create base text
            GameObject textObj = new GameObject(text);
            textObj.hideFlags = HideFlags.DontSave;
            if (parent != null) {
                textObj.transform.SetParent(parent.transform, false);
            }
            textObj.transform.localPosition = new Vector3(center.x, center.y, 0);
            TextMesh tm = textObj.AddComponent<TextMesh>();
            tm.font = labelFont;
            textObj.GetComponent<Renderer>().sharedMaterial = tm.font.material;
            tm.alignment = TextAlignment.Center;
            tm.anchor = anchor;
            tm.color = textColor;
            tm.text = text;

            // add shadow
            if (showShadow) {
                GameObject shadow = GameObject.Instantiate(textObj);
                shadow.hideFlags = HideFlags.DontSave;
                shadow.hideFlags |= HideFlags.HideInHierarchy;
                shadow.name = "shadow";
                shadow.transform.SetParent(textObj.transform, false);
                shadow.transform.localScale = Misc.Vector3one;
                shadow.transform.localPosition = shadowOffset;
                //shadow.transform.localPosition = new Vector3(Mathf.Max(center.x / 100.0f, 1), Mathf.Min(center.y / 100.0f, -1), 0);
                shadow.GetComponent<Renderer>().sharedMaterial = shadowMaterial;
                shadowTextMesh = shadow.GetComponent<TextMesh>();
                shadowTextMesh.color = shadowColor;
            } else {
                shadowTextMesh = null;
            }
            return tm;
        }

        /// <summary>
        /// Draws a dashed line.
        /// </summary>
        /// <param name="points">Sequence of pair of points.</param>
        public static MeshFilter DrawDashedLine(GameObject parent, List<Vector3> points, float thickness, bool worldSpace, Material sharedMaterial, ref Vector3[] meshPoints, ref int[] triPoints, ref Vector2[] uv) {
            MeshFilter meshFilter = parent.AddComponent<MeshFilter>();
            UpdateDashedLine(meshFilter, points, thickness, worldSpace, ref meshPoints, ref triPoints, ref uv);
            Renderer renderer = parent.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = sharedMaterial;
            return meshFilter;
        }

        public static void UpdateDashedLine(MeshFilter meshFilter, List<Vector3> points, float thickness, bool worldSpace, ref Vector3[] meshPoints, ref int[] triPoints, ref Vector2[] uv) {

            int max = (points.Count / 2) * 2;
            if (max == 0)
                return;

            int numPoints = 8 * 3 * max / 2;
            if (numPoints > 65000)
                return;

            if (meshPoints == null || meshPoints.Length < numPoints) {
                int newCapacity = meshPoints == null ? 256 : (meshPoints.Length + 1) * 2;
                if (newCapacity < numPoints) newCapacity = numPoints;
                meshPoints = new Vector3[newCapacity];
                triPoints = new int[newCapacity];
                uv = new Vector2[newCapacity];
            }

            int mp = 0;
            thickness *= 0.5f;
            float y0 = 0f; //Mathf.Sin (0.0f * Mathf.Deg2Rad);
            float x0 = 1f; //Mathf.Cos (0.0f * Mathf.Deg2Rad);
            float y1 = 0.8660254f; //Mathf.Sin (120.0f * Mathf.Deg2Rad);
            float x1 = -0.5f; //Mathf.Cos (120.0f * Mathf.Deg2Rad);
            float y2 = -0.8660254f; //Mathf.Sin (240.0f * Mathf.Deg2Rad);
            float x2 = -0.5f; //Mathf.Cos (240.0f * Mathf.Deg2Rad);
            Vector3 up = WMSK.instance.currentCamera.transform.forward;
            Vector3 axis = Misc.Vector3zero;
            float scaleX = worldSpace ? 1f : 0.5f;

            Vector3 tmp = Misc.Vector3zero;
            for (int p = 0; p < max; p += 2) {
                Vector3 p0 = points[p];
                Vector3 p1 = points[p + 1];

                FastVector.NormalizedDirection(ref p0, ref p1, ref tmp);

                // Front triangle
                Vector3 right = Vector3.Cross(up, tmp);

                //				Vector3 axis = (up * y0 + right * x0).normalized;
                FastVector.CombineAndNormalize(ref up, y0, ref right, x0, ref axis);
                axis.x *= scaleX;
                meshPoints[mp + 0] = p0 + axis * thickness;

                //				axis = (up * y2 + right * x2).normalized;
                FastVector.CombineAndNormalize(ref up, y2, ref right, x2, ref axis);
                axis.x *= scaleX;
                meshPoints[mp + 1] = p0 + axis * thickness;

                //				axis = (up * y1 + right * x1).normalized;
                FastVector.CombineAndNormalize(ref up, y1, ref right, x1, ref axis);
                axis.x *= scaleX;
                meshPoints[mp + 2] = p0 + axis * thickness;

                triPoints[mp + 0] = mp + 0;
                triPoints[mp + 1] = mp + 1;
                triPoints[mp + 2] = mp + 2;
                uv[mp + 0] = Misc.Vector2zero;
                uv[mp + 1] = Misc.Vector2right;
                uv[mp + 2] = Misc.Vector2up;

                // Back triangle
                //				axis =  (up * y0 + right * x0).normalized;
                FastVector.CombineAndNormalize(ref up, y0, ref right, x0, ref axis);
                axis.x *= scaleX;
                meshPoints[mp + 3] = p1 + axis * thickness;

                //				axis = (up * y1 + right * x1).normalized;
                FastVector.CombineAndNormalize(ref up, y1, ref right, x1, ref axis);
                axis.x *= scaleX;
                meshPoints[mp + 4] = p1 + axis * thickness;

                //				axis = (up * y2 + right * x2).normalized;
                FastVector.CombineAndNormalize(ref up, y2, ref right, x2, ref axis);
                axis.x *= scaleX;
                meshPoints[mp + 5] = p1 + axis * thickness;

                triPoints[mp + 3] = mp + 3;
                triPoints[mp + 4] = mp + 4;
                triPoints[mp + 5] = mp + 5;
                uv[mp + 3] = Misc.Vector2zero;
                uv[mp + 4] = Misc.Vector2one;
                uv[mp + 5] = Misc.Vector2right;

                // One side
                meshPoints[mp + 6] = meshPoints[mp + 0];
                triPoints[mp + 6] = mp + 6;
                uv[mp + 6] = Misc.Vector2up;
                meshPoints[mp + 7] = meshPoints[mp + 3];
                triPoints[mp + 7] = mp + 7;
                uv[mp + 7] = Misc.Vector2one;
                meshPoints[mp + 8] = meshPoints[mp + 1];
                triPoints[mp + 8] = mp + 8;
                uv[mp + 8] = Misc.Vector2zero;

                meshPoints[mp + 9] = meshPoints[mp + 1];
                triPoints[mp + 9] = mp + 9;
                uv[mp + 9] = Misc.Vector2zero;
                meshPoints[mp + 10] = meshPoints[mp + 3];
                triPoints[mp + 10] = mp + 10;
                uv[mp + 10] = Misc.Vector2one;
                meshPoints[mp + 11] = meshPoints[mp + 5];
                triPoints[mp + 11] = mp + 11;
                uv[mp + 11] = Misc.Vector2zero;

                // Second side
                meshPoints[mp + 12] = meshPoints[mp + 1];
                triPoints[mp + 12] = mp + 12;
                uv[mp + 12] = Misc.Vector2zero;
                meshPoints[mp + 13] = meshPoints[mp + 5];
                triPoints[mp + 13] = mp + 13;
                uv[mp + 13] = Misc.Vector2right;
                meshPoints[mp + 14] = meshPoints[mp + 2];
                triPoints[mp + 14] = mp + 14;
                uv[mp + 14] = Misc.Vector2up;

                meshPoints[mp + 15] = meshPoints[mp + 2];
                triPoints[mp + 15] = mp + 15;
                uv[mp + 15] = Misc.Vector2up;
                meshPoints[mp + 16] = meshPoints[mp + 5];
                triPoints[mp + 16] = mp + 16;
                uv[mp + 16] = Misc.Vector2right;
                meshPoints[mp + 17] = meshPoints[mp + 4];
                triPoints[mp + 17] = mp + 17;
                uv[mp + 17] = Misc.Vector2up;

                // Third side
                meshPoints[mp + 18] = meshPoints[mp + 0];
                triPoints[mp + 18] = mp + 18;
                uv[mp + 18] = Misc.Vector2right;
                meshPoints[mp + 19] = meshPoints[mp + 4];
                triPoints[mp + 19] = mp + 19;
                uv[mp + 19] = Misc.Vector2up;
                meshPoints[mp + 20] = meshPoints[mp + 3];
                triPoints[mp + 20] = mp + 20;
                uv[mp + 20] = Misc.Vector2zero;

                meshPoints[mp + 21] = meshPoints[mp + 0];
                triPoints[mp + 21] = mp + 21;
                uv[mp + 21] = Misc.Vector2zero;
                meshPoints[mp + 22] = meshPoints[mp + 2];
                triPoints[mp + 22] = mp + 22;
                uv[mp + 22] = Misc.Vector2one;
                meshPoints[mp + 23] = meshPoints[mp + 4];
                triPoints[mp + 23] = mp + 23;
                uv[mp + 23] = Misc.Vector2up;

                mp += 24;
            }

            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null) {
                mesh = new Mesh();
                mesh.hideFlags = HideFlags.DontSave;
            } else {
                mesh.Clear();
            }
            mesh.SetVertices(meshPoints, 0, numPoints);
            mesh.SetUVs(0, uv, 0, numPoints);
            mesh.SetTriangles(triPoints, 0, numPoints, 0);
            mesh.RecalculateNormals();
            meshFilter.sharedMesh = mesh;
        }

        static int[][] contourX;

#if !UNITY_WSA
        [MethodImpl(256)]
#endif
        static int ABS(int x) {
            return x >= 0 ? x : -x;
        }

        // Scans a side of a triangle setting min X and max X in ContourX[][]
        // (using the Bresenham's line drawing algorithm).
        static void ScanLine(int x1, int y1, int x2, int y2, int height, int[][] contourX) {
            int sx, sy, dx1, dy1, dx2, dy2, x, y, m, n, k, cnt;

            sx = x2 - x1;
            sy = y2 - y1;

            if (sx > 0)
                dx1 = 1;
            else if (sx < 0)
                dx1 = -1;
            else {
                dy1 = 0;
                dx1 = 0;
            }

            if (sy > 0)
                dy1 = 1;
            else if (sy < 0)
                dy1 = -1;
            else
                dy1 = 0;

            m = ABS(sx);
            n = ABS(sy);
            dx2 = dx1;
            dy2 = 0;

            if (m < n) {
                m = ABS(sy);
                n = ABS(sx);
                dx2 = 0;
                dy2 = dy1;
            }

            x = x1;
            y = y1;
            cnt = m + 1;
            k = n / 2;

            while (cnt-- > 0) {
                if ((y >= 0) && (y < height)) {
                    if (x < contourX[y][0])
                        contourX[y][0] = x;
                    if (x > contourX[y][1])
                        contourX[y][1] = x;
                }

                k += n;
                if (k < m) {
                    x += dx2;
                    y += dy2;
                } else {
                    k -= m;
                    x += dx1;
                    y += dy1;
                }
            }
        }


        public static void DrawTriangle(Color[] colors, int width, int height, Vector2 p1, Vector2 p2, Vector2 p3, Color color, bool alphaBlending) {
            int y;
            if (contourX == null || contourX.Length < height) {
                contourX = new int[height][];
                for (int k = 0; k < height; k++) {
                    contourX[k] = new int[2];
                }
            }
            for (y = 0; y < height; y++) {
                contourX[y][0] = int.MaxValue; // min X
                contourX[y][1] = int.MinValue; // max X
            }

            ScanLine((int)p1.x, (int)p1.y, (int)p2.x, (int)p2.y, height, contourX);
            ScanLine((int)p2.x, (int)p2.y, (int)p3.x, (int)p3.y, height, contourX);
            ScanLine((int)p3.x, (int)p3.y, (int)p1.x, (int)p1.y, height, contourX);

            for (y = 0; y < height; y++) {
                if (contourX[y][1] >= contourX[y][0]) {
                    if (contourX[y][0] < 0)
                        contourX[y][0] = 0;
                    if (contourX[y][1] >= width)
                        contourX[y][1] = width - 1;
                }
            }

            float ca = color.a;
            if (alphaBlending && ca < 1f) { // blend operation
                float invca = 1.0f - ca;
                float cr = color.r * ca;
                float cg = color.g * ca;
                float cb = color.b * ca;
                for (y = 0; y < height; y++) {
                    if (contourX[y][1] >= contourX[y][0]) {
                        int x = contourX[y][0];
                        int len = 1 + contourX[y][1] - contourX[y][0];
                        int bufferStart = y * width + x;
                        int bufferEnd = bufferStart + len;
                        while (bufferStart < bufferEnd) {
                            Color currentColor = colors[bufferStart];
                            float r = currentColor.r * invca + cr;
                            float g = currentColor.g * invca + cg;
                            float b = currentColor.b * invca + cb;
                            currentColor.r = r;
                            currentColor.g = g;
                            currentColor.b = b;
                            currentColor.a = 1;
                            colors[bufferStart++] = currentColor;
                        }
                    }
                }
            } else {
                for (y = 0; y < height; y++) {
                    if (contourX[y][1] >= contourX[y][0]) {
                        int x = contourX[y][0];
                        int len = 1 + contourX[y][1] - contourX[y][0];
                        int bufferStart = y * width + x;
                        int bufferEnd = bufferStart + len;
                        while (bufferStart < bufferEnd) {
                            colors[bufferStart++] = color;
                        }
                    }
                }
            }
        }


        public static void DrawTriangle(Color[] colors, int width, int height, Vector2 p1, Vector2 p2, Vector2 p3, float[] heights, float minHeight, int heightsWidth, int heightsHeight, Gradient gradient) {
            int y;
            if (contourX == null || contourX.Length < height) {
                contourX = new int[height][];
                for (int k = 0; k < height; k++) {
                    contourX[k] = new int[2];
                }
            }
            for (y = 0; y < height; y++) {
                contourX[y][0] = int.MaxValue; // min X
                contourX[y][1] = int.MinValue; // max X
            }

            ScanLine((int)p1.x, (int)p1.y, (int)p2.x, (int)p2.y, height, contourX);
            ScanLine((int)p2.x, (int)p2.y, (int)p3.x, (int)p3.y, height, contourX);
            ScanLine((int)p3.x, (int)p3.y, (int)p1.x, (int)p1.y, height, contourX);

            for (y = 0; y < height; y++) {
                if (contourX[y][1] >= contourX[y][0]) {
                    if (contourX[y][0] < 0)
                        contourX[y][0] = 0;
                    if (contourX[y][1] >= width)
                        contourX[y][1] = width - 1;
                }
            }

            if (heightsWidth == width && heightsHeight == height) {
                for (y = 0; y < height; y++) {
                    if (contourX[y][1] >= contourX[y][0]) {
                        int x = contourX[y][0];
                        int len = 1 + contourX[y][1] - contourX[y][0];
                        int bufferStart = y * width + x;
                        int bufferEnd = bufferStart + len;
                        while (bufferStart < bufferEnd) {
                            float h = heights[bufferStart];
                            if (h < minHeight) {
                                h = minHeight;
                            }
                            Color color = gradient.Evaluate(h);
                            color.a = h;
                            colors[bufferStart++] = color;
                        }
                    }
                }
            } else {
                for (y = 0; y < height; y++) {
                    if (contourX[y][1] >= contourX[y][0]) {
                        int x = contourX[y][0];
                        int len = 1 + contourX[y][1] - contourX[y][0];
                        int bufferStart = y * width + x;
                        int bufferEnd = bufferStart + len;
                        float heightsIndex = (y * heightsHeight / height) * heightsWidth + x * heightsWidth / width;
                        float hx = (float)heightsWidth / width;
                        while (bufferStart < bufferEnd) {
                            int i = (int)heightsIndex;
                            float h = heights[i];
                            if (h < minHeight) {
                                h = minHeight;
                            }
                            Color color = gradient.Evaluate(h);
                            color.a = h;
                            colors[bufferStart++] = color;
                            heightsIndex += hx;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Creates a 2D pie
        /// </summary>
        public static GameObject DrawCircle(string name, Vector3 localPosition, float width, float height, float angleStart, float angleEnd, float ringWidthMin, float ringWidthMax, int numSteps, Material material) {

            GameObject hexa = new GameObject(name, typeof(MeshRenderer), typeof(MeshFilter));
            hexa.isStatic = true;

            // create the points - start with a circle
            numSteps = Mathf.FloorToInt(numSteps * (angleEnd - angleStart) / (2 * Mathf.PI));
            numSteps = Mathf.Clamp(numSteps, 12, 32);

            // if ringWidthMin == 0 we only need one triangle per step
            int numPoints = ringWidthMin == 0 ? numSteps * 3 : numSteps * 6;
            Vector3[] points = new Vector3[numPoints];
            Vector2[] uv = new Vector2[numPoints];
            int pointIndex = -1;

            width *= 0.5f;
            height *= 0.5f;

            float angleStep = (angleEnd - angleStart) / numSteps;
            float px, py;
            for (int stepIndex = 0; stepIndex < numSteps; stepIndex++) {
                float angle0 = angleStart + stepIndex * angleStep;
                float angle1 = angle0 + angleStep;

                // first triangle
                // 1
                px = Mathf.Cos(angle0) * ringWidthMax;
                py = Mathf.Sin(angle0) * ringWidthMax;
                points[++pointIndex] = new Vector3(px * width, py * height, 0);
                uv[pointIndex] = new Vector2(px + 0.5f, py + 0.5f);
                // 2
                px = Mathf.Cos(angle0) * ringWidthMin;
                py = Mathf.Sin(angle0) * ringWidthMin;
                points[++pointIndex] = new Vector3(px * width, py * height, 0);
                uv[pointIndex] = new Vector2(px + 0.5f, py + 0.5f);
                // 3
                px = Mathf.Cos(angle1) * ringWidthMax;
                py = Mathf.Sin(angle1) * ringWidthMax;
                points[++pointIndex] = new Vector3(px * width, py * height, 0);
                uv[pointIndex] = new Vector2(px + 0.5f, py + 0.5f);

                // second triangle
                if (ringWidthMin != 0) {
                    // 1
                    points[++pointIndex] = points[pointIndex - 2];
                    uv[pointIndex] = uv[pointIndex - 2];
                    // 2
                    px = Mathf.Cos(angle1) * ringWidthMin;
                    py = Mathf.Sin(angle1) * ringWidthMin;
                    points[++pointIndex] = new Vector3(px * width, py * height, 0);
                    uv[pointIndex] = new Vector2(px + 0.5f, py + 0.5f);
                    // 3
                    points[++pointIndex] = points[pointIndex - 3];
                    uv[pointIndex] = uv[pointIndex - 3];
                }
            }

            // triangles
            int[] triPoints = new int[numPoints];
            for (int p = 0; p < numPoints; p++) {
                triPoints[p] = p;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = points;
            mesh.triangles = triPoints;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            MeshFilter meshFilter = hexa.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            hexa.GetComponent<Renderer>().sharedMaterial = material;

            hexa.transform.localPosition = localPosition;
            hexa.transform.localScale = Misc.Vector3one;

            return hexa;

        }

        static Vector2 GetCirclePoint(float lat, float lon, float d, float angle, float ringSize) {
            float latitude = Mathf.Asin(Mathf.Sin(lat) * Mathf.Cos(d) + Mathf.Cos(lat) * Mathf.Sin(d) * Mathf.Cos(angle));
            float longitude = lon + Mathf.Atan2(Mathf.Sin(angle) * Mathf.Sin(d) * Mathf.Cos(lat), Mathf.Cos(d) - Mathf.Sin(lat) * Mathf.Sin(latitude));
            Vector2 latlon = new Vector2(latitude, longitude);
            latlon.x = Mathf.Lerp(lat, latlon.x, ringSize) * Mathf.Rad2Deg;
            latlon.y = Mathf.Lerp(lon, latlon.y, ringSize) * Mathf.Rad2Deg;
            return latlon;
        }

        /// <summary>
        /// Creates a 2D pie using latitude/longitude coordinates
        /// </summary>
        public static GameObject DrawCircleOnSphere(string name, float latitude, float longitude, float kmRadius, float angleStart, float angleEnd, float ringWidthMin, float ringWidthMax, int numSteps, Material material) {

            GameObject hexa = new GameObject(name, typeof(MeshRenderer), typeof(MeshFilter));
            hexa.isStatic = true;

            // create the points - start with a circle
            numSteps = Mathf.FloorToInt(numSteps * (angleEnd - angleStart) / (2 * Mathf.PI));
            numSteps = Mathf.Clamp(numSteps, 12, 32);

            // if ringWidthMin == 0 we only need one triangle per step
            int numPoints = ringWidthMin == 0 ? numSteps * 3 : numSteps * 6;
            Vector3[] points = new Vector3[numPoints];
            Vector2[] uv = new Vector2[numPoints];
            int pointIndex = -1;

            float R = Conversion.EARTH_RADIUS_KM;
            float d = kmRadius / R;

            float angleStep = (angleEnd - angleStart) / numSteps;
            float lat0 = latitude * Mathf.Deg2Rad;
            float lon0 = longitude * Mathf.Deg2Rad;
            Vector2 latlon;
            for (int stepIndex = 0; stepIndex < numSteps; stepIndex++) {
                float angle0 = angleStart + stepIndex * angleStep;
                float angle1 = angle0 + angleStep;

                // first triangle
                // 1
                latlon = GetCirclePoint(lat0, lon0, d, angle0, ringWidthMax);
                points[++pointIndex] = Conversion.GetLocalPositionFromLatLon(latlon);
                uv[pointIndex] = new Vector2(1, 1);
                // 2
                latlon = GetCirclePoint(lat0, lon0, d, angle1, ringWidthMax);
                points[++pointIndex] = Conversion.GetLocalPositionFromLatLon(latlon);
                uv[pointIndex] = new Vector2(0, 1);
                // 3
                latlon = GetCirclePoint(lat0, lon0, d, angle0, ringWidthMin);
                points[++pointIndex] = Conversion.GetLocalPositionFromLatLon(latlon);
                uv[pointIndex] = new Vector2(1, 0);

                // second triangle
                if (ringWidthMin != 0) {
                    // 1
                    points[++pointIndex] = points[pointIndex - 2];
                    uv[pointIndex] = new Vector2(1, 0);
                    // 2
                    latlon = GetCirclePoint(lat0, lon0, d, angle1, ringWidthMin);
                    points[++pointIndex] = Conversion.GetLocalPositionFromLatLon(latlon);
                    uv[pointIndex] = new Vector2(0, 0);
                    // 3
                    points[++pointIndex] = points[pointIndex - 3];
                    uv[pointIndex] = new Vector2(0, 1);
                }
            }

            // triangles
            int[] triPoints = new int[numPoints];
            for (int p = 0; p < numPoints; p++) {
                triPoints[p] = p;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = points;
            mesh.triangles = triPoints;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            MeshFilter meshFilter = hexa.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            hexa.GetComponent<Renderer>().sharedMaterial = material;

            hexa.transform.localPosition = Misc.Vector3zero;
            hexa.transform.localScale = Misc.Vector3one;

            return hexa;

        }


    }


}



