using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;
public class OnGameUI : MonoBehaviour
{
    public Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Text score in GetComponentsInChildren<Text>())
        {
            if (score.name.ToLower().Contains("score"))
            {
                scoreText = score;
                break;
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SetScore();
    }
    
    public void SetScore()
    {
        scoreText.text = DataScript.score.ToString();
    }

}
