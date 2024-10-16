using Gameplay.Scenarios.Events.Data;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;
using UI;

using Events = Gameplay.Scenarios.Events;

public static class PanelFabric
{
    public static EventPanel CreateEvent(Transform parent, EventDesidion desidionPrefab, EventPanel eventPrefab, Events.Base baseEvent, Sprite sprite, string title, string territory, string description, List<IDesidion> desidions,
                        CivPiece piece = null, TM_Region region = null)
    {
        EventPanel panel = EventPanel.Instantiate(eventPrefab, parent);
        panel.Event = baseEvent;
        panel.Image = sprite;
        panel.Title = title;
        panel.Description.text = territory;
        panel.Description.text += "\n" + description;
        foreach (var desidion in desidions)
        {
            if (desidion is DesidionPiece desP)
                AddDesidion(desidionPrefab, piece, desP, panel.Desidions, panel);
            else if (desidion is DesidionRegion desR)
                AddDesidion(desidionPrefab, region, desR, panel.Desidions, panel);
        }

        return panel;
    }

    private static void AddDesidion(EventDesidion prefab, CivPiece piece, DesidionPiece desidion, Transform parent, EventPanel panel)
    {
        EventDesidion desidionObject = EventDesidion.Instantiate(prefab, parent);
        desidionObject.Init(piece, desidion);
        desidionObject.Close += panel.CloseWindow;
        desidionObject.Title = desidion.Title;
        desidionObject.Points = desidion.CostFunc(piece);
    }

    private static void AddDesidion(EventDesidion prefab, TM_Region region, DesidionRegion desidion, Transform parent, EventPanel panel)
    {
        EventDesidion desidionObject = EventDesidion.Instantiate(prefab, parent);
        desidionObject.Init(region, desidion);
        desidionObject.Close += panel.CloseWindow;
        desidionObject.Title = desidion.Title;
        desidionObject.Points = desidion.CostFunc(region);
    }

    public static void CreateNews(Transform parent, NewsPanel newsPrefab, NewsSO news)
    {
        NewsPanel panel = NewsPanel.Instantiate(newsPrefab, parent);
        panel.gameObject.SetActive(false);
        panel.Init(news);
    }
}
