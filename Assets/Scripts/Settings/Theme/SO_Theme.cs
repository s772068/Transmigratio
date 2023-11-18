using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Theme", fileName = "Theme")]
public class SO_Theme : ScriptableObject {
    [SerializeField] private S_Value<int[]>[] Paramiters;
    [SerializeField] private Color[] Colors;

    public Color GetColor(int paramiterIndex, int detailIndex) => Colors[Paramiters[paramiterIndex].Value[detailIndex]];
}
