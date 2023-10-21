using UnityEngine.EventSystems;
using UnityEngine;

public class GUI_BasePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    // [SerializeField] private bool shouldReturn;
    
    private Vector2 startMousePosition;
    private Vector2 startDragPosition;

    public void OnPointerDown(PointerEventData eventData) {
        startDragPosition = transform.position;
        startMousePosition = eventData.pointerCurrentRaycast.screenPosition;
    }

    public void OnPointerUp(PointerEventData eventData) {
    //    if (shouldReturn) transform.position = startDragPosition;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = startDragPosition + (eventData.pointerCurrentRaycast.screenPosition - startMousePosition);
    }
}
