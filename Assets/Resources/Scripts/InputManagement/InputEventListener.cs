using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UtmostInput;

public class InputEventListener : MonoBehaviour
{
    public static InputEventListener inputEvent;

    InputX inputX;
    GeneralMOInput touch;

    private void Awake()
    {
        if (inputEvent == null)
            inputEvent = this;
        else
            Destroy(this);
    }

    void Start()
    {
        inputX = new InputX();
    }

    void Update()
    {
        if(inputX.SetInputs())
        {
            touch = inputX.GetInput(0);

            if (touch.phase == IPhase.Began)
            {
                TouchStarted(touch.currentPosition);
            }
            else if (touch.phase == IPhase.Moved || touch.phase == IPhase.Stationary)
            {
                TouchProceed(touch.currentPosition);
            }
            else
            {
                 TouchEnd(touch.currentPosition,touch.delta);
            }
        }
    }

    public event Action<Vector2> onTouchStarted;
    public void TouchStarted(Vector2 touchPos)
    {
        if (onTouchStarted != null) //if any other object is using this event
        {
            onTouchStarted(touchPos);
        }
    }

    public event Action<Vector2> onTouch;
    public void TouchProceed(Vector2 touchPos)
    {
        if (onTouch != null) //if any other object is using this event
        {
            onTouch(touchPos);
        }
    }

    public event Action<Vector2, Vector2> onTouchEndDelta;
    public event Action<Vector2> onTouchEnd;
    public void TouchEnd(Vector2 touchPos, Vector2 touchDelta)
    {
        if (onTouchEnd != null) //if any other object is using this event
        {
            onTouchEnd(touchPos);
        }

        if (onTouchEndDelta != null)
        {
            onTouchEndDelta(touchPos, touchDelta);
        }
    }

    

}
