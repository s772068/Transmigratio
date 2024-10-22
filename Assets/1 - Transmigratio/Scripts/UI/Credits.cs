using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class Credits : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _contact;

        private const string _discord = "https://discord.gg/fMjb5ZKEyW";
        private const string _mail = "mailto:transmigratio.game@gmail.com";

        public void OnPointerClick(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_contact, Input.mousePosition, null);
            if (linkIndex < 0)
                return;

            var linkId = _contact.textInfo.linkInfo[linkIndex].GetLinkID();

            var url = linkId switch
            {
                "Dicsord" => _discord,
                "Mail" => _mail,
                _ => _discord
            };

            Debug.Log($"URL clicked: linkInfo[{linkIndex}].id={linkId}   ==>   url={url}");

            Application.OpenURL(url);
        }
    }
}
