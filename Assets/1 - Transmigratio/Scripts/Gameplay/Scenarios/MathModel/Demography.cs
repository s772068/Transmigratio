using System;

namespace Gameplay.Scenarios {
    public static class Demography {
        public static Data data;

        private static CivPiece _piece;
        private static Population _population;
        private static Paramiter _flora;
        private static Paramiter _fauna;
        private static float _floraKr;
        private static float _faunaKr;
        private static float _floraGrow;
        private static float _faunaGrow;
        private static float _prodModeK;
        private static float _populationGrowPercent;
        private static float _governmentCorruption;

        public static Action<CivPiece> OnUpdateDemography;

        private static int Population {
            get => _population.Value;
            set {
                _population.onUpdate?.Invoke(Population, value, _piece);
                _population.Value = value;
            }
        }

        private static float PopulationGrow {
            get => _piece.PopulationGrow.value;
            set {
                _piece.PopulationGrow.onUpdate?.Invoke(PopulationGrow, value, _piece);
                _piece.PopulationGrow.value = value;
            }
        }

        private static float ReserveFood {
            get => _piece.ReserveFood.value;
            set {
                _piece.ReserveFood.onUpdate?.Invoke(ReserveFood, value, _piece);
                _piece.ReserveFood.value = value;
            }
        }

        private static float RequestFood {
            get => _piece.RequestFood.value;
            set {
                _piece.RequestFood.onUpdate?.Invoke(RequestFood, value, _piece);
                _piece.RequestFood.value = value;
            }
        }

        private static float TakenFood {
            get => _piece.TakenFood.value;
            set {
                _piece.TakenFood.onUpdate?.Invoke(TakenFood, value, _piece);
                _piece.TakenFood.value = value;
            }
        }

        private static float GivenFood {
            get => _piece.GivenFood.value;
            set {
                _piece.GivenFood.onUpdate?.Invoke(GivenFood, value, _piece);
                _piece.GivenFood.value = value;
            }
        }

        private static float Flora {
            get => _flora["Flora"];
            set {
                _flora.GetValue("Flora").onUpdate?.Invoke(Flora, value, _piece);
                _flora["Flora"] = value;
            }
        }

        private static float Fauna {
            get => _fauna["Fauna"];
            set {
                _fauna.GetValue("Fauna").onUpdate?.Invoke(Fauna, value, _piece);
                _fauna["Fauna"] = value;
            }
        }

        public static void Play(CivPiece piece) {
            Init(piece);
            Update();
        }

        private static void Init(CivPiece piece) {
            _piece = piece;
            _population = _piece.Population;
            _flora = _piece.Region.Flora;
            _fauna = _piece.Region.Fauna;
        }

        private static void Update() {
            string ecoCulture = _piece.EcoCulture.GetMax().key;
            string prodMode = _piece.ProdMode.GetMax().key;
            string government = _piece.Government.GetMax().key;

            _prodModeK = prodMode switch {
                "PrimitiveCommunism" => data.prodModeK_PC,
                "Slavery" => data.prodModeK_S,
                _ => 1
            };

            _governmentCorruption = government switch {
                "Leaderism" => data.governmentCorruption_L,
                "Monarchy" => data.governmentCorruption_M,
                _ => 1
            };

            _floraGrow = data.val1 / Flora;
            _faunaGrow = (ecoCulture == "Nomads" ? data.val2 : data.val3) / Fauna;
            Population += (int) PopulationGrow;
            if (Population <= data.MinPiecePopulation) { _piece.Destroy?.Invoke(); return; }
            ReserveFood += TakenFood - GivenFood;
            RequestFood = Population / data.val4;
            GivenFood = ReserveFood > RequestFood ? RequestFood : ReserveFood;
            PopulationGrow = Population * _governmentCorruption * GivenFood / RequestFood - Population / data.val5;
            _populationGrowPercent = PopulationGrow / Population * data.val6;
            Flora = Math.Min(Flora - TakenFood/ data.val7 + _floraGrow, _flora.GetStartValue("Flora"));
            Fauna = Math.Min(Fauna - TakenFood/ data.val8 + _faunaGrow, _fauna.GetStartValue("Fauna"));
            _floraKr = (float) (Math.Pow(Flora, data.val9) / data.val10);
            _faunaKr = (float) (Math.Pow(Fauna, data.val11) / data.val12);
            TakenFood = ecoCulture == "Farmers" ?
                Population / data.val13 * _prodModeK * _floraKr :
                Population / data.val14 * _prodModeK * _faunaKr;

            OnUpdateDemography?.Invoke(_piece);
        }

        [Serializable]
        public class Data {
            public float prodModeK_PC = 0.6f;
            public float prodModeK_S = 0.65f;
            public float governmentCorruption_L = 0.4f;
            public float governmentCorruption_M = 0.45f;
            public float MinPiecePopulation = 50;
            public float val1 = 50f;
            public float val2 = 60f;
            public float val3 = 50f;
            public float val4 = 150f;
            public float val5 = 3f;
            public float val6 = 100f;
            public float val7 = 10f;
            public float val8 = 10f;
            public double val9 = 0.58d;
            public float val10 = 10f;
            public double val11 = 0.58d;
            public float val12 = 10f;
            public float val13 = 100f;
            public float val14 = 100f;
        }
    }
}
