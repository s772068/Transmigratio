using UnityEngine;
using UnityEngine.UI;

namespace Chronicles.Prefabs {
    public static class Factory {
        public static Panel.Element CreatePanelElement(Transform parent, Panel.Element prefab, Data.Panel.Element data, ToggleGroup group) {
            Panel.Element element = Object.Instantiate(prefab, parent);
            element.transform.SetAsFirstSibling();
            element.Toggle.group = group;
            element.Data = data;
            return element;
        }
    }
}
