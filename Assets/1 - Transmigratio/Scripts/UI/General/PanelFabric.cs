using Gameplay.Scenarios.Events.Data;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;
using UI;

using Events = Gameplay.Scenarios.Events;

public static class PanelFabric
{
    public static void CreateEvent(Transform parent, EventDesidion desidionPrefab, EventPanel eventPrefab, Events.Base baseEvent,
                        CivPiece piece, Sprite sprite, string title, string territory, string description, List<Desidion> desidions)
    {
        EventPanel panel = EventPanel.Instantiate(eventPrefab, parent);
        panel.Event = baseEvent;
        panel.Image = sprite;
        panel.Title = title;
        panel.Territory = territory;
        panel.Description = description;
        foreach (var desidion in desidions)
            AddDesidion(desidionPrefab, piece, desidion, panel.Desidions, panel);
    }

    private static void AddDesidion(EventDesidion prefab, CivPiece piece, Desidion desidion, Transform parent, EventPanel panel)
    {
        EventDesidion desidionObject = EventDesidion.Instantiate(prefab, parent);
        desidionObject.Init(piece, desidion);
        desidionObject.ActionClick += desidion.ActionClick;
        desidionObject.Close += panel.CloseWindow;
        desidionObject.Title = desidion.Title;
        desidionObject.Points = desidion.CostFunc(piece);
    }

    public static void CreateNews(Transform parent, NewsPanel newsPrefab, NewsSO news)
    {
        NewsPanel panel = NewsPanel.Instantiate(newsPrefab, parent);
        panel.Init(news);
    }
}
