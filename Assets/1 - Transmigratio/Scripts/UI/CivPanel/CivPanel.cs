using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CivPanel : Panel
    {
        [SerializeField] private Image _civAvatar;
        [SerializeField] private List<Toggle> _tabs;
        private GameObject _currentTab;
        private Civilization _civ;

        public void Open(Civilization civilization)
        {
            _civ = civilization;
            _civAvatar.sprite = Transmigratio.Instance.TMDB.humanity.GetIcon(civilization.Name);
            _tabs[0].isOn = true;
        }
    }
}
