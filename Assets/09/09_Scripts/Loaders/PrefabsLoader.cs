using UnityEngine;

public static class PrefabsLoader {
    public static T Load<T>(out T res, Transform parent) where T : MonoBehaviour => res = Object.Instantiate(Resources.Load<T>($"Prefabs/{typeof(T)}"), parent);
    public static T Load<T>(out T res) where T : MonoBehaviour => res = Object.Instantiate(Resources.Load<T>($"Prefabs/{typeof(T)}"));
}
