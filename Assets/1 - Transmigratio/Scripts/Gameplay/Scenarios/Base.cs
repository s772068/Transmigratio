using UnityEngine;

namespace Gameplay.Scenarios {
    public class Base : ScriptableObject {
        private protected CivPiece _piece;
        public virtual void Init() { }
        public virtual void Play(CivPiece piece) { _piece = piece; Play(); }
        private protected virtual void Play() { }
        private protected virtual void OnAddPiece(CivPiece civPiece) { }
        private protected virtual void OnRemovePiece(CivPiece civPiece) { }
    }
}
