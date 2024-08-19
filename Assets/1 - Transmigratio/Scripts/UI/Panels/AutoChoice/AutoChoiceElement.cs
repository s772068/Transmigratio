using System;
using TMPro;
using UnityEngine;

namespace Gameplay.Scenarios.Events {
    public class AutoChoiceElement : MonoBehaviour {
        [SerializeField] private TMP_Text _title;
        private Base _event;

        public static event Action<Base> SelectElement;

        public void Init(Base newEvent) {
            if (_event != null)
                return;

            _event = newEvent;
            _title.text = _event.Local("Title");
        }

        public void Select() => SelectElement?.Invoke(_event);
    }
}
