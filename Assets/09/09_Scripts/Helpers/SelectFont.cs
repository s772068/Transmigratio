using UnityEngine;
using TMPro;

public class SelectFont : MonoBehaviour {
    [SerializeField] private TMP_FontAsset[] fonts;
    [SerializeField] private int selectedIndex;

    private void Awake() {
        Destroy(gameObject);
    }

    public void Select() {
        TMP_Text[] texts = FindObjectsOfType<TMP_Text>();
        for(int i = 0; i < texts.Length; ++i) {
            texts[i].font = fonts[selectedIndex];
        }
    }
}
