using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CivPanel
{
    public class CivSystem : MonoBehaviour
    {
        [SerializeField] private List<TMP_Text> _tmpsGoldAmount;
        [SerializeField] private SerializedDictionary<CivSystemType, List<CivSystemElement>> _systemElements;
        [SerializeField] private SerializedDictionary<CivSystemType, CircularStat> _stats;
        [Header("Polit System")] 
        [SerializeField] private ToggleGroup _politGroup;
        [SerializeField] private BuyPanelCivSystem _politBuyPanel;
        [Header("Prod System")]
        [SerializeField] private ToggleGroup _prodGroup;
        [SerializeField] private BuyPanelCivSystem _prodBuyPanel;
        [Header("Polit System")] 
        [SerializeField] private ToggleGroup _ecocultureGroup;
        [SerializeField] private BuyPanelCivSystem _ecocultureBuyPanel;
        
        private Civilization _civ;

        public void Init(Civilization civ)
        {
            _civ = civ;
            UpdatePanels();
        }

        private void UpdatePanels()
        {
            foreach (var (key, _systemElements) in _systemElements)
            {
                Dictionary<string, float> paramitter = new();
                switch (key)
                {
                    case CivSystemType.Government:
                        paramitter = _civ.Government.GetValues();
                        break;
                    case CivSystemType.ProdMode:
                        paramitter = _civ.ProdMode.GetValues();
                        break;
                    case CivSystemType.EcoCulture:
                        paramitter = _civ.EcoCulture.GetValues();
                        break;
                    default:
                        new Exception("non CivSystemType");
                        break;
                }

                var sortedParamitter = paramitter.OrderBy(x => x.Value);
                
                List<float> allocation = new();
                float curPercent = 0f;
                int index = 0;
                for (int i = sortedParamitter.Count() - 1; i >= 0; i--)
                {
                    if (index >= _systemElements.Count) break;
                    
                    var element = sortedParamitter.ElementAt(i);
                    allocation.Add(element.Value);
                    if (element.Value <= 0)
                    {
                        _systemElements[index].Clear();
                        index += 1;
                        continue;
                    }

                    _systemElements[index].Init(element.Key, element.Value, curPercent);
                    curPercent += element.Value;
                    index += 1;
                }

                _stats[key].Init(allocation);
            }

            UpdateGoldText();
        }
        
        

        private void UpdateGoldText()
        {
            foreach (var gold in _tmpsGoldAmount) gold.text = _civ.AllGold.ToString("0.00");
        }
    }

    public enum CivSystemType
    {
        Government,
        ProdMode,
        EcoCulture
    }
}
