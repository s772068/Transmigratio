using System;
using System.Collections.Generic;
using System.Text;

namespace WorldMapStrategyKit.PathFinding {
    interface IPathFinder {

        HeuristicFormula Formula {
            get;
            set;
        }

        float MaxSearchCost {
            get;
            set;
        }

        int MaxSteps {
            get;
            set;
        }

        List<PathFinderNode> FindPath(Point start, Point end, out float totalCost);

    }
}
