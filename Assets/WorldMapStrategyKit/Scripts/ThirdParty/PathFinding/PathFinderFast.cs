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
//  Some modifications by Kronnect Games to reuse grid buffers between calls and to allow different grid configurations in same grid array (uses bitwise differentiator)

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldMapStrategyKit.PathFinding {

    public delegate float OnCellCross(int location);

    public class PathFinderFast : IPathFinder {

        // Heap variables are initializated to default, but I like to do it anyway
        private byte[] mGrid;
        private PriorityQueueB<int> mOpen;
        private readonly List<PathFinderNode> mClose = new List<PathFinderNode>();
        private HeuristicFormula mFormula = HeuristicFormula.Manhattan;
        private bool mDiagonals = true;
        private int mMaxSteps = 2000;
        private float mMaxSearchCost = 100000;
        private PathFinderNodeFast[] mCalcGrid;
        private byte mOpenNodeValue = 1;
        private byte mCloseNodeValue = 2;
        private byte mGridBit = 1;
        private float[] mCustomCosts;
        // optional values for custom validation

        //Promoted local variables to member variables to avoid recreation between calls
        private float mH;
        private int mLocation;
        private int mNewLocation;
        private ushort mLocationX;
        private ushort mLocationY;
        private ushort mNewLocationX;
        private ushort mNewLocationY;
        private int mCloseNodeCounter;
        private ushort mGridX;
        private ushort mGridY;
        private ushort mGridXMinus1;
        private ushort mGridYLog2;
        private bool mFound;


        readonly sbyte[,] offsetsWithDiagonals = new sbyte[8, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 1, -1 }, { 1, 1 }, { -1, 1 }, { -1, -1 } };
        readonly sbyte[,] offsetsWithoutDiagonals = new sbyte[4, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };

        private sbyte[,] mDirection;

        private int mEndLocation = 0;
        private float mNewG;

        public PathFinderFast(byte[] grid, byte gridBit, int gridWidth, int gridHeight, float[] customCosts) {
            mDirection = offsetsWithDiagonals;
            mGrid = grid;
            mCustomCosts = customCosts;
            mGridBit = gridBit;
            mGridX = (ushort)gridWidth; 
            mGridY = (ushort)gridHeight;
            mGridXMinus1 = (ushort)(mGridX - 1);
            mGridYLog2 = (ushort)Math.Log(mGridX, 2);

            // This should be done at the constructor, for now we leave it here.
            if (Math.Log(mGridX, 2) != (int)Math.Log(mGridX, 2) ||
                Math.Log(mGridY, 2) != (int)Math.Log(mGridY, 2)) {
                throw new Exception("Invalid Grid, size in X and Y must be power of 2");
            }

            if (mCalcGrid == null || mCalcGrid.Length != (mGridX * mGridY)) {
                mCalcGrid = new PathFinderNodeFast[mGridX * mGridY];
            }

            mOpen = new PriorityQueueB<int>(new ComparePFNodeMatrix(mCalcGrid));
        }

        public void SetCalcMatrix(byte[] grid, byte gridBit) {
            if (grid == null)
                throw new Exception("Grid cannot be null");
            if (grid.Length != mGrid.Length) // mGridX != (ushort) (mGrid.GetUpperBound(0) + 1) || mGridY != (ushort) (mGrid.GetUpperBound(1) + 1))
                throw new Exception("SetCalcMatrix called with matrix with different dimensions. Call constructor instead.");
            mGrid = grid;
            mGridBit = gridBit;

            Array.Clear(mCalcGrid, 0, mCalcGrid.Length);
            ComparePFNodeMatrix comparer = (ComparePFNodeMatrix)mOpen.comparer;
            comparer.SetMatrix(mCalcGrid);
        }

        public void SetCustomRouteMatrix(float[] newRouteMatrix) {
            if (newRouteMatrix != null && newRouteMatrix.Length != mCustomCosts.Length) {
                throw new Exception("SetCustomRouteMatrix called with matrix with different dimensions.");
            }
            mCustomCosts = newRouteMatrix;
        }

        public HeuristicFormula Formula {
            get { return mFormula; }
            set { mFormula = value; }
        }

        public bool Diagonals {
            get { return mDiagonals; }
            set {
                mDiagonals = value;
                if (mDiagonals) {
                    mDirection = offsetsWithDiagonals;
                } else {
                    mDirection = offsetsWithoutDiagonals;
                }
            }
        }

        public float MaxSearchCost {
            get { return mMaxSearchCost; }
            set { mMaxSearchCost = value; }
        }

        public int MaxSteps {
            get { return mMaxSteps; }
            set { mMaxSteps = value; }
        }

        public List<Vector2> GetExaminedPlaces() {
            List<Vector2> places = new List<Vector2>(mCalcGrid.Length);
            for (int k = 0; k < mCalcGrid.Length; k++) {
                if (mCalcGrid[k].Status == mOpenNodeValue) {
                    float x = ((float)mCalcGrid[k].PX / mGridX) - 0.5f;
                    float y = ((float)mCalcGrid[k].PY / mGridY) - 0.5f;
                    places.Add(new Vector2(x, y));
                }
            }
            return places;
        }

        public OnCellCross OnCellCross { get; set; }

        public List<PathFinderNode> FindPath(Point start, Point end, out float totalCost) {
            totalCost = 0;
            int numDirections = mDiagonals ? 8 : 4;

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

            mLocation = (start.Y << mGridYLog2) + start.X;
            mEndLocation = (end.Y << mGridYLog2) + end.X;
            mCalcGrid[mLocation].G = 0;
            mCalcGrid[mLocation].F = 0;
            mCalcGrid[mLocation].PX = (ushort)start.X;
            mCalcGrid[mLocation].PY = (ushort)start.Y;
            mCalcGrid[mLocation].Status = mOpenNodeValue;

            mOpen.Push(mLocation);

            while (mOpen.Count > 0) {
                mLocation = mOpen.Pop();

                //Is it in closed list? means this node was already processed
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

                mLocationX = (ushort)(mLocation & mGridXMinus1);
                mLocationY = (ushort)(mLocation >> mGridYLog2);

                // Lets calculate each successor
                for (int i = 0; i < numDirections; i++) {
                    mNewLocationX = (mLocationX == 0 && mDirection[i, 0] < 0) ? (ushort)(mGridX - 1) : (ushort)(mLocationX + mDirection[i, 0]);
                    mNewLocationY = (ushort)(mLocationY + mDirection[i, 1]);

                    if (mNewLocationY >= mGridY)
                        continue;

                    if (mNewLocationX >= mGridX)
                        mNewLocationX = 0;

                    mNewLocation = (mNewLocationY << mGridYLog2) + mNewLocationX;

                    // mGridBit contains bitwise terrain capability (0=do not consider terrain type, 2=only ground, 4=only water)
                    float cellCost = 1f;
                    if (mGridBit > 0 && (mGrid[mNewLocation] & mGridBit) == 0) continue;

                    // Check custom validator
                    if (mCustomCosts != null) {
                        float customValue = mCustomCosts[mNewLocation];
                        if (customValue < 0 && OnCellCross != null) {
                            customValue = OnCellCross(mNewLocation);
                        }
                        if (customValue == 0) {
                            continue;
                        } else if (customValue < 0) {
                            customValue = 0;
                        }
                        cellCost += customValue;
                    }

                    // is diagonal?
                    if (i > 3) {
                        cellCost *= 1.4142135f;
                    }

                    mNewG = mCalcGrid[mLocation].G + cellCost;

                    if (mNewG > mMaxSearchCost)
                        continue;

                    // is it open or closed?
                    if (mCalcGrid[mNewLocation].Status == mOpenNodeValue || mCalcGrid[mNewLocation].Status == mCloseNodeValue) {
                        // The current node has less code than the previous? then skip this node
                        if (mCalcGrid[mNewLocation].G <= mNewG)
                            continue;
                    }

                    int distX = FastMath.Abs(mNewLocationX - end.X);
                    distX = Math.Min(distX, mGridX - distX); // world-wrapping
                    int distY = FastMath.Abs(mNewLocationY - end.Y);
                    switch (mFormula) {
                        default:
                        case HeuristicFormula.Manhattan:
                            mH = distX + distY;
                            break;
                        case HeuristicFormula.MaxDXDY:
                            mH = Math.Max(distX, distY);
                            break;
                        case HeuristicFormula.DiagonalShortCut:
                            float h_diagonal = Math.Min(distX, distY);
                            float h_straight = (distX + distY);
                            mH = 2 * h_diagonal + (h_straight - 2 * h_diagonal);
                            break;
                        case HeuristicFormula.Euclidean:
                            mH = Mathf.Sqrt(distX * distX + distY * distY);
                            break;
                    }

                    mCalcGrid[mNewLocation].PX = mLocationX;
                    mCalcGrid[mNewLocation].PY = mLocationY;
                    mCalcGrid[mNewLocation].G = mNewG;
                    mCalcGrid[mNewLocation].F = mNewG + mH;
                    mCalcGrid[mNewLocation].Status = mOpenNodeValue;

                    mOpen.Push(mNewLocation);
                }
                mCloseNodeCounter++;
                mCalcGrid[mLocation].Status = mCloseNodeValue;
            }

            if (mFound) {
                mClose.Clear();
                PathFinderNodeFast fNodeTmp = mCalcGrid[(end.Y << mGridYLog2) + end.X];
                totalCost = fNodeTmp.G;
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
                    int posX = fNode.PX;
                    int posY = fNode.PY;
                    fNodeTmp = mCalcGrid[(posY << mGridYLog2) + posX];
                    fNode.F = fNodeTmp.F;
                    fNode.G = fNodeTmp.G;
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

#if !UNITY_WSA
            [MethodImpl(256)] // equals to MethodImplOptions.AggressiveInlining
#endif
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
