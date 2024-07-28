using UnityEngine;
using System;

public class DragFingerControler : MonoBehaviour
{
    public static event Action<Vector2> OnTouched;

    private void Update()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Input.touchCount > 0)
        { 
            if (OnTouched != null)
            {
                OnTouched(Input.touches[0].position);
            }
            else
            {
                //Debug.Log("ontouch is null");
            }
        }
#endif
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (OnTouched != null)
            {
                OnTouched(Input.mousePosition);
            }
        }
#endif
    }
}
