using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializationScript : MonoBehaviour
{
   
    void Awake()
    {
        DataScript.inputLock = false;
        DataScript.totalAttackerCount = 0;
        DataScript.tackledAttackerCount = 0;
        DataScript.isLevelPassed = false;
        DataScript.isGameOver = false;

        DataScript.maxLevel = 2;
        DataScript.currentLevel = PlayerPrefs.GetInt("Current Level", 1);
        
        GameObject primaryMap = Instantiate(Resources.Load<GameObject>("Levels/" + DataScript.currentLevel.ToString()), Vector3.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
