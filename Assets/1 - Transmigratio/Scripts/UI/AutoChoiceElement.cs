using System;
using TMPro;
using UnityEngine;

public class AutoChoiceElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    private Events.Controllers.Base _event;

    public static event Action<Events.Controllers.Base> SelectElement;

    public void Init(Events.Controllers.Base newEvent)
    {
        if (_event != null)
            return;

        _event = newEvent;
        _title.text = _event.Local("Title");
    }

    public void Select() => SelectElement?.Invoke(_event);
}
