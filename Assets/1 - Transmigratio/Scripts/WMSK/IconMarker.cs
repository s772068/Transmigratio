using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class IconMarker : MonoBehaviour {
    private SpriteRenderer _sr;

    public Sprite Sprite { set => _sr.sprite = value; }
    
    public Action onClick;
    public Action OnTimeDestroy;

    public void Click() {
        onClick?.Invoke();
    }

    public void Destroy() => Destroy(gameObject);

    private void Awake() {
        _sr = GetComponent<SpriteRenderer>();
    }
}
