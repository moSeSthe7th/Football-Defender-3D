using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject gamePanel;
    public GameObject homePanel;
    public GameObject settingPanel;
    public GameObject onGamePanel;
    public Image gameButton;
    public Image homebutton;

    [SerializeField] private Image victory;
    [SerializeField] private Image fail;
    
    Sprite startSprite;
    Sprite nextSprite;
    Sprite restartSprite;
    Sprite homeSprite;
    Sprite levelsSprite;

    GameObject pitch;
    Text infoText;

    [HideInInspector] public bool pressedPlay;
    [HideInInspector] public bool counted;

    void Start()
    {
        startSprite = Resources.Load<Sprite>("UITextures/Play");
        nextSprite = Resources.Load<Sprite>("UITextures/ForwardRight");
        restartSprite = Resources.Load<Sprite>("UITextures/Retry");
        homeSprite = Resources.Load<Sprite>("UITextures/Home");
        levelsSprite = Resources.Load<Sprite>("UITextures/Levels");

        pressedPlay = false;
        counted = false;

        pitch = GameObject.FindWithTag("Pitch");
        infoText = pitch.GetComponentInChildren<Text>();
        infoText.text = "";

        fail.enabled = false;
        victory.enabled = false;
        
        if (DataScript.GetState() == DataScript.GameState.HomePage)
        {
            gameButton.sprite = startSprite;
            homebutton.sprite = levelsSprite;
            gamePanel.SetActive(true);
            homePanel.SetActive(true);
            onGamePanel.SetActive(false);
            settingPanel.SetActive(false);
        }
        else
        {
            pressedPlay = true;
            StartCoroutine(CountDown());
            gamePanel.SetActive(false);
            homePanel.SetActive(false);
            onGamePanel.SetActive(true);
            settingPanel.SetActive(false);
        }

    }

    public void OnPressedPlay()
    {
        if (DataScript.GetState() == DataScript.GameState.HomePage)
        {
            pressedPlay = true;
            StartCoroutine(CountDown());
            onGamePanel.SetActive(true);
            gamePanel.SetActive(false);
            homePanel.SetActive(false);
        }
        else //if(DataScript.GetState() == DataScript.GameState.PassedLevel)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        //else if(DataScript.GetState() == DataScript.GameState.GameOver)
    }

    public void OnPressedHome()
    {
        if (DataScript.GetState() == DataScript.GameState.HomePage)
        {
            //open levels etc not decided yet. Currently changes level
            if (DataScript.currentLevel < DataScript.maxLevel)
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
        else //if(DataScript.GetState() == DataScript.GameState.PassedLevel)
        {
            GoToHomePage();
        }
    }

    public void GoToHomePage()
    {
        pressedPlay = false;
        counted = false;
        DataScript.ChangeState(DataScript.GameState.HomePage);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void OnPressedSetting()
    {
        gamePanel.SetActive(false);
        homePanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    public void GameOver()
    {
        gameButton.sprite = restartSprite;
        homebutton.sprite = homeSprite;
        infoText.text = "GOAL!";
        //onGamePanel.SetActive(false);
        fail.enabled = true;
        gamePanel.SetActive(true);
    }
    
    public void LevelPassed()
    {
        gameButton.sprite = nextSprite;
        homebutton.sprite = homeSprite;
        infoText.text = "WOW!";
        //onGamePanel.SetActive(false);
        victory.enabled = true;
        gamePanel.SetActive(true);
    }

    IEnumerator CountDown()
    {
        int count = 3;
        while(count > 0)
        {
            //infoText.text = count.ToString();
            yield return new WaitForSecondsRealtime(1f);
            count--;
        }
        counted = true;
        infoText.text = "TACKLE EM";
        yield return new WaitForSecondsRealtime(1f);
        infoText.text = "";

        StopCoroutine(CountDown());
    }

}
