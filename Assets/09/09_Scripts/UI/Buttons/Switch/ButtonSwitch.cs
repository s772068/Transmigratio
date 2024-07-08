using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using System;

[RequireComponent(typeof(Image))]
public class ButtonSwitch : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Sprite deactiveSprite;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite highlightedSprite;

    [Space]
    public UnityEvent onActivate = new UnityEvent();
    public UnityEvent onDeactivate = new UnityEvent();

    public Action<ButtonSwitch> onGroupClick = default;

    private bool isActive;
    private Image image;

    private bool IsActive => isActive;

    private void Awake() {
        image = GetComponent<Image>();
    }

    private void Start() { }

    public void OnPointerClick(PointerEventData eventData) {
        if (onGroupClick != default) {
            onGroupClick?.Invoke(this);
        } else if (isActive) {
            isActive = false;
            Deactivate();
        } else {
            isActive = true;
            Activate();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (highlightedSprite == null) return;
        image.sprite = highlightedSprite;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (highlightedSprite == null) return;
        if (isActive && activeSprite != null) image.sprite = activeSprite;
        else if(deactiveSprite != null) image.sprite = deactiveSprite;
    }

    public void Activate() {
        onActivate.Invoke();
        if (activeSprite != null) image.sprite = activeSprite;
    }

    public void Deactivate() {
        onDeactivate.Invoke();
        if (deactiveSprite != null) image.sprite = deactiveSprite;
    }
}
