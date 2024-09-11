using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace RegionDetails.Defoult.Descriptions {
    public class Icon : MonoBehaviour {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _label;

        public Sprite Sprite { set => _image.sprite = value; }
        public string Label { set => _label.text = value; }
    }
}
