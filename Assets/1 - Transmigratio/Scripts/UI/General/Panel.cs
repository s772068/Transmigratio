using System;
using UnityEngine;

namespace UI
{
    public class Panel : MonoBehaviour
    {
        public static event Action PanelOpen;
        public static event Action PanelClose;

        private protected virtual void OnEnable()
        {
            PanelOpen?.Invoke();
        }

        private protected virtual void OnDisable()
        {
            PanelClose?.Invoke();
        }

        public void CloseWindow()
        {
            Destroy(gameObject);
        }
    }
}
