using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Theme", fileName = "Theme")]
public class SO_Theme : ScriptableObject {
    [SerializeField] private ST_Paramiter<ST_Detail[]>[] Ecology;
    [SerializeField] private ST_Paramiter<ST_Detail[]>[] Civilization;
    [SerializeField] private Color[] Colors;
    [SerializeField] private Sprite[] Sprites;

    public Color GetEcologyColor(int paramiterIndex, int detailIndex) => Colors[Ecology[paramiterIndex].Value[detailIndex].Color];
    public Color GetCivilizationColor(int paramiterIndex, int detailIndex) => Colors[Civilization[paramiterIndex].Value[detailIndex].Color];

    public Sprite GetEcologySprite(int paramiterIndex, int detailIndex) =>
        detailIndex < 0 ?
        Sprites[Ecology[paramiterIndex].DescriptionSprite] :
        Sprites[Ecology[paramiterIndex].Value[detailIndex].DescriptionSprite];

    public Sprite GetCivilizationSprite(int paramiterIndex, int detailIndex) =>
        detailIndex < 0 ?
        Sprites[Civilization[paramiterIndex].DescriptionSprite] :
        Sprites[Civilization[paramiterIndex].Value[detailIndex].DescriptionSprite];
}
