using UnityEngine.UI;
using UnityEngine;
using System;

[RequireComponent(typeof(Button))]
public class GUI_EventResult : MonoBehaviour {
    [SerializeField] private Text nameResult;
    [SerializeField] private Text description;

    public int Index { private get; set; }
    public string Name { set => nameResult.text = value; }
    public string Description { set => description.text = value; }

    public Action<int> OnClick;

    public void Init(S_EventResult data) {
        Name = data.Name;
        Description = data.Description;
    }

    public void Click() => OnClick?.Invoke(Index);

    public void DestroyGO() {
        Destroy(gameObject);
    }
}
