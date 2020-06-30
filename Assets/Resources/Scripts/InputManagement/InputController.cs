using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InputController : IDisposable
{
    protected Vector3 startingPosition;

    protected List<Vector3> touchPositions;
    protected bool touchStarted;

    protected bool canPlayerMove = false;

    public InputController(Vector3 startPos)
    {
        startingPosition = startPos;
        touchPositions = new List<Vector3>();

        AddEvents(this);
    }

    public void Dispose()
    {
        RemoveEvents(this);
        GC.SuppressFinalize(this);
    }

    protected void AddEvents(InputController ic)
    {
        DataScript.onGameStarted += ic.gameStarted;
        //Set input events. This void will be called when input even occurs
        InputEventListener.inputEvent.onTouchStarted += ic.OnTouchStarted;
        InputEventListener.inputEvent.onTouch += ic.OnTouch;
        InputEventListener.inputEvent.onTouchEndDelta += ic.OnTouchEndedDelta;
    }

    protected void RemoveEvents(InputController ic)
    {
        DataScript.onGameStarted -= ic.gameStarted;
        //Set input events. This void will be called when input even occurs
        InputEventListener.inputEvent.onTouchStarted -= ic.OnTouchStarted;
        InputEventListener.inputEvent.onTouch -= ic.OnTouch;
        InputEventListener.inputEvent.onTouchEndDelta -= ic.OnTouchEndedDelta;
    }

    void gameStarted()
    {
        canPlayerMove = true;
    }
    
    protected virtual void OnTouchStarted(Vector2 touchPos)
    {
        if (canPlayerMove)
        {
            Debug.Log("base");

            touchPositions.Clear();
            touchStarted = true;
            AddTouchPoisition(touchPos);
        }

    }

    protected virtual void OnTouch(Vector2 touchPos)
    {
        if (touchStarted)
        {
            AddTouchPoisition(touchPos);
        }
    }

    protected virtual void OnTouchEndedDelta(Vector2 touchPos,Vector2 touchDelta)
    {
        if (touchStarted)
        {
            AddTouchPoisition(touchPos);
        }
    }


    protected bool AddTouchPoisition(Vector2 touchPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit))
        {
            if (raycastHit.transform.gameObject.tag == "Pitch")
            {
                touchPositions.Add(raycastHit.point);
                return true;
            }

        }

        return false;
    }
}
