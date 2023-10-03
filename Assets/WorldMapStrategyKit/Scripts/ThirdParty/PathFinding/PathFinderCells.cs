//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
//
//  Email:  gustavo_franco@hotmail.com
//
//  Copyright (C) 2006 Franco, Gustavo 
//
//  Some modifications by Kronnect to reuse grid buffers between calls and to allow different grid configurations in same grid array (uses bitwise differentiator)
//  Also including support for hexagonal grids and some other improvements

using UnityEngine;
using System;
using System.Collections.Generic;

namespace WorldMapStrategyKit.PathFinding {

    public class PathFinderCells : IPathFinder {

        // Heap variables are initializated to default, but I like to do it anyway
        private CellCosts[] mGrid;
        private PriorityQueueB<int> mOpen;
        private readonly List<PathFinderNode> mClose = new List<PathFinderNode>();
        private HeuristicFormula mFormula = HeuristicFormula.Manhattan;
        private int mMaxSteps = 2000;
        private float mMaxSearchCost = 100000;
        private PathFinderNodeFast[] mCalcGrid;
        private byte mOpenNodeValue = 1;
        private byte mCloseNodeValue = 2;
        private OnCellCross mOnCellCross;
        private float mMinAltitude;
        private float mMaxAltitude = 1f;

        //Promoted local variables to member variables to avoid recreation between calls
        private float mH;
        private int mLocation;
        private int mNewLocation;
        private int mLocationX;
        private int mLocationY;
        private int mNewLocationX;
        private int mNewLocationY;
        private int mCloseNodeCounter;
        private int mGridX;
        private int mGridY;
        private bool mFound;
        private readonly sbyte[,] mDirectionHex0 = new sbyte[6, 2] {
                                                { 0, -1 },
                                                { 1, 0 },
                                                { 0, 1 },
                                                { -1, 0 },
                                                {   1,  1   },
                                                {   -1, 1   }
                                };
        private readonly sbyte[,] mDirectionHex1 = new sbyte[6, 2] {
                                                { 0, -1 },
                                                { 1, 0 },
                                                { 0, 1 },
                                                { -1, 0 },
                                                {   -1, -1  },
                                                { 1, -1 }
                                };
        private readonly int[] mCellSide0 = new int[6] {
                                                (int)CELL_SIDE.Bottom,
                                                (int)CELL_SIDE.BottomRight,
                                                (int)CELL_SIDE.Top,
                                                (int)CELL_SIDE.BottomLeft,
                                                (int)CELL_SIDE.TopRight,
                                                (int)CELL_SIDE.TopLeft
                                };
        private readonly int[] mCellSide1 = new int[6] {
                                                (int)CELL_SIDE.Bottom,
                                                (int)CELL_SIDE.TopRight,
                                                (int)CELL_SIDE.Top,
                                                (int)CELL_SIDE.TopLeft,
                                                (int)CELL_SIDE.BottomLeft,
                                                (int)CELL_SIDE.BottomRight
                                };
        private int mEndLocation;
        private float mNewG;
        private TERRAIN_CAPABILITY mTerrainCapability = TERRAIN_CAPABILITY.Any;
        private int callNumber;

        public PathFinderCells(CellCosts[] grid, int gridWidth, int gridHeight) {
            if (grid == null)
                throw new Exception("Grid cannot be null");

            mGrid = grid;
            mGridX = gridWidth;
            mGridY = gridHeight;

            if (mCalcGrid == null || mCalcGrid.Length != (mGridX * mGridY))
                mCalcGrid = new PathFinderNodeFast[mGridX * mGridY];

            mOpen = new PriorityQueueB<int>(new ComparePFNodeMatrix(mCalcGrid));
        }

        public void SetCustomCellsCosts(CellCosts[] cellsCosts) {
            mGrid = cellsCosts;
        }

        public HeuristicFormula Formula {
            get { return mFormula; }
            set { mFormula = value; }
        }

        public TERRAIN_CAPABILITY TerrainCapability {
            get { return mTerrainCapability; }
            set { mTerrainCapability = value; }
        }

