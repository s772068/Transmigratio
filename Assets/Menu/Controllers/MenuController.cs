using UnityEngine.UI;
using UnityEngine;

public class MenuController : BaseController
    {

    public Transform Credits;
    public Transform Welcome;
    public Button openCredits;
    public Button closeCredits;
    public void UrlOpen(string url) {
        Application.OpenURL(url);
    }
    public void OpenCredits()
    {
        Welcome.gameObject.SetActive(false);
        Credits.gameObject.SetActive(true);
        openCredits.gameObject.SetActive(false);
        closeCredits.gameObject.SetActive(true);
    }
    public void CloseCredits() 
    {
        Credits.gameObject.SetActive(false);
        Welcome.gameObject.SetActive(true);
        openCredits.gameObject.SetActive(true);
        closeCredits.gameObject.SetActive(false);
    }
    private void Start() {
        //Localization();
    }
}
