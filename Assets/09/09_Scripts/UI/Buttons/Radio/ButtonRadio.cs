using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class ButtonRadio : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite deactiveSprite;
    [SerializeField] private Sprite highlightedSprite;

    [Space]
    public UnityEvent<int> onClick = new UnityEvent<int>();

    private bool isActive;
    private Image image;

    public int Index { private get; set; }
    private void Awake() {
        image = GetComponent<Image>();
    }

    public void Activate() {
        isActive = true;
        image.sprite = activeSprite;
    }

    public void Deactivate() {
        isActive = false;
        image.sprite = deactiveSprite;
    }

    public void OnPointerClick(PointerEventData eventData) {
        onClick.Invoke(Index);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(highlightedSprite == null) return;
        if (!isActive) image.sprite = highlightedSprite;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (highlightedSprite == null) return;
        if (isActive) Activate();
        else Deactivate();
    }
}