        public float MaxSearchCost {
            get { return mMaxSearchCost; }
            set { mMaxSearchCost = value; }
        }

        public int MaxSteps {
            get { return mMaxSteps; }
            set { mMaxSteps = value; }
        }

        public OnCellCross OnCellCross {
            get { return mOnCellCross; }
            set { mOnCellCross = value; }
        }

        public float MinAltitude {
            get { return mMinAltitude; }
            set { mMinAltitude = value; }
        }

        public float MaxAltitude {
            get { return mMaxAltitude; }
            set { mMaxAltitude = value; }
        }

        public List<PathFinderNode> FindPath(Point start, Point end, out float totalCost) {
            totalCost = 0;

            mFound = false;
            mCloseNodeCounter = 0;
            if (mOpenNodeValue > 250) {
                Array.Clear(mCalcGrid, 0, mCalcGrid.Length);
                mOpenNodeValue = 1;
                mCloseNodeValue = 2;
            } else {
                mOpenNodeValue += 2;
                mCloseNodeValue += 2;
            }
            mOpen.Clear();
            mClose.Clear();
            callNumber++;

            mLocation = (start.Y * mGridX) + start.X;
            mEndLocation = (end.Y * mGridX) + end.X;
            mCalcGrid[mLocation].G = 0;
            mCalcGrid[mLocation].F = 0;
            mCalcGrid[mLocation].PX = (ushort)start.X;
            mCalcGrid[mLocation].PY = (ushort)start.Y;
            mCalcGrid[mLocation].Status = mOpenNodeValue;

            mOpen.Push(mLocation);
            while (mOpen.Count > 0) {
                mLocation = mOpen.Pop();

                // Is it in closed list? means this node was already processed
                if (mCalcGrid[mLocation].Status == mCloseNodeValue)
                    continue;

                if (mLocation == mEndLocation) {
                    mCalcGrid[mLocation].Status = mCloseNodeValue;
                    mFound = true;
                    break;
                }

                if (mCloseNodeCounter > mMaxSteps) {
                    return null;
                }

                mLocationX = mLocation % mGridX;
                mLocationY = mLocation / mGridX;

                //Lets calculate each successors
                for (int i = 0; i < 6; i++) {
                    int cellSide;
                    if (mLocationX % 2 == 0) {
                        mNewLocationX = mLocationX + mDirectionHex0[i, 0];
                        mNewLocationY = mLocationY + mDirectionHex0[i, 1];
                        cellSide = mCellSide0[i];
                    } else {
                        mNewLocationX = mLocationX + mDirectionHex1[i, 0];
                        mNewLocationY = mLocationY + mDirectionHex1[i, 1];
                        cellSide = mCellSide1[i];
                    }

                    if (mNewLocationY >= mGridY) {
                        mNewLocationY = 0;
                    } else if (mNewLocationY < 0) {
                        mNewLocationY = mGridY - 1;
                    }

                    if (mNewLocationX >= mGridX) {
                        mNewLocationX = 0;
                    } else if (mNewLocationX < 0) {
                        mNewLocationX = mGridX - 1;
                    }

                    // Unbreakeable?
                    mNewLocation = (mNewLocationY * mGridX) + mNewLocationX;
                    if (mGrid[mNewLocation].isBlocked)
                        continue;

                    if (mGrid[mNewLocation].altitude < mMinAltitude || mGrid[mNewLocation].altitude > mMaxAltitude)
                        continue;

                    if (((byte)mTerrainCapability & mGrid[mNewLocation].groundType) == 0) continue;

                    float cellCost = 1;
                    float[] sideCosts = mGrid[mLocation].crossCost;
                    if (sideCosts != null) {
                        cellCost = sideCosts[cellSide];
                    }

                    // Check custom validator
                    if (mOnCellCross != null) {
                        if (mGrid[mNewLocation].cachedCallNumber != callNumber) {
                            mGrid[mNewLocation].cachedCallNumber = callNumber;
                            mGrid[mNewLocation].cachedEventCostValue = mOnCellCross(mNewLocation);
                        }
                        cellCost += mGrid[mNewLocation].cachedEventCostValue;
                    }
                    if (cellCost <= 0)
                        cellCost = 1;

                    mNewG = mCalcGrid[mLocation].G + cellCost;

                    if (mNewG > mMaxSearchCost)
                        continue;

                    //Is it open or closed?
                    if (mCalcGrid[mNewLocation].Status == mOpenNodeValue || mCalcGrid[mNewLocation].Status == mCloseNodeValue) {
                        // The current node has less code than the previous? then skip this node
                        if (mCalcGrid[mNewLocation].G <= mNewG)
                            continue;
                    }

                    mCalcGrid[mNewLocation].PX = (ushort)mLocationX;
                    mCalcGrid[mNewLocation].PY = (ushort)mLocationY;
                    mCalcGrid[mNewLocation].G = mNewG;

                    int distX = FastMath.Abs(mNewLocationX - end.X);
                    distX = Math.Min(distX, mGridX - distX);

                    switch (mFormula) {
                        default:
                        case HeuristicFormula.Manhattan:
                            mH = distX + FastMath.Abs(mNewLocationY - end.Y);
                            break;
                        case HeuristicFormula.MaxDXDY:
                            mH = Math.Max(distX, FastMath.Abs(mNewLocationY - end.Y));
                            break;
                        case HeuristicFormula.DiagonalShortCut:
                            float h_diagonal = Math.Min(distX, FastMath.Abs(mNewLocationY - end.Y));
                            float h_straight = distX + FastMath.Abs(mNewLocationY - end.Y);
                            mH = (2 * h_diagonal) + (h_straight - 2 * h_diagonal);
                            break;
                        case HeuristicFormula.Euclidean:
                            mH = Mathf.Sqrt(distX * distX + Mathf.Pow((mNewLocationY - end.Y), 2));
                            break;
                    }
                    mCalcGrid[mNewLocation].F = mNewG + mH;

                    mOpen.Push(mNewLocation);
                    mCalcGrid[mNewLocation].Status = mOpenNodeValue;
                }

                mCloseNodeCounter++;
                mCalcGrid[mLocation].Status = mCloseNodeValue;
            }

            if (mFound) {
                mClose.Clear();
                int posX, posY;

                PathFinderNodeFast fNodeTmp = mCalcGrid[mEndLocation];
                totalCost = fNodeTmp.G;
                mGrid[mEndLocation].lastPathFindingCost = totalCost;
                PathFinderNode fNode;
                fNode.F = fNodeTmp.F;
                fNode.G = fNodeTmp.G;
                fNode.H = 0;
                fNode.PX = fNodeTmp.PX;
                fNode.PY = fNodeTmp.PY;
                fNode.X = end.X;
                fNode.Y = end.Y;

                while (fNode.X != fNode.PX || fNode.Y != fNode.PY) {
                    mClose.Add(fNode);
                    posX = fNode.PX;
                    posY = fNode.PY;
                    int loc = (posY * mGridX) + posX;
                    fNodeTmp = mCalcGrid[loc];
                    fNode.F = fNodeTmp.F;
                    fNode.G = fNodeTmp.G;
                    mGrid[loc].lastPathFindingCost = fNodeTmp.G;
                    fNode.H = 0;
                    fNode.PX = fNodeTmp.PX;
                    fNode.PY = fNodeTmp.PY;
                    fNode.X = posX;
                    fNode.Y = posY;
                }

                mClose.Add(fNode);

                return mClose;
            }
            return null;
        }

        internal class ComparePFNodeMatrix : IComparer<int> {
            protected PathFinderNodeFast[] mMatrix;

            public ComparePFNodeMatrix(PathFinderNodeFast[] matrix) {
                mMatrix = matrix;
            }

            public int Compare(int a, int b) {
                if (mMatrix[a].F > mMatrix[b].F)
                    return 1;
                else if (mMatrix[a].F < mMatrix[b].F)
                    return -1;
                return 0;
            }

            public void SetMatrix(PathFinderNodeFast[] matrix) {
                mMatrix = matrix;
            }
        }
    }
}
