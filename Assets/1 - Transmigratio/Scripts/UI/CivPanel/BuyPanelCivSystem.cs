using System;
using TMPro;
using UI.CivPanel;
using UnityEngine;
using UnityEngine.UI;

public class BuyPanelCivSystem : MonoBehaviour
{
    [SerializeField] private ToggleGroup _group;
    [SerializeField] private TMP_Text _tmpPercent;
    [SerializeField] private TMP_Text _tmpPrice;
    
    public string UpdatePercent { set => _tmpPercent.text = value + "%"; }
    public string UpdatePrice { set => _tmpPrice.text = "<sprite=\"Gold-Inter\" name=\"Gold\">" + value; }
    
    public event Action ActionCivBuy;
    public event Action ActionCivSell;

    public void Buy() => ActionCivBuy?.Invoke();
    public void Sell() => ActionCivSell?.Invoke();

    public void OnActiveToggleChanged(bool active)
    {
        if (!active) return;
        
        CivSystemElement element = _group.GetFirstActiveToggle().GetComponent<CivSystemElement>();
        _tmpPercent.text = element.Percent;
    }
}
