using UnityEngine;
using System;

namespace Chronicles.Data.Panel {
    public struct Element {
        public Sprite Sprite;
        public CivPiece Piece;
        public string EventName;
        public string DescriptionName;
        public string Desidion;
        public string Result;
        public int RegionID;
        public int StartYear;
        public bool IsActive;

        public int StartPop;
        public int EndPop;

        public Action<CivPiece> Click;
    }
}
