using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitchGroup : MonoBehaviour {
    [SerializeField] private int maxSelected;
    [SerializeField] private List<ButtonSwitch> buttons;
    
    private List<ButtonSwitch> selected = new();

    private void Awake() {
        for(int i = 0; i < buttons.Count; ++i) {
            buttons[i].onGroupClick = OnClick;
        }
    }

    private void OnClick(ButtonSwitch button) {
        if (selected.Contains(button)) {
            button.Deactivate();
            selected.Remove(button);
        } else {
            if (selected.Count == maxSelected) {
                selected[selected.Count - 1].Deactivate();
                selected.RemoveAt(selected.Count - 1);
            }
            button.Activate();
            selected.Insert(0, button);
        }
    }
}
