using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class IconMarker : MonoBehaviour {
    [SerializeField] private SpriteRenderer _backGround;
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private SpriteRenderer _countSR;
    [SerializeField] private Sprite _clickedStateSprite;
    [SerializeField] private List<Sprite> _countSprites;

    private Sprite _standartStateSprite;
    private int _count = 0;

    public Sprite Sprite { set => _sr.sprite = value; }
    public int SetCount {
        get => _count;
        set {
            if (value <= 0)
                return;

            _count = value;
            _countSR.sprite = _countSprites[value - 1];
        }
    }
    public CivPiece Piece;

    public Action<CivPiece> onClick;
    public Action OnTimeDestroy;

    public static event Action MarkerInst;

    private void Start() {
        MarkerInst?.Invoke();
        _standartStateSprite = _backGround.sprite;
    }

    public void Click() {
        //onClick?.Invoke(Piece);
    }

    public void Destroy() => Destroy(gameObject);

    public void MouseDown() {
        _backGround.sprite = _clickedStateSprite;
    }

    public void MouseUp() {
        _backGround.sprite = _standartStateSprite;
        onClick?.Invoke(Piece);
    }
}
