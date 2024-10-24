using UnityEngine;
using System;

public class TutorialPanel : MonoBehaviour {
    public static event Action<string> onClick;

    public void OnClick(string name) {
        onClick?.Invoke(name);
    }
}
