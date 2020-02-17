using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject levelPassedPanel;

    void Start()
    {
        gameOverPanel.SetActive(false);
        levelPassedPanel.SetActive(false);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void LevelPassed()
    {
        levelPassedPanel.SetActive(true);
    }
}
