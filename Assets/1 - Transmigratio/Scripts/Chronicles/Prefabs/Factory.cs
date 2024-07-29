using UnityEngine;

namespace Chronicles.Prefabs {
    public static class Factory {
        public static Panel.Element CreatePanelElement(Transform parent, Panel.Element prefab, Data.Panel.Element data) {
            Panel.Element element = Object.Instantiate(prefab, parent);
            element.Data = data;
            return element;
        }
    }
}
