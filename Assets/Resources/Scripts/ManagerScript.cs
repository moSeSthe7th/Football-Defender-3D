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

    private DefenderScript player;
    private List<AttackerScript> attackers;

    private bool slowed = false;
    private bool failed = false;
  
    void Start()
    {
        DataScript.score = PlayerPrefs.GetInt("Score", 0);
        DataScript.animHash = new HashData();
        //Application.targetFrameRate = 30;
        //DataScript.inputLock = false;
        DataScript.goalCount = 0;
        DataScript.totalDefenderCount = 0;
        DataScript.slidedDefenderCount = 0;
        DataScript.totalAttackerCount = 0;
        DataScript.tackledAttackerCount = 0;
        DataScript.isLevelPassed = false;
        DataScript.isLevelAnimPlayed = false;
        DataScript.isOnSlowDown = false;
        DataScript.isOnSpeedUp = false;
        
        GameObject[] allMaps = Resources.LoadAll<GameObject>("Levels/");
        int lastMap = allMaps.Length - 1;

        DataScript.maxLevel = lastMap;
        DataScript.currentLevel = PlayerPrefs.GetInt("Current Level", 1);

        primaryMap = Instantiate(allMaps[DataScript.currentLevel - 1], Vector3.zero, Quaternion.identity);

        uIManager = FindObjectOfType(typeof(UIManager)) as UIManager;
        crowdController = FindObjectOfType(typeof(CrowdController)) as CrowdController;

        player = primaryMap.GetComponentInChildren<DefenderScript>();
        attackers = new List<AttackerScript>();
        attackers.AddRange(primaryMap.GetComponentsInChildren<AttackerScript>());

        startingBall = transform.GetChild(0).gameObject;
        cam = Camera.main.GetComponent<CameraScrıpt>();
        cam.followedObject =player.transform;

        StartCoroutine(GameStartCorountine());
    }


    //Check if level passed. If passed call uimanager level passed
    private void Update()
    {
       if(DataScript.GetState() == DataScript.GameState.onGame)
       {
            CheckState();
            CheckSpeedState();
       }
    }

    //Finishes the game if following conditions are true
    //Win if all attackers are tackled
    //Lose if all defenders are slided and all attackers are not tackled
    void CheckState()
    {
        if (goalCount <= 0 && DataScript.tackledAttackerCount >= DataScript.totalAttackerCount /*&& DataScript.slidedDefenderCount >= DataScript.totalDefenderCount*/ && !DataScript.isLevelPassed)
        {
            Debug.Log($"DataScript.tackledAttackerCount {DataScript.tackledAttackerCount} DataScript.totalAttackerCount {DataScript.totalAttackerCount}");
            LevelPassed();
        }
        else if (goalCount > 0)//(DataScript.slidedDefenderCount >= DataScript.totalDefenderCount) //GAME OVER! all defenders slided but not all attackers tackled
        {
            if (!failed && DataScript.goalCount <= (DataScript.totalAttackerCount - DataScript.tackledAttackerCount))
            {
                failed = true;
                SpeedUpGame();
            }
            else if (DataScript.goalCount == (DataScript.totalAttackerCount - DataScript.tackledAttackerCount))
            {
                LevelFailed();
            }
            else if (DataScript.tackledAttackerCount >= DataScript.totalAttackerCount)
            {
                //burada aslinda kazanabilir. gol yemis ama hepsine celmeyi takmis oluyor
                LevelPassed();
            }
        }
    }


    void CheckSpeedState()
    {
        if (DataScript.isOnSlowDown && slowed == false)
        {
            SlowDownAll();
            slowed = true;
            DataScript.isOnSlowDown = false;
        }
        else if (DataScript.isOnSpeedUp && slowed)
        {
            SpeedUpAll();
            slowed = false;
            DataScript.isOnSpeedUp = false;
        }
        else if(slowed && failed)
        {
            SpeedUpAll();
            player.Lose();
            slowed = false;
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

    public void SlowDownAll()
    {
        float slowDownRatio = 10f;
        
        //slow down attackers
        foreach (var attacker in attackers)
        {
            attacker.attackerSpeed /= slowDownRatio;
            attacker.attackerAnimator.speed /= slowDownRatio;
            attacker.myBall.rb.drag += slowDownRatio;
        }
        
        //slow down defender
        player.defenderSpeed /= slowDownRatio;
        player.animator.speed /= slowDownRatio;
        
        //set camera
        cam.LookAtSmooth();
    }

    public void SpeedUpAll()
    {
        float speedUpRatio = 10f;
        
        //speed up attackers
        foreach (var attacker in attackers)
        {
            attacker.attackerSpeed *= speedUpRatio;
            attacker.attackerAnimator.speed *= speedUpRatio;
            attacker.myBall.rb.drag -= speedUpRatio;
        }
        
        //speed up defender
        player.defenderSpeed *= speedUpRatio;
        player.animator.speed *= speedUpRatio;
        
        //set camera
        cam.ReturnSmooth();
    }
    
    IEnumerator LevelPassedAnimations()
    {
       // GameObject defender = GameObject.FindWithTag("Defender");
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
            
            Vector3 targetPos = player.transform.position;
            FlowToDirection cubeFlow = flowCube.GetComponent<FlowToDirection>();
            cubeFlow.flowPoint = targetPos;
            StartCoroutine(cubeFlow.FlowToDefender(player.gameObject));
            //yield return new WaitForEndOfFrame();
        }


    }

    void LevelPassed()
    {
        Debug.Log("Level Passed");

        NormalizeSpeed();
        
        //DefenderScript defender = primaryMap.GetComponentInChildren<DefenderScript>();
        //cam.SpanTo(player.transform);
        player.Win();
        
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

        //DefenderScript defender = primaryMap.GetComponentInChildren<DefenderScript>();
        //cam.SpanTo(player.transform);
        player.Lose();

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
        //Instantiate(startingParticleSys, startingBall.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(CountDown());
        //wait a little to see animation than rotate
        cam.RotateToGamePlayRotation();
          
        cam.SpanTo(player.transform,10f,true);
        
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
