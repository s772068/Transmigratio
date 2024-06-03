using UnityEngine;

public static class MaterialLoader {
    public static Material Load(string name) => Resources.Load<Material>($"Materials/{name}");
}
