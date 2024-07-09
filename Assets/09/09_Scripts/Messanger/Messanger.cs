using UnityEngine;
using TMPro;

public class Messanger : MonoBehaviour {
    [SerializeField] private TMP_Text text;

    public string Message { set => text.text = value; }

    public void OnClickClose() {
        gameObject.SetActive(false);
    }
}
