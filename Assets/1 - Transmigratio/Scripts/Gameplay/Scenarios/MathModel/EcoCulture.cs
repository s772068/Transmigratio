using System;

namespace Gameplay.Scenarios {
    public static class EcoCulture {
        public static Data data;

        private static CivPiece _piece;
        private static Paramiter _ecoCulture;
        private static Paramiter _terrain;

        private static float Hunters {
            get => _ecoCulture["Hunters"];
            set => _ecoCulture["Hunters"] = value;
        }

        private static float Farmers {
            get => _ecoCulture["Farmers"];
            set => _ecoCulture["Farmers"] = value;
        }

        private static float Nomads {
            get => _ecoCulture["Nomads"];
            set => _ecoCulture["Nomads"] = value;
        }

        private static float Plain => _terrain["Plain"];
        private static float Forest => _terrain["Forest"];
        private static float Desert => _terrain["Desert"];
        private static float Mountain => _terrain["Mountain"];
        private static float Steppe => _terrain["Steppe"];
        private static float Tundra => _terrain["Tundra"];

        public static void Play(CivPiece piece) {
            Init(piece);
            Update();
        }

        private static void Init(CivPiece piece) {
            _piece = piece;
            _ecoCulture = _piece.EcoCulture;
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
