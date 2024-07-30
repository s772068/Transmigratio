using System;

namespace Events.Data
{
    public struct Desidion
    {
        public Func<Func<int>, bool> ActionClick;
        public string Title;
        public Func<int> Cost;

        public Desidion(Func<Func<int>, bool> action, string title, Func<int> cost)
        {
            ActionClick = action;
            Title = title;
            Cost = cost;
        }
    }
}
