using UnityEngine.UI;
using UnityEngine;
using System;

[RequireComponent(typeof(Button))]
public class GUIE_EventResult : MonoBehaviour {
    [SerializeField] private Text nameResult;
    [SerializeField] private Text description;

    public int Index { get; set; }
    public string Name { set => nameResult.text = value; }
    public string Description { set => description.text = value; }

    public Action<int> OnClick;

    public void Click() => OnClick?.Invoke(Index);

    public void DestroyGO() {
        Destroy(gameObject);
    }
}
