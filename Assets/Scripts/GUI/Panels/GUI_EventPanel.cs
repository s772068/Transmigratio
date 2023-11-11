using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GUI_EventPanel : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private Text eventName;
    [SerializeField] private Text description;
    [SerializeField] private Transform content;
    [SerializeField] private GUI_EventResult resultPref;
    [SerializeField] private List<Sprite> iconSprites;
    
    private List<GUI_EventResult> results = new();

    [HideInInspector] public int index;

    public Action<int> OnClickResult;
    public Action OnClose;

    public string Name { set => eventName.text = value; }
    public string Description { set => description.text = value; }
    public Sprite Icon { set => icon.sprite = value; }

    public void Init(S_Event data) {
        Name = data.Name;
        Description = data.Description;
        Icon = iconSprites[data.IconIndex];
    }

    public GUI_EventResult Build(S_EventResult data, int resultIndex) {
        GUI_EventResult result = Instantiate(resultPref, content);
        result.Index = resultIndex;
        result.Init(data);
        result.OnClick = ClickResult;
        return result;
    }

    public void ClickResult(int resultIndex) {
        OnClickResult?.Invoke(resultIndex);
        Clear();
        gameObject.SetActive(false);
    }

    private void Clear() {
        while (results.Count > 0) {
            results[0].DestroyGO();
            results.RemoveAt(0);
        }
    }

    public void Close() {
        Clear();
        OnClose?.Invoke();
    }

    private void OnDestroy() {
        Clear();
    }
}
