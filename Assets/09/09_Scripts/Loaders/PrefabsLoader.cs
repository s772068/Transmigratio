using UnityEngine;

public static class PrefabsLoader {
    public static T Load<T>(string name, Transform parent) where T : MonoBehaviour => Object.Instantiate(Resources.Load<T>($"Prefabs/{name}"), parent);
}
