using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Chronicles.Prefabs {
    public class Consequence : MonoBehaviour {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text txt_title;
        [SerializeField] private TMP_Text txt_description;

        public void Init(Data.Panel.Element data) {
            image.sprite = data.sprite;
            txt_title.text = data.eventName;
            txt_description.text = data.description;
        }
    }
}
