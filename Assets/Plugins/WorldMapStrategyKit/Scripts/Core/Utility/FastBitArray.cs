using UnityEngine;
using System.Runtime.CompilerServices;

namespace WorldMapStrategyKit {

    public class FastBitArray {

		public uint[] bits;

		public FastBitArray (int capacity) {
			bits = new uint[capacity];
		}

        public FastBitArray(Color32[] colors, byte minValue, byte maxValue) {
            int pixelCount = colors != null ? colors.Length : 0;
            bits = new uint[pixelCount >> 5];
            for (int k = 0; k < pixelCount; k++) {
                if (colors[k].r >= minValue && colors[k].r <= maxValue) {
                    bits[k >> 5] |= (uint)(1 << (k & 31));
                }
            }
        }

        public FastBitArray (byte[] colors, byte minValue, byte maxValue) {
			int pixelCount = colors != null ? colors.Length : 0;
			bits = new uint[pixelCount >> 5];
			for (int k = 0; k < pixelCount; k++) {
				if (colors [k] >= minValue && colors[k] <= maxValue) {
					bits [k >> 5] |= (uint)(1 << (k & 31));
				}
			}
		}

		#if !UNITY_WSA
		[MethodImpl(256)] // equals to MethodImplOptions.AggressiveInlining
		#endif
		public void SetBit (int index) {
			bits [index >> 5] |= (uint)(1 << (index & 31));
		}

		#if !UNITY_WSA
		[MethodImpl(256)] // equals to MethodImplOptions.AggressiveInlining
		#endif
		public void ClearBit (int index) {
			bits [index >> 5] &= (uint)(~(1 << (index & 31)));
		}

		#if !UNITY_WSA
		[MethodImpl(256)] // equals to MethodImplOptions.AggressiveInlining
		#endif
		public bool GetBit(int index) {
			return (bits [index >> 5] & (1 << (index & 31))) != 0;
		}

	}

}