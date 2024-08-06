using System;

namespace Gameplay.Scenarios {
    public static class ProdMode {
        public static Data data;

        private static CivPiece _piece;
        private static Paramiter _prodMode;

        public static Action<CivPiece> onPlay;

        private static float PrimitiveCommunism {
            get => _prodMode["PrimitiveCommunism"].Value;
            set => _prodMode["PrimitiveCommunism"].Value = value;
        }

        private static float Slavery {
            get => _prodMode["Slavery"].Value;
            set => _prodMode["Slavery"].Value = value;
        }

        private static float Feodalism {
            get => _prodMode["Feodalism"].Value;
            set => _prodMode["Feodalism"].Value = value;
        }

        public static void Play(CivPiece piece) {
            Init(piece);
            Update();
            onPlay.Invoke(piece);
        }

        private static void Init(CivPiece piece) {
            _piece = piece;
            _prodMode = _piece.Civilization.ProdMode;
        }

        private static void Update() {
            string ecoCulture = _piece.Civilization.EcoCulture.GetMaxQuantity().key;
            if (ecoCulture == "Hunters") PrimitiveCommunism += data.add_PC;
            if (ecoCulture == "Farmers" || ecoCulture == "Nomads") {
                Slavery += data.add_Slavery;
                Feodalism += data.add_Feodalism;
            }
        }

        [Serializable]
        public class Data {
            public float add_PC = 1;
            public float add_Slavery = 1;
            public float add_Feodalism = 1;
        }
    }
}
