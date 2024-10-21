using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���������, ����������� ���� � ��
/// </summary>
public class HUD : StaticInstance<HUD> {
    [SerializeField] private Messanger _messanger;
    [SerializeField] private Transform _panelsParent;
    [SerializeField] private GameObject _topPanels;
    [SerializeField] private GameObject _bottomPanels;
    [SerializeField] private GameObject _autoChoicePanel;
    [SerializeField] private RectTransform _menuButton;
    [SerializeField] private RectTransform _helpButton;
    [SerializeField] private RectTransform _layerButton;
    [SerializeField] private RectTransform _chronicleButton;
    [SerializeField] private RectTransform _autoChoiceButton;
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
        //_goldTxt.text = ((int)_humanity.AllGold).ToString();
    }

    public void OnShowLayers(bool isShow) {
        _menuButton.DOPause();
        _menuButton.DOAnchorPosY(isShow ? 80 : - 46 , 0.25f).SetEase(isShow ? Ease.InQuad : Ease.OutQuad);

        _helpButton.DOPause();
        _helpButton.DOAnchorPosY(isShow ? 80 : - 46, 0.25f).SetEase(isShow ? Ease.InQuad : Ease.OutQuad);
        
        _chronicleButton.DOPause();
        _chronicleButton.DOAnchorPosY(isShow ? -180 : - 50, 0.25f).SetEase(isShow ? Ease.InQuad : Ease.OutQuad);
        
        _autoChoiceButton.DOPause();
        _autoChoiceButton.DOAnchorPosY(isShow ? -180 : - 50, 0.25f).SetEase(isShow ? Ease.InQuad : Ease.OutQuad);
    }
}
