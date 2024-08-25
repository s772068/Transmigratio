using System;

namespace Gameplay.Scenarios {
    public static class EcoCulture {
        public static Data data;

        private static CivPiece _piece;
        private static Paramiter _ecoCulture;
        private static Paramiter _terrain;

        public static Action<CivPiece> OnUpdateEcoCulture;

        private static float Hunters {
            get => _ecoCulture["Hunters"];
            set {
                _ecoCulture.GetValue("Hunters").onUpdate?.Invoke(Hunters, value, _piece);
                _ecoCulture["Hunters"] = value;
            }
        }

        private static float Farmers {
            get => _ecoCulture["Farmers"];
            set {
                _ecoCulture.GetValue("Farmers").onUpdate?.Invoke(Farmers, value, _piece);
                _ecoCulture["Farmers"] = value;
            }
        }

        private static float Nomads {
            get => _ecoCulture["Nomads"];
            set {
                _ecoCulture.GetValue("Nomads").onUpdate?.Invoke(Nomads, value, _piece);
                _ecoCulture["Nomads"] = value;
            }
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
            _ecoCulture = piece.EcoCulture;
            _terrain = piece.Region.Terrain;
        }

        private static void Update() {
            Hunters += (Forest + Mountain + Steppe + Tundra) /data.huntersDivision;
            Farmers += Plain / data.farmersDivision;
            Nomads += (Desert + Mountain + Steppe) / data.nomadsDivision;

            OnUpdateEcoCulture?.Invoke(_piece);

            if (_ecoCulture.GetMax().key == "Farmers")
                News.NewsTrigger?.Invoke("GreenRevolution");
        }

        [Serializable]
        public class Data {
            public float huntersDivision;
            public float farmersDivision;
            public float nomadsDivision;
        }
    }
}
