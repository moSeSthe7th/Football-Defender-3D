using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown 
{
    Coroutine countDown;
    MonoBehaviour owner;

    public bool counted;

    public CountDown(MonoBehaviour owner)
    {
        this.owner = owner;
    }

    public void StartCountDown()
    {
        counted = false;
        countDown = owner.StartCoroutine(startCountDown());
    }

    IEnumerator startCountDown()
    {
        int count = 3;
        while (count > 0)
        {
            
            yield return new WaitForSecondsRealtime(1f);
            count--;
        }

        counted = true;
        
        //yield return new WaitForSecondsRealtime(1f);
        
        owner.StopCoroutine(countDown);
    }
}
