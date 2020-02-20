using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject levelPassedPanel;

    public Button settingsButton;
    public Button trophiesButton;
    public Button shareButton;

    GameObject pitch;
    Text infoText;

    void Start()
    {
        gameOverPanel.SetActive(false);
        levelPassedPanel.SetActive(false);

        pitch = GameObject.FindWithTag("Pitch");
        infoText = pitch.GetComponentInChildren<Text>();
        infoText.text = "";

    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        infoText.text = "GOAL!!";

        settingsButton.gameObject.SetActive(true);
        trophiesButton.gameObject.SetActive(true);
        shareButton.gameObject.SetActive(true);

    }

    public void LevelPassed()
    {
        levelPassedPanel.SetActive(true);
        infoText.text = "WOW!!";

        settingsButton.gameObject.SetActive(true);
        trophiesButton.gameObject.SetActive(true);
        shareButton.gameObject.SetActive(true);
    }

    public void SlideStarted()
    {
        settingsButton.gameObject.SetActive(false);
        trophiesButton.gameObject.SetActive(false);
        shareButton.gameObject.SetActive(false);
    }
}
