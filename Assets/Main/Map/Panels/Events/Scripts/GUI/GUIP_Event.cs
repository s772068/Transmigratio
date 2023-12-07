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
    
    private List<GUIE_EventResult> results = new();

    [HideInInspector] public int index;

    private TimelineController timeline;

    public Action<int> OnClickResult;
    public Action OnClose;

    public string Name { set => eventName.text = value; }
    public string Description { set => description.text = value; }
    public Sprite Icon { set => icon.sprite = value; }
    public GameController GameController {
        set {
            value.Get(out timeline);
        }
    }

    public void Open() {

    }

    public void Localization() {

    }

    public void Init(S_Event data) {
        Clear();
        Name = data.Name;
        Description = data.Description;
        Icon = iconSprites[data.IconIndex];
        timeline.Pouse();
    }

    public GUIE_EventResult Build(S_EventResult data, int resultIndex) {
        GUIE_EventResult result = Instantiate(resultPref, content);
        result.Index = resultIndex;
        result.Init(data);
        result.OnClick = ClickResult;
        results.Add(result);
        return result;
    }

    public void ClickResult(int resultIndex) {
        OnClickResult?.Invoke(resultIndex);
        Clear();
        OnClose?.Invoke();
        gameObject.SetActive(false);
        timeline.Play();
    }

    private void Clear() {
        for(int i = 0; i < results.Count; ++i) {
            results[i].DestroyGO();
        }
        results.Clear();
    }

    public void Close() {
        timeline.Play();
        Clear();
        OnClose?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        Clear();
    }

    public void Init() {
    }
}
