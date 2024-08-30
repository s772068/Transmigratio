using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Scenarios.Events {
    public class AutoChoiceElement : MonoBehaviour {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _priority;
        private Base _event;

        public static event Action<Base> SelectElement;

        public void Init(Base newEvent) {
            if (_event != null)
                return;

            _event = newEvent;
            _title.text = _event.Local("Title");
            _image.sprite = newEvent.PanelSprite;
        }

        public void Select() => SelectElement?.Invoke(_event);
    }
}
