using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

[RequireComponent(typeof(Image))]
public class GUI_ParamsGroup : MonoBehaviour {
    [SerializeField] private Text label;
    [SerializeField] private Transform content;
    [SerializeField] private GUI_Param paramPref;

    private List<GUI_Param> paramList = new();
    private Image background;

    public Action<int, int> OnClickParam;

    public GUI_Param GetParam(int index) => paramList[index];
    public int Index { private get; set; }
    public string Label { set => label.text = value; }
    public Color BackgroundColor { set => background.color = value; }

    public float Height {
        set {
            Vector2 size = GetComponent<RectTransform>().sizeDelta;
            size.y = value;
            GetComponent<RectTransform>().sizeDelta = size;
        }
    }

    public void Init(Data data) {
        Label = data.Label;
        Height = data.Height;
        BackgroundColor = data.BackgroundColor;
        for(int i = 0; i < data.Params.Length; ++i) {
            paramList.Add(Build(data.Params[i]));
        }
    }

    private GUI_Param Build(GUI_Param.Data data) {
        GUI_Param param = Instantiate(paramPref, content);
        param.Index = paramList.Count;
        param.Init(data);
        param.OnClick = ClickParam;
        return param;
    }

    private void ClickParam(int paramIndex) => OnClickParam?.Invoke(Index, paramIndex);

    public void Clear() {
        while(paramList.Count > 0) {
            paramList[0].DestroyGO();
            paramList.RemoveAt(0);
        }
    }

    public void DestroyGO() {
        Destroy(gameObject);
    }

    private void Awake() {
        background = GetComponent<Image>();
    }

    private void OnDestroy() {
        Clear();
    }

    [Serializable]
    public struct Data {
        public string Label;
        public float Height;
        public Vector4 BackgroundColor;
        public GUI_Param.Data[] Params;
    }
}
