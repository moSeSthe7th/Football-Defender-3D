#define DEBUG_MAGANER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    GameObject primaryMap;

    UIManager uIManager;
    CameraScrıpt cam;

    GameObject startingBall;

    bool fasted = false;

    void Awake()
    {
        //Application.targetFrameRate = 30;
        //DataScript.inputLock = false;
        DataScript.goalCount = 0;
        DataScript.totalDefenderCount = 0;
        DataScript.slidedDefenderCount = 0;
        DataScript.totalAttackerCount = 0;
        DataScript.tackledAttackerCount = 0;
        DataScript.isLevelPassed = false;

        DataScript.maxLevel = 3;
        DataScript.currentLevel = PlayerPrefs.GetInt("Current Level", 1);
        
        primaryMap = Instantiate(Resources.Load<GameObject>("Levels/" + DataScript.currentLevel.ToString()), Vector3.zero, Quaternion.identity);

        uIManager = FindObjectOfType(typeof(UIManager)) as UIManager;

        startingBall = transform.GetChild(0).gameObject;
        cam = Camera.main.GetComponent<CameraScrıpt>();

        StartCoroutine(GameStartCorountine());
    }


    //Check if level passed. If passed call uimanager level passed
    private void Update()
    {
       if(DataScript.GetState() == DataScript.GameState.onGame)
       {
            CheckState();
       }
    }

    void CheckState()
    {
        if (DataScript.tackledAttackerCount >= DataScript.totalAttackerCount && DataScript.slidedDefenderCount >= DataScript.totalDefenderCount && !DataScript.isLevelPassed)
        {
            LevelPassed();
        }
        else if (DataScript.slidedDefenderCount >= DataScript.totalDefenderCount) //GAME OVER! all defenders slided but not all attackers tackled
        {
            if (!fasted && DataScript.goalCount <= (DataScript.totalAttackerCount - DataScript.tackledAttackerCount))
            {
                fasted = true;
                SpeedUpGame();
            }
            else if (DataScript.goalCount == (DataScript.totalAttackerCount - DataScript.tackledAttackerCount))
            {
                NormalizeSpeed();
                LevelFailed();
            }
        }
    }

    void SpeedUpGame()
    {
        Debug.Log("Speed Up");
        TimeEngine.SpeedUp(1f);
    }

    void NormalizeSpeed()
    {
        TimeEngine.DefaultSpeed();
    }

    void LevelPassed()
    {
        Debug.Log("Level Passed");

        Transform defender = primaryMap.GetComponentInChildren<DefenderScript>().transform;
        cam.SpanTo(defender);
        //defender.gameObject.SetActive(false);

        DataScript.isLevelPassed = true;

        if (DataScript.currentLevel < DataScript.maxLevel)
        {
            DataScript.currentLevel++;
            PlayerPrefs.SetInt("Current Level", DataScript.currentLevel);
        }
        else
        {
            PlayerPrefs.SetInt("Current Level", 1);
        }

        DataScript.ChangeState(DataScript.GameState.PassedLevel);
        uIManager.LevelPassed();
    }

    void LevelFailed()
    {
        Debug.Log("Level Failed");

        Transform attacker = primaryMap.GetComponentInChildren<AttackerScript>().transform;
        cam.SpanTo(attacker);

        DataScript.ChangeState(DataScript.GameState.GameOver);
        uIManager.GameOver();
    }

    IEnumerator GameStartCorountine()
    {
        if(DataScript.GetState() == DataScript.GameState.HomePage)// if on home page wait until pressing start
        {
            yield return new WaitUntil(() => uIManager.pressedPlay == true);
        }
        //wait just a little before returning and beginning animation
        yield return new WaitForSeconds(0.5f);


        BallScript[] balls = primaryMap.GetComponentsInChildren<BallScript>();
        StartingAnimation(balls);

        yield return new WaitForSeconds(0.8f);
        //wait a little to see animation than rotate
        cam.RotateToGamePlayRotation();

        yield return new WaitUntil(() => uIManager.counted == true); // wait until count down to end

        DataScript.ChangeState(DataScript.GameState.onGame);

        yield return null;
    }

    void StartingAnimation(BallScript[] balls)
    {
        foreach(BallScript ball in balls)
        {
            ball.FallToAttacker(startingBall.transform.position);
        }
        //BallExplodeAnimationGoesThere
        startingBall.SetActive(false);
    }



#if DEBUG_MAGANER
    [SerializeField] int goalCount = 0;
    [SerializeField] int totalDefenderCount = 0;
    [SerializeField] int slidedDefenderCount = 0;
    [SerializeField] int totalAttackerCount = 0;
    [SerializeField] int tackledAttackerCount = 0;
    [SerializeField] bool isLevelPassed = false;
    [SerializeField] float timeScale = 0f;
    [SerializeField] DataScript.GameState state = DataScript.GameState.HomePage;

    void DisplayData()
    {
        goalCount = DataScript.goalCount;
        totalDefenderCount = DataScript.totalDefenderCount;
        slidedDefenderCount = DataScript.slidedDefenderCount;
        totalAttackerCount = DataScript.totalAttackerCount;
        tackledAttackerCount = DataScript.tackledAttackerCount;
        isLevelPassed = DataScript.isLevelPassed;
        timeScale = Time.timeScale;
        state = DataScript.GetState();
    }

    private void LateUpdate()
    {
        DisplayData();
    }

#endif
}
