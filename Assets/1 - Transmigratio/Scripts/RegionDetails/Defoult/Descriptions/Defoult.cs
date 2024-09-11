using UnityEngine;
using TMPro;

namespace RegionDetails.Defoult.Descriptions {
    public class Defoult : BaseDescription {
        [SerializeField] TMP_Text _description;

        public string Desctiption { set => _description.text = value; }
    }
}
