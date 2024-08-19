using System;

namespace Gameplay.Scenarios {
    public static class ProdMode {
        public static Data data;

        private static CivPiece _piece;
        private static Paramiter _prodMode;
        private static Paramiter _ecoCulture;

        private static float PrimitiveCommunism {
            get => _prodMode["PrimitiveCommunism"];
            set {
                _prodMode.GetValue("PrimitiveCommunism").onUpdate(PrimitiveCommunism, value, _piece);
                _prodMode["PrimitiveCommunism"] = value;
            }
        }

        private static float Slavery {
            get => _prodMode["Slavery"];
            set {
                _prodMode.GetValue("Slavery").onUpdate(Slavery, value, _piece);
                _prodMode["Slavery"] = value;
            }
        }

        private static float Feodalism {
            get => _prodMode["Feodalism"];
            set {
                _prodMode.GetValue("Feodalism").onUpdate(Feodalism, value, _piece);
                _prodMode["Feodalism"] = value;
            }
        }

        public static void Play(CivPiece piece) {
            Init(piece);
            Update();
        }

        private static void Init(CivPiece piece) {
            _piece = piece;
            _ecoCulture = piece.EcoCulture;
            _prodMode = piece.ProdMode;
        }

        private static void Update() {
            string ecoCulture = _ecoCulture.GetMax().key;
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
