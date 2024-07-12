using UnityEngine;
using TMPro;

public class Messanger : MonoBehaviour {
    [SerializeField] private TMP_Text _text;

    public string Message { set => _text.text = value; }

    public void OnClickClose() {
        gameObject.SetActive(false);
    }
}
