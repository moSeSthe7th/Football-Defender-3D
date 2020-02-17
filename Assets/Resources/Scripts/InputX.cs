using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtmostInput
{
    public class InputX
    {
        List<GeneralInput> generalInputs;

        public InputX()
        {
            generalInputs = new List<GeneralInput>();
        }

        public bool GetInputs()
        {
            ResetEndedInput();

            if (ExtendedInput.isInputEntered() && generalInputs.Count == 0) // for now works with only one input
            {
                GeneralInput tmpGi = new GeneralInput();

                tmpGi.phase = IPhase.Began;

                tmpGi.startPosition = ExtendedInput.GetPoint();
                tmpGi.currentPosition = tmpGi.startPosition;
                tmpGi.lastFramePosition = tmpGi.startPosition;
                tmpGi.delta = Vector2.zero;

                generalInputs.Add(tmpGi);

                return true;
            }
            else if (ExtendedInput.isInput())
            {
                GeneralInput tmpGi = generalInputs[0];

                tmpGi.lastFramePosition = tmpGi.currentPosition; // Before changing current position update last frame poisition

                tmpGi.currentPosition = ExtendedInput.GetPoint();
                tmpGi.delta = tmpGi.currentPosition - tmpGi.lastFramePosition;

                if (tmpGi.delta.magnitude < 0.2f && tmpGi.delta.magnitude > -0.2f)
                {
                    tmpGi.phase = IPhase.Stationary;
                }
                else
                {
                    tmpGi.phase = IPhase.Moved;
                }

                generalInputs[0] = tmpGi;

                return true;
            }
            else if (generalInputs.Count > 0)// && isinput değilse 
            {
                GeneralInput tmpGi = generalInputs[0];

                tmpGi.phase = IPhase.Ended;
                tmpGi.lastFramePosition = tmpGi.currentPosition; // Before changing current position update last frame poisition
                tmpGi.currentPosition = ExtendedInput.GetPoint();
                tmpGi.delta = tmpGi.currentPosition - tmpGi.lastFramePosition;

                generalInputs[0] = tmpGi;

                return true;
            }

            return false;
        }

        void ResetEndedInput()
        {
            if (generalInputs.Count > 0 && generalInputs[0].phase == IPhase.Ended)
            {
                generalInputs.RemoveAt(0);
            }
        }

        public GeneralInput GetInput(int index)
        {
            return generalInputs[index];
        }
    }


    public enum IPhase
    {
        Began,
        Moved,
        Stationary,
        Ended,
        Canceled
    }

    /// <summary>
    ///   <para>General input container</para>
    /// </summary>
    [System.Serializable]
    public struct GeneralInput
    {
        public IPhase phase;

        public Vector2 lastFramePosition
        {
            get;
            set;
        }

        public Vector2 currentPosition
        {
            get;
            set;
        }

        public Vector2 startPosition
        {
            get;
            set;
        }

        public Vector2 delta
        {
            get;
            set;
        }
    }

    /*   //Gives 1 for maximum left and minus -1 for maximum right
       public Vector2 GetDirection()
       {
           Vector2 direction = Vector2.up;

           float diff = StartingPoint.x - DraggedPoint.x;
           CurrentDirection = diff;

           if (Mathf.Abs(diff) > DirectionZone / 2f)
           {
               direction.x = diff / Mathf.Abs(diff);
           }
           else
           {
               direction.x = diff / (DirectionZone / 2f);
           }

           return direction;
       }*/


    class ExtendedInput
    {

#if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID
        public static bool isInputEntered()
        {
            if (Input.GetMouseButtonDown(0))
                return true;

            return false;
        }

        public static bool isInput()
        {
            if (Input.GetMouseButton(0))
                return true;

            return false;
        }

        public static bool isInputExited()
        {
            if (Input.GetMouseButtonUp(0))
                return true;

            return false;
        }

        public static Vector2 GetPoint()
        {
            return Input.mousePosition;
        }

#elif UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
        public static bool isInputEntered()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                return true;

            return false;
        }

        public static bool isInput()
        {
            if (Input.touchCount > 0)
                return true;

            return false;
        }


        public static bool isInputExited()
        {
            if (Input.touchCount <= 0)
                return true;

            return false;
        }

        public static Vector2 GetPoint()
        {
            return Input.touches[0].position;
        }
#endif
    }
}
