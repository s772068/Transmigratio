using UnityEngine;
using System;

namespace Chronicles.Data.Panel {
    public struct Element {
        public Sprite sprite;
        public CivPiece piece;
        public string eventName;
        public string description;
        public int regionID;
        public int startYear;
        public bool isActive;

        public Action<CivPiece> onClick;
    }
}
