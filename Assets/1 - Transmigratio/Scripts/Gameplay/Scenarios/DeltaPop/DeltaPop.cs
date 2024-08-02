using UnityEngine;
using System;
using WorldMapStrategyKit;

namespace Gameplay.Scenarios {
    [CreateAssetMenu(menuName = "Gameplay/DeltaPop", fileName = "DeltaPop")]
    public class DeltaPop : Base {
        public double val1 = 0.58d;
        public float val2 = 10f;
        public float val3 = 100f;
        public float val4 = 150f;
        public float val5 = 3f;
        public int MinPiecePopulation = 50;

        private static float faunaKr;

        float PopulationGrow {
            get => _piece.PopulationGrow;
            set => _piece.PopulationGrow = value;
        }
        float ReserveFood {
            get => _piece.ReserveFood;
            set => _piece.ReserveFood = value;
        }
        float RequestFood {
            get => _piece.RequestFood;
            set => _piece.RequestFood = value;
        }
        float TakenFood {
            get => _piece.TakenFood;
            set => _piece.TakenFood = value;
        }
        float GivenFood {
            get => _piece.GivenFood;
            set => _piece.GivenFood = value;
        }
        int Population {
            get => _piece.Population.value;
            set => _piece.Population.value = value;
        }

        float PrevPopulationGrow { set => _piece.PrevPopulationGrow = value; }

        float GovernmentCorruption => _piece.Civilization.GovernmentCorruption;
        float ProdModeK => _piece.Civilization.ProdModeK;
        int MaxQuantity => _piece.Region.Fauna.GetMaxQuantity().value;

        private protected override void Play() {
            faunaKr = (float) (Math.Pow(MaxQuantity, val1) / val2);
            TakenFood = Population / val3 * faunaKr * ProdModeK;
            RequestFood = Population / val4;
            GivenFood = ReserveFood > RequestFood ? RequestFood : ReserveFood;
            ReserveFood += TakenFood - GivenFood;
            PrevPopulationGrow = PopulationGrow;
            PopulationGrow = Population * GovernmentCorruption * GivenFood / RequestFood - Population / val5;

            Population += (int) PopulationGrow;
            if (Population <= MinPiecePopulation) _piece.Destroy?.Invoke();
        }
    }
}
