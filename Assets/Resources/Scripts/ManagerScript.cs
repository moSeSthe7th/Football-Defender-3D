#define DEBUG_MAGANER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    GameObject primaryMap;
    CrowdController crowdController;
    UIManager uIManager;
    CameraScrıpt cam;

    GameObject startingBall;
    GameObject startingParticleSys;

    bool fasted = false;

    void Start()
    {
        //Application.targetFrameRate = 30;
        //DataScript.inputLock = false;
        DataScript.goalCount = 0;
        DataScript.totalDefenderCount = 0;
        DataScript.slidedDefenderCount = 0;
        DataScript.totalAttackerCount = 0;
        DataScript.tackledAttackerCount = 0;
        DataScript.isLevelPassed = false;
        DataScript.isLevelAnimPlayed = false;

        DataScript.maxLevel = 3;
        DataScript.currentLevel = PlayerPrefs.GetInt("Current Level", 1);
        
        
        primaryMap = Instantiate(Resources.Load<GameObject>("Levels/" + DataScript.currentLevel.ToString()), Vector3.zero, Quaternion.identity);

        uIManager = FindObjectOfType(typeof(UIManager)) as UIManager;
        crowdController = FindObjectOfType(typeof(CrowdController)) as CrowdController;

        startingBall = transform.GetChild(0).gameObject;
        cam = Camera.main.GetComponent<CameraScrıpt>();
        cam.followedObject = primaryMap.GetComponentInChildren<DefenderScript>().transform;

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

    //Finishes the game if following conditions are true
    //Win if all attackers are tackled
    //Lose if all defenders are slided and all attackers are not tackled
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
                LevelFailed();
            }
        }
        else if(DataScript.totalAttackerCount == DataScript.tackledAttackerCount)
        {
            LevelPassed();
        }
        else if (DataScript.goalCount != 0 && DataScript.goalCount == (DataScript.totalAttackerCount - DataScript.tackledAttackerCount))
        {
            LevelFailed();
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

    IEnumerator LevelPassedAnimations()
    {
        GameObject defender = GameObject.FindWithTag("Defender");
        /*GameObject hex = Resources.Load<GameObject>("Prefabs/altigen");
        Vector3 hexPos = defender.transform.position;
        hexPos.y = 0f;
        Quaternion hexRotation = Quaternion.identity;
        hexRotation.eulerAngles = new Vector3(90f, 0, 0);

        Instantiate(hex, hexPos, hexRotation);

        for(int i = 0; i < 10; i++)
        {
            hexPos.y += 0.1f;
            hex.transform.position = hexPos;
        }*/
        yield return new WaitForSecondsRealtime(1f);
        
        Debug.Log("Level Passed Animations started");
        GameObject[] flowCubes = GameObject.FindGameObjectsWithTag("FlowCube");
        foreach(GameObject flowCube in flowCubes)
        {
            flowCube.transform.localScale -= Vector3.one * 0.02f;

            Vector3 targetPos = defender.transform.position;
            FlowToDirection cubeFlow = flowCube.GetComponent<FlowToDirection>();
            cubeFlow.flowPoint = targetPos;
            StartCoroutine(cubeFlow.FlowToDefender(defender));
            yield return new WaitForEndOfFrame();
        }

    }

    void LevelPassed()
    {
        Debug.Log("Level Passed");

        NormalizeSpeed();

        Transform defender = primaryMap.GetComponentInChildren<DefenderScript>().transform;
        cam.SpanTo(defender);

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

        StartCoroutine(LevelPassedAnimations());
        uIManager.LevelPassed();
    }

    void LevelFailed()
    {
        Debug.Log("Level Failed");

        NormalizeSpeed();

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
        //yield return new WaitForSeconds(0.5f);
        

        BallScript[] balls = primaryMap.GetComponentsInChildren<BallScript>();
        startingParticleSys = Resources.Load<GameObject>("Prefabs/StartingParticleSys");

        yield return new WaitForSeconds(0.2f);
        StartingAnimation(balls);
        Instantiate(startingParticleSys, startingBall.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(CountDown());
        //wait a little to see animation than rotate
        cam.RotateToGamePlayRotation();
          
        yield return new WaitUntil(() => counted == true); // wait until count down to end

        DataScript.ChangeState(DataScript.GameState.onGame);

        yield return null;
    }

    bool counted = false;
    IEnumerator CountDown()
    {
        SpriteToBoards countDownSprites = new SpriteToBoards("LifterSprites/CountDown");
        countDownSprites.spriteMaps.Reverse();

        int count = 0;
        while (count < 3)
        {
            crowdController.StartLiftingArea(count, countDownSprites.spriteMaps);
            yield return new WaitForSecondsRealtime(1f);
            count++;
        }
        counted = true;
        //yield return new WaitForSeconds(1f);
;
        //SpriteToBoards GoSprites = new SpriteToBoards("LifterSprites/Go");
        //crowdController.StartLifting(GoSprites.spriteMaps);

        StopCoroutine(CountDown());
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
