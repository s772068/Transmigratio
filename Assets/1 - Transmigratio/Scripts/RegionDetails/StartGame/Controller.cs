using UnityEngine.UI;
using UnityEngine;
using System;

namespace RegionDetails {
    public class Controller : MonoBehaviour {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private StartGame.Panel _startPanel;

        private StartGame.Panel __startPanel;

        public Action onClose;

        public static event Action<bool> onOpenStartRegionPanel;
        public static event Action<bool> onOpenRegionPanel;
        public static event Action onStartGame;

        private void Awake() {
            MapData.onClickRegion += OpenStartPanel;
            _closeBtn.onClick.AddListener(Close);
        }

        public void Close() {
            _closeBtn.gameObject.SetActive(false);
            onClose?.Invoke();
        }

        private void OpenStartPanel(int regionID) {
            __startPanel = StartGame.Factory.CreateController(_startPanel, transform);
            _closeBtn.gameObject.SetActive(true);
            onOpenRegionPanel?.Invoke(true);
            onClose = CloseStartPanel;
        }

        private void CloseStartPanel() {
            __startPanel.Close();
        }
    }
}
