using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button startButton;
    public Button creditsButton;
    public Button closeCreditsButton;
    public RectTransform creditsPanel;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        creditsButton.onClick.AddListener(Credits);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
    public void Credits()
    {
        creditsPanel.gameObject.SetActive(true);
        closeCreditsButton.onClick.AddListener(CloseCredits);
    }
    public void CloseCredits()
    {
        creditsPanel.gameObject.SetActive(false);
    }
    public void UrlOpen(string url)
    {
        Application.OpenURL(url);
    }
}
