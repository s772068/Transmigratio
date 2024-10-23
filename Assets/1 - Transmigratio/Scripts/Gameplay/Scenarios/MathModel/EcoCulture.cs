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

        private static float ÑattleBreeders {
            get => _ecoCulture["ÑattleBreeders"];
            set {
                _ecoCulture.GetValue("ÑattleBreeders").onUpdate?.Invoke(ÑattleBreeders, value, _piece);
                _ecoCulture["ÑattleBreeders"] = value;
            }
        }

        private static float Townsman
        {
            get => _ecoCulture["Townsman"];
            set
            {
                _ecoCulture.GetValue("Townsman").onUpdate?.Invoke(Townsman, value, _piece);
                _ecoCulture["Townsman"] = value;
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
            Hunters += (Forest + Mountain + Steppe + Tundra) /data.EcoCultureDivider;
            Farmers += Plain / data.EcoCultureDivider;
            ÑattleBreeders += (Desert + Mountain + Steppe) / data.EcoCultureDivider;

            if (_piece.Population.Value > data.UrbanMinPop)
                Townsman += (Farmers + ÑattleBreeders) / data.EcoCultureDivider;

            OnUpdateEcoCulture?.Invoke(_piece);

            if (_ecoCulture.GetMax().key == "Farmers")
                News.NewsTrigger?.Invoke("GreenRevolution");
        }

        [Serializable]
        public class Data {
            public float EcoCultureDivider = 100f;
            public float UrbanMinPop = 1000f;
        }
    }
}
