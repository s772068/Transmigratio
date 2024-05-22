using UnityEngine;

public static class SpritesLoader {
    public static Sprite LoadPictogram(string name) => Resources.Load<Sprite>($"Sprites/RegionDetails/Pictograms/{name}");
    public static Sprite LoadAvatar(string name) => Resources.Load<Sprite>($"Sprites/Avatars/{name}");
    public static Sprite LoadParamDescription(string name) => Resources.Load<Sprite>($"Sprites/ParamDescription/{name}");
}
