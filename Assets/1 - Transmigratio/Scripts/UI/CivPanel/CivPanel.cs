using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CivPanel
{
    public class CivPanel : Panel
    {
        [Header("Main Settings")]
        [SerializeField] private TMP_Text _civName;
        [SerializeField] private Image _civAvatar;
        [SerializeField] private List<Toggle> _tabs;

        [Header("Tab Settings")] 
        [SerializeField] private CivSystem _civSystem;
        
        private GameObject _currentTab;
        private Civilization _civ;

        public void Open(Civilization civilization)
        {
            _civName.text = civilization.Name;
            _civ = civilization;
            _civAvatar.sprite = Transmigratio.Instance.TMDB.humanity.GetIcon(civilization.Name);
            _tabs[0].isOn = true;
        }
    }
}
