using UnityEngine;

namespace WorldMapStrategyKit {

    public interface IInputProxy {

        void Init();
        bool GetButtonDown(string buttonName);
        bool GetButtonUp(string buttonName);
        bool GetMouseButton(int buttonIndex);
        bool GetMouseButtonDown(int buttonIndex);
        bool GetMouseButtonUp(int buttonIndex);
        Vector3 mousePosition { get; }
        bool touchSupported { get; }
        int touchCount { get; }
        bool GetKey(string name);
        bool GetKey(KeyCode keyCode);
        bool GetKeyDown(string name);
        bool GetKeyDown(KeyCode keyCode);
        bool GetKeyUp(string name);
        bool GetKeyUp(KeyCode keyCode);
        bool IsTouchStarting(int touchIndex);
        bool IsTouchEnding(int touchIndex);
        float GetAxis(string axisName);
        int GetFingerIdFromTouch(int touchIndex);
        Vector2 GetTouchDeltaPosition(int touchIndex);
        Vector2 GetTouchPosition(int touchIndex);
        bool IsPointerOverUI();
        float GetMouseScrollWheel();
    }

}