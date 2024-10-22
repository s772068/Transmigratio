using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CivPanel
{
    public class PolitElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _politName;
        [SerializeField] private TMP_Text _percent;
        [SerializeField] private Image _circularFill;
        [SerializeField] private Transform _effectsParent;
        [SerializeField] private Image _effectsPrefab;

        private List<Image> _effects;

        public void Init(string politName, int percent, List<Sprite> effects = null)
        {
            _politName.text = politName;
            _percent.text = $"{percent}%";

            if (_effects != null)
            {
                foreach (var go in _effects)
                {
                    Destroy(go);
                }
                _effects.Clear();
            }
            else
                _effects = new List<Image>();

            if (effects != null)
            {
                foreach (var effect in effects)
                {
                    var effectObject = Instantiate(_effectsPrefab, _effectsParent);
                    effectObject.sprite = effect;
                    _effects.Add(effectObject);
                }
            }
        }
    }
}