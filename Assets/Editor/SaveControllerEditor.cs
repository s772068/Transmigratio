using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveController))]
public class SaveControllerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        SaveController child = (SaveController) target;
        if (GUILayout.Button("Load")) {
            child.Load();
        }
        if (GUILayout.Button("Save")) {
            child.Save();
        }
    }
}
