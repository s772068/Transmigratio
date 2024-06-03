using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Migration : MonoBehaviour {
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text info;
    [SerializeField] private TMP_Text trialsTitle;
    [SerializeField] private MigrationParamGroup paramiters;
    [SerializeField] private MigrationTrialsGroup trials;

    private TM_Region region;

    public TM_Region Region { set {
            region = value;
        }
    }

}
