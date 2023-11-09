using System;
using UnityEngine;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

    public class LineMarkerAnimator : MonoBehaviour {

        const int MINIMUM_POINTS = 64;
        // increase to improve line resolution

        public WMSK map;

        /// <summary>
        /// The list of map points to be traversed by the line.
        /// </summary>
        public Vector2[] path;

        /// <summary>
        /// The color of the line.
        /// </summary>
        public Color color = Misc.ColorWhite;

        /// <summary>
        /// Line width (default: 0.01f)
        /// </summary>
        public float lineWidth = 0.01f;

        /// <summary>
        /// Arc of the line. A value of zero means the line will be drawn flat, on the ground.
        /// </summary>
        public float arcElevation;

        /// <summary>
        /// The duration of the drawing of the line. Zero means instant drawing.
        /// </summary>
        public float drawingDuration;

        /// <summary>
        /// The line material. If not supplied it will use the default lineMarkerMaterial.
        /// </summary>
        public Material lineMaterial;

        /// <summary>
        /// Specifies the duration in seconds for the line before it fades out
        /// </summary>
        public float autoFadeAfter;

        /// <summary>
        /// The duration of the fade out.
        /// </summary>
        public float fadeOutDuration = 1.0f;

        /// <summary>
        /// 0 for continuous line.
        /// </summary>
        public float dashInterval;

        /// <summary>
        /// Duration of a cycle in seconds. 0.1f can be a good value. 0 = no animation.
        /// </summary>
        public float dashAnimationDuration;

        /// <summary>
        /// Number of points for the line. By default it will create a number of points based on path length and the MINIMUM_POINTS constant.
        /// </summary>
        public int numPoints;


        /// <summary>
        /// The starting line cap model or sprite.
        /// </summary>
        public GameObject startCap;

        /// <summary>
        /// Flips the cap
        /// </summary>
        public bool startCapFlipDirection;

        /// <summary>
        /// Scale for the start cap
        /// </summary>
        public Vector3 startCapScale = Misc.Vector3one;

        /// <summary>
        /// Offset for the start cap position
        /// </summary>
        public float startCapOffset = 0.1f;

        /// <summary>
        /// Optional material for the start cap
        /// </summary>
        public Material startCapMaterial;

        /// <summary>
        /// The end line cap model or sprite.
        /// </summary>
        public GameObject endCap;

        /// <summary>
        /// Flips the cap
        /// </summary>
        public bool endCapFlipDirection;

        /// <summary>
        /// Scale for the end cap
        /// </summary>
        public Vector3 endCapScale = Misc.Vector3one;

        /// <summary>
        /// Offset for the end cap position
        /// </summary>
        public float endCapOffset = 0.1f;

        /// <summary>
        /// Optional material for the end cap
        /// </summary>
        public Material endCapMaterial;


        /// <summary>
        /// Makes the line show in full at the start and reduce it progressively with time
        /// </summary>
        public bool reverseMode;

        /// <summary>
        /// The current position of the head of the line
        /// </summary>
        [NonSerialized]
        public Vector2 currentMap2DPosition;

        /* Internal fields */
        float startTime, startAutoFadeTime;
        List<Vector3> vertices;
        List<Vector2> map2dPositions;
        LineRenderer lr;
        LineRenderer2 lrd;
        // for dashed lines
        Color colorTransparent;
        bool usesViewport;
        bool isFading;
        GameObject startCapPlaceholder, startCapGO;
        GameObject endCapPlaceholder, endCapGO;
        bool useArrowEndCap, useArrowStartCap;
        Vector3 startCapArrowTipPos, startCapArrowBasePos;
        Vector3 endCapArrowTipPos, endCapArrowBasePos;
        SpriteRenderer capSpriteRenderer;
        MeshRenderer capMeshRenderer;
        Color capStartColor;
        bool needStart;
        Vector3 lastMapperCamPosition;
        float elevationStart, elevationEnd;

        static class ShaderParams {
            public static int ViewportInvMatrix = Shader.PropertyToID("_ViewportInvProj");

            public const string VIEWPORT_CROP = "_VIEWPORT_CROP";
        }

        void OnEnable() {
            needStart = true;
        }

        void OnDisable() {
            // reset properties due to pooling
            if (lr != null) lr.enabled = false;
            if (lrd != null) lrd.enabled = false;
            color = Misc.ColorWhite;
            lineWidth = 0.01f;
            arcElevation = 0;
            drawingDuration = 0;
            autoFadeAfter = 0;
            fadeOutDuration = 1.0f;
            dashInterval = 0;
            dashAnimationDuration = 0;
            startCap = null;
            startCapFlipDirection = false;
            startCapScale = Misc.Vector3one;
            startCapOffset = 0.1f;
            endCap = null;
            endCapFlipDirection = false;
            endCapScale = Misc.Vector3one;
            endCapOffset = 0.1f;
            reverseMode = false;
        }

        void DoInit() {
            needStart = false;

            startAutoFadeTime = float.MaxValue;
            colorTransparent = new Color(color.r, color.g, color.b, 0);

            if (arcElevation == 0 && drawingDuration == 0) {
                numPoints = path.Length;
            }

            // Compute path points on viewport or on 2D map
            usesViewport = map.renderViewportIsEnabled && arcElevation > 0;
            isFading = false;

            // Make line compatible with wrapping mode by offseting vertices according to the minimum distance
            if (map.wrapHorizontally) {
                for (int k = 0; k < path.Length - 1; k++) {
                    float x0 = path[k].x;
                    float x1 = path[k + 1].x;
                    float dist = Mathf.Abs(x1 - x0);
                    if (1f - dist < dist) {
                        if (x1 > 0) {
                            path[k + 1].x -= 1f;
                        } else {
                            path[k + 1].x += 1f;
                        }
                    }
                }
            }

            // Prepare elevation range
            if (usesViewport) {
                elevationStart = map.ComputeEarthHeight(path[0], false);
                elevationEnd = map.ComputeEarthHeight(path[path.Length - 1], false);
            }

            // Create line vertices
            if (dashInterval > 0) {
                SetupDashedLine();
            } else {
                SetupLine();
            }

            useArrowStartCap = startCap != null;
            useArrowEndCap = endCap != null;
        }


        // Update is called once per frame
        void Update() {
            if (needStart) DoInit();
            UpdateLine();
            if (map.time >= startAutoFadeTime) {
                UpdateFade();
            }
            lineMaterial.SetMatrix(ShaderParams.ViewportInvMatrix, map.renderViewport.transform.worldToLocalMatrix);
        }


        void UpdateLine() {
            float t;

            if (drawingDuration == 0)
                t = 1.0f;
            else
                t = (map.time - startTime) / drawingDuration;
            if (t >= 1.0f) {
                t = 1.0f;
                if (autoFadeAfter == 0) {
                    if (!usesViewport && dashAnimationDuration == 0 && !isFading) {
                        enabled = false;    // disable this behaviour
                    }
                } else if (!isFading) {
                    startAutoFadeTime = map.time;
                    isFading = true;
                }
            }

            if (reverseMode) {
                t = 1f - t;
            }

            if (dashInterval > 0) {
                UpdateDashedLine(t);
            } else {
                UpdateContinousLine(t);
            }

            if (useArrowStartCap) {
                UpdateArrowCap(ref startCapPlaceholder, ref startCapGO, startCap, startCapFlipDirection, startCapArrowBasePos, startCapArrowTipPos, startCapScale, startCapOffset, startCapMaterial);
            }
            if (useArrowEndCap && endCapArrowTipPos != Misc.Vector3zero) {
                UpdateArrowCap(ref endCapPlaceholder, ref endCapGO, endCap, endCapFlipDirection, endCapArrowBasePos, endCapArrowTipPos, endCapScale, endCapOffset, endCapMaterial);
            }
        }

        void UpdateArrowCap(ref GameObject placeholder, ref GameObject obj, GameObject cap, bool flipDirection, Vector3 arrowBasePos, Vector3 arrowTipPos, Vector3 scale, float offset, Material capMaterial) {
            if (obj == null) {
                placeholder = new GameObject("CapPlaceholder");
                placeholder.transform.SetParent(transform, false);
                if (!usesViewport) {
                    placeholder.transform.localScale = new Vector3(1f / transform.lossyScale.x, 1f / transform.lossyScale.y, 1f);
                }
                obj = Instantiate(cap);
                obj.layer = gameObject.layer;
                capSpriteRenderer = obj.GetComponent<SpriteRenderer>();
                if (capSpriteRenderer != null) {
                    capSpriteRenderer.color = lineMaterial.color;
                    capStartColor = capSpriteRenderer.color;
                }
                capMeshRenderer = obj.GetComponent<MeshRenderer>();
                if (capMeshRenderer != null) {
                    capMeshRenderer.sharedMaterial = capMaterial != null ? capMaterial : lineMaterial;
                    capStartColor = capMeshRenderer.sharedMaterial.color;
                }
                obj.transform.SetParent(placeholder.transform);
                obj.SetActive(false);
            }

            // set position
            if (!usesViewport) {
                arrowBasePos = map.transform.TransformPoint(arrowBasePos);
                arrowTipPos = map.transform.TransformPoint(arrowTipPos);
            }

            // Length of cap based on size
            if (arrowBasePos != arrowTipPos) {
                Vector3 dir = Misc.Vector3zero;
                FastVector.NormalizedDirection(ref arrowBasePos, ref arrowTipPos, ref dir);
                Vector3 pos = arrowTipPos - dir * offset;
                if (!obj.activeSelf) {
                    obj.SetActive(true);
                }
                obj.transform.position = pos;

                // look to camera
                if (usesViewport) {
                    Vector3 camDir = pos - map.cameraMain.transform.position;
                    obj.transform.LookAt(arrowTipPos + dir * 100f, camDir);
                } else {
                    Vector3 prdir = Vector3.ProjectOnPlane(dir, map.transform.forward);
                    obj.transform.LookAt(pos + map.transform.forward, prdir);
                    obj.transform.Rotate(obj.transform.forward, 90, Space.Self);
                }

                if (flipDirection) {
                    obj.transform.Rotate(180, 0, 0, Space.Self);
                }

            }
            obj.transform.localScale = scale;

        }


        void UpdateFade() {
            float t = map.time - startAutoFadeTime;
            if (t < autoFadeAfter)
                return;

            t = (t - autoFadeAfter) / fadeOutDuration;
            if (t >= 1.0f) {
                t = 1.0f;
                if (lineMaterial != null) {
                    Destroy(lineMaterial);
                }
                LinesPool.Release(gameObject);
            }

            Color fadeColor = Color.Lerp(color, colorTransparent, t);
            lineMaterial.color = fadeColor;

            if (capSpriteRenderer != null) {
                capSpriteRenderer.color = Color.Lerp(capStartColor, colorTransparent, t);
            }

            if (capMeshRenderer != null) {
                capMeshRenderer.sharedMaterial.color = Color.Lerp(capStartColor, colorTransparent, t);
            }

        }

        /// <summary>
        /// Fades out current line.
        /// </summary>
        public void FadeOut(float duration) {
            startAutoFadeTime = map.time;
            fadeOutDuration = duration;
            isFading = true;
            enabled = true;
        }


        void WorldWrapLine() {
            // hide wrapping segments
            if (!map.wrapHorizontally) return;

            Vector3 disp = map.renderViewport.transform.forward * 100f;
            int vertexCount = vertices.Count - 1;
            float worldWrapDistance = map.renderViewport.transform.localScale.x * 0.5f;
            for (int k = 0; k < vertexCount; k++) {
                Vector3 v0 = vertices[k];
                Vector3 v1 = vertices[k + 1];
                if (FastVector.SqrDistance(ref v0, ref v1) >= worldWrapDistance) {
                    v0 += disp;
                    v1 += disp;
                    vertices.Insert(k + 1, v1);
                    vertices.Insert(k + 1, v0);
                    Vector2 mapPos0 = map2dPositions[k];
                    Vector2 mapPos1 = map2dPositions[k + 1];
                    map2dPositions.Insert(k + 1, mapPos1);
                    map2dPositions.Insert(k + 1, mapPos0);
                    break;
                }
            }

        }


        #region Continuous line

        void SetupLine() {
            // Create the line mesh
            if (numPoints <= 0)
                numPoints = Mathf.Max(MINIMUM_POINTS, path.Length - 1);
            startTime = map.time;
            lr = transform.GetComponent<LineRenderer>();
            if (lr == null) {
                lr = gameObject.AddComponent<LineRenderer>();
            } else {
                lr.enabled = true;
            }
            lr.useWorldSpace = usesViewport;
            lineMaterial = Instantiate(lineMaterial);
            lineMaterial.color = color;

            if (usesViewport) {
                lineMaterial.EnableKeyword(ShaderParams.VIEWPORT_CROP);
            } else {
                lineMaterial.DisableKeyword(ShaderParams.VIEWPORT_CROP);
                arcElevation *= 100f;
            }

            lr.material = lineMaterial; // needs to instantiate to preserve individual color so can't use sharedMaterial
            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
        }

        bool CreateLineVertices() {

            if (vertices == null) {
                vertices = new List<Vector3>(numPoints + 1);
                map2dPositions = new List<Vector2>(numPoints + 1);
            } else {
                if (!usesViewport) return false; // line vertices already created and no need to recompute vertices since viewport is not enabled

                // if mapper cam has not moved, return and reuse vertices
                Vector3 mapperCamPosition = map.currentCamera.transform.position;
                if (mapperCamPosition == lastMapperCamPosition) return false;
                lastMapperCamPosition = mapperCamPosition;

                vertices.Clear();
                map2dPositions.Clear();
            }

            Vector2 mapPos;
            Vector3 worldPos = Misc.Vector3zero;
            for (int s = 0; s <= numPoints; s++) {
                float t = (float)s / numPoints;
                int index = (int)((path.Length - 1) * t);
                int findex = Mathf.Min(index + 1, path.Length - 1);
                float t0 = t * (path.Length - 1);
                t0 -= index;
                mapPos = Vector2.Lerp(path[index], path[findex], t0);
                if (usesViewport) {
                    float elevation = Mathf.Lerp(elevationStart, elevationEnd, t);
                    elevation += arcElevation > 0 ? Mathf.Sin(t * Mathf.PI) * arcElevation : 0;
                    worldPos = map.Map2DToWorldPosition(mapPos, elevation, HEIGHT_OFFSET_MODE.ABSOLUTE_CLAMPED, false);
                    vertices.Add(worldPos);
                } else {
                    worldPos.x = mapPos.x;
                    worldPos.y = mapPos.y;
                    if (arcElevation > 0) {
                        worldPos.z = -Mathf.Sin(t0 * Mathf.PI) * arcElevation;
                    }
                    vertices.Add(worldPos);
                }
                map2dPositions.Add(mapPos);
            }

            WorldWrapLine();

            return true;
        }

        void UpdateContinousLine(float t) {
            if (!CreateLineVertices() && t >= 1f) return;

            int vertexCount = vertices.Count;
            float vertexIndex = 1 + (vertexCount - 2) * t;
            int currentVertex = (int)(vertexIndex);
            if (currentVertex >= 0 && currentVertex < vertexCount) {
                lr.positionCount = currentVertex + 1;
                for (int k = 0; k < currentVertex; k++) {
                    lr.SetPosition(k, vertices[k]);
                }
                // adjust last segment
                Vector3 currentVertexPos = vertices[currentVertex > 0 ? currentVertex - 1 : 0];
                Vector3 nextVertexPos = vertices[currentVertex];
                Vector3 progress;
                Vector3 nextMapPos = map2dPositions[currentVertex];
                if (t >= 1f) {
                    progress = nextVertexPos;
                    currentMap2DPosition = nextMapPos;
                } else {
                    float subt = vertexIndex - currentVertex;
                    progress = Vector3.Lerp(currentVertexPos, nextVertexPos, subt);

                    Vector3 currentMapPos = map2dPositions[currentVertex > 0 ? currentVertex - 1 : 0];
                    currentMap2DPosition = Vector3.Lerp(currentMapPos, nextMapPos, subt);
                }
                lr.SetPosition(currentVertex, progress);

                // set line cap positions
                startCapArrowTipPos = vertices[0];
                endCapArrowTipPos = progress;
                if (vertexCount > 1) {
                    startCapArrowBasePos = vertices[1];
                    endCapArrowBasePos = currentVertexPos;
                } else {
                    startCapArrowBasePos = startCapArrowTipPos;
                    endCapArrowBasePos = endCapArrowTipPos;
                }
            }
        }

        #endregion

        #region Dashed line

        void SetupDashedLine() {
            // Create the line mesh
            startTime = map.time;
            if (!usesViewport) {
                arcElevation *= 100f;
            }
            lrd = transform.GetComponent<LineRenderer2>();
            if (lrd == null) {
                lrd = gameObject.AddComponent<LineRenderer2>();
            } else {
                lrd.enabled = true;
            }
            lrd.useWorldSpace = usesViewport; // needed since thickness should be independent of parent scale
            lineMaterial = Instantiate(lineMaterial);  // needs to instantiate to preserve individual color so can't use sharedMaterial
            lineMaterial.color = color;

            if (usesViewport) {
                lineMaterial.EnableKeyword(ShaderParams.VIEWPORT_CROP);
            } else {
                lineMaterial.DisableKeyword(ShaderParams.VIEWPORT_CROP);
                lineWidth /= map.transform.localScale.x;
            }

            lrd.material = lineMaterial;
            lrd.SetColors(color, color);
            lrd.SetWidth(lineWidth, lineWidth);
        }

        void CreateDashedLineVertices() {

            // Compute dash segments
            if (vertices == null) {
                vertices = new List<Vector3>(100);
                map2dPositions = new List<Vector2>(100);
            } else {
                vertices.Clear();
                map2dPositions.Clear();
            }

            // Calculate total line distance
            float totalDistance = 0;
            Vector2 prev = Misc.Vector2zero;
            int max = path.Length - 1;
            for (int s = 0; s <= max; s++) {
                Vector2 current = path[s];
                if (s > 0) {
                    totalDistance += Vector2.Distance(current, prev);
                }
                prev = current;
            }

            // Dash animation?
            float startingDistance = 0;
            float step = dashInterval * 2f;
            if (dashAnimationDuration > 0) {
                float ett = map.time / dashAnimationDuration;
                float elapsed = ett - (int)ett;
                startingDistance = elapsed * step;
            }

            if (totalDistance == 0)
                return;

            int pair = 0;
            Vector2 mapPos;
            Vector3 worldPos = Misc.Vector3zero;

            for (float distanceAcum = startingDistance; distanceAcum < totalDistance + step; distanceAcum += dashInterval, pair++) {
                float t0 = Mathf.Clamp01(distanceAcum / totalDistance);
                float t = t0 * (path.Length - 1);
                int index = (int)t;
                int findex = Mathf.Min(index + 1, path.Length - 1);

                t -= index;
                if (index < 0 || index >= path.Length || findex < 0 || findex >= path.Length)
                    continue;

                mapPos = Vector2.Lerp(path[index], path[findex], t);

                if (usesViewport) {
                    float elevation = Mathf.Lerp(elevationStart, elevationEnd, t0);
                    elevation += arcElevation > 0 ? Mathf.Sin(t0 * Mathf.PI) * arcElevation : 0;
                    worldPos = map.Map2DToWorldPosition(mapPos, elevation, HEIGHT_OFFSET_MODE.ABSOLUTE_CLAMPED, false);
                    if (vertices.Count > 0 || (pair % 2 == 0)) {
                        vertices.Add(worldPos);
                    }
                } else {
                    worldPos.x = mapPos.x;
                    worldPos.y = mapPos.y;
                    if (arcElevation > 0) {
                        worldPos.z = -Mathf.Sin(t0 * Mathf.PI) * arcElevation;
                    }
                    vertices.Add(worldPos);
                }

                map2dPositions.Add(mapPos);
            }

            WorldWrapLine();
        }

        void UpdateDashedLine(float t) {
            // pass current vertices
            CreateDashedLineVertices();

            int vertexCount = vertices.Count;
            float vertexIndex = 1f + (vertexCount - 2) * t;
            int currentVertex = (int)(vertexIndex);
            lrd.SetVertexCount(currentVertex + 1);
            if (currentVertex >= 0 && currentVertex < vertexCount) {
                for (int k = 0; k < currentVertex; k++) {
                    lrd.SetPosition(k, vertices[k]);
                }

                // adjust last segment
                Vector3 nextVertexPos = vertices[currentVertex];
                Vector3 progress;
                Vector3 nextMapPos = map2dPositions[currentVertex];
                if (t >= 1) {
                    progress = nextVertexPos;
                    currentMap2DPosition = nextMapPos;
                } else {
                    Vector3 currentVertexPos = vertices[currentVertex > 0 ? currentVertex - 1 : 0];
                    float subt = vertexIndex - currentVertex;
                    progress = Vector3.Lerp(currentVertexPos, nextVertexPos, subt);
                    Vector3 currentMapPos = map2dPositions[currentVertex > 0 ? currentVertex - 1 : 0];
                    currentMap2DPosition = Vector3.Lerp(currentMapPos, nextMapPos, subt);
                }
                lrd.SetPosition(currentVertex, progress);

                // set line cap positions
                startCapArrowTipPos = vertices[0];
                if (vertexCount > 1) {
                    startCapArrowBasePos = vertices[1];
                } else {
                    startCapArrowBasePos = startCapArrowTipPos;
                }

                if (useArrowEndCap && t > 0.1f) {
                    t += 0.1f;
                    if (t > 1f)
                        t = 1f;
                    int index = (int)((path.Length - 1) * t);
                    int findex = Mathf.Min(index + 1, path.Length - 1);
                    float t0 = t * (path.Length - 1);
                    t0 -= index;
                    Vector3 mapPos = Vector2.Lerp(path[index], path[findex], t0);
                    if (usesViewport) {
                        if (map.renderViewportRect.Contains(map.Map2DToWrappedRenderViewport(mapPos))) {
                            float elevation = Mathf.Lerp(elevationStart, elevationEnd, t);
                            elevation += arcElevation > 0 ? Mathf.Sin(t * Mathf.PI) * arcElevation : 0;
                            mapPos = map.Map2DToWorldPosition(mapPos, elevation, HEIGHT_OFFSET_MODE.ABSOLUTE_CLAMPED, false);
                        }
                    } else {
                        if (arcElevation > 0) {
                            mapPos.z = -Mathf.Sin(t0 * Mathf.PI) * arcElevation;
                        }
                    }
                    endCapArrowBasePos = endCapArrowTipPos;
                    endCapArrowTipPos = mapPos;
                }
            }
        }

        #endregion

    }
}