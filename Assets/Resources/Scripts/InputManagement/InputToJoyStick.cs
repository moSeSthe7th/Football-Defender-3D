using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputToJoyStick : InputController
{
    public Transform targetTransform;
    public float sharpness = 15f;
    bool isStaniory { get; set; }
    bool isInput { get; set; }
    Vector2 touchStartPos;

    //float velocity;
    float maxVelocity;

    public InputToJoyStick(Transform startTransform) : base(startTransform.position)
    {
        targetTransform = new GameObject("dummyDefender").transform;
        targetTransform.position = startTransform.position;
        targetTransform.rotation = startTransform.rotation;

        //velocity = charVel;
        maxVelocity = 0.25f;
    }


    protected override void OnTouchStarted(Vector2 touchPos)
    {
        if (canPlayerMove)
        {
            touchPositions.Clear();
            touchStarted = true;
            AddTouchPoisition(touchPos);

            touchStartPos = touchPos;
            isInput = true;
        }
    }

    protected override void OnTouch(Vector2 touchPos)
    {
        base.OnTouch(touchPos);

        if(touchStarted)
        {
            Vector2 touchDelta = (touchPos - touchStartPos);// / (Screen.width / 2f);

            touchStartPos += 0.25f * touchDelta;
            JoyStickMovement(touchDelta);
        }
    }

    protected override void OnTouchEnded(Vector2 touchPos)
    {
        base.OnTouchEnded(touchPos);
        touchStartPos = Vector2.zero;
        isInput = false;
    }

    public bool isSliding()
    {
        return isInput;
    }

    public bool didStoppedToLong()
    {
        return isStaniory && slowCounter > 5;
    }

    int slowCounter = 0;
    void JoyStickMovement(Vector2 posDelta)
    {
        //posDelta = posDelta / (100 * (11 - velocity));
        if(posDelta.magnitude < 0.1f)
        {
            slowCounter++;
            isStaniory = true;
        }

        Vector2 delta = Vector2.ClampMagnitude(posDelta, maxVelocity);// max speed
        Vector3 movementVec = new Vector3(delta.x, 0f, delta.y);

        // Debug.Log("Movement Vector is : " + movementVec + " Position is :" + transform.position);
        //targetTransform.position += movementVec;//  Vector3.Lerp(transform.position, movementVec, speedModifier * Time.deltaTime);
                                          // Debug.Log("New transform position is : " + transform.position); 

        Quaternion targetRotation = Quaternion.LookRotation(movementVec);

        targetTransform.rotation = Quaternion.RotateTowards(targetTransform.rotation, targetRotation, sharpness);
        //transform.rotation = Quaternion.RotateTowards(horizontalRotation, targetRotation, 30f);// * Quaternion.FromToRotation(localAxisToTarget, Vector3.forward) * Quaternion.FromToRotation(localAxisToUp, Vector3.up);

        targetTransform.position += targetTransform.forward.normalized * maxVelocity;

        //Quaternion lookRotation = Quaternion.LookRotation(transform.position);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed);

    }
}
