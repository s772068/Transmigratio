using System.Collections.Generic;

namespace Gameplay {
    public static class Controller {
        private static List<Scenarios.Base> _scenarios;

        public static void Init() {
            _scenarios = Transmigratio.Instance.TMDB.humanity.scenarios;
        }

        public static void GamePlay(CivPiece piece) {
            for (int i = 0; i < _scenarios.Count; ++i) {
                _scenarios[i].Play(piece);
            }
        }
    }
}
