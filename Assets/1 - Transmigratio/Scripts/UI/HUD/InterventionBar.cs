using UnityEngine.UI;
using UnityEngine;
using Gameplay;
using TMPro;

namespace UI {
    public class InterventionBar : MonoBehaviour {
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_Text _tmpPoints;
        [SerializeField] private Transform _handle;

        private int Points => Transmigratio.Instance.Intervention.InterventionPoints;
        private int MaxPoints => Transmigratio.Instance.MaxInterventionPoints;

        private void OnEnable() {
            Intervention.InterventionPointsUpdate += UpdateBar;
            UpdateBar();
        }

        private void OnDisable() {
            Intervention.InterventionPointsUpdate -= UpdateBar;
        }

        private void UpdateBar() {
            float value = (float)Points / (float)MaxPoints;
            _handle.gameObject.SetActive(value > 0);
            _progressBar.value = value;
            _tmpPoints.text = $"<sprite=\"Icons\" name=\"Intervention\">{value * 100}%";
        }
    }
}
