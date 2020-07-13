using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingManager : MonoBehaviour
{
    private Text gameStyle;
    private Text cameraStyle;

    private void Start()
    {
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            if (button.gameObject.name.ToLower().Contains("slide"))
            {
                gameStyle = button.gameObject.GetComponentInChildren<Text>();
                if (DataScript.isSliding)
                {
                    gameStyle.text = "ON";
                }
                else
                {
                    gameStyle.text = "OFF";
                }
                
            }
            else if(button.gameObject.name.ToLower().Contains("camera"))
            {
                cameraStyle = button.gameObject.GetComponentInChildren<Text>();
            }
        }
    }

    public void ChangeCameraType()
    {
        
    }

    public void ChangeGamePlayStyle()
    {
        if (DataScript.isSliding == false)
        {
            DataScript.isSliding = true;
            gameStyle.text = "ON";
        }
        else
        {
            DataScript.isSliding = false;
            gameStyle.text = "OFF";
        }
    }


    public void sellCubes()
    {
        PlayerPrefs.SetInt("Score", 0);
        DataScript.score = PlayerPrefs.GetInt("Score");
    }
}
