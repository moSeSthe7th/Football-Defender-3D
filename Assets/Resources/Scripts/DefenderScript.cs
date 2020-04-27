using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtmostInput;

public class DefenderScript : MonoBehaviour
{
    Animator animator;
    InputToBezierRoute bezierCalculator;
    List<Vector3> defenderSlidePositions;

    VibrationHandler vibration;

    void Start()
    {
        defenderSlidePositions = new List<Vector3>();
        animator = GetComponentInChildren<Animator>();
        vibration = new VibrationHandler();

        DataScript.totalDefenderCount++;

        bezierCalculator = new InputToBezierRoute(transform.position);
    }

  
    void Update()
    {
        if(bezierCalculator.routeComplete)
        {
            defenderSlidePositions = bezierCalculator.bezierPoints;
            StartCoroutine(SlidingTackle());

            bezierCalculator.routeComplete = false;
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Attacker")
        {
            other.GetComponent<AttackerScript>().Tackled(transform.position);
            DataScript.tackledAttackerCount++;

            vibration.vibrate(2);
        }
      
    }

    private void OnDestroy()
    {
        bezierCalculator.Dispose();
    }

    IEnumerator SlidingTackle()
    {
        animator.SetBool("isTackling", true);

        float slideSpeed = 15f; //Vector3.SqrMagnitude(slideTo) / 500f;

        for (int i = 0;i < defenderSlidePositions.Count-1; i++)
        {
            while (Vector3.SqrMagnitude(defenderSlidePositions[i] - transform.position) > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(defenderSlidePositions[i] - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, slideSpeed * Time.deltaTime);

                transform.position = Vector3.MoveTowards(transform.position, defenderSlidePositions[i], slideSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

        animator.SetBool("isTackling", false);
        animator.SetBool("isTackleEnded", true);

        DataScript.slidedDefenderCount++;

        yield return new WaitForEndOfFrame();//WaitForSeconds(0.1f);

        if (!DataScript.isLevelPassed)
        {
            animator.SetBool("isLost", true);
        }
        else
        {
            animator.SetBool("isWon", true);
        }
            
    }

    void OpenRagdollPhysics()
    {
        transform.parent = null;

        Collider[] colliders = GetComponentsInChildren<Collider>();
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Collider collider in colliders)
        {
            collider.isTrigger = false;
        }

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.useGravity = true;
            //rigidbody.isKinematic = false;
        }
        GetComponent<Animator>().enabled = false;
    }

    void CloseRagdollPhysics()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != this.gameObject)
            {
                collider.isTrigger = true;
            }
        }

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            if (rigidbody.gameObject != this.gameObject)
            {
                rigidbody.useGravity = false;
                //rigidbody.isKinematic = true;
            }
        }
    }
}
