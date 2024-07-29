using System;

namespace Events.Data
{
    public struct Desidion
    {
        public Action<Func<int>> ActionClick;
        public string Title;
        public Func<int> Cost;

        public Desidion(Action<Func<int>> action, string title, Func<int> cost)
        {
            ActionClick = action;
            Title = title;
            Cost = cost;
        }
    }
}
