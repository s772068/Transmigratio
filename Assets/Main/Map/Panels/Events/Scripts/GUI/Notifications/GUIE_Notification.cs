using UnityEngine.UI;
using UnityEngine;

public class GUIE_Notification : MonoBehaviour {
    [SerializeField] private Text _notification;
    public string Notification { set => _notification.text = value; }
}
