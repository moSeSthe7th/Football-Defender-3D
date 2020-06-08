using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelButton : MonoBehaviour
{
    public void GoNextLevel()
    {
        if(DataScript.currentLevel < DataScript.maxLevel)
        {
            DataScript.currentLevel++;
            PlayerPrefs.SetInt("Current Level", DataScript.currentLevel);
        }
        else
        {
            PlayerPrefs.SetInt("Current Level", 1);
        }
      
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
