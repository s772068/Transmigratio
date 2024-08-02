using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class DragPanelControl : MonoBehaviour
{
    [SerializeField] private bool _horizontal = true;
    [SerializeField] private bool _vertical = true;
    [SerializeField] private DragElement _draggingObj;
    [SerializeField] private float _dragSpeed = 15;
    [SerializeField] private float _swap_gap = 10;
    private static event Action<List<DragElement>> EndDrag;
    public static event Action DragElementsSorted;

    private Vector2 _maxYPoints;

    [SerializeField]
    List<DragElement> _elements = new List<DragElement>();
    [SerializeField]
    List<Vector2> ElementsOriginPos = new List<Vector2>();

    public List<DragElement> Elements => _elements;

    private void Awake()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            _elements.Add(child.GetComponent<DragElement>());
            ElementsOriginPos.Add(child.transform.position);

            if (_elements.Count > i)
            {
                DragElement element = _elements[i].GetComponent<DragElement>();

                element.SortOrder = i;
            }
            else
            {
                _elements[i].gameObject.SetActive(false);
                _elements[i].GetComponent<DragElement>().SortOrder = 999 + i;
                i++;
                continue;
            }

            EventTrigger trigger = child.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            EventTrigger.Entry entry_dragging = new EventTrigger.Entry();
            EventTrigger.Entry entry_end = new EventTrigger.Entry();

            //entry.eventID = EventTriggerType.PointerDown;
            entry.eventID = EventTriggerType.InitializePotentialDrag;
            entry_dragging.eventID = EventTriggerType.Drag;
            entry_end.eventID = EventTriggerType.EndDrag;

            entry.callback.AddListener((eventdata) => { Element_OnSelected(); });
            entry_dragging.callback.AddListener((eventdata) => { SortWhenDragging(); });
            entry_end.callback.AddListener((eventdata) => { OnEndDrag(); });

            trigger.triggers.Add(entry);
            trigger.triggers.Add(entry_dragging);
            trigger.triggers.Add(entry_end);

            child.name = i++.ToString();
        }

        DragFingerControler.OnTouched += Dragging;
        RectTransform rect = GetComponent<RectTransform>();
        _maxYPoints.x = transform.position.y + rect.sizeDelta.y / 2;
        _maxYPoints.y = transform.position.y - rect.sizeDelta.y / 2;
    }
    private void OnDestroy()
    {
        DragFingerControler.OnTouched -= Dragging;
    }

    public void Dragging(Vector2 goal)
    {
        if (_draggingObj == null) { return; }
        Vector3 move = _draggingObj.transform.position;

        if (_horizontal)
            move.x = Mathf.Lerp(_draggingObj.transform.position.x, goal.x, _dragSpeed * Time.deltaTime);
        if (_vertical)
            move.y = Mathf.Lerp(_draggingObj.transform.position.y, goal.y, _dragSpeed * Time.deltaTime);
        /*
        Vector3 _move = new Vector3(
                        graggingObj.transform.position.x,
                        Mathf.Lerp(graggingObj.transform.position.y, _goal.y, gragSpeed * Time.deltaTime),
                        0);*/
        if (move.y > _maxYPoints.x)
            move.y = _maxYPoints.x;
        else if (move.y < _maxYPoints.y)
            move.y = _maxYPoints.y;

        _draggingObj.transform.position = move;
    }

    public void Element_OnSelected()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            return;

        DragElement selected = EventSystem.current.currentSelectedGameObject.GetComponent<DragElement>();

        if (selected == null)
            return;

        _draggingObj = selected;
    }

    public void SortWhenDragging()
    {
        foreach (DragElement element in _elements)
        {
            if (element == _draggingObj || _draggingObj == null) { continue; }

            if (Vector2.Distance(_draggingObj.transform.position, element.transform.position) < _swap_gap)
            {
                //Debug.Log(graggingObj.name + " swap " + _ui.name + " : " + Vector2.Distance(graggingObj.transform.position, _ui.transform.position));
                SwapOriginPos(_draggingObj, element);
                break;
            }


        }
    }

    void SwapOriginPos(DragElement dragable, DragElement swapElement)
    {

        dragable.transform.position = ElementsOriginPos[swapElement.SortOrder];
        swapElement.transform.position = ElementsOriginPos[dragable.SortOrder];

        string temp = dragable.SortOrder.ToString();
        dragable.name = swapElement.SortOrder.ToString();
        swapElement.name = temp;

        int temp_order = dragable.SortOrder;
        dragable.SortOrder = swapElement.SortOrder;
        swapElement.SortOrder = temp_order;

    }

    void OnEndDrag()
    {
        _draggingObj.transform.position = ElementsOriginPos[_draggingObj.SortOrder];
        List<DragElement> oldElements = new(_elements);
        _elements.Sort((x, y) => x.SortOrder.CompareTo(y.SortOrder));

        for (int i = 0; i < oldElements.Count; i++)
        {
            if (oldElements[i] != _elements[i])
            {
                DragElementsSorted?.Invoke();
                break;
            }
        }

        _draggingObj = null;

        EndDrag?.Invoke(_elements);
    }


}
