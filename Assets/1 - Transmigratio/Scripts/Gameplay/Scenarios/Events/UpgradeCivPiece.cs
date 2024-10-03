using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Scenarios.Events {
    [CreateAssetMenu(menuName = "ScriptableObjects/Scenarios/Events/UpgradeCivPiece", fileName = "UpgradeCivPiece")]
    public class UpgradeCivPiece : Scenarios.Base {
        [Header("Settings")]
        [SerializeField] private int _minPopulationForUpgrade = 1000;
        [SerializeField][Range(0, 100)] private int _chanceToUpgradeTierOne = 60;
        [Header("Civ List")]
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

            Government.OnUpdateMaxGovernment += OnUpdateMaxGovernment;
        }
        ~UpgradeCivPiece() {
            Government.OnUpdateMaxGovernment -= OnUpdateMaxGovernment;
        }
        

        private void OnUpdateMaxGovernment(string maxGovernment, CivPiece piece) {
            OnUpgrade(piece);
        }

        private void OnUpgrade(CivPiece piece) {
            if (piece.Population.Value < _minPopulationForUpgrade)
                return;

            if (piece.Category == 3)
            {
                UpgradeCiv(2, piece);
                if (!_firstUpgrade)
                {
                    _firstUpgrade = true;
                    News.NewsTrigger?.Invoke("FirstUpgradeCivilization");
                }
            }
            else if (piece.Category == 2)
            {
                if (_random.Next(0, 100) <= _chanceToUpgradeTierOne) { 
                    UpgradeCiv(1, piece);
                    if (!_secondUpgrade)
                    {
                        _secondUpgrade = true;
                        News.NewsTrigger?.Invoke("SecondUpgradeCivilization");
                    }
                }
                else
                    UpgradeCiv(2, piece);
            }
        }

        private void UpgradeCiv(int category, CivPiece piece) {
            string oldCivName = piece.CivName;
            string newCivName = GetCivName(category);
            if (!_civilizations.ContainsKey(newCivName))
            {
                _civilizations[newCivName] = new(newCivName);
            }
            else
                return;

            _civilizations[newCivName].AddPiece(piece, newCivName, category);
            _civilizations[oldCivName].RemovePiece(piece.Region.Id);
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
