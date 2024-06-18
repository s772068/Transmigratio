using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class ButtonSwitch : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Sprite deactiveSprite;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite highlightedSprite;

    [Space]
    public UnityEvent onActivate = new UnityEvent();
    public UnityEvent onDeactivate = new UnityEvent();

    private bool isActive;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
    }

    private void Start() { }

    public void OnPointerClick(PointerEventData eventData) {
        if (isActive) {
            isActive = false;
            onDeactivate.Invoke();
            image.sprite = deactiveSprite;
        } else {
            isActive = true;
            onActivate.Invoke();
            image.sprite = activeSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        image.sprite = highlightedSprite;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (isActive) image.sprite = activeSprite;
        else image.sprite = deactiveSprite;
    }
}
