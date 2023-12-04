using System;

namespace WorldMapStrategyKit.PathFinding {
    public struct Point {
        public int X;
        public int Y;

        public Point(int x, int y) {
            this.X = x;
            this.Y = y;
        }

        // For debugging
        public override string ToString() {
            return string.Format("{0}, {1}", this.X, this.Y);
        }
    }
}
