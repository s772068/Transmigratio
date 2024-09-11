using UnityEngine.UI;
using UnityEngine;
using TMPro;

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

        public float Percent {
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
