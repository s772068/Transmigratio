using Gameplay.Scenarios.Events.Data;
using System;
using System.Collections.Generic;
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
            UpdateTextPriority(newEvent.Desidions);
        }

        public void Select() => SelectElement?.Invoke(_event);

        public void UpdateTextPriority(List<IDesidion> desidions)
        {
            _priority.text = "";
            for (int i = 0; i < desidions.Count; ++i)
            {
                _priority.text += desidions[i].Title;
                if (i + 1 != desidions.Count)
                    _priority.text += " > ";
            }
        }
    }
}
