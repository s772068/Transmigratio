using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace Events.Prefabs {
    public class Panel : MonoBehaviour {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text territory;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Image image;
        [SerializeField] private Toggle dontShowAgain;
        [SerializeField] private EventDesidion desidion;
        [SerializeField] private Transform desidionsContent;

        private List<EventDesidion> desidions = new();

        public static event Action<bool> EventPanelOpen;
        public static event Action<bool> EventPanelClose;
        public Action<int> OnClickDesidion;

        public string Title { set => title.text = value; }
        public string Territory { set => territory.text = value; }
        public string Description { set => description.text = value; }
        public Sprite Image { set => image.sprite = value; }

        public bool IsShowAgain {
            get => !dontShowAgain.isOn;
            set => dontShowAgain.isOn = !value;
        }

        public void Open() {
            Transmigratio.Instance.IsClickableMarker = false;
            ClearDesidions();
            gameObject.SetActive(true);
            EventPanelOpen?.Invoke(true);
        }

        public void Close() {
            Transmigratio.Instance.IsClickableMarker = true;
            gameObject.SetActive(false);
            EventPanelClose?.Invoke(true);
            Timeline.Instance.Play();
        }

        public void AddDesidion(Data.Desidion val) {
            var _desidion = Instantiate(desidion, desidionsContent);
            _desidion.Index = desidions.Count;
            _desidion.Title = val.Title;
            _desidion.Points = val.OnGetPoints();
            _desidion.onClick = OnClickDesidion;
            desidions.Add(_desidion);
        }

        public void ClearDesidions() {
            for (int i = 0; i < desidions.Count; ++i) {
                desidions[i].Destroy();
            }
            desidions.Clear();
        }
    }
}
