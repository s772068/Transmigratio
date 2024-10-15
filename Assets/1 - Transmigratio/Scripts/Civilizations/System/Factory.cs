using System.Collections.Generic;
using UnityEngine;

namespace Civilizations {
    public static class Factory {
        public static Element Create(Element prefab, Transform parent, string title, Sprite icon) {
            Element newPref = Object.Instantiate(prefab, parent);
            newPref.name = title;
            newPref.Title = title;
            newPref.Icon = icon;
            return newPref;
        }
    }
}
