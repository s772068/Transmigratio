using System;
using UnityEngine;

namespace UI
{
    public class Panel : MonoBehaviour
    {
        public static event Action<bool> PanelOpen;
        public static event Action<bool> PanelClose;

        private protected virtual void OnEnable()
        {
            PanelOpen?.Invoke(true);
        }

        private protected virtual void OnDisable()
        {
            PanelClose?.Invoke(true);
        }

        public void CloseWindow()
        {
            Destroy(gameObject);
        }
    }
}
