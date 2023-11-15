using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Theme", fileName = "Theme")]
public class SO_Theme : ScriptableObject {
    [SerializeField] private int[] Climates;
    [SerializeField] private int[] Terrains;
    [SerializeField] private int[] Civilizations;
    [SerializeField] private int[] Production;
    [SerializeField] private int[] Economics;
    [SerializeField] private int[] Goverments;
    [SerializeField] private Color[] colors;
    public Color ClimateColor(int index) => colors[Climates[index]];
    public Color TerrainColor(int index) => colors[Terrains[index]];
    public Color CivilizationColor(int index) => colors[Civilizations[index]];
    public Color ProductionColor(int index) => colors[Production[index]];
    public Color EconomicsColor(int index) => colors[Economics[index]];
    public Color GovermentColor(int index) => colors[Goverments[index]];
}
