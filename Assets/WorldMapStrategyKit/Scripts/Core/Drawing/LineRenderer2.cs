using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

    /// <summary>
    /// Replacement class for Unity's standard LineRenderer
    /// </summary>
    public class LineRenderer2 : MonoBehaviour {

        public float width;
        public Material material;
        public Color color;
        public List<Vector3> vertices;
        public bool useWorldSpace;


        MeshFilter lineMeshFilter;
        MeshRenderer lineRenderer;

        bool needLineRebuild;
        Vector3[] meshVertices;
        int[] meshTriangles;
        Vector2[] meshUV;

        void Start() {
            LateUpdate();
        }

        void OnDisable() {
            if (lineRenderer != null) {
                lineRenderer.enabled = false;
            }
        }

        void OnEnable() {
            if (lineRenderer != null) {
                lineRenderer.enabled = true;
            }
        }

        // Update is called once per frame
        void LateUpdate() {
            if (!needLineRebuild) return;

            if (lineMeshFilter != null) {
                Drawing.UpdateDashedLine(lineMeshFilter, vertices, width, useWorldSpace, ref meshVertices, ref meshTriangles, ref meshUV);
            } else {
                material.color = color;
                lineMeshFilter = Drawing.DrawDashedLine(gameObject, vertices, width, useWorldSpace, material, ref meshVertices, ref meshTriangles, ref meshUV);
                lineRenderer = lineMeshFilter.GetComponent<MeshRenderer>();
            }
            needLineRebuild = false;
        }

        public void SetWidth(float startWidth, float endWidth) {
            this.width = startWidth;
            needLineRebuild = true;
        }

        public void SetColors(Color startColor, Color endColor) {
            this.color = startColor;
            material.color = color;
            if (lineRenderer != null) {
                lineRenderer.sharedMaterial = material;
            }
        }

        public void SetVertexCount(int vertexCount) {
            if (vertices == null) {
                vertices = new List<Vector3>(vertexCount);
            } else {
                vertices.Clear();
            }
            needLineRebuild = true;
        }

        public void SetPosition(int index, Vector3 position) {
            if (index >= vertices.Count) {
                vertices.Add(position);
            } else {
                vertices[index] = position;
            }
            needLineRebuild = true;
        }

    }

}