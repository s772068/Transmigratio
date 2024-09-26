using AYellowpaper.SerializedCollections;
using UnityEngine;
using System;
using TMPro;

namespace RegionDetails.Defoult {
    public class Panel : UI.Panel {
        [SerializeField] private TMP_Text _region;
        [SerializeField] private Paramiters.Group _paramiters;
        [SerializeField] private Elements.Group _elements;
        [SerializeField] private Transform _descriptionContent;
        [SerializeField] private GameObject _tutorial;

        [Header("Prefabs")]
        [SerializeField] private Descriptions.Civilization _civDescriptionPref;
        [SerializeField] private Descriptions.Defoult _defDescriptionPref;

        [SerializeField] private SerializedDictionary<string, Sprite> civAvatars;

        private Descriptions.BaseDescription _description;
        private string _paramiter;
        private string _element;

        public static event Action<bool> onCloseRegionDetails;

        public void SetActiveParamiter(string paramiter) {
            _paramiters.SetActiveParamiter(paramiter, true);
        }
        public bool IsActiveParamiters {
            set => _paramiters.IsActive = value;
        }
        public bool IsActiveElements {
            set => _elements.IsActive = value;
        }

        private void Awake() {
            Tutorial.OnShowTutorial += ShowTutorial;
            _paramiters.onSelect = OnSelectParamiter;
            _elements.onSelect = OnSelectElement;
        }

        private void Start() {
            _region.text = Transmigratio.Instance.TMDB.map.AllRegions[MapData.RegionID].Name;
            IsActiveParamiters = true;
        }

        private void ShowTutorial(string tutName) {
            if (tutName == "RegionDetails") {
                IsActiveParamiters = false;
                _tutorial?.SetActive(true);
            }
        }


        public void OnSelectParamiter(string paramiter) {
            _elements.Label = paramiter;
            _paramiter = paramiter;
            /// Избавится от этого костыля
            _description?.Destroy();
            _description = null;
            _elements.IsSelectable = paramiter == "Government" ||
               paramiter == "EcoCulture" ||
               paramiter == "ProdMode" ||
               paramiter == "Civilizations";
            if (_paramiter != "Civilizations") {
                _description = Factory.Create(_defDescriptionPref, _descriptionContent, Localization.Load(paramiter, $"{paramiter}Describe"));
            }
            ///
            _elements.UpdateElements(paramiter);
        }

        public void OnSelectElement(string element) {
            _element = element;
            _description?.Destroy();
            if (_paramiter == "Civilizations") {
                _description = Factory.Create(_civDescriptionPref, _descriptionContent, element, civAvatars[element], Localization.Load("Civilizations", element), "Blah-Blah-Blah", OnClickLink, OnClickInfluence);
            } else {
                _description = Factory.Create(_defDescriptionPref, _descriptionContent, Localization.Load(_paramiter, $"{element}Describe"));
            }
        }

        private void OnClickLink() {
            Debug.Log("Link");
        }

        private void OnClickInfluence() {
            Debug.Log("Influence");
        }

        public void Close() {
            MapData.WMSK.ToggleCountrySurface(MapData.RegionID, true, Color.clear);
            onCloseRegionDetails?.Invoke(true);
            Destroy(gameObject);
        }
    }
}
