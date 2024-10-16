using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace Civilizations {
    public class Element : MonoBehaviour {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _btnImg;
        [SerializeField] private Sprite _activeBtn;

        private Sprite _disactiveBtn;
        private string _name;
        private bool _isActive;

        public event Action<Element> onClick;

        public string Title {
            get => _name;
            set {
                _name = value;
                _title.text = Localization.Load("Civilizations", value);
            }
        }

        public Sprite Icon { set => _icon.sprite = value; }

        private void Awake() {
            _disactiveBtn = _btnImg.sprite;
        }

        public void Click() => onClick?.Invoke(this);

        public void Activate() {
            _btnImg.sprite = _activeBtn;
            _isActive = true;
        }

        public void Disactivate() {
            _btnImg.sprite = _disactiveBtn;
            _isActive = false;
        }

        public void Destroy() {
            Destroy(gameObject);
        }

        [Serializable]
        public struct Data {
            public string title;
            public Sprite icon;
        }
    }
}
