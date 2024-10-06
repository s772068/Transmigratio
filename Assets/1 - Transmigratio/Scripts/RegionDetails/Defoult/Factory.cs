using UnityEngine;

namespace RegionDetails.Defoult {
    public static class Factory {
        public static Panel Create(Panel prefab, Transform parent) {
            Panel res = Object.Instantiate(prefab, parent);
            return res;
        }

        public static Paramiters.Element Create(Paramiters.Element prefab, Transform parent, string name, Sprite pictogram) {
            Paramiters.Element res = Object.Instantiate(prefab, parent);
            res.Name = name;
            res.Pictogram = pictogram;
            return res;
        }

        public static Elements.Element Create(Elements.Element prefab, Transform parent, int index, string paramiter, string label, float value) {
            Elements.Element res = Object.Instantiate(prefab, parent);
            res.Init(index, paramiter, label, value);
            res.name = label;
            return res;
        }

        public static Descriptions.Defoult Create(Descriptions.Defoult prefab, Transform parent, string desctiption) {
            Descriptions.Defoult res = Object.Instantiate(prefab, parent);
            res.Desctiption = desctiption;
            return res;
        }

        public static Descriptions.Civilization Create(Descriptions.Civilization prefab, Transform parent, string civName, Sprite avatar, string descriptionCiv, string description, System.Action onClickLink, System.Action onClickInfluence) {
            Descriptions.Civilization res = Object.Instantiate(prefab, parent);
            res.Label = civName;
            res.Avatar = avatar;
            res.DescriptionCiv = descriptionCiv;
            res.Description = description;
            res.onClickLink = onClickLink;
            res.onClickInfluence = onClickInfluence;
            return res;
        }
    }
}
