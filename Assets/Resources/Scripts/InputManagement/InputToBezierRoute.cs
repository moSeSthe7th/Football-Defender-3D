using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InputToBezierRoute : InputController
{
    public Transform targetTransform;
    List<Vector3> bezierControlPoints;
    int controlPointCount;
    public bool startBezier = false;

    public List<Vector3> bezierPoints;
    public bool routeComplete = false; //defender will check this routeComplete flag in order to start sliding

    private LineRenderer line;
    
    public InputToBezierRoute(Transform startTransfrom,LineRenderer lineRenderer) : base(startTransfrom.position)
    {
        targetTransform = startTransfrom;
        line = lineRenderer;
        line.positionCount = 0;
        line.SetPositions(new Vector3[]{ });
        line.enabled = false;
        
        controlPointCount = 4;
        bezierControlPoints = new List<Vector3>(controlPointCount);
        bezierPoints = new List<Vector3>();
        touchPositions = new List<Vector3>();
        
        startBezier = false;
        routeComplete = false;
    }

    protected override void OnTouchStarted(Vector2 touchPos)
    {
        if (canPlayerMove && startBezier)
        {
            touchPositions.Clear();
Debug.Log($"started bezier ");
            touchStarted = true;
            AddTouchPoisition(touchPos);
            
            line.enabled = true;
            
            line.positionCount = touchPositions.Count;
            line.SetPositions(touchPositions.ToArray());
            
            bezierControlPoints.Clear();
            startingPosition = targetTransform.position;
        }

    }

    protected override void OnTouch(Vector2 touchPos)
    {
        if (touchStarted)
        {
            base.OnTouch(touchPos);
            line.positionCount = touchPositions.Count;
            line.SetPositions(touchPositions.ToArray());
        }
    }
    
    protected override void OnTouchEndedDelta(Vector2 touchPos, Vector2 touchDelta)
    {

        if(touchStarted)
        {
            //touchPositions.Add(touchPos);
            AddTouchPoisition(touchPos);
            //Caluclate control points positions
            //find the middle point of touch created curve. Than calculate control points according to middle point
            int middlePos = touchPositions.Count / 2;

            //There will be 4 control points
            // one in start pos, one in end position
            //remaining will be in the middle. We will just go %20 of the middle position in both direction in order to calculate both middle positions

            //add start pos
            bezierControlPoints.Add(touchPositions[0]);

            //add middle pos
            int firstMiddleControl = middlePos - (middlePos / 5);
            bezierControlPoints.Add(touchPositions[firstMiddleControl]);

            int secondMiddleControl = middlePos + (middlePos / 5);
            bezierControlPoints.Add(touchPositions[secondMiddleControl]);

            //add end pos
            bezierControlPoints.Add(touchPositions[touchPositions.Count - 1]);

            Debug.Log(" Total touch count is : " + touchPositions.Count + " \n Middle is : " + middlePos + " \n firstMiddleControl : " 
                + firstMiddleControl + " \n secondMiddleControl : " + secondMiddleControl);

            bezierPoints = SetBezierPoints();

            routeComplete = true;
            touchStarted = false;

            line.enabled = false;
            
            //line.SetPositions(new Vector3[]{ });
            
            CreateDebugObject();
        }
    }

    List<Vector3> SetBezierPoints()
    {
        List<Vector3> bPoints = new List<Vector3>();

        Vector3 curveStartPosDiff = startingPosition - bezierControlPoints[0];
        curveStartPosDiff.y = 0f;

        for (float t = 0; t <= 1; t += 0.05f)
        {
            Vector3 initialPos = Mathf.Pow(1 - t, 3) * bezierControlPoints[0] +
                3 * Mathf.Pow(1 - t, 2) * t * bezierControlPoints[1] +
                3 * (1 - t) * Mathf.Pow(t, 2) * bezierControlPoints[2] +
                Mathf.Pow(t, 3) * bezierControlPoints[3];

            initialPos.y = 0f;

            bPoints.Add(initialPos + curveStartPosDiff);
        }

        return bPoints;
    }

    public void Reset()
    {
        bezierControlPoints.Clear();
        controlPointCount = 0;
        startBezier = false;
        bezierPoints.Clear();
        routeComplete = false; 
    }

    void CreateDebugObject()
    {
        GameObject points = new GameObject("Points");

        int count = 0;
        foreach(Vector3 cp in bezierControlPoints)
        {
            GameObject controlPoint = new GameObject(count.ToString());

            controlPoint.transform.position = cp;

            controlPoint.transform.parent = points.transform;

            count++;
        }

        BezierRouteScript brs = points.AddComponent<BezierRouteScript>();
        brs.startingPosition = this.startingPosition;
    }

}
