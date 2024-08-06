using System;

namespace Gameplay.Scenarios {
    public static class Government {
        public static Data data;

        private static CivPiece _piece;
        private static float add_L_H = 1;
        private static float add_L_PC = 1;
        private static float add_M_F = 1;
        private static float add_M_S = 1;

        private static Paramiter _government;

        public static Action<CivPiece> onPlay;

        private static float Leaderism {
            get => _government["Leaderism"].Value;
            set => _government["Leaderism"].Value = value;
        }

        private static float Monarchy {
            get => _government["Monarchy"].Value;
            set => _government["Monarchy"].Value = value;
        }

        public static void Play(CivPiece piece) {
            Init(piece);
            Update();
            onPlay.Invoke(piece);
        }

        private static void Init(CivPiece piece) {
            _piece = piece;
            _government = piece.Civilization.Government;
        }

        private static void Update() {
            string ecoCulture = _piece.Civilization.EcoCulture.GetMaxQuantity().key;
            string prodMode = _piece.Civilization.ProdMode.GetMaxQuantity().key;

            if (ecoCulture == "Hunters") Leaderism += add_L_H;
            if (prodMode == "PrimitiveCommunism") Leaderism += add_L_PC;

            if(ecoCulture == "Farmers") Monarchy += add_M_F;
            if (prodMode == "Slavery") Monarchy += add_M_S;
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
