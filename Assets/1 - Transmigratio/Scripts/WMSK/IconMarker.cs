using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class IconMarker : MonoBehaviour {
    private SpriteRenderer _sr;

    public Sprite Sprite { set => _sr.sprite = value; }
    public CivPiece Piece;

    public Action<CivPiece> onClick;
    public Action OnTimeDestroy;

    public void Click() {
        onClick?.Invoke(Piece);
    }

    public void Destroy() => Destroy(gameObject);

    private void Awake() {
        _sr = GetComponent<SpriteRenderer>();
    }
}
