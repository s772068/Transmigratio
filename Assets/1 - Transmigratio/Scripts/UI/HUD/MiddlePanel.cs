using TMPro;
using UnityEngine;

public class MiddlePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _populationTxt;
    private int Population => Transmigratio.Instance.TMDB.humanity.TotalEarthPop;

    private void Awake()
    {
        Timeline.TickShow += UpdatePopulation;
    }

    private void OnDestroy()
    {
        Timeline.TickShow -= UpdatePopulation;
    }

    private void UpdatePopulation()
    {
        _populationTxt.text = $"{Population.ToString("### ### ###")}";
    }
}
