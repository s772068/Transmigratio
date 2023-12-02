using UnityEngine.SceneManagement;
using AYellowpaper;
using UnityEngine;

public class GameController : MonoBehaviour {
    [SerializeField] private InterfaceReference<IGameConnecter, MonoBehaviour>[] connecters;

    public bool Get<T>(out T value) where T : IGameConnecter {
        for (int i = 0; i < connecters.Length; ++i) {
            if (connecters[i].Value is T) {
                value = (T) connecters[i].Value;
                return true;
            }
        }
        value = default;
        return false;
    }

    public T Get<T>() where T : IGameConnecter {
        for (int i = 0; i < connecters.Length; ++i) {
            if (connecters[i].Value is T) {
                return (T) connecters[i].Value;
            }
        }
        return default;
    }

    public void OpenScene(int index) => SceneManager.LoadScene(index);

    private void Awake() {
        for (int i = 0; i < connecters.Length; ++i) {
            connecters[i].Value.GameController = this;
            connecters[i].Value.Init();
        }
    }
}
