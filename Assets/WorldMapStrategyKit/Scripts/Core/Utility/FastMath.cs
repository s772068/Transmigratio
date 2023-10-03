using System.Runtime.CompilerServices;

namespace WorldMapStrategyKit {

	public static class FastMath {

		#if !UNITY_WSA
		[MethodImpl(256)] // equals to MethodImplOptions.AggressiveInlining
		#endif
		public static int Abs(int x) {
			return (x ^ (x >> 31)) - (x >> 31);
		}
	}
}