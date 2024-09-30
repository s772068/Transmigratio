using UnityEngine.UI;
using UnityEngine;
using System;

namespace RegionDetails {
    public class Controller : MonoBehaviour {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private StartGame.Panel _startPanel;
        [SerializeField] private Defoult.Panel _defoultPanel;

        private StartGame.Panel __startPanel;
        private bool _isStartedGame;

        public Action onClose;

        public static event Action<bool> onOpenStartRegionPanel;
        public static event Action<bool> onOpenRegionPanel;
        public static event Action onStartGame;

        private void Awake() {
            MapData.onClickRegion += OpenPanel;
            _closeBtn.onClick.AddListener(Close);
            StartGame.Panel.onStartGame += OnStartGame;
        }

        public void Close() {
            _closeBtn.gameObject.SetActive(false);
            onClose?.Invoke();
        }

        private void OpenPanel(int regionID) {
            if (_isStartedGame) OpenDefoultPanel();
            else                OpenStartPanel();
        }


        private void OpenStartPanel() {
            __startPanel = StartGame.Factory.Create(_startPanel, transform);
            _closeBtn.gameObject.SetActive(true);
            onOpenStartRegionPanel?.Invoke(true);
            onClose = CloseStartPanel;
        }

        private void OpenDefoultPanel() {
            Defoult.Factory.Create(_defoultPanel, transform);
            onOpenRegionPanel?.Invoke(true);
        }

        private void CloseStartPanel() {
            __startPanel?.Close();
        }

        private void OnStartGame() {
            _isStartedGame = true;
            __startPanel = null;
            _closeBtn.gameObject.SetActive(false);
            Defoult.Factory.Create(_defoultPanel, transform);
            onOpenRegionPanel?.Invoke(true);
        }
    }
}
