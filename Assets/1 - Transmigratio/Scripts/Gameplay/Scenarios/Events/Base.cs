using Gameplay.Scenarios.Events.Data;
using System.Collections.Generic;
using WorldMapStrategyKit;
using UnityEngine;
using System;

namespace Gameplay.Scenarios.Events {
    public abstract class Base : Scenarios.Base {
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
        [Header("Desidions Settings")]
        [SerializeField] protected int _minDesidionCost = 1;

        public int GetDesidionCost(int cost) => cost >= _minDesidionCost ? cost : _minDesidionCost;

        private protected IDesidion _activeDesidion => _desidions[_activateIndex];
        private protected List<IDesidion> _desidions = new();
        private protected int _activateIndex;

        public Sprite PanelSprite => panelSprite;
        public List<IDesidion> Desidions => _desidions;
        public bool AutoChoice = false;
        public int MaxAutoInterventionPoints = 10;

        private protected abstract string Name { get; }
        private protected abstract string Territory(CivPiece piece);

        private protected Chronicles.Controller ChroniclesController => Chronicles.Controller.Instance;
        private protected Map Map => Transmigratio.Instance.TMDB.map;
        private protected WMSK WMSK => MapData.WMSK;
        private protected Func<int, bool> _useIntervention => Gameplay.Intervention.UseIntervention;
        public string Local(string key) => Localization.Load(Name, key);

        private protected abstract void ActivateEvents();
        private protected abstract void InitDesidions();
        private protected abstract void OpenPanel(CivPiece piece);
        private protected abstract void CreateMarker(CivPiece piece = null);

        public override void Init() {
            InitDesidions();
            ActivateEvents();
            AutoChoice = false;
            Civilization.onAddPiece += OnAddPiece;
            Civilization.onRemovePiece += OnRemovePiece;
        }

        private protected void AddDesidion(Func<CivPiece, Func<CivPiece, int>, bool> click, string title, Func<CivPiece, int> points) {
            _desidions.Add(new DesidionPiece(click, title, points));
        }
        private protected void AddDesidion(Func<TM_Region, Func<TM_Region, int>, bool> click, string title, Func<TM_Region, int> points){
            _desidions.Add(new DesidionRegion(click, title, points));
        }

        private protected virtual IconMarker CreateMarker(Vector3 position, CivPiece piece) {
            IconMarker marker = Instantiate(markerPrefab);
            marker.Piece = piece;
            marker.Sprite = markerSprite;
            marker.SetCount = 1;
            position.z = -0.1f;

            MarkerClickHandler handler = WMSK.AddMarker2DSprite(marker.gameObject, position, 0.018f, true, true);
            handler.allowDrag = false;
            return marker;
        }
    }
}
