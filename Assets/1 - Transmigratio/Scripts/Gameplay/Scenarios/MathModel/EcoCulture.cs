using System;

namespace Gameplay.Scenarios {
    public static class EcoCulture {
        public static Data data;

        private static CivPiece _piece;
        private static Paramiter _ecoCulture;
        private static Paramiter _terrain;

        public static Action<CivPiece> onPlay;
        
        private static float Hunters {
            get => _ecoCulture["Hunters"].Value;
            set => _ecoCulture["Hunters"].Value = value;
        }

        private static float Farmers {
            get => _ecoCulture["Farmers"].Value;
            set => _ecoCulture["Farmers"].Value = value;
        }

        private static float Nomads {
            get => _ecoCulture["Nomads"].Value;
            set => _ecoCulture["Nomads"].Value = value;
        }

        private static float Plain => _terrain["Plain"].Value;
        private static float Forest => _terrain["Forest"].Value;
        private static float Desert => _terrain["Desert"].Value;
        private static float Mountain => _terrain["Mountain"].Value;
        private static float Steppe => _terrain["Steppe"].Value;
        private static float Tundra => _terrain["Tundra"].Value;

        public static void Play(CivPiece piece) {
            Init(piece);
            Update();
            onPlay.Invoke(piece);
        }

        private static void Init(CivPiece piece) {
            _piece = piece;
            _ecoCulture = _piece.Civilization.EcoCulture;
            _terrain = _piece.Region.Terrain;
        }

        private static void Update() {
            Hunters += (Forest + Mountain + Steppe + Tundra) /data.huntersDivision;
            Farmers += Plain / data.farmersDivision;
            Nomads += (Desert + Mountain + Steppe) / data.nomadsDivision;
        }

        [Serializable]
        public class Data {
            public float huntersDivision;
            public float farmersDivision;
            public float nomadsDivision;
        }
    }
}
