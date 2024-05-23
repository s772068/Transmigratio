using Unity.VisualScripting;
using UnityEngine;

public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour {
    public static T Instance { get; set; }
    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }
}

public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour {
    protected override void Awake() {
        if(Instance != null) Destroy(gameObject);
        base.Awake();
    }
}

public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour {
    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}

public abstract class SingletonScriptable<T> : ScriptableObject where T : SingletonScriptable<T> {
    protected static string FilePath { get; }
    private static T _instance;
    public static T Instance {
        get {
            if (_instance == null) {
                _instance = Resources.Load<T>($"Settings/{typeof(T).ToString()}");
                //(_instance as SingletonScriptable<T>).OnInitialize();
            }
            return _instance;
        }
    }
}
