using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class IconMarker : MonoBehaviour {
    private SpriteRenderer sr;
    private int liveTime;

    public Sprite Sprite { set => sr.sprite = value; }
    public int LiveTime { set => liveTime = value; }
    
    public Action<IconMarker> OnClick;

    public void Click() => OnClick?.Invoke(this);
    public void DestroyGO() => Destroy(gameObject);

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        StartCoroutine(LiveTimer());
    }

    private IEnumerator LiveTimer() {
        if (liveTime < 0) yield break;
        yield return new WaitForSeconds(liveTime);
        Destroy(gameObject);
    }
}
