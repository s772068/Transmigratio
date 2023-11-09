using UnityEngine;
using System;


namespace WorldMapStrategyKit {


    public class MarkerClickHandler : MonoBehaviour {

        public bool captureClickEvents = true;
        public bool allowDrag = true;

        [NonSerialized]
        public bool isMouseOver;

        [NonSerialized]
        public bool wasInside;

        [NonSerialized]
        public Renderer markerRenderer;

        [NonSerialized]
        public Vector2[] triangleVertices;

        void Start() {
            markerRenderer = GetComponentInChildren<Renderer>();
            WMSK.RegisterInteractiveMarker(this);
        }

        private void OnDestroy() {
            WMSK.UnregisterInteractiveMarker(this);
        }

        public void ComputeTrianglesInLocalCoordinates() {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf == null) return;

            Mesh mesh = mf.sharedMesh;
            if (mesh == null) return;

            int[] triangles = mesh.triangles;
            triangleVertices = new Vector2[triangles.Length];
            Vector3[] vertices = mesh.vertices;
            int triangleVerticesCount = triangleVertices.Length;
            Vector2 center = transform.localPosition;
            for (int k = 0; k < triangleVerticesCount; k++) {
                Vector2 v = vertices[triangles[k]];
                triangleVertices[k] = v + center;
            }
        }
    }

}

