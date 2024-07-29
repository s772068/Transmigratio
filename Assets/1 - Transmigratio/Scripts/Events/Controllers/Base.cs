using System.Collections.Generic;
using WorldMapStrategyKit;
using UnityEngine;
using Events.Data;
using System;

namespace Events.Controllers {
    public abstract class Base : MonoBehaviour {
        [Header("Panel")]
        [SerializeField] private protected EventPanel panel;
        [SerializeField] private protected EventDesidion _desidionPrefab;
        [SerializeField] private protected Sprite panelSprite;
        [Header("Marker")]
        [SerializeField] private protected IconMarker markerPrefab;
        [SerializeField] private protected Sprite markerSprite;
        [Header("Colors")]
        [SerializeField] private protected Color regionColor;
        [SerializeField] private protected Color civColor;

        private protected Desidion _activeDesidion => _desidions[_activateIndex];
        private protected List<Desidion> _desidions = new();
        private protected int _activateIndex;

        public Sprite PanelSprite => panelSprite;
        public List<Desidion> Desidions => _desidions;
        public bool AutoChoice = false;

        private protected abstract string Name { get; }
        private protected abstract string Territory { get; }

        private protected Chronicles.Controller ChroniclesController => Chronicles.Controller.Instance;
        private protected Map Map => Transmigratio.Instance.TMDB.map;
        private protected WMSK WMSK => Map.WMSK;
        private protected Intervention _intervention => Intervention.Instance;

        private protected abstract void ActivateEvents();
        private protected abstract void DeactivateEvents();
        private protected abstract void InitDesidions();
        private protected abstract void OpenPanel();
        private protected abstract void CreateMarker(CivPiece piece = null);

        public string Local(string key) => Localization.Load(Name, key);
        
        private protected void OnEnable() {
            InitDesidions();
            ActivateEvents();
        }

        private protected void OnDisable() {
            DeactivateEvents();
            _desidions.Clear();
        }

        private protected void AddDesidion(Action<Func<int>> click, string title, Func<int> points)
        {
            _desidions.Add(new(click, title, points));
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
