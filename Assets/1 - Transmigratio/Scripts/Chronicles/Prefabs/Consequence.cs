using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;

namespace Chronicles.Prefabs {
    public class Consequence : MonoBehaviour {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private LocalizeStringEvent _description;
        [SerializeField] private TMP_Text _result;

        public void Init(Data.Panel.Element data) {
            image.sprite = data.Sprite;
            _title.text = data.EventName;
            _description.SetEntry(data.DescriptionName);
            _result.text = $"{Localization.Load("Chronicles", "Desidion")}{data.Desidion}\n {Localization.Load("Chronicles", "Result")}{data.Result}";
        }
    }
}
