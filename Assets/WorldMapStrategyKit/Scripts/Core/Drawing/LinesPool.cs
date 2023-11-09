using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit {

    public static class LinesPool {

        public const int CAPACITY = 32;

        struct LinesPoolEntry {
            public GameObject line;
            public bool inUse;
        }

        static LinesPoolEntry[] lines = new LinesPoolEntry[CAPACITY];

        public static GameObject Get() {
            int buffersLength = lines.Length;
            for (int k = 0; k < buffersLength; k++) {
                if (!lines[k].inUse && lines[k].line != null) {
                    lines[k].inUse = true;
                    lines[k].line.SetActive(true);
                    return lines[k].line;
                }
            }
            return null;
        }

        public static void Push(GameObject prototype) {
            int buffersLength = lines.Length;
            for (int k = 0; k < buffersLength; k++) {
                if (lines[k].line == null) {
                    lines[k].inUse = true;
                    lines[k].line = prototype;
                    return;
                }
            }
            System.Array.Resize(ref lines, lines.Length * 2);
            Push(prototype);
        }

        public static void Release(GameObject line) {
            if (lines == null) return;
            int buffersLength = lines.Length;
            for (int k = 0; k < buffersLength; k++) {
                if (lines[k].line == line) {
                    lines[k].inUse = false;
                    line.SetActive(false);
                    return;
                }
            }
        }


    }

}