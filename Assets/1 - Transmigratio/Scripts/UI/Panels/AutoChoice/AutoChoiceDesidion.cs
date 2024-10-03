using TMPro;
using UnityEngine;

public class AutoChoiceDesidion : MonoBehaviour {
    [SerializeField] TMP_Text _numTxt;
    [SerializeField] TMP_Text _title;

    public TMP_Text Num => _numTxt;
    public TMP_Text Title => _title;
}
