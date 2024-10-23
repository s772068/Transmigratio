using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Welcome : MonoBehaviour {
    [SerializeField] private Image _background;
    [SerializeField] private Image _panel;
    [SerializeField] private Image _topElement;
    [SerializeField] private Image _bottomElement;
    [SerializeField] private Image _bubbles;
    [SerializeField] private TMP_Text _text;

    [SerializeField] private float _duration;
    [SerializeField] private float _maxBubblesScale;
    [SerializeField] private Ease _ease;

    private int _curCompleted = 0;
    private int _allCompleted = 0;
    private Vector2 _startBubblesScale;

    private void Awake() {
        _startBubblesScale = _bubbles.GetComponent<RectTransform>().localScale;
    }

    public void OnEnable() {
        ShowBottomElement();
        ShowBackground();
        ShowTopElement();
        ShowBubbles();
        ShowPanel();
        ShowText();
    }

    private void AddCompleted() {
        ++_curCompleted;
        Debug.Log(_curCompleted + " : " + _allCompleted);
        if (_curCompleted >= _allCompleted) {
            _curCompleted = 0;
            _allCompleted = 0;
            Completed();
        }
    }

    private void Completed() {
        _bubbles.GetComponent<RectTransform>().localScale = _startBubblesScale;
        gameObject.SetActive(false);
    }

    private void ShowBackground() {
        ++_allCompleted;
        _background.DOFade(1f, _duration).SetLoops(2, LoopType.Yoyo).SetEase(_ease).OnComplete(AddCompleted);
    }

    private void ShowPanel() {
        ++_allCompleted;
        _panel.DOFade(1f, _duration).SetLoops(2, LoopType.Yoyo).SetEase(_ease).OnComplete(AddCompleted);
    }

    private void ShowTopElement() {
        ++_allCompleted;
        _topElement.DOFade(1f, _duration).SetLoops(2, LoopType.Yoyo).SetEase(_ease).OnComplete(AddCompleted);
    }

    private void ShowBottomElement() {
        ++_allCompleted;
        _bottomElement.DOFade(1f, _duration).SetLoops(2, LoopType.Yoyo).SetEase(_ease).OnComplete(AddCompleted);
    }

    private void ShowBubbles() {
        _allCompleted += 2;
        _bubbles.GetComponent<RectTransform>().DOScale(Vector2.one * _maxBubblesScale, _duration * 2).OnComplete(AddCompleted);
        _bubbles.DOFade(1f, _duration).SetLoops(2, LoopType.Yoyo).SetEase(_ease).OnComplete(AddCompleted);
    }

    private void ShowText() {
        ++_allCompleted;
        _text.DOFade(1f, _duration).SetLoops(2, LoopType.Yoyo).SetEase(_ease).OnComplete(AddCompleted);
    }
}
