using UnityEngine;
using Events.Data;
using System.Collections.Generic;

public static class PanelFabric
{
    public static void CreateEvent(Transform parent, EventDesidion desidionPrefab, EventPanel eventPrefab, bool showAgain,
                        Sprite sprite, string title, string territory, string description, List<Desidion> desidions)
    {
        EventPanel panel = EventPanel.Instantiate(eventPrefab, parent);
        panel.IsShowAgain = showAgain;
        panel.Image = sprite;
        panel.Title = title;
        panel.Territory = territory;
        panel.Description = description;
        foreach (var desidion in desidions)
            AddDesidion(desidionPrefab, desidion, panel.Desidions, panel);
    }

    private static void AddDesidion(EventDesidion prefab, Desidion desidion, Transform parent, EventPanel panel)
    {
        EventDesidion desidionObject = EventDesidion.Instantiate(prefab, parent);
        desidionObject.ActionClick = desidion.ActionClick;
        desidionObject.ActionClick += panel.CloseWindow;
        desidionObject.Title = desidion.Title;
        desidionObject.Points = desidion.Cost();
    }
}
