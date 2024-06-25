using System.Collections.Generic;
using UnityEngine;
using System;

public class ButtonsRadioGroup : MonoBehaviour {
    [SerializeField] private List<ButtonRadio> buttons;

    private int activeElement = -1;
    
    public Action<int> onClick;

    private void Awake() {
        for(int i = 0; i < buttons.Count; ++i) {
            buttons[i].Index = i;
            buttons[i].onClick.AddListener(Click);
        }
    }

    public void Click(int newActiveIndex) {
        if (activeElement != -1) buttons[activeElement].Deactivate();
        activeElement = newActiveIndex;
        buttons[activeElement].Activate();
        onClick?.Invoke(activeElement);
    }
}
