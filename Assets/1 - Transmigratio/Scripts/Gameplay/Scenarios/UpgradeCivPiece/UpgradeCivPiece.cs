using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Scenarios.Events {
    [CreateAssetMenu(menuName = "Gameplay/UpgradeCivPiece", fileName = "UpgradeCivPiece")]
    public class UpgradeCivPiece : Scenarios.Base {
        [SerializeField] private List<string> category1;
        [SerializeField] private List<string> category2;

        private System.Random _random;
        SerializedDictionary<string, Civilization> _civilizations;

        public static Action<CivPiece> OnUpgradeCivPiece;

        public override void Init() {
            _random = new();
            _civilizations = Transmigratio.Instance.TMDB.humanity.Civilizations;

            Civilization.onAddPiece += OnAddPiece;
            Civilization.onRemovePiece += OnRemovePiece;
        }

        private protected override void OnAddPiece(CivPiece civPiece) {
            civPiece.Government.GetValue("Monarchy").onUpdate += OnUpgradeToMonarchy;
            civPiece.Government.GetValue("CityState").onUpdate += OnUpgradeToCityState;
        }

        private protected override void OnRemovePiece(CivPiece civPiece) {
            civPiece.Government.GetValue("Monarchy").onUpdate -= OnUpgradeToMonarchy;
            civPiece.Government.GetValue("CityState").onUpdate -= OnUpgradeToCityState;
        }

        private void OnUpgradeToMonarchy(float prev, float cur, CivPiece piece) {
            if (prev == 0 && cur > 0) {
                int newCategory = _random.Next(1, 100) <= 60 || _piece.Category == 3 ? 2 : 3;
                UpgradeCiv(newCategory);
            }
        }

        private void OnUpgradeToCityState(float prev, float cur, CivPiece piece) {
            if (prev == 0 && cur > 0) {
                int newCategory = _random.Next(1, 100) <= 60 || _piece.Category == 3 ? 1 : 2;
                UpgradeCiv(newCategory);
            }
        }

        private void UpgradeCiv(int category) {
            if (category == 3) return;
            string oldCivName = _piece.CivName;
            string newCivName = GetCivName(category);
            if (!_civilizations.ContainsKey(newCivName)) {
                _civilizations[newCivName] = new(newCivName);
            }
            _civilizations[newCivName].AddPiece(_piece, newCivName, category);
            _civilizations[oldCivName].RemovePiece(_piece.RegionID);
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
