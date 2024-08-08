using UnityEngine;

namespace Gameplay.Scenarios {
    public class Base : ScriptableObject {
        private protected CivPiece _piece;
        public virtual void Init() {}
        private protected virtual void Play() {}
        public void Play(CivPiece piece) {
            _piece = piece;
            Play();
        }
    }
}
