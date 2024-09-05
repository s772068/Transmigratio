using System.Collections.Generic;
using UnityEngine;

namespace RegionDetails.StartGame {
    public static class Factory {
        public static Panel CreateController(Panel prefab, Transform parent) {
            return Object.Instantiate(prefab, parent);
        }

        public static List<Element> CreateElements(Element prefab, Transform parent, string paramiter) {
            List<Element> elements = new();
            Dictionary<string, float> dic = Transmigratio.Instance.TMDB.GetParam(MapData.RegionID, paramiter);
            foreach(var pair in dic) {
                elements.Add(CreateElement(prefab, parent, paramiter, pair.Key, pair.Value));
            }
            return elements;
        }

        public static Element CreateElement(Element prefab, Transform parent, string paramiter, string name, float value) {
            Element element = Object.Instantiate(prefab, parent);
            element.Label = name;
            element.Percent = value;
            element.Color = Transmigratio.Instance.TMDB.GetParamiterColor(paramiter, name);
            return element;
        }
    }
}
