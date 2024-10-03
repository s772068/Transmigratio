using UnityEngine;
using DG.Tweening;

namespace Tutorials.Pointers.Finters {
    public class CirclesScale : MonoBehaviour {
        [SerializeField] private Vector2 _endValue;
        [SerializeField] private float _duration;
        [Min(-1)]
        [SerializeField] private int _loopCount;
        [SerializeField] private LoopType _loopType;
        [SerializeField] private Ease _ease;

        private RectTransform _rect;

        private void Awake() {
            _rect = GetComponent<RectTransform>();
        }

        private void OnEnable() {
            _rect.DOScale(_endValue, _duration)
                 .SetLoops(-1, _loopType)
                 .SetEase(_ease);
        }

        private void OnDisable() {
            _rect.DOPause();
        }
    }
}
