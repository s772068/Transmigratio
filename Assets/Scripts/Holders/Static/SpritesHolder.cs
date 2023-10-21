using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Holders/Sprites", fileName = "SpritesHolder")]
public class SpritesHolder : SO_Singleton<SpritesHolder> {
    public List<Sprite> pathIcons;
}
