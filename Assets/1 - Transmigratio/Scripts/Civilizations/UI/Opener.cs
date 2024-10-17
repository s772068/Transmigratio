using UnityEngine.UI;
using UnityEngine;

namespace Civilizations {
    public class Opener : MonoBehaviour {
        [SerializeField] private Image _iconImg;

        private Humanity _humanity;
        private Sprite _baseIcon;

        private void Awake() {
            _baseIcon = _iconImg.sprite;
            Selector.onSelect += OnSelect;
            Selector.onUnselect += OnUnselect;
        }

        private void Start() {
            _humanity = Transmigratio.Instance.TMDB.humanity;
        }

        private void OnSelect(string civ) {
            _iconImg.sprite = _humanity.GetIcon(civ);
        }

        private void OnUnselect() {
            _iconImg.sprite = _baseIcon;
        }
    }
}
