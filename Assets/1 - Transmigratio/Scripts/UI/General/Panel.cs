using System;
using UnityEngine;

namespace UI
{
    public class Panel : MonoBehaviour
    {
        public static Action<bool> PanelOpen;
        public static Action<bool> PanelClose;
    }
}
