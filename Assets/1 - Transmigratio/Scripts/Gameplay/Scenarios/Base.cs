using UnityEngine;

namespace Gameplay.Scenarios {
    public abstract class Base : ScriptableObject {
        private protected CivPiece _piece;
        private protected abstract void Play();
        public void Play(CivPiece piece) {
            _piece = piece;
            Play();
        }
    }
}
