using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace RegionDetails {
    public class CenterSide : MonoBehaviour {
        [SerializeField] private TMP_Text title;
        [SerializeField] private Paramiter paramiter;
        [SerializeField] private Transform content;
        [SerializeField] private Dictionary<string, Paramiter> paramiters = new();

        public string Title { set => title.text = value; }

        public Action<string> onClick;

        public void SetParamiter(string element, string title, float value) {
            if (!paramiters.ContainsKey(title)) {
                var _paramiter = Instantiate(paramiter, content);
                _paramiter.SetTitle(element, title);
                _paramiter.Click = OnClick;
                paramiters[title] = _paramiter;
            }
            paramiters[title].Value = value;
        }

        public void ClearParams() {
            foreach (var pair in paramiters) {
                pair.Value.Destroy();
            }
            paramiters.Clear();
        }

        private void OnClick(string key) => onClick?.Invoke(key);
    }
}
