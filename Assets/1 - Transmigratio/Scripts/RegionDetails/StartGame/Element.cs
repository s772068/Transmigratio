using UnityEngine.UI;
using UnityEngine;

namespace RegionDetails.StartGame {
    public class Element : Base.Element {
        [SerializeField] private Image _backGradient;

        public Color Color { set { if (_backGradient != null) _backGradient.color = value; } }
        public void Init(Color color, string paramiter, string label, float value) {
            Label = Localization.Load(paramiter, label);
            SetValue(value, false);
            Color = color;
        }
    }
}
