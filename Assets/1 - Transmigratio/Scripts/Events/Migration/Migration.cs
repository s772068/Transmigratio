using UnityEngine.UI;
using Database.Data;
using UnityEngine;
using TMPro;

public class Migration : MonoBehaviour {
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text info;
    [SerializeField] private TMP_Text trialsTitle;
    [SerializeField] private MigrationParamGroup paramiters;
    [SerializeField] private MigrationTrialsGroup trials;

    private Region region;

    public Region Region { set {
            region = value;
        }
    }

}
