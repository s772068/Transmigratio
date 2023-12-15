using UnityEngine.UI;
using UnityEngine;
using System;

public class GUIP_Info : GUI_BasePanel {
    [SerializeField] private Text infoTxt;

    public Action OnOpen;
    public Action OnClose;

    public string Info { set => infoTxt.text = value; }

    public void Open() {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
            OnOpen?.Invoke();
        }
    }

    public void Close() {
        OnClose?.Invoke();
        gameObject.SetActive(false);
    }
}
