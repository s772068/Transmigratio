using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitchGroup : MonoBehaviour {
    [SerializeField] private int maxSelected;
    [SerializeField] private List<ButtonSwitch> buttons;
    
    private List<ButtonSwitch> _selected = new();

    private void Awake() {
        for(int i = 0; i < buttons.Count; ++i) {
            buttons[i].onGroupClick = OnClick;
        }
    }

    private void OnClick(ButtonSwitch button) {
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
