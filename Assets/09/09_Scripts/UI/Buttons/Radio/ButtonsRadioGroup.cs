using System.Collections.Generic;
using UnityEngine;
using System;

public class ButtonsRadioGroup : MonoBehaviour {
    [SerializeField] private List<ButtonRadio> buttons;

    private int _activeElement = -1;
    
    public Action<int> onClick;

    private void Awake() {
        for(int i = 0; i < buttons.Count; ++i) {
            buttons[i].Index = i;
            buttons[i].onClick.AddListener(Click);
        }
    }

    public void Click(int newActiveIndex) {
        if (_activeElement != -1) buttons[_activeElement].Deactivate();
        _activeElement = newActiveIndex;
        buttons[_activeElement].Activate();
        onClick?.Invoke(_activeElement);
    }
}
