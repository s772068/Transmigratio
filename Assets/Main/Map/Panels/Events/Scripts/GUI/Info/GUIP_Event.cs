using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GUIP_Event : MonoBehaviour, IGameConnecter {
    [SerializeField] private Image icon;
    [SerializeField] private Text eventName;
    [SerializeField] private Text description;
    [SerializeField] private Transform content;
    [SerializeField] private GUIE_EventResult resultPref;
    [SerializeField] private List<Sprite> iconSprites;
    
    private TimelineController timeline;
    private SettingsController settings;
    private EventsController events;
    private InfoController info;

    private List<GUIE_EventResult> elements = new();
    private I_Event _e;

    private bool _isShowInfo;

    public Action OnClickResult;
    public Action OnClose;

    public string Name { set => eventName.text = value; }
    public string Description { set => description.text = value; }
    public Sprite Icon { set => icon.sprite = value; }
    public GameController GameController {
        set {
            value.Get(out timeline);
            value.Get(out settings);
            value.Get(out events);
            value.Get(out info);
        }
    }

    public void Open(I_Event e) {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
            if (_isShowInfo) {
                _isShowInfo = false;
                info.EventInfo();
            }
            timeline.Pouse();
        }
        
        Clear();
        _e = e;
        for (int i = 0; i < e.CountResults; ++i) {
            if (e.CheckBuild(i)) CreateElement(i);
        }

        Theme();
        Localization();
    }

    public void CreateElement(int result) {
        GUIE_EventResult element = Instantiate(resultPref, content);
        element.Index = result;
        element.OnClick = ClickResult;
        elements.Add(element);
    }

    public void Localization() {
        Name = settings.Localization.Events[_e.Index].Name;
        Description = settings.Localization.Events[_e.Index].Description;
        for(int i = 0; i < elements.Count; ++i) {
            LocalizationElement(i, elements[i].Index);
        }
    }

    public void LocalizationElement(int index, int result) {
        elements[index].Name = settings.Localization.Events[_e.Index].Results[result].Name;
        elements[index].Description = settings.Localization.Events[_e.Index].Results[result].Description;
    }

    public void Theme() {
        Icon = settings.Theme.GetEventIcon(_e.Index);
    }

    public void ClickResult(int result) {
        OnClickResult?.Invoke();
        _e.Use(result);
        Clear();
        gameObject.SetActive(false);
    }

    private void Clear() {
        while (elements.Count > 0) {
            elements[0].DestroyGO();
            elements.RemoveAt(0);
        }
    }

    public void Close() {
        Clear();
        OnClose?.Invoke();
        _e.Use(0);
        gameObject.SetActive(false);
    }

    public void Init() {
        _isShowInfo = true;
        events.OnOpenPanel += Open;
    }

    private void OnDestroy() {
        Clear();
    }
}
