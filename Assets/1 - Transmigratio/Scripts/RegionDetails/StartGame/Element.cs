using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace RegionDetails.StartGame {
    public class Element : MonoBehaviour {
        [SerializeField] private Image _backGradient;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private TMP_Text _percent;
        [SerializeField] private Slider _slider;

        public Color Color { set => _backGradient.color = value; }
        public string Label { set => _label.text = value; }
        public float Percent{
            set {
                _percent.text = $"{value}%";
                _slider.value = value / 100f;
            }
        }

        public void Destroy() {
            Destroy(gameObject);
        }
    }
}
