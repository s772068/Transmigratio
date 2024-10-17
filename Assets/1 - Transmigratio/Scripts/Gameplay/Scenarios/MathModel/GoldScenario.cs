using System;

namespace Gameplay.Scenarios {
    public static class GoldScenario {
        public static Data data;

        private static CivPiece _piece;

        private static float Population => _piece.Population.Value;
        private static float Hunters => _piece.EcoCulture["Hunters"] / AllEco * 100;
        private static float Farmers => _piece.EcoCulture["Farmers"] / AllEco * 100;
        private static float Nomads => _piece.EcoCulture["Nomads"] / AllEco * 100;
        private static float Townsman => _piece.EcoCulture["Townsman"] / AllEco * 100;
        private static float AllEco => _piece.EcoCulture["Hunters"] +
                                       _piece.EcoCulture["Farmers"] +
                                       _piece.EcoCulture["Nomads"]  +
                                       _piece.EcoCulture["Townsman"];
        private static float Gold {
            get => _piece.gold;
            set => _piece.gold += value;
        }

        public static void Play(CivPiece piece) {
            Init(piece);
            Update();
        }

        private static void Init(CivPiece piece) {
            _piece = piece;
        }

        private static void Update() {
            Gold += Population * (Hunters  * data.Hunters +
                                  Farmers  * data.Farmers +
                                  Nomads   * data.Nomads  +
                                  Townsman * data.Townsman) / data.GoldGeneralDivision;
        }

        [Serializable]
        public class Data {
            public float Hunters = 0f;
            public float Farmers = 0.1f;
            public float Nomads = 0.1f;
            public float Townsman = 1f;
            public float GoldGeneralDivision = 100f;
        }
    }
}
