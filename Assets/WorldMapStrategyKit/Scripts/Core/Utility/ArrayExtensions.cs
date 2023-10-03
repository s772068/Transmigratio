using System;


namespace WorldMapStrategyKit {

	public static class ArrayExtensions {
		public static void Fill<T> (this T[] destinationArray, params T[] value) {
			if (destinationArray == null) {
				throw new ArgumentNullException ("destinationArray");
			}

			if (value.Length >= destinationArray.Length) {
				throw new ArgumentException ("Length of value array must be less than length of destination");
			}

			// set the initial array value
			Array.Copy (value, destinationArray, value.Length);

			int arrayToFillHalfLength = destinationArray.Length / 2;
			int copyLength;

			for (copyLength = value.Length; copyLength < arrayToFillHalfLength; copyLength <<= 1) {
				Array.Copy (destinationArray, 0, destinationArray, copyLength, copyLength);
			}

			Array.Copy (destinationArray, 0, destinationArray, copyLength, destinationArray.Length - copyLength);
		}

		public static void Fill<T> (this T[] destinationArray, T value) {
			if (destinationArray == null) {
				throw new ArgumentNullException ("destinationArray");
			}

			if (0 >= destinationArray.Length) {
				throw new ArgumentException ("Length of value array must be less than length of destination");
			}

			// set the initial array value
			destinationArray [0] = value;

			int arrayToFillHalfLength = destinationArray.Length / 2;
			int copyLength;

			for (copyLength = 1; copyLength < arrayToFillHalfLength; copyLength <<= 1) {
				Array.Copy (destinationArray, 0, destinationArray, copyLength, copyLength);
			}

			Array.Copy (destinationArray, 0, destinationArray, copyLength, destinationArray.Length - copyLength);
		}

		public static T[] Purge<T> (this T[] array, int index) {
            int arrayLength = array.Length;
            T[] newArray = new T[arrayLength - 1];
            if (index > 0) {
                Array.Copy(array, newArray, index);
            }
            if (index < arrayLength - 1) {
                Array.Copy(array, index + 1, newArray, index, arrayLength - index - 1);
            }
            return newArray;
        }

		public static T[] Purge<T> (this T[] array, int index1, int index2) {
            int arrayLength = array.Length;
            T[] newArray = new T[arrayLength - 2];
            for (int k = 0, c = 0; k < arrayLength; k++) {
				if (k != index1 && k != index2) {
					newArray [c++] = array [k];
				}
			}
			return newArray;
		}
	}
}

