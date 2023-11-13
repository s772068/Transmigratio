using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameController))]
public class SaveControllerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GameController child = (GameController) target;
        if (GUILayout.Button("Load")) {
            child.Load();
        }
        if (GUILayout.Button("Save")) {
            child.Save();
        }
    }
}
