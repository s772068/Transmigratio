using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class IconMarker : MonoBehaviour {
    public int Index { private get; set; }

    private SpriteRenderer sr;
    private bool _isEnable = true;
    public Sprite Sprite { set => sr.sprite = value; }
    
    public Action<int> onClick;
    public Action OnTimeDestroy;

    public void Click()
    {
        if (!_isEnable)
            return;

        _isEnable = false;
        sr.enabled = false;
        onClick?.Invoke(Index);
    }
    public void Destroy() => Destroy(gameObject);

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }
}
