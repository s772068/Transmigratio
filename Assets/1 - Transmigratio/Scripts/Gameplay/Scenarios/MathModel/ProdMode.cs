using System;

namespace Gameplay.Scenarios {
    public static class ProdMode {
        public static Data data;

        private static CivPiece _piece;
        private static Paramiter _prodMode;
        private static Paramiter _ecoCulture;

        public static Action<CivPiece> OnUpdateProdMode;

        private static float PrimitiveCommunism {
            get => _prodMode["PrimitiveCommunism"];
            set {
                _prodMode.GetValue("PrimitiveCommunism").onUpdate?.Invoke(PrimitiveCommunism, value, _piece);
                _prodMode["PrimitiveCommunism"] = value;
            }
        }

        private static float Slavery {
            get => _prodMode["Slavery"];
            set {
                _prodMode.GetValue("Slavery").onUpdate?.Invoke(Slavery, value, _piece);
                _prodMode["Slavery"] = value;
            }
        }

        private static float Feodalism {
            get => _prodMode["Feodalism"];
            set {
                _prodMode.GetValue("Feodalism").onUpdate?.Invoke(Feodalism, value, _piece);
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
            if (ecoCulture == "Hunters") PrimitiveCommunism += data.AddProdMode;
            if (ecoCulture == "Farmers") {
                Slavery += data.AddProdMode;
                Feodalism += data.AddProdMode;
            }
            if (ecoCulture == "Nomads")
                Feodalism += data.AddProdMode;

            OnUpdateProdMode?.Invoke(_piece);
        }

        [Serializable]
        public class Data {
            public float AddProdMode = 1f;
        }
    }
}
