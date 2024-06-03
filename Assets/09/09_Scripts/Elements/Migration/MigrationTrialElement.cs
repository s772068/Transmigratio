using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class MigrationTrialElement : MonoBehaviour {
    private Image image;
    public Sprite Sprite { set => image.sprite = value; }
    private void Awake() {
        image = GetComponent<Image>();
    }

    public void Destroy() {
        Destroy(gameObject);
    }
}
