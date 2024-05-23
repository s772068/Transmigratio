using UnityEngine;

public static class PrefabsLoader {
    public static T Load<T>(out T res, string name, Transform parent) where T : MonoBehaviour => res = Object.Instantiate(Resources.Load<T>($"Prefabs/{name}"), parent);
}
