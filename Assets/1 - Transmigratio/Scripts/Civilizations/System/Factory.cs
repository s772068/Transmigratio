using UnityEngine;

namespace Civilizations {
    public static class Factory {
        public static Element Create(Element prefab, Transform parent, Element.Data data) {
            Element newPref = Object.Instantiate(prefab, parent);
            newPref.name = data.title;
            newPref.Title = data.title;
            newPref.Icon = data.icon;
            return newPref;
        }
    }
}
