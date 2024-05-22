using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class RegionDetailsRightSide : MonoBehaviour {
    [SerializeField] TMP_Text description;
    [SerializeField] Image avatar;

    [SerializeField] public string Description { set => description.text = value; }
    [SerializeField] public Sprite Avatar { set => avatar.sprite = value; }

    public void UpdateData(string val) {
        Description = StringLoader.Load($"{val}RightSide");
        Avatar = SpritesLoader.LoadParamDescription(val);
    }
}
