using WorldMapStrategyKit;
using UnityEngine;
using Events.Data;
using System.Collections.Generic;
using System;

namespace Events.Controllers {
    public abstract class Base : MonoBehaviour {
        [Header("Panel")]
        [SerializeField] private protected Prefabs.Panel panel;
        [SerializeField] private protected Sprite panelSprite;
        [Header("Marker")]
        [SerializeField] private protected IconMarker markerPrefab;
        [SerializeField] private protected Sprite markerSprite;
        [Header("Colors")]
        [SerializeField] private protected Color regionColor;
        [SerializeField] private protected Color civColor;

        private protected Desidion activeDesidion => desidions[activateIndex];
        private protected List<Desidion> desidions = new();
        private protected int activateIndex;

        private protected bool isShowAgain = true;

        private protected abstract bool ActiveSlider { get; }
        private protected abstract string Name { get; }
        private protected abstract string Territory { get; }

        private protected Map Map => Transmigratio.Instance.TMDB.map;
        private protected WMSK WMSK => Map.WMSK;

        private protected abstract void ActivateEvents();
        private protected abstract void DeactivateEvents();
        private protected abstract void InitDesidions();
        public abstract void CreateMarker();

        private protected string Local(string key) => Localization.Load(Name, key);
        
        private protected void OnEnable() {
            ActivateEvents();
            InitDesidions();
        }

        private protected void OnDisable() {
            DeactivateEvents();
            desidions.Clear();
        }

        private protected void OpenPanel() {
            Timeline.Instance.Pause();
            panel.Open();
            panel.IsShowAgain = isShowAgain;
            panel.ActivateSlider = ActiveSlider;
            panel.Image = panelSprite;
            panel.Title = Local("Title");
            panel.Description = Local("Description");
            panel.Territory = Territory;
            panel.OnClickDesidion = OnClickDesidion;
            for (int i = 0; i < desidions.Count; ++i) {
                panel.AddDesidion(desidions[i]);
            }
        }

        private protected void ClosePanel() {
            isShowAgain = panel.IsShowAgain;
            panel.Close();
        }

        private protected void AddDesidion(Action onClick, string title, Func<int> points) {
            desidions.Add(new() { OnClick = onClick, Title = title, OnGetPoints = points });
        }

        private protected void OnClickDesidion(int desidionIndex) {
            activateIndex = desidionIndex;
            isShowAgain = panel.IsShowAgain;
            desidions[desidionIndex].OnClick?.Invoke();
            CreateMarker();
            ClosePanel();
        }

        private protected virtual IconMarker CreateMarker(Vector3 position) {
            IconMarker marker = Instantiate(markerPrefab);
            marker.Sprite = markerSprite;
            position.z = -0.1f;

            MarkerClickHandler handler = WMSK.AddMarker2DSprite(marker.gameObject, position, 0.03f, true, true);
            handler.allowDrag = false;
            return marker;
        }
    }
}
