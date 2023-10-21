using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapHolder))]
public class MapEditor : Editor {
    public override void OnInspectorGUI() {
        MapHolder child = (MapHolder)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Load")) {
            child.Load();
        }
        if (GUILayout.Button("Save")) {
            child.Save();
        }
    }
}
