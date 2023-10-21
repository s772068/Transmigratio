using UnityEngine;

[CreateAssetMenu(menuName = "Holders/Colors", fileName = "ColorsHolder")]
public class ColorsHolder : SO_Singleton<ColorsHolder> {
    [SerializeField] private Color selectCountry;
    public Color SelectCountry => selectCountry;
}
