using UnityEngine;
using Zenject;

public class MonoSingleton<T> : MonoInstaller where T : Component {
    public override void InstallBindings() {
        Container.Bind<T>().FromInstance(this as T).AsSingle();
    }
}