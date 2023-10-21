using UnityEngine;
using Zenject;

public class SO_Singleton<T> : ScriptableObjectInstaller where T : ScriptableObject {
    public override void InstallBindings() {
        Container.Bind<T>().FromInstance(this as T).AsSingle();
    }
}
