#define JOYSTICK

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderScript : MonoBehaviour
{
    enum State
    {
        Idle,
        Running,
        Slide,
        Win,
        Lost
    }
    
    public Animator animator;
    Rigidbody defenderRb;

    private State myState;
    
#if JOYSTICK
        InputToJoyStick inputController;
        private InputToBezierRoute inputBezierController;
#else
        InputToBezierRoute inputController;
#endif
    List<Vector3> defenderSlidePositions;
    public float defenderSpeed;
    
    VibrationHandler vibration;
    private Coroutine defenderCorountine;

    private float slideTime = 0f;

    private Camera mainCam;

    private bool cuttedWhileSliding;
    
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        defenderRb = GetComponent<Rigidbody>();
        vibration = new VibrationHandler();

        DataScript.totalDefenderCount++;
        
        myState = State.Idle;

#if JOYSTICK
        inputController = new InputToJoyStick(transform);
        inputBezierController = new InputToBezierRoute(transform);
#else
        inputController = new InputToBezierRoute(transform.position);
        //defenderSlidePositions = new List<Vector3>();
#endif
        defenderSlidePositions = new List<Vector3>();
        defenderSpeed = 15f;
        
        mainCam = Camera.main;
    }

    void Update()
    {
#if JOYSTICK

        if (DataScript.isSliding && DataScript.GetState() == DataScript.GameState.onGame)
        {
            if (inputController.inputStarted && defenderCorountine == null && myState == State.Idle)
            {
                myState = State.Slide;
                defenderCorountine = StartCoroutine( SlidingTackle());
            }
        }
        else if(DataScript.GetState() == DataScript.GameState.onGame)
        {
            if (inputController.inputStarted)
            {
                if(defenderCorountine == null && myState == State.Idle)
                {
                    myState = State.Running;

                    defenderCorountine = StartCoroutine( RunJoyStick());
                }
                else if(defenderCorountine != null && myState == State.Slide && slideTime > 0.3f)
                {
                    myState = State.Idle;
                    cuttedWhileSliding = true;
                    //StopCoroutine(defenderCorountine);
                    //defenderCorountine = null;
                    //Debug.LogError(("start run"));
                    //defenderCorountine = StartCoroutine( RunJoyStick());
                }
            }
            else if (cuttedWhileSliding && defenderCorountine == null && myState == State.Idle)
            {
                myState = State.Running;
                defenderCorountine = StartCoroutine( RunJoyStick());
            }

            /*  else if(running && defenderCorountine == null)
              {
                  Debug.LogError(("start run"));
                  defenderCorountine = StartCoroutine( RunJoyStick());
              }
      */
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


    IEnumerator RunJoyStick()
    {
        Debug.LogError("basladim kosmaya kardas");

        
        ResetAnimator();
        animator.SetBool(DataScript.animHash.defnderHash.run, true);
        
        bool inputEnded = false;

        while(!inputEnded)
        {
            if(!inputController.isPresssing() || DataScript.GetState() != DataScript.GameState.onGame) // The game is over start slow down, than the tackeling will be end
            {
                inputEnded = true;
            }

            Vector3 targetDirection = inputController.moveVector;//mainCam.transform.TransformDirection(inputController.moveVector);
            //targetDirection.z = inputController.moveVector.z;// * ((transform.forward.z >= -0.1f) ? 1f : -1f);
           
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, defenderSpeed);

            transform.position += transform.forward.normalized * 0.1f;
            
            yield return new WaitForEndOfFrame();
        }

        //TimeEngine.SpeedDown(0.5f);
        
        Vector2 delta = inputController.LastDelta();
        Vector3 slideRotation = new Vector3(delta.normalized.x, 0f, delta.normalized.y);
        slideRotation = mainCam.transform.TransformDirection(slideRotation);
        slideRotation.y = 0f;
        
        Quaternion targetRotSlide = Quaternion.LookRotation(transform.forward + slideRotation);
        
        float maxDistance = 7f;
        float maxVelocity = 3f;
        
        //transform.rotation = targetRotSlide;
        Vector3 targetPos = transform.position + (transform.forward * maxDistance);
        
        //animator.SetBool(DataScript.animHash.defnderHash.run, false);
        //animator.SetBool(DataScript.animHash.defnderHash.tackle, true);
        
        yield return new WaitForEndOfFrame();

        if(myState == State.Running)
            myState = State.Slide; //enter sliding mode
        
        
       /* while (myState == State.Slide)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, maxVelocity * Time.deltaTime);

            if (Mathf.Abs(Vector3.Distance(transform.position, targetPos)) < 0.5f)
                myState = State.Idle;
            
            slideTime += Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }

        //TimeEngine.SpeedUp(0.5f);
        
        Debug.LogError("bitirdim kosmayi kardas");

        
        slideTime = 0f;
        
        animator.SetBool(DataScript.animHash.defnderHash.tackle, false);
        //animator.SetBool(DataScript.animHash.defnderHash.tackleEnd, true);
        */
        StopCoroutine(defenderCorountine);
        defenderCorountine = null;

        
        if(DataScript.GetState() == DataScript.GameState.onGame)
            StartSlidingBezier();
    }

    void StartSlidingBezier()
    {
        if (defenderCorountine == null)
            defenderCorountine = StartCoroutine(SlidingBezier());
    }

    IEnumerator SlidingBezier()
    {
        DataScript.isOnSlowDown = true;//slow down all characters on manager update

        inputBezierController.startBezier = true;
        
        animator.SetBool(DataScript.animHash.defnderHash.run, false);
        animator.SetBool(DataScript.animHash.defnderHash.tackle, true);

        
        while (inputBezierController.routeComplete == false)
        {
            //inputBezierController.routeComplete = true;
            yield return new WaitForEndOfFrame();
        }

        DataScript.isOnSpeedUp = true;//speed up all characters on manager update

  /*      if (!inputBezierController.routeComplete)
        {
            ResetAnimator();
            StopCoroutine(defenderCorountine);
            defenderCorountine = null;
        }
        */
        defenderSlidePositions = inputBezierController.bezierPoints;

        inputBezierController.startBezier = false;

        for (int i = 0; i < defenderSlidePositions.Count - 1; i++)
        {
            Vector3 slidePosition = new Vector3(defenderSlidePositions[i].x, transform.position.y, defenderSlidePositions[i].z);

            while (Vector3.SqrMagnitude(slidePosition - transform.position) > 0.01f && myState == State.Slide)
            {
                Quaternion targetRotation = Quaternion.LookRotation(slidePosition - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, defenderSpeed * Time.deltaTime);

                Vector3 newPosition = Vector3.MoveTowards(defenderRb.position, slidePosition, defenderSpeed * Time.deltaTime);

                //defenderRb.MovePosition(newPosition);
                transform.position = Vector3.MoveTowards(transform.position, slidePosition, defenderSpeed * Time.deltaTime);
                
                slideTime += Time.deltaTime;
                
                yield return new WaitForEndOfFrame();
            }
            
            if (myState == State.Idle)
                break;
        }

        inputBezierController.Reset();

        if (myState != State.Idle)
            myState = State.Idle;
        
        animator.SetBool(DataScript.animHash.defnderHash.tackle, false);
        animator.SetBool(DataScript.animHash.defnderHash.run, false);
        
        slideTime = 0f;
        
        StopCoroutine(defenderCorountine);
        defenderCorountine = null;
    }


    IEnumerator SlidingTackle()
    {
        animator.SetBool(DataScript.animHash.defnderHash.tackle, true);

#if JOYSTICK

        float slideSpeed = defenderSpeed / 5f;
        bool slideEnded = false;
        bool onSlowDown = false;

        Debug.Log("slided");
        
        while(!slideEnded)
        {
            if(!inputController.isPresssing() || DataScript.GetState() != DataScript.GameState.onGame) // The game is over start slow down, than the tackeling will be end
            {
                onSlowDown = true;
            }

            if (onSlowDown)
            {
                slideSpeed = Mathf.Lerp(slideSpeed,0f, 0.5f);
                
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
            
            transform.position = Vector3.Lerp(transform.position, inputController.targetTransform.position, slideSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, inputController.targetTransform.rotation, slideSpeed * Time.deltaTime); //inputController.targetTransform.rotation;

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


        animator.SetBool(DataScript.animHash.defnderHash.tackle, false);
        //animator.SetBool(DataScript.animHash.defnderHash.tackleEnd, true);

        DataScript.slidedDefenderCount++;
        myState = State.Idle;
        
        //yield return new WaitForEndOfFrame();//WaitForSeconds(0.1f);
        
        StopCoroutine(defenderCorountine);
        defenderCorountine = null;
    }


    public void Win()
    {
        ResetPlayer();
        animator.SetBool(DataScript.animHash.defnderHash.win, true);

        
       /* if (animator.GetBool(DataScript.animHash.defnderHash.tackle))
        {
            StartCoroutine(WaitSlideEnd(DataScript.animHash.defnderHash.win));
        }
        else
        {
            ResetAnimator();
            animator.SetBool(DataScript.animHash.defnderHash.win, true);
        }*/
    }
    public void Lose()
    {
        ResetPlayer();
        animator.SetBool(DataScript.animHash.defnderHash.lost, true);
/*
        if (animator.GetBool(DataScript.animHash.defnderHash.tackle))
        {
            StartCoroutine(WaitSlideEnd(DataScript.animHash.defnderHash.lost));
        }
        else
        {
            ResetAnimator();
            animator.SetBool(DataScript.animHash.defnderHash.lost, true);
        }*/
    }
    IEnumerator WaitSlideEnd(int animHash)
    {
        while (animator.GetBool(DataScript.animHash.defnderHash.tackle))
        {
            yield return new WaitForEndOfFrame();
        }

        ResetPlayer();
        animator.SetBool(animHash, true);
    }

    private void ResetPlayer()
    {
        inputBezierController.Reset();

        if (myState != State.Idle)
            myState = State.Idle;
        
        StopAllCoroutines();
        ResetAnimator();

    }

    private void ResetAnimator()
    {
        animator.SetBool(DataScript.animHash.defnderHash.tackle, false);
        animator.SetBool(DataScript.animHash.defnderHash.tackleEnd, false);
        animator.SetBool(DataScript.animHash.defnderHash.run, false);
        animator.SetBool(DataScript.animHash.defnderHash.lost, false);
        animator.SetBool(DataScript.animHash.defnderHash.win, false);
        animator.SetBool(DataScript.animHash.defnderHash.fall, false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Attacker") && myState == State.Slide)
        {
            AttackerScript attacker = other.GetComponent<AttackerScript>();
            if (attacker.isTackled == false)
            {
                attacker.Tackled(transform.position);
                DataScript.tackledAttackerCount++;
                vibration.vibrate(2);
            }

        }
        else if(other.gameObject.CompareTag("Attacker"))
        {
            animator.SetBool(DataScript.animHash.defnderHash.fall, true);
        }
    }
}
