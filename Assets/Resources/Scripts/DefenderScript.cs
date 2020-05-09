#define JOYSTICK

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtmostInput;
public class DefenderScript : MonoBehaviour
{
    Animator animator;
    Rigidbody defenderRb;

#if JOYSTICK
        InputToJoyStick inputController;
#else
        InputToBezierRoute inputController;
#endif
    List<Vector3> defenderSlidePositions;
    [Range(1f,10f)]public float defenderSpeed;
    bool slided;

    VibrationHandler vibration;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        defenderRb = GetComponent<Rigidbody>();
        vibration = new VibrationHandler();

        DataScript.totalDefenderCount++;

        defenderSpeed = 10f;
        slided = false;

#if JOYSTICK
        inputController = new InputToJoyStick(transform);
#else
        inputController = new InputToBezierRoute(transform.position);
        defenderSlidePositions = new List<Vector3>();
#endif
    }

    void Update()
    {
#if JOYSTICK
        if (inputController.isSliding() && !slided)
        {
            StartCoroutine(SlidingTackle());
            slided = true;
        }

#else
        if (inputController.routeComplete && !slided)
        {
            defenderSlidePositions = inputController.bezierPoints;
            StartCoroutine(SlidingTackle());

            inputController.routeComplete = false;
            slided = true;
        }
#endif

    }

    private void OnDestroy()
    {
        inputController.Dispose();
    }


    IEnumerator SlidingTackle()
    {
        animator.SetBool("isTackling", true);

#if JOYSTICK

        float slideSpeed = defenderSpeed;
        bool slideEnded = false;
        bool onSlowDown = false;

        while(!slideEnded )
        {
            if(!inputController.isSliding() || DataScript.GetState() != DataScript.GameState.onGame) // The game is over start slow down, than the tackeling will be end
            {
                onSlowDown = true;
            }

            if (onSlowDown)
            {
                slideSpeed = Mathf.Lerp(slideSpeed,0f, 0.2f);
                
                if(slideSpeed <= 0.1f)
                {
                    slideSpeed = 0f;
                    slideEnded = true; // slow down ended to break while loop
                }
            }

            if(inputController.didStoppedToLong())
            {
                slideEnded = true; // break while loop since player did not played a while
            }

            transform.position = Vector3.Lerp(transform.position, inputController.targetTransform.position, slideSpeed / 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, inputController.targetTransform.rotation, slideSpeed / 10f); //inputController.targetTransform.rotation;


            yield return new WaitForEndOfFrame();
        }
#else
        float slideSpeed = defenderSpeed; //Vector3.SqrMagnitude(slideTo) / 500f;

        for (int i = 0; i < defenderSlidePositions.Count - 1; i++)
        {
            Vector3 slidePosition = new Vector3(defenderSlidePositions[i].x, transform.position.y, defenderSlidePositions[i].z);

            while (Vector3.SqrMagnitude(slidePosition - transform.position) > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(slidePosition - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, slideSpeed * Time.deltaTime);

                Vector3 newPosition = Vector3.MoveTowards(defenderRb.position, slidePosition, slideSpeed * Time.deltaTime);

                //defenderRb.MovePosition(newPosition);
                transform.position = Vector3.MoveTowards(transform.position, slidePosition, slideSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
#endif


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


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Attacker")
        {
            other.GetComponent<AttackerScript>().Tackled(transform.position);
            DataScript.tackledAttackerCount++;

            vibration.vibrate(2);
        }
      
    }

    
    
    void OpenColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.isTrigger = false;
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
