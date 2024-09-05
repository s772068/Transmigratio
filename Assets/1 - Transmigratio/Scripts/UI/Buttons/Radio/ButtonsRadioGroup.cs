using System.Collections.Generic;
using UnityEngine;
using System;

namespace UI
{
    public class ButtonsRadioGroup : MonoBehaviour
    {
        [SerializeField] private List<ButtonRadio> buttons;
        [SerializeField] private bool _waitGameStart = false;

        private int _activeElement = -1;

        public Action<int> onClick;

        private void Start() {
            for (int i = 0; i < buttons.Count; ++i) {
                buttons[i].Index = i;
                buttons[i].onClick.AddListener(Select);
            }

            if (_waitGameStart) {
                foreach (var button in buttons) {
                    button.Deactivate();
                    button.IsInterectable = false;
                }
                Transmigratio.GameStarted += OnGameStarted;
            }
        }

        private void OnDestroy() {
            Transmigratio.GameStarted -= OnGameStarted;
        }

        public void Select(int newActiveIndex) {
            if (_waitGameStart)
                return;

            if (_activeElement != -1) buttons[_activeElement].Deactivate();
            _activeElement = newActiveIndex;
            buttons[_activeElement].Activate();
            onClick?.Invoke(_activeElement);
        }

        private void OnGameStarted() {
            _waitGameStart = false;
            foreach (var button in buttons)
                button.IsInterectable = true;
        }
    }
}
