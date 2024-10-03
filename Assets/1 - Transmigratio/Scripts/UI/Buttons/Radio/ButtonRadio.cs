using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class ButtonRadio : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite deactiveSprite;
    [SerializeField] private Sprite highlightedSprite;
    
    public bool IsInterectable = true;

    [Space]
    public UnityEvent<int> onSelect = new UnityEvent<int>();
    public UnityEvent<int> onUnselect = new UnityEvent<int>();

    private Image _image;
    private bool _isSelected;

    public int Index { private get; set; }
    
    private void Awake() {
        _image ??= GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData) => Click();

    public void OnPointerEnter(PointerEventData eventData) {
        if (highlightedSprite == null) return;
        if (!_isSelected) _image.sprite = highlightedSprite;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (highlightedSprite == null) return;
        if (_isSelected) Activate();
        else Deactivate();
    }

    public void Click() {
        if (IsInterectable)
            onSelect.Invoke(Index);
    }

    public void Activate() {
        _isSelected = true;
        _image.sprite = activeSprite;
    }

    public void Deactivate() {
        _isSelected = false;
        _image.sprite = deactiveSprite;
    }
}
