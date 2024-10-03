using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace RegionDetails.Defoult.Descriptions {
    public class Civilization : BaseDescription {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _avatar;
        [SerializeField] private TMP_Text _descriptionCiv;
        [SerializeField] private TMP_Text _description;

        public Action onClickLink;
        public Action onClickInfluence;

        public string Label { set =>  _label.text = value; }
        public Sprite Avatar { set => _avatar.sprite = value; }
        public string DescriptionCiv { set =>  _descriptionCiv.text = value; }
        public string Description { set =>  _description.text = value; }

        public void ClickLink() => onClickLink?.Invoke();
        public void ClickInfluence() => onClickInfluence?.Invoke();
    }
}
