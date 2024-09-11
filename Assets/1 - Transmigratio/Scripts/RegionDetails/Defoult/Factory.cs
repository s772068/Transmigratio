using UnityEngine;

namespace RegionDetails.Defoult {
    public static class Factory {
        public static Panel Create(Panel prefab, Transform parent) {
            return null;
        }

        public static Paramiters.Element Create(Paramiters.Element prefab, Transform parent, string name, Sprite pictogram) {
            Paramiters.Element res = Object.Instantiate(prefab, parent);
            res.Name = name;
            res.Pictogram = pictogram;
            return res;
        }

        public static Elements.Element Create(Elements.Element prefab, Transform parent, int index, string label, float percent) {
            Elements.Element res = Object.Instantiate(prefab, parent);
            res.Index = index;
            res.Label = label;
            res.Percent = percent;
            return res;
        }

        public static Descriptions.Defoult Create(Descriptions.Defoult prefab, Transform parent) {
            Descriptions.Defoult res = Object.Instantiate(prefab, parent);
            return null;
        }

        public static Descriptions.Civilization Create(Descriptions.Civilization prefab, Transform parent) {
            Descriptions.Civilization res = Object.Instantiate(prefab, parent);
            return null;
        }
    }
}
