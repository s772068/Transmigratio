using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Scenarios.Events {
    [CreateAssetMenu(menuName = "ScriptableObjects/Scenarios/Events/UpgradeCivPiece", fileName = "UpgradeCivPiece")]
    public class UpgradeCivPiece : Scenarios.Base {
        [SerializeField] private List<string> category1;
        [SerializeField] private List<string> category2;

        private bool _firstUpgrade = false;
        private bool _secondUpgrade = false;

        private System.Random _random;
        SerializedDictionary<string, Civilization> _civilizations;

        public static Action<CivPiece> OnUpgradeCivPiece;

        public override void Init() {
            _random = new();
            _civilizations = Transmigratio.Instance.TMDB.humanity.Civilizations;

            _firstUpgrade = false;
            _secondUpgrade = false;

            Civilization.onAddPiece += OnAddPiece;
            Civilization.onRemovePiece += OnRemovePiece;
        }

        private protected override void OnAddPiece(CivPiece civPiece) {
            Government.OnUpdateMaxGovernment += OnUpdateMaxGovernment;
            // civPiece.Government.GetValue("Monarchy").onUpdate += OnUpgradeToMonarchy;
            // civPiece.Government.GetValue("CityState").onUpdate += OnUpgradeToCityState;
        }

        private protected override void OnRemovePiece(CivPiece civPiece) {
            Government.OnUpdateMaxGovernment -= OnUpdateMaxGovernment;
            // civPiece.Government.GetValue("Monarchy").onUpdate -= OnUpgradeToMonarchy;
            // civPiece.Government.GetValue("CityState").onUpdate -= OnUpgradeToCityState;
        }

        private void OnUpdateMaxGovernment(string maxGovernment) {
            switch (maxGovernment) {
                case "Monarchy":
                    OnUpgradeToMonarchy();
                    break;
                case "CityState": {
                        OnUpgradeToCityState();
                        break;
                    }
            }
        }

        private void OnUpgradeToMonarchy() {
            UpgradeCiv(2);
            if (!_firstUpgrade)
            {
                _firstUpgrade = true;
                News.NewsTrigger?.Invoke("FirstUpgradeCivilization");
            }
        }

        private void OnUpgradeToCityState() {
            if (_random.Next(1, 100) <= 60) {
                UpgradeCiv(1);
                if (!_secondUpgrade)
                {
                    _secondUpgrade = true;
                    News.NewsTrigger?.Invoke("SecondUpgradeCivilization");
                }
            }
        }

        private void UpgradeCiv(int category) {
            string oldCivName = _piece.CivName;
            string newCivName = GetCivName(category);
            if (!_civilizations.ContainsKey(newCivName)) {
                _civilizations[newCivName] = new(newCivName);
            }
            _civilizations[newCivName].AddPiece(_piece, newCivName, category);
            _civilizations[oldCivName].RemovePiece(_piece.Region.Id);
        }

        private string GetCivName(int category) {
            List<string> curCivNames = Transmigratio.Instance.TMDB.humanity.Civilizations.Keys.ToList();
            List<string> newCivNames = category switch {
                1 => category1.Except(curCivNames).ToList(),
                2 => category2.Except(curCivNames).ToList(),
                _ => new()
            };
            int countNewCivNames = newCivNames.Count;
            if (countNewCivNames == 0) return "Uncivilized";
            return newCivNames[_random.Next(0, countNewCivNames - 1)];
        }
    }
}
