using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SelectFont))]
public class SelectFontEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        SelectFont owner = (SelectFont)target;

        if (GUILayout.Button("Select")) {
            owner.Select();
        }
    }
}
