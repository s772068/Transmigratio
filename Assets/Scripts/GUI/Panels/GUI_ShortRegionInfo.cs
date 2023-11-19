using UnityEngine.UI;
using UnityEngine;
using System;

public class GUI_ShortRegionInfo : MonoBehaviour {
    [SerializeField] private Text countryName;
    [SerializeField] private Text populationName;
    [SerializeField] private Text eventName;
    public Action<int> OnClick;

    public int Index { private get; set; }

    public string CountryName { set => countryName.text = value; }
    public string PopulationName { set => populationName.text = value; }
    public string EventName { set => eventName.text = value; }

    public void Click() => OnClick?.Invoke(Index);
}
