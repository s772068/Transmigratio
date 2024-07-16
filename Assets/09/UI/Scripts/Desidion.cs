using System;
public struct Desidion
{
    public Action ActionClick;
    public string Title;
    public int Cost;

    public Desidion(Action action, string title, int cost)
    {
        ActionClick = action;
        Title = title;
        Cost = cost;
    }
}
