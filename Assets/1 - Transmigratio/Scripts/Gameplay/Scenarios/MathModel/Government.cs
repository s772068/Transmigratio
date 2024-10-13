using System;

namespace Gameplay.Scenarios {
    public static class Government {
        public static Data data;

        private static CivPiece _piece;

        private static Paramiter _government;

        public static Action<string, CivPiece> OnUpdateMaxGovernment;

        private static float Leaderism {
            get => _government["Leaderism"];
            set {
                _government.GetValue("Leaderism").onUpdate?.Invoke(Leaderism, value, _piece);
                _government["Leaderism"] = value;
            }
        }

        private static float Monarchy {
            get => _government["Monarchy"];
            set {
                _government.GetValue("Monarchy").onUpdate?.Invoke(Monarchy, value, _piece);
                _government["Monarchy"] = value;
            }
        }

        private static float CityState
        {
            get => _government["CityState"];
            set
            {
                _government.GetValue("CityState").onUpdate?.Invoke(CityState, value, _piece);
                _government["CityState"] = value;
            }
        }

        private static float Empire
        {
            get => _government["Empire"];
            set
            {
                _government.GetValue("Empire").onUpdate?.Invoke(Empire, value, _piece);
                _government["Empire"] = value;
            }
        }

        public static void Play(CivPiece piece) {
            Init(piece);
            Update();
        }

        private static void Init(CivPiece piece) {
            _piece = piece;
            _government = piece.Government;
        }

        private static void Update() {
            string ecoCulture = _piece.EcoCulture.GetMax().key;
            string prodMode = _piece.ProdMode.GetMax().key;

            string prevMax = _government.GetMax().key;

            if (ecoCulture == "Hunters") Leaderism += data.AddGov;
            if (prodMode == "PrimitiveCommunism") Leaderism += data.AddGov;

            if (prodMode != "PrimitiveCommunism" || ecoCulture != "Hunters") 
                CityState += _piece.EcoCulture.GetValue("Townsman").value * data.AddGov * data.CityStateMultiplier;

            if(ecoCulture == "Farmers" || ecoCulture == "Nomads") Monarchy += data.AddGov;
            if (prodMode == "Slavery") Monarchy += data.AddGov;

            string curMax = _government.GetMax().key;
            if (SplitCheck())
                OnUpdateMaxGovernment?.Invoke(curMax, _piece);
        }

        private static bool SplitCheck()
        {
            if (_piece.Civilization.Pieces.Count < 2) return false;

            foreach (CivPiece piece in _piece.Civilization.Pieces.Values)
            {
                if (piece.Government.GetMax().key != _piece.Government.GetMax().key)
                    return true;
            }

            return false;
        }

        [Serializable]
        public class Data {
            public float AddGov = 1f;
            public float CityStateMultiplier = 100f;
        }
    }
}
