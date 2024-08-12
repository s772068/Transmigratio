using UnityEngine;

namespace Gameplay.Scenarios {
    public class Base : ScriptableObject {
        private protected CivPiece _piece;
        public CivPiece Piece { set { _piece = value; } }
        public virtual void Init() {}
        public virtual void Play() {}
    }
}
