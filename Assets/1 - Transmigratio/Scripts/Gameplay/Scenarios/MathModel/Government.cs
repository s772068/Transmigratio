using System;
using UnityEngine;

namespace Gameplay.Scenarios {
    public static class Government {
        public static Data data;

        private static CivPiece _piece;
        private static float add_L_H = 1;
        private static float add_L_PC = 1;
        private static float add_M_F = 1;
        private static float add_M_S = 1;

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

            if (ecoCulture == "Hunters") Leaderism += add_L_H;
            if (prodMode == "PrimitiveCommunism") Leaderism += add_L_PC;

            if(ecoCulture == "Farmers") Monarchy += add_M_F;
            if (prodMode == "Slavery") Monarchy += add_M_S;

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
            public float add_L_H = 1;
            public float add_L_PC = 1;
            public float add_M_F = 1;
            public float add_M_S = 1;
        }
    }
}
