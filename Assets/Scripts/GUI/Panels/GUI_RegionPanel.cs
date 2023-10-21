using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GUI_RegionPanel : GUI_BasePanel {
    [Header("Text")]
    [SerializeField] private Text countryName;
    [SerializeField] private Text closeTxt;
    [Header("Other")]
    [SerializeField] private Transform content;
    [SerializeField] private Button closeBtn;
    [SerializeField] private GUI_ParamsGroup groupPref; // ToDo: Move to holder

    private List<GUI_ParamsGroup> groupList = new();

    public Action<int, int> OnClickParam;
    public Action OnClose;

    public GUI_ParamsGroup GetGroup(int index) => groupList[index];
    public GUI_Param GetParam(int groupIndex, int paramIndex) => groupList[groupIndex].GetParam(paramIndex);
    public PrefabsHolder PrefabsHolder { private get; set; }
    public string CountryName { set => countryName.text = value; }
    public string CloseString { set => closeTxt.text = value; }

    public void Init(Data data, PrefabsHolder prefabsHolder) {
        CountryName = data.CountryName;
        CloseString = data.CloseString;
        for (int i = 0; i < data.Groups.Length; ++i) {
            groupList.Add(Build(data.Groups[i], prefabsHolder));
        }
    }

    private GUI_ParamsGroup Build(GUI_ParamsGroup.Data data, PrefabsHolder prefabsHolder) {
        GUI_ParamsGroup group = Instantiate(prefabsHolder.ParamsGroup, content);
        group.Index = groupList.Count;
        group.Init(data, prefabsHolder);
        group.OnClickParam = ClickParam;
        return group;
    }

    private void ClickParam(int groupIndex, int paramIndex) => print(groupIndex + " | " + paramIndex); // OnClickParam?.Invoke(groupIndex, paramIndex);

    private void Clear() {
        while (groupList.Count > 0) {
            groupList[0].DestroyGO();
            groupList.RemoveAt(0);
        }
    }

    public void Close() {
        OnClose?.Invoke();
        Destroy(gameObject);
    }

    private void OnDestroy() {
        Clear();
    }

    [System.Serializable]
    public struct Data {
        public string CountryName;
        public string CloseString;
        public GUI_ParamsGroup.Data[] Groups;
    }
}
