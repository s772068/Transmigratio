using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class IconMarker : MonoBehaviour {
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private SpriteRenderer _countSR;
    [SerializeField] private List<Sprite> _countSprites;

    private int _count = 0;

    public Sprite Sprite { set => _sr.sprite = value; }
    public int SetCount
    {
        get => _count;
        set
        {
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

    private void Start()
    {
        MarkerInst?.Invoke();
    }

    public void Click() {
        onClick?.Invoke(Piece);
    }

    public void Destroy() => Destroy(gameObject);

}
