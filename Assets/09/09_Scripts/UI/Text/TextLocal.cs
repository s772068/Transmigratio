using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextLocal : MonoBehaviour {
    [SerializeField] private string key;
    private void Awake() {
        if (key == "") return;
        GetComponent<TMP_Text>().text = StringLoader.Load(key);
    }
}
