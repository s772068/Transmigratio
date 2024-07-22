using UnityEngine;
using TMPro;

namespace UI.Text {
    [RequireComponent(typeof(TMP_Text))]
    public class TextLocalization : MonoBehaviour {
        [SerializeField] private string table;
        [SerializeField] private bool isUse;
        private void Awake() {
            if (!isUse) return;
            TMP_Text text = GetComponent<TMP_Text>();
            text.text = Utilits.Localization.Load(table, text.text);
        }
    }
}
