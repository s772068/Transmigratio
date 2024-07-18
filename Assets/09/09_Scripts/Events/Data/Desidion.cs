using System;

namespace Events.Data
{
    public struct Desidion
    {
        public Action ActionClick;
        public string Title;
        public Func<int> Cost;

        public Desidion(Action action, string title, Func<int> cost)
        {
            ActionClick = action;
            Title = title;
            Cost = cost;
        }
    }
}
