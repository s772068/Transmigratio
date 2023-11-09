﻿// World Map Strategy Kit for Unity - Main Script
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

        // Materials and resources
        Material gridMat, hudMatCell;

        // Cell mesh data
        const string SKW_WATER_MASK = "USE_MASK";
        const string CELLS_LAYER_NAME = "Grid";
        const float GRID_ENCLOSING_THRESHOLD = 0.3f;
        // size of the enclosing viewport rect
        Vector3[][] cellMeshBorders;
        Vector2[][] cellUVs;
        int[][] cellMeshIndices;
        bool recreateCells;

        // Common territory & cell structures
        Vector2[] gridPoints;
        readonly int[] hexIndices = new int[] { 0, 1, 5, 1, 2, 5, 5, 2, 4, 2, 3, 4 };
        readonly int[] hexIndicesWrapped = new int[] {
            0,
            1,
            5,
            1,
            2,
            5,
            5,
            2,
            4,
            6,
            7,
            8
        };
        readonly Vector3[] hexPoints = new Vector3[9];

        // Placeholders and layers
        GameObject _gridSurfacesLayer;

        GameObject gridSurfacesLayer {
            get {
                if (_gridSurfacesLayer == null)
                    CreateGridSurfacesLayer();
                return _gridSurfacesLayer;
            }
        }

        GameObject cellLayer;
        Rect gridRect;

        // Caches
        Dictionary<Cell, int> _cellLookup;
        int lastCellLookupCount = -1;
        bool refreshMesh = false;
        CellSegment[] sides;

        // Cell highlighting
        Renderer cellHighlightedObjRenderer;
        Cell _cellHighlighted;
        int _cellHighlightedIndex = -1;
        float highlightFadeStart;
        int _cellLastClickedIndex = -1;
        int _cellLastPointerOn = -1;

        Dictionary<Cell, int> cellLookup {
            get {
                if (_cellLookup != null && cells.Length == lastCellLookupCount)
                    return _cellLookup;
                if (_cellLookup == null) {
                    _cellLookup = new Dictionary<Cell, int>();
                } else {
                    _cellLookup.Clear();
                }
                for (int k = 0; k < cells.Length; k++) {
                    if (cells[k] != null)
                        _cellLookup[cells[k]] = k;
                }
                lastCellLookupCount = cells.Length;
                return _cellLookup;
            }
        }


        #region Initialization

        void CreateGridSurfacesLayer() {
            Transform t = transform.Find("GridSurfaces");
            if (t != null) {
                DestroyImmediate(t.gameObject);
            }
            _gridSurfacesLayer = new GameObject("GridSurfaces");
            _gridSurfacesLayer.transform.SetParent(transform, false);
            _gridSurfacesLayer.transform.localPosition = Misc.Vector3back * 0.001f;
            _gridSurfacesLayer.layer = gameObject.layer;
        }

        void DestroyGridSurfaces() {
            HideCellHighlight();
            if (cells != null) {
                for (int k = 0; k < cells.Length; k++) {
                    if (cells[k] != null && cells[k].renderer != null) {

                        DestroyImmediate(cells[k].renderer.gameObject);
                    }
                }
            }
            if (_gridSurfacesLayer != null)
                DestroyImmediate(_gridSurfacesLayer);
        }

        #endregion

        #region Map generation


        void CreateCells() {

            int newLength = _gridRows * _gridColumns;
            if (cells == null || cells.Length != newLength) {
                cells = new Cell[newLength];
            }
            if (_cellsCosts == null || _cellsCosts.Length != cells.Length) {
                _cellsCosts = new CellCosts[newLength];
            }
            lastCellLookupCount = -1;
            _cellLastPointerOn = -1;
            float qxOffset = _wrapHorizontally ? 0 : 0.25f;

            float qx = _gridColumns * 3f / 4f + qxOffset;

            float stepX = 1f / qx;
            float stepY = 1f / _gridRows;

            float halfStepX = stepX * 0.5f;
            float centerOffset = halfStepX;
            float halfStepY = stepY * 0.5f;
            float halfStepX2 = stepX * 0.25f;

            int sidesCount = _gridRows * _gridColumns * 6;
            if (sides == null || sides.Length < sidesCount) {
                sides = new CellSegment[_gridRows * _gridColumns * 6]; // 0 = left-up, 1 = top, 2 = right-up, 3 = right-down, 4 = down, 5 = left-down
            }
            Vector2 center, start, end;

            int sideIndex = 0;
            int cellIndex = 0;
            for (int j = 0; j < _gridRows; j++) {
                center.y = (float)j / _gridRows - 0.5f + halfStepY;
                for (int k = 0; k < _gridColumns; k++, cellIndex++) {
                    center.x = (float)k / qx - 0.5f + centerOffset;
                    center.x -= k * halfStepX2;
                    Cell cell = new Cell(j, k, center);

                    float offsetY = (k % 2 == 0) ? 0 : -halfStepY;

                    start.x = center.x - halfStepX;
                    start.y = center.y + offsetY;
                    end.x = center.x - halfStepX2;
                    end.y = center.y + halfStepY + offsetY;
                    CellSegment leftUp;
                    if (k > 0 && offsetY < 0) {
                        // leftUp is right-down edge of (k-1, j) but swapped
                        leftUp = sides[sideIndex - 3].swapped; // was sides[k - 1, j, 3].swapped;; sideIndex - 3 at this point equals to (k-1, j, 3)
                    } else {
                        leftUp = new CellSegment(start, end);
                    }
                    sides[sideIndex++] = leftUp; // 0

                    start.x = center.x - halfStepX2;
                    start.y = center.y + halfStepY + offsetY;
                    end.x = center.x + halfStepX2;
                    end.y = center.y + halfStepY + offsetY;
                    CellSegment top = new CellSegment(start, end);
                    sides[sideIndex++] = top;  // 1

                    start.x = center.x + halfStepX2;
                    start.y = center.y + halfStepY + offsetY;
                    end.x = center.x + halfStepX;
                    end.y = center.y + offsetY;
                    CellSegment rightUp = new CellSegment(start, end);
                    if (_wrapHorizontally && k == _gridColumns - 1) {
                        rightUp.isRepeated = true;
                    }
                    sides[sideIndex++] = rightUp; // 2

                    CellSegment rightDown;
                    if (j > 0 && k < _gridColumns - 1 && offsetY < 0) {
                        // rightDown is left-up edge of (k+1, j-1) but swapped
                        rightDown = sides[sideIndex - _gridColumns * 6 + 3].swapped; // was sides[k + 1, j - 1, 0].swapped
                    } else {
                        start.x = center.x + halfStepX;
                        start.y = center.y + offsetY;
                        end.x = center.x + halfStepX2;
                        end.y = center.y - halfStepY + offsetY;
                        rightDown = new CellSegment(start, end);
                        if (_wrapHorizontally && k == _gridColumns - 1) {
                            rightDown.isRepeated = true;
                        }
                    }
                    sides[sideIndex++] = rightDown; // 3

                    CellSegment bottom;
                    if (j > 0) {
                        // bottom is top edge from (k, j-1) but swapped
                        bottom = sides[sideIndex - _gridColumns * 6 - 3].swapped; // was sides[k, j - 1, 1].swapped
                    } else {
                        start.x = center.x + halfStepX2;
                        start.y = center.y - halfStepY + offsetY;
                        end.x = center.x - halfStepX2;
                        end.y = center.y - halfStepY + offsetY;
                        bottom = new CellSegment(start, end);
                    }
                    sides[sideIndex++] = bottom; // 4

                    CellSegment leftDown;
                    if (offsetY < 0 && j > 0) {
                        // leftDown is right up from (k-1, j-1) but swapped
                        leftDown = sides[sideIndex - _gridColumns * 6 - 9].swapped; // was  sides [k - 1, j - 1, 2].swapped
                    } else if (offsetY == 0 && k > 0) {
                        // leftDOwn is right up from (k-1, j) but swapped
                        leftDown = sides[sideIndex - 9].swapped; // was sides [k - 1, j, 2].swapped
                    } else {
                        start.x = center.x - halfStepX2;
                        start.y = center.y - halfStepY + offsetY;
                        end.x = center.x - halfStepX;
                        end.y = center.y + offsetY;
                        leftDown = new CellSegment(start, end);
                    }
                    sides[sideIndex++] = leftDown; // 5

                    if (j > 0 || offsetY == 0) {
                        cell.center.y += offsetY;

                        if (j == 1) {
                            bottom.isRepeated = false;
                        } else if (j == 0) {
                            leftDown.isRepeated = false;
                        }

                        cell.segments[0] = leftUp;
                        cell.segments[1] = top;
                        cell.segments[2] = rightUp;
                        cell.segments[3] = rightDown;
                        cell.segments[4] = bottom;
                        cell.segments[5] = leftDown;
                        if (_wrapHorizontally && k == _gridColumns - 1) {
                            cell.isWrapped = true;
                        }
                        cell.rect2D = new Rect(leftUp.start.x, bottom.start.y, rightUp.end.x - leftUp.start.x, top.start.y - bottom.start.y);
                        cells[cellIndex] = cell;
                    }
                }
            }
        }

        void GenerateCellsMesh() {

            if (gridPoints == null || gridPoints.Length == 0)
                gridPoints = new Vector2[200000];

            int gridPointsCount = 0;
            int y0 = (int)((gridRect.yMin + 0.5f) * _gridRows);
            int y1 = (int)((gridRect.yMax + 0.5f) * _gridRows);
            y0 = Mathf.Clamp(y0, 0, _gridRows - 1);
            y1 = Mathf.Clamp(y1, 0, _gridRows - 1);
            for (int y = y0; y <= y1; y++) {
                int yy = y * _gridColumns;
                int x0 = (int)((gridRect.xMin + 0.5f) * _gridColumns);
                int x1 = (int)((gridRect.xMax + 0.5f) * _gridColumns);
                for (int x = x0; x <= x1; x++) {
                    int wrapX = x;
                    if (_wrapHorizontally) {
                        if (x < 0)
                            wrapX += _gridColumns;
                        else if (x >= gridColumns)
                            wrapX -= gridColumns;
                    }
                    if (wrapX < 0 || wrapX >= _gridColumns)
                        continue;
                    Cell cell = cells[yy + wrapX];
                    if (cell != null) {
                        if (gridPoints.Length <= gridPointsCount + 12) {
                            // Resize and copy elements; similar to C# standard list but we avoid excesive calls when accessing elements
                            int newSize = gridPoints.Length * 2;
                            Vector2[] tmp = new Vector2[newSize];
                            Array.Copy(gridPoints, tmp, gridPointsCount);
                            gridPoints = tmp;
                        }
                        for (int i = 0; i < 6; i++) {
                            CellSegment s = cell.segments[i];
                            if (!s.isRepeated) {
                                gridPoints[gridPointsCount++] = s.start;
                                gridPoints[gridPointsCount++] = s.end;
                            }
                        }
                    }
                }
            }

            int meshGroups = (gridPointsCount / 65000) + 1;
            int meshIndex = -1;
            if (cellMeshIndices == null || cellMeshIndices.Length != meshGroups) {
                cellMeshIndices = new int[meshGroups][];
                cellMeshBorders = new Vector3[meshGroups][];
                cellUVs = new Vector2[meshGroups][];
            }
            if (gridPointsCount == 0) {
                cellMeshBorders[0] = new Vector3[0];
                cellMeshIndices[0] = new int[0];
                cellUVs[0] = new Vector2[0];
            } else {
                for (int k = 0; k < gridPointsCount; k += 65000) {
                    int max = Mathf.Min(gridPointsCount - k, 65000);
                    ++meshIndex;
                    if (cellMeshBorders[meshIndex] == null || cellMeshBorders[meshIndex].Length != max) {
                        cellMeshBorders[meshIndex] = new Vector3[max];
                        cellMeshIndices[meshIndex] = new int[max];
                        cellUVs[meshIndex] = new Vector2[max];
                    }
                    for (int j = 0; j < max; j++) {
                        cellMeshBorders[meshIndex][j].x = gridPoints[j + k].x;
                        cellMeshBorders[meshIndex][j].y = gridPoints[j + k].y;
                        cellMeshIndices[meshIndex][j] = j;
                        cellUVs[meshIndex][j].x = gridPoints[j + k].x + 0.5f;
                        cellUVs[meshIndex][j].y = gridPoints[j + k].y + 0.5f;
                    }
                }
            }
            refreshMesh = false; // mesh creation finished at this point
        }


        #endregion

        #region Drawing stuff


        public void GenerateGrid() {
            recreateCells = true;
            if (_wrapHorizontally && (_gridColumns % 2) != 0) {
                _gridColumns++; // in wrapped mode, only even columns are allowed
            }
            DrawGrid();
        }


        /// <summary>
        /// Determines if grid needs to be generated again, based on current viewport position
        /// </summary>
        public void CheckGridRect() {
            ComputeViewportRect();

            // Check rect size thresholds
            bool validGrid = true;
            float dx = renderViewportRect.width;
            float dy = renderViewportRect.height;
            if (dx > gridRect.width || dy > gridRect.height) {
                validGrid = false;
            } else if (dx < gridRect.width * GRID_ENCLOSING_THRESHOLD || dy < gridRect.height * GRID_ENCLOSING_THRESHOLD) {
                validGrid = false;
            } else {
                // if current viewport rect is inside grid rect and viewport size is between 0.8 and 1 from grid size then we're ok and exit.
                Vector2 p0 = new Vector2(_renderViewportRect.xMin, _renderViewportRect.yMax);
                Vector2 p1 = new Vector2(_renderViewportRect.xMax, _renderViewportRect.yMin);
                if (!gridRect.Contains(p0) || !gridRect.Contains(p1))
                    validGrid = false;
            }
            if (validGrid) {
                AdjustsGridAlpha();
                return;
            }

            refreshMesh = true;
            CheckCells();
            DrawCellBorders();
        }

        public void DrawGrid() {

            if (!gameObject.activeInHierarchy)
                return;

            // Initialize surface cache
            DestroyGridSurfaces();
            if (!_showGrid)
                return;

            refreshMesh = true;
            gridRect = new Rect(-1000, -1000, 1, 1);

            CheckCells();
            if (_showGrid) {
                DrawCellBorders();
                DrawColorizedCells();
            }
            recreateCells = false;
        }


        void CheckCells() {
            if (!_showGrid && !_enableCellHighlight)
                return;
            if (cells == null || recreateCells) {
                CreateCells();
                refreshMesh = true;
            }
            if (refreshMesh) {
                float f = GRID_ENCLOSING_THRESHOLD + (1f - GRID_ENCLOSING_THRESHOLD) * 0.5f;
                float gridWidth = renderViewportRect.width / f;
                float gridHeight = renderViewportRect.height / f;
                gridRect = new Rect(_renderViewportRect.center.x - gridWidth * 0.5f, _renderViewportRect.center.y - gridHeight * 0.5f, gridWidth, gridHeight);
                GenerateCellsMesh();
            }

        }

        bool CheckCellIndex(int cellIndex) {
            if (cells == null || cellIndex < 0 || cellIndex >= cells.Length) return false;
            return true;
        }

        void DrawCellBorders() {

            Transform t;
            if (cellLayer != null) {
                t = cellLayer.transform;
            } else {
                t = transform.Find(CELLS_LAYER_NAME);
            }

            if (t != null) {
                MeshFilter[] meshes = t.GetComponentsInChildren<MeshFilter>(true);
                foreach (MeshFilter mf in meshes) {
                    if (mf.sharedMesh != null) {
                        DestroyImmediate(mf.sharedMesh);
                    }
                }
                DestroyImmediate(t.gameObject);
            }

            if (cells.Length == 0)
                return;

            cellLayer = new GameObject(CELLS_LAYER_NAME);
            if (disposalManager != null) {
                disposalManager.MarkForDisposal(cellLayer);
            }
            cellLayer.transform.SetParent(transform, false);
            cellLayer.transform.localPosition = Vector3.back * 0.001f;
            int layer = transform.gameObject.layer;
            cellLayer.layer = layer;

            for (int k = 0; k < cellMeshBorders.Length; k++) {
                GameObject flayer = new GameObject("flayer");
                if (disposalManager != null) {
                    disposalManager.MarkForDisposal(flayer);
                }
                flayer.layer = layer;
                flayer.transform.SetParent(cellLayer.transform, false);
                flayer.transform.localPosition = Vector3.zero;
                flayer.transform.localRotation = Quaternion.Euler(Vector3.zero);

                Mesh mesh = new Mesh();
                mesh.vertices = cellMeshBorders[k];
                mesh.SetIndices(cellMeshIndices[k], MeshTopology.Lines, 0, true);
                mesh.uv = cellUVs[k];

                if (disposalManager != null) {
                    disposalManager.MarkForDisposal(mesh);
                }

                MeshFilter mf = flayer.AddComponent<MeshFilter>();
                mf.sharedMesh = mesh;

                MeshRenderer mr = flayer.AddComponent<MeshRenderer>();
                mr.receiveShadows = false;
                mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.sharedMaterial = gridMat;
            }
            AdjustsGridAlpha();
        }

        // Adjusts alpha according to minimum and maximum distance
        void AdjustsGridAlpha() {
            float gridAlpha;
            if (!_showGrid)
                return;
            if (lastDistanceFromCamera < _gridMinDistance) {
                gridAlpha = 1f - (_gridMinDistance - lastDistanceFromCamera) / (_gridMinDistance * 0.2f);
            } else if (lastDistanceFromCamera > _gridMaxDistance) {
                gridAlpha = 1f - (lastDistanceFromCamera - _gridMaxDistance) / (_gridMaxDistance * 0.5f);
            } else {
                gridAlpha = 1f;
            }
            gridAlpha = Mathf.Clamp01(_gridColor.a * gridAlpha);
            if (gridAlpha != gridMat.color.a) {
                gridMat.color = new Color(_gridColor.r, _gridColor.g, _gridColor.b, gridAlpha);
            }
            gridMat.SetFloat(ShaderParams.WaterLevel, _waterLevel);
            gridMat.SetFloat(ShaderParams.AlphaOnWater, _gridAphaOnWater);
            gridMat.SetTexture(ShaderParams.MainTex, _scenicWaterMask);
            if (cellLayer != null) {
                cellLayer.SetActive(_showGrid && gridAlpha > 0);
            }
            hudMatCell.SetFloat(ShaderParams.WaterLevel, _waterLevel);
            hudMatCell.SetTexture(ShaderParams.MainTex, _scenicWaterMask);
            hudMatCell.SetFloat(ShaderParams.AlphaOnWater, _gridAphaOnWater);
            if (_gridCutOutBorders && _gridAphaOnWater < 1f) {
                hudMatCell.EnableKeyword(SKW_WATER_MASK);
            } else {
                hudMatCell.DisableKeyword(SKW_WATER_MASK);
            }
        }


        void DrawColorizedCells() {
            int cellsCount = cells.Length;
            for (int k = 0; k < cellsCount; k++) {
                Cell cell = cells[k];
                if (cell == null)
                    continue;
                if (cell.customMaterial != null) { // && cell.visible) {
                    ToggleCellSurface(k, true, cell.customMaterial.color, false, (Texture2D)cell.customMaterial.mainTexture, cell.customTextureScale, cell.customTextureOffset, cell.customTextureRotation);
                }
            }
        }

        Renderer GenerateCellSurface(int cellIndex, Material material, Vector2 textureScale, Vector2 textureOffset, float textureRotation) {
            if (cellIndex < 0 || cellIndex >= cells.Length)
                return null;
            return GenerateCellSurface(cells[cellIndex], material, textureScale, textureOffset, textureRotation);
        }

        Renderer GenerateCellSurface(Cell cell, Material material, Vector2 textureScale, Vector2 textureOffset, float textureRotation) {
            Rect rect = cell.rect2D;
            Vector2[] thePoints = cell.points;  // this method is expensive
            int pointCount = thePoints.Length;
            for (int k = 0; k < pointCount; k++) {
                hexPoints[k] = thePoints[k];
            }
            if (cell.isWrapped) {
                hexPoints[6] = hexPoints[2] + Misc.Vector3left;
                hexPoints[7] = hexPoints[3] + Misc.Vector3left;
                hexPoints[8] = hexPoints[4] + Misc.Vector3left;
            }
            Renderer renderer = Drawing.CreateSurface("Cell", hexPoints, cell.isWrapped ? hexIndicesWrapped : hexIndices, material, rect, textureScale, textureOffset, textureRotation, disposalManager, _gridCutOutBorders && _gridAphaOnWater < 1f);
            GameObject surf = renderer.gameObject;
            surf.transform.SetParent(gridSurfacesLayer.transform, false);
            surf.transform.localPosition = Misc.Vector3zero;
            surf.layer = gameObject.layer;
            cell.renderer = renderer;
            return renderer;
        }


        #endregion

        #region Highlighting

        void GridCheckMousePos() {
            if (!isPlaying || !_showGrid)
                return;

            if (!lastMouseMapHitPosGood) {
                HideCellHighlight();
                return;
            }

            if (_exclusiveHighlight && ((_enableCountryHighlight && _countryHighlightedIndex >= 0 && _countries[_countryHighlightedIndex].allowHighlight) || (_enableProvinceHighlight && _provinceHighlightedIndex >= 0 && _provinces[_provinceHighlightedIndex].allowHighlight && _countries[_provinces[_provinceHighlightedIndex].countryIndex].allowProvincesHighlight))) {
                HideCellHighlight();
                return;
            }

            // verify if last highlighted cell remains active
            if (_cellHighlightedIndex >= 0) {
                if (_cellHighlighted.Contains(lastMouseMapLocalHitPos)) {
                    return;
                }
            }
            int newCellHighlightedIndex = GetCellIndex(lastMouseMapLocalHitPos);
            if (OnCellExit != null && _cellLastPointerOn >= 0 && _cellLastPointerOn != newCellHighlightedIndex) {
                OnCellExit(_cellLastPointerOn);
            }
            if (newCellHighlightedIndex >= 0) {
                if (_cellHighlightedIndex != newCellHighlightedIndex) {
                    HighlightCell(newCellHighlightedIndex, false);
                }
                if (OnCellEnter != null && _cellLastPointerOn != newCellHighlightedIndex)
                    OnCellEnter(newCellHighlightedIndex);
                _cellLastPointerOn = newCellHighlightedIndex;
            } else {
                _cellLastPointerOn = -1;
                HideCellHighlight();
            }
        }


        void GridUpdateHighlightFade() {
            if (_highlightFadeAmount == 0)
                return;

            if (cellHighlightedObjRenderer != null) {
                float newAlpha = 1.0f - Mathf.PingPong(time - highlightFadeStart, _highlightFadeAmount);
                Material mat = cellHighlightedObjRenderer.sharedMaterial;
                if (mat != hudMatCell) {
                    cellHighlightedObjRenderer.sharedMaterial = hudMatCell;
                }
                Color color = hudMatCell.color;
                Color newColor = new Color(color.r, color.g, color.b, newAlpha);
                hudMatCell.color = newColor;
            }

        }


        /// <summary>
        /// Highlights the cell region specified. Returns the generated highlight surface gameObject.
        /// Internally used by the Map UI and the Editor component, but you can use it as well to temporarily mark a territory region.
        /// </summary>
        /// <param name="refreshGeometry">Pass true only if you're sure you want to force refresh the geometry of the highlight (for instance, if the frontiers data has changed). If you're unsure, pass false.</param>
        void HighlightCell(int cellIndex, bool refreshGeometry) {
            if (cellHighlightedObjRenderer != null)
                HideCellHighlight();
            if (cellIndex < 0 || cellIndex >= cells.Length)
                return;

            if (!cells[cellIndex].isFading && _enableCellHighlight) {
                bool existsInCache = cells[cellIndex].renderer != null;
                if (refreshGeometry && existsInCache) {
                    GameObject obj = cells[cellIndex].renderer.gameObject;
                    cells[cellIndex].renderer = null;
                    DestroyImmediate(obj);
                    existsInCache = false;
                }
                if (existsInCache) {
                    cellHighlightedObjRenderer = cells[cellIndex].renderer;
                    if (cellHighlightedObjRenderer != null) {
                        cellHighlightedObjRenderer.enabled = true;
                        cellHighlightedObjRenderer.sharedMaterial = hudMatCell;
                    }
                } else {
                    cellHighlightedObjRenderer = GenerateCellSurface(cellIndex, hudMatCell, Misc.Vector2one, Misc.Vector2zero, 0);
                }
                highlightFadeStart = time;
            }

            _cellHighlighted = cells[cellIndex];
            _cellHighlightedIndex = cellIndex;
        }

        void HideCellHighlight() {
            if (cellHighlighted == null)
                return;
            if (cellHighlightedObjRenderer != null) {
                if (!cellHighlighted.isFading) {
                    if (cellHighlighted.customMaterial != null) {
                        cellHighlightedObjRenderer.sharedMaterial = cellHighlighted.customMaterial;
                    } else {
                        cellHighlightedObjRenderer.enabled = false;
                    }
                }
                cellHighlightedObjRenderer = null;
            }
            _cellHighlighted = null;
            _cellHighlightedIndex = -1;
        }


        #endregion


        #region Geometric functions

        Vector3 GetWorldSpacePosition(Vector2 localPosition) {
            return transform.TransformPoint(localPosition);
        }


        #endregion



        #region Cell stuff

        bool ValidCellIndex(int cellIndex) {
            return cellIndex >= 0 && cells != null && cellIndex < cells.Length;
        }

        List<int> GetCellsWithinRect(Rect rect2D) {
            int r0 = (int)((rect2D.yMin + 0.5f) * _gridRows);
            int r1 = (int)((rect2D.yMax + 0.5f) * _gridRows);
            r1 = Mathf.Clamp(r1 + 1, 0, _gridRows - 1);
            int c0 = (int)((rect2D.xMin + 0.5f) * _gridColumns);
            int c1 = (int)((rect2D.xMax + 0.5f) * _gridColumns);
            List<int> indices = new List<int>();
            if (cells == null) return indices;
            for (int r = r0; r <= r1; r++) {
                int rr = r * _gridColumns;
                for (int c = c0; c <= c1; c++) {
                    int cellIndex = rr + c;
                    Cell cell = cells[cellIndex];
                    if (cell != null && cell.rect2D.yMin <= rect2D.yMax && cell.rect2D.yMax >= rect2D.yMin)
                        indices.Add(cellIndex);
                }
            }
            return indices;
        }

        void CellAnimate(FADER_STYLE style, int cellIndex, Color color, float duration) {
            if (cellIndex < 0 || cells == null || cellIndex >= cells.Length) {
                return;
            }
            if (cellIndex == _cellHighlightedIndex) {
                cells[cellIndex].isFading = true;
                HideCellHighlight();
            }
            Color initialColor = Misc.ColorClear;
            Renderer renderer = cells[cellIndex].renderer;
            if (renderer == null || renderer.sharedMaterial == null) {
                renderer = SetCellTemporaryColor(cellIndex, initialColor);
            } else {
                if (renderer.enabled) {
                    initialColor = renderer.sharedMaterial.color;
                } else {
                    renderer.enabled = true;
                }
            }
            SurfaceFader.Animate(style, cells[cellIndex], renderer, initialColor, color, duration);
        }


        #endregion

    }

}