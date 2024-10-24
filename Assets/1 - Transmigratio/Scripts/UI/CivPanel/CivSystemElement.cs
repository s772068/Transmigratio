using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CivPanel
{
    public class CivSystemElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _politName;
        [SerializeField] private TMP_Text _percent;
        [SerializeField] private Transform _effectsParent;
        [SerializeField] private Image _effectsPrefab;
        [SerializeField] private CircularStat _stat;

        public string Percent => _percent.text;
        
        private List<Image> _effects;

        public void Init(string politName, float percent, float percentForStat = 0, List<Sprite> effects = null)
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
                _effectsParent.gameObject.SetActive(true);
                foreach (var effect in effects)
                {
                    var effectObject = Instantiate(_effectsPrefab, _effectsParent);
                    effectObject.sprite = effect;
                    _effects.Add(effectObject);
                }
            }

            _stat.gameObject.SetActive(true);
            _stat.Init(new List<float>() { percentForStat,percent });
        }

        public void Clear()
        {
            _politName.text ="";
            _percent.text = "";
            _effectsParent.gameObject.SetActive(false);
            _stat.gameObject.SetActive(false);
        }
    }
}