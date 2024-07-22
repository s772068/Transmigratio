using System.Collections.Generic;
using UnityEngine;

namespace UI.Buttons.Switch {
    public class Group : MonoBehaviour {
        [SerializeField] private int maxSelected;
        [SerializeField] private List<Button> buttons;

        private List<Button> _selected = new();

        private void Awake() {
            for (int i = 0; i < buttons.Count; ++i) {
                buttons[i].onGroupClick = OnClick;
            }
        }

        private void OnClick(Button button) {
            if (_selected.Contains(button)) {
                button.Deactivate();
                _selected.Remove(button);
            } else {
                if (_selected.Count == maxSelected) {
                    _selected[_selected.Count - 1].Deactivate();
                    _selected.RemoveAt(_selected.Count - 1);
                }
                button.Activate();
                _selected.Insert(0, button);
            }
        }
    }
}
