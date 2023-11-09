using UnityEngine;

public abstract class BaseController : MonoBehaviour {
    public virtual GameController GameController { set { } }
    public virtual void Init() { }
}
