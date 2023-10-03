﻿using UnityEngine;
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
        public MeshFilter lineMeshFilter;

        bool needRedraw;
        Vector3[] meshVertices;
        int[] meshTriangles;
        Vector2[] meshUV;

        void Start() {
            Update();
        }

        // Update is called once per frame
        void Update() {
            if (needRedraw) {
                if (material != null && material.color != color) {
                    material = Instantiate(material);
                    //material.hideFlags = HideFlags.DontSave;
                    material.color = color;
                }
                if (lineMeshFilter != null) {
                    Drawing.UpdateDashedLine(lineMeshFilter, vertices, width, useWorldSpace, ref meshVertices, ref meshTriangles, ref meshUV);
                } else {
                    Drawing.DrawDashedLine(gameObject, vertices, width, useWorldSpace, material, ref meshVertices, ref meshTriangles, ref meshUV);
                    lineMeshFilter = gameObject.GetComponent<MeshFilter>();
                }
                needRedraw = false;
            }

        }

        public void SetWidth(float startWidth, float endWidth) {
            this.width = startWidth;
            needRedraw = true;
        }

        public void SetColors(Color startColor, Color endColor) {
            this.color = startColor;
            needRedraw = true;
        }

        public void SetVertexCount(int vertexCount) {
            if (vertices == null) {
                vertices = new List<Vector3>(vertexCount);
            } else {
                vertices.Clear();
            }
            needRedraw = true;
        }

        public void SetPosition(int index, Vector3 position) {
            if (index >= vertices.Count) {
                vertices.Add(position);
            } else {
                vertices[index] = position;
            }
            needRedraw = true;
        }

    }

}