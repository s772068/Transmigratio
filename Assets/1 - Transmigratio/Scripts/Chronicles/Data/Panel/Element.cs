using System;
using UnityEngine;

namespace Chronicles.Data.Panel {
    public struct Element {
        public Sprite Sprite;
        public string EventName;
        public string Desidion;
        public string DesidionName;
        public string ResultName;
        public int RegionID;
        public int StartYear;
        public bool IsActive;

        public LocalVariablesChronicles LocalVariables;
    }

    [Serializable]
    public struct LocalVariablesChronicles
    {
        public string RegionFirst;
        public string RegionSecond;
        public int Count;
    }
}
