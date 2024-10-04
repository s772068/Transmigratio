using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RegionDetails.StartGame;

namespace RegionDetails.Base {
    public class Element : MonoBehaviour {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private TMP_Text _percent;
        [SerializeField] private Slider _slider;
        
        public string Label {
            set {
                _label.text = value;
                name = value;
            }
        }

        public void SetValue(float value, bool isPercent = true) {
            _percent.text = $"{(int) value}{(isPercent ? "%" : "")}";
            _slider.value = value / 100f;
        }

        public void Destroy() {
            Destroy(gameObject);
        }
    }
}
