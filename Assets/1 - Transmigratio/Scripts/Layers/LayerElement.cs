using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Layers {
    public class LayerElement : UI.ScrollVew.Element {
        [Header("Layer")]
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text title;

        public void UpdateSize(Vector2 size, float time) {
            transform.localScale = Vector2.Lerp(transform.localScale, size, time);
        }

        public void UpdateAlpha(float alpha, float time) {
            UpdateIconAlpha(alpha, time);
            UpdateTextAlpha(alpha, time);
        }

        private void UpdateIconAlpha(float alpha, float time) {
            Color color = icon.color;
            color.a = alpha;
            icon.color = Color.Lerp(icon.color, color, time);
        }

        private void UpdateTextAlpha(float alpha, float time) {
            Color color = title.color;
            color.a = alpha;
            title.color = Color.Lerp(title.color, color, time);
        }
    }
}
