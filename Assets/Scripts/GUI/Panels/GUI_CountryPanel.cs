using UnityEngine.UI;
using UnityEngine;
using System;

public class GUI_CountryPanel : GUI_BasePanel {
    [Header("Text")]
    [SerializeField] private Text countryName;
    [SerializeField] private Text closeTxt;
    [Header("Ecology")]
    [SerializeField] private Text ecologyGroupName;
    [SerializeField] private GUI_Paramiter flora;
    [SerializeField] private GUI_Paramiter fauna;
    [Header("Population")]
    [SerializeField] private Text populationGroupName;
    [SerializeField] private GUI_Paramiter population;
    [Header("Other")]
    [SerializeField] private GUI_Paramiter[] paramiters;
    [SerializeField] private Transform content;
    [SerializeField] private Button closeBtn;

    [HideInInspector] public int index;

    private int populationVal;
    private string emptyPopulation;

    public Action<int> OnClickParam;
    public Action OnClose;

    // public string Terrain { set => terrain.Value = value; }
    // public string Climate { set => climate.Value = value; }
    public int Flora { set { flora.Value = value.ToString(); flora.Fill = 100f / value; } }
    public int Fauna { set { fauna.Value = value.ToString(); fauna.Fill = 100f / value; } }
    public int Population {
        set {
            population.Value = value > 0 ? value.ToString() : emptyPopulation;
            if (populationVal > 0 && value == 0) {
                ShowParamiters(false);
            } else if(populationVal == 0 && value > 0) {
                ShowParamiters(true);
            }
            populationVal = value;
        }
    }
    //public string Production { set => production.Value = value; }
    //public string Economics { set => economics.Value = value; }
    //public string Goverment { set => goverment.Value = value; }
    //public string Civilization { set => civilization.Value = value; }

    public void SetParam(int index, string value) {
        paramiters[index].Value = value;
    }

    public void Localization(string name, SL_System system, SL_Map map) {
        countryName.text = name;

        ecologyGroupName.text = map.EcologyGroup;
        // terrain.Label = map.Terrain;
        // climate.Label = map.Climate;
        flora.Label = map.Flora;
        fauna.Label = map.Fauna;

        populationGroupName.text = map.PopulationGroup;
        population.Label = map.Population;
        emptyPopulation = map.EmptyPopulation;
        // production.Label = map.Production;
        // economics.Label = map.Economics;
        // goverment.Label = map.Goverment;
        // civilization.Label = map.Civilization;

        for(int i = 0; i < paramiters.Length; ++i) {
            paramiters[i].Label = map.Paramiters[i].Name;
        }

        // print(system.Close);
        
        closeTxt.text = system.Close;
    }

    public void ClickParam(int paramiter) =>  OnClickParam?.Invoke(paramiter);

    public void Close() => OnClose?.Invoke();

    private void ShowParamiters(bool isShow) {
        for (int i = 0; i < paramiters.Length; ++i) {
            if (!paramiters[i].IsHideble) return;
            paramiters[i].gameObject.SetActive(isShow);
        }
    }
}
