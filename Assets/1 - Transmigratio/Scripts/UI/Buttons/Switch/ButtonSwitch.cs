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

    private bool _isActive;
    private Image _image;

    private bool IsActive => _isActive;

    private void Awake() {
        _image = GetComponent<Image>();
    }

    private void Start() { }

    public void OnPointerClick(PointerEventData eventData) => Click();

    public void OnPointerEnter(PointerEventData eventData) {
        if (highlightedSprite == null) return;
        _image.sprite = highlightedSprite;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (highlightedSprite == null) return;
        if (_isActive && activeSprite != null) _image.sprite = activeSprite;
        else if (deactiveSprite != null) _image.sprite = deactiveSprite;
    }

    public void Click() {
        if (onGroupClick != default) {
            onGroupClick?.Invoke(this);
        } else if (_isActive) {
            _isActive = false;
            Deactivate();
        } else {
            _isActive = true;
            Activate();
        }
    }

    public void Activate() {
        onActivate.Invoke();
        if (activeSprite != null) _image.sprite = activeSprite;
    }

    public void Deactivate() {
        onDeactivate.Invoke();
        if (deactiveSprite != null) _image.sprite = deactiveSprite;
    }
}
