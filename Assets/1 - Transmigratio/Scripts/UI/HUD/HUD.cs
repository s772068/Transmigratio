using TMPro;
using UnityEngine;

/// <summary>
/// Интерфейс, всплывающие окна и тд
/// </summary>
public class HUD : StaticInstance<HUD> {
    [SerializeField] private Messanger _messanger;
    [SerializeField] private Transform _panelsParent;
    [SerializeField] private GameObject _topPanels;
    [SerializeField] private GameObject _bottomPanels;
    [SerializeField] private GameObject _autoChoicePanel;
    [SerializeField] private TMP_Text _goldTxt;

    [SerializeField] private Migration _migration;

    public bool IsShowMigration = true;

    public Transform PanelsParent => _panelsParent;

    private Humanity _humanity;

    private void OnEnable()
    {
        RegionDetails.StartGame.Panel.onStartGame += OnStartGame;
    }

    private void Start() {
        _humanity = Transmigratio.Instance.TMDB.humanity;
        _topPanels.SetActive(false);
        _bottomPanels.SetActive(false);
        Timeline.TickShow += UpdateShowGold;
    }

    private void OnDisable()
    {
        RegionDetails.StartGame.Panel.onStartGame -= OnStartGame;
    }

    private void OnShowMessage(string message) {
        _messanger.gameObject.SetActive(true);
        _messanger.Message = message;
    }

    public void ShowMigration() {
        if (!IsShowMigration) return;
        //migration
    }

    private void OnStartGame() {
        _topPanels.SetActive(true);
        _bottomPanels.SetActive(true);
    }

    public void OpenAutoChoice() {
        _autoChoicePanel.SetActive(true);
    }

    private void UpdateShowGold() {
        _goldTxt.text = ((int)_humanity.AllGold).ToString();
    }
}
