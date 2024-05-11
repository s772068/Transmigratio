using System.Collections.Generic;
using UnityEngine;

public class ButtonsGroup : MonoBehaviour {
    [SerializeField] private List<ButtonRadio> buttons;

    private int activeIndex = -1;

    private void Awake() {
        for(int i = 0; i < buttons.Count; ++i) {
            buttons[i].Index = i;
            buttons[i].onClick.AddListener(OnClick);
        }
    }

    private void OnClick(int newActiveIndex) {
        if(activeIndex != -1) buttons[activeIndex].Deactivate();
        activeIndex = newActiveIndex;
    }
}
