using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace Tutorials.Pointers.Finters {
    [RequireComponent(typeof(Image))]
    public class CirclesAlpha : MonoBehaviour {
        [SerializeField] private float _endValue;
        [SerializeField] private float _duration;
        [Min(-1)]
        [SerializeField] private int _loopCount;
        [SerializeField] private LoopType _loopType;
        [SerializeField] private Ease _ease;

        private Image _img;

        private void Awake() {
            _img = GetComponent<Image>();
        }

        private void OnEnable() {
            _img.DOFade(_endValue, _duration / 2)
                 .SetLoops(-1, _loopType)
                 .SetEase(_ease);
        }

        private void OnDisable() {
            _img.DOPause();
        }
    }
}
