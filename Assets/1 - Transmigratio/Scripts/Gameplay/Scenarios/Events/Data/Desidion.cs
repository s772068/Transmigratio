using System;

namespace Gameplay.Scenarios.Events.Data {
    public struct Desidion {
        public string Title;
        public Func<CivPiece, Func<CivPiece, int>, bool> ActionClick;
        public Func<CivPiece, int> CostFunc;

        public Desidion(Func<CivPiece, Func<CivPiece, int>, bool> action, string title, Func<CivPiece, int> cost) {
            ActionClick = action;
            Title = title;
            CostFunc = cost;
        }
    }
}
