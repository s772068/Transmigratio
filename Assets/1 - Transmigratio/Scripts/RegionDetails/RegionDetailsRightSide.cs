using UnityEngine.UI;
using UnityEngine;
using TMPro;
using AYellowpaper.SerializedCollections;

namespace RegionDetails.Old {
    public class RegionDetailsRightSide : MonoBehaviour {
        [SerializeField] TMP_Text description;
        [SerializeField] Image avatar;
        [SerializeField] SerializedDictionary<string, Sprite> avatars;

        [SerializeField] public string Description { set => description.text = value; }
        [SerializeField] public Sprite Avatar { set => avatar.sprite = value; }

        public void UpdateData(string element, string val) {
            Debug.Log($"Element: {element} | {val}Describe");
            Description = Localization.Load(element, $"{val}Describe");
            Avatar = avatars[val];
        }
    }
}
