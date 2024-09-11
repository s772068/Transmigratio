using UnityEngine;
using TMPro;

namespace RegionDetails.Defoult {
    public class Panel : MonoBehaviour {
        [SerializeField] private TMP_Text _region;
        [SerializeField] private Paramiters.Group _paramiters;
        [SerializeField] private Elements.Group _elements;
        [SerializeField] private Transform _descriptionContent;

        [Header("Prefabs")]
        [SerializeField] private Descriptions.Civilization _civDescriptionPref;
        [SerializeField] private Descriptions.Defoult _defDescriptionPref;

        private Descriptions.Civilization _civDescription;
        private Descriptions.Defoult _defDescription;
        private string _paramiter;
        private string _element;

        private void Start() {
            _region.text = Transmigratio.Instance.TMDB.map.AllRegions[MapData.RegionID].Name;
            _paramiters.onSelect = OnSelectParamiter;
            _elements.onSelect = OnSelectElement;
        }

        private void OnSelectParamiter(string paramiter) {
            _elements.Label = paramiter;
            _paramiter = paramiter;
            _elements.UpdateElements(paramiter);
            Debug.Log(_paramiter);
        }

        private void OnSelectElement(string element) {
            _element = element;
            Debug.Log(_element);
        }
    }
}
