public static class ArrayHelper {
    public static T[] Add<T>(this T[] owner, T newObject) {
        if (newObject == null) return owner;
        T[] arr = new T[owner.Length + 1];
        
        for (int i = 0; i < owner.Length; ++i) {
            arr[i] = owner[i];
        }
        
        arr[owner.Length] = newObject;
        
        owner = new T[arr.Length];
        for (int i = 0; i < arr.Length; ++i) {
            owner[i] = arr[i];
        }
        return owner;
    }

    public static void Set<T>(this T[] owner, int index, T newObject) {
        if (index < 0) return;
        if (owner != null && index < owner.Length) {
            owner[index] = newObject;
            return;
        } else {
            T[] arr = new T[index + 1];
            
            for (int i = 0; i < owner.Length; ++i) {
                arr[i] = owner[i];
            }
            
            for (int i = owner.Length; i < index; ++i) {
                arr[i] = default;
            }

            arr[index] = newObject;
            
            owner = new T[arr.Length];
            for (int i = 0; i < arr.Length; ++i) {
                owner[i] = arr[i];
            }

        }
    }

    public static void Remove<T>(this T[] owner, int index) {
        T[] arr = new T[owner.Length];
        for (int i = 0; i < owner.Length; ++i) {
            if (i == index) continue;
            arr[i] = owner[i];
        }

        owner = new T[arr.Length];
        for (int i = 0; i < arr.Length; ++i) {
            owner[i] = arr[i];
        }
    }

    public static void Clear<T>(this T[] owner) {
        owner = null;
    }
}
