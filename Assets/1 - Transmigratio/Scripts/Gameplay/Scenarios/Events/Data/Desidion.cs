using System;

namespace Gameplay.Scenarios.Events.Data {

    public interface IDesidion { 
        public string Title { get; }
    }

    public struct DesidionPiece : IDesidion {
        public string Title;
        public Func<CivPiece, Func<CivPiece, int>, bool> ActionClick;
        public Func<CivPiece, int> CostFunc;

        public DesidionPiece(Func<CivPiece, Func<CivPiece, int>, bool> action, string title, Func<CivPiece, int> cost) {
            ActionClick = action;
            Title = title;
            CostFunc = cost;
        }

        string IDesidion.Title => Title;
    }

    public struct DesidionRegion : IDesidion {
        public string Title;
        public Func<TM_Region, Func<TM_Region, int>, bool> ActionClick;
        public Func<TM_Region, int> CostFunc;

        public DesidionRegion(Func<TM_Region, Func<TM_Region, int>, bool> action, string title, Func<TM_Region, int> cost)
        {
            ActionClick = action;
            Title = title;
            CostFunc = cost;
        }

        string IDesidion.Title => Title;
    }
}
