using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CivPanel
{
    public class CivPanel : Panel
    {
        [Header("Main Settings")]
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _civName;
        [SerializeField] private Image _civAvatar;
        [SerializeField] private List<Toggle> _tabs;

        [Header("Tab Settings")] 
        [SerializeField] private CivSystem _civSystem;
        
        private GameObject _currentTab;
        private Civilization _civ;
        
        public Civilization Civ => _civ;
        
        public static Action<Civilization> OpenCivPanel;

        private protected override void OnEnable()
        {
            base.OnEnable();
            OpenCivPanel += Init;
        }

        private protected override void OnDisable()
        {
            base.OnDisable();
            OpenCivPanel -= Init;
        }

        public void Init(Civilization civilization)
        {
            _civName.text = civilization.Name;
            _civ = civilization;
            _civAvatar.sprite = Transmigratio.Instance.TMDB.humanity.GetPotrtrait(civilization.Name);
            _tabs[0].isOn = true;
            
            InitTabs();
            
            _panel.SetActive(true);
        }

        private void InitTabs()
        {
            _civSystem.Init(_civ);
        }
    }
}
