using UnityEngine.UI;
using UnityEngine;
using System;

public class GUI_CountryPanel : GUI_BasePanel {
    [Header("Text")]
    [SerializeField] private Text countryName;
    [SerializeField] private Text closeTxt;
    [Header("Ecology")]
    [SerializeField] private Text ecologyGroupName;
    [SerializeField] private GUI_Paramiter terrain;
    [SerializeField] private GUI_Paramiter climate;
    [SerializeField] private GUI_Paramiter flora;
    [SerializeField] private GUI_Paramiter fauna;
    [Header("Population")]
    [SerializeField] private Text populationGroupName;
    [SerializeField] private GUI_Paramiter population;
    [SerializeField] private GUI_Paramiter production;
    [SerializeField] private GUI_Paramiter economics;
    [SerializeField] private GUI_Paramiter goverment;
    [SerializeField] private GUI_Paramiter civilization;
    [Header("Other")]
    [SerializeField] private Transform content;
    [SerializeField] private Button closeBtn;

    [HideInInspector] public int index;

    public Action<int, int> OnClickParam;
    public Action OnClose;

    public string Terrain { set => terrain.Value = value; }
    public string Climate { set => climate.Value = value; }
    public int Flora { set { flora.Value = value.ToString(); flora.Fill = 100f / value; } }
    public int Fauna { set { fauna.Value = value.ToString(); fauna.Fill = 100f / value; } }
    public int Population { set => population.Value = value.ToString(); }
    public string Production { set => production.Value = value; }
    public string Economics { set => economics.Value = value; }
    public string Goverment { set => goverment.Value = value; }
    public string Civilization { set => civilization.Value = value; }

    public void Localization(string name, SL_System system, SL_Map map) {
        countryName.text = name;

        ecologyGroupName.text = map.EcologyGroup;
        terrain.Label = map.Terrain;
        climate.Label = map.Climate;
        flora.Label = map.Flora;
        fauna.Label = map.Fauna;

        populationGroupName.text = map.PopulationGroup;
        population.Label = map.Population;
        production.Label = map.Production;
        economics.Label = map.Economic;
        goverment.Label = map.Goverment;
        civilization.Label = map.Civilization;
        
        closeTxt.text = system.Close;
    }

    private int FindMaxIndex(int[] arr) {
        int res = -1;
        int value = -1;
        for(int i = 0; i < arr.Length; ++i) {
            if (arr[i] > value) {
                res = i;
            }
        }
        return res;
    }

    public void UpdatePanel(ref S_Country value) {
    }

    public void ClickParam(int groupIndex, int paramIndex) =>  OnClickParam?.Invoke(groupIndex, paramIndex);

    public void Close() => OnClose?.Invoke();
}
