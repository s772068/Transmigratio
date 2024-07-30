using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InterventionBar : MonoBehaviour
    {
        [SerializeField] private Image _progressBar;
        [SerializeField] private TMP_Text _tmpPoints;
        private float _maxPoints;

        private int Points => Transmigratio.Instance.Intervention.InterventionPoints;

        private void OnEnable()
        {
            Intervention.InterventionPointsUpdate += UpdateBar;
        }

        private void Start()
        {
            _maxPoints = Points;
            UpdateBar();
        }

        private void OnDisable()
        {
            Intervention.InterventionPointsUpdate -= UpdateBar;
        }

        private void UpdateBar()
        {
            _progressBar.fillAmount = Points / _maxPoints;
            _tmpPoints.text = Points.ToString();
        }
    }
}
