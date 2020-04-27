using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScrıpt : MonoBehaviour
{
    Camera thisCam;

    enum CameraState
    {
        OnHome,
        OnGame,
        Spanned
    }

    CameraState camState;

    Vector3 StartingPosition; //0, 35, 0
    Quaternion StartingRotation; //90, 0, 0
    float StartingOffset; //60f

    Vector3 GamePlayPosition;
    Quaternion GamePlayRotation;
    float GamePlayFOV;

    public Vector3 offsetToSpanned;
    Vector3 spannedPosition;

    Coroutine camMovement = null;

    [Range(1f,10f)]public float turnSpeed;
    void Start()
    {
        thisCam = GetComponent<Camera>();
        camState = CameraState.OnHome;

        StartingPosition = this.transform.position; // new Vector3(0f,30f,0f);
        StartingRotation = this.transform.rotation; // new Quaternion(90f, 0f, 0f, 1f);
        StartingOffset = thisCam.fieldOfView;

        GamePlayPosition = new Vector3/*(0f, 25f, -15f);*/ (0f, 10f, -15f); //0 10 -15  . when fov 100 
        GamePlayRotation = Quaternion.Euler/*(55f, 0f, 0f);*/ (35f, 0f, 0f); //30 0 0 
        GamePlayFOV = 80f;

        offsetToSpanned = new Vector3(0f, 1f, -3f);

        if(turnSpeed == 0f)
        {
            Debug.LogWarning("Set turn speed to camera. Setting default : 5 ");
            turnSpeed = 5f;
        }

    }
    float spannedTime;
    private void LateUpdate()
    {
        if(camState == CameraState.Spanned)
        {
            Quaternion mouseRotYAxis = Quaternion.AngleAxis(3f * turnSpeed * Time.deltaTime, Vector3.up);
            mouseRotYAxis = Quaternion.Euler(0f, mouseRotYAxis.eulerAngles.y, 0f);

            offsetToSpanned =/* mouseRotXAxis */ mouseRotYAxis * offsetToSpanned;
            transform.position = spannedPosition + offsetToSpanned;

            Quaternion targetRotation = Quaternion.LookRotation(spannedPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.5f);

            spannedTime += Time.deltaTime;

        }
    }

    public void RotateToGamePlayRotation()
    {
        if (camMovement == null)
            camMovement = StartCoroutine(RotateOnStart());
        else
        {
            Debug.LogWarning("camMovement is on use stopping other corountine");
            StopCoroutine(camMovement);
            camMovement = StartCoroutine(RotateOnStart());
        }

    }

    public void SpanTo(Transform character)
    {
        Debug.Log(character.position);

        if (camMovement == null)
            camMovement = StartCoroutine(SpanToPosition(character.position));
        else
        {
            Debug.LogWarning("camMovement is on use stopping other corountine");
            StopCoroutine(camMovement);
            camMovement = StartCoroutine(SpanToPosition(character.position));
        }
    }

    IEnumerator RotateOnStart()
    {
        bool rotated = false;
        bool positioned = false;
        bool fovSetted = false;
        while(!rotated || !positioned || !fovSetted)
        {
            if (Vector3.Distance(transform.position, GamePlayPosition) > 1f) //!Mathf.Approximately(transform.position.sqrMagnitude, GamePlayPosition.sqrMagnitude))
            {
                transform.position = Vector3.Lerp(transform.position, GamePlayPosition, turnSpeed / 100f);
            }
            else
            {
                //transform.position = GamePlayPosition;
                positioned = true;
            }

            if (Vector3.Distance(transform.rotation.eulerAngles, GamePlayRotation.eulerAngles) > 1f)//!Mathf.Approximately(transform.rotation.eulerAngles.sqrMagnitude, GamePlayRotation.eulerAngles.sqrMagnitude))
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, GamePlayRotation, turnSpeed / 100f);
            }
            else
            {
                //transform.rotation = transform.rotation;
                rotated = true;
            }

            if(Mathf.Abs(thisCam.fieldOfView - GamePlayFOV) > 1f)
            {
                thisCam.fieldOfView = Mathf.Lerp(thisCam.fieldOfView, GamePlayFOV, turnSpeed / 100f);
            }
            else
            {
                //thisCam.fieldOfView = GamePlayFOV;
                fovSetted = true;
            }

            yield return new WaitForEndOfFrame();
        }

        camState = CameraState.OnGame;
        StopCoroutine(camMovement);
        camMovement = null;

        yield return null;
    }
     
    IEnumerator SpanToPosition(Vector3 spanPosition)
    {
        spannedPosition = spanPosition;

        bool spanned = false;
        while(!spanned)
        {
            float distanceToSpan = Vector3.Distance(transform.position, spanPosition + offsetToSpanned);
            if (distanceToSpan > 0.5f)
            {
                transform.position = Vector3.Slerp(transform.position, spanPosition + offsetToSpanned, turnSpeed / 150f);

                //if(distanceToSpan < 5f)
                {
                    var targetRotation = Quaternion.LookRotation(spanPosition - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f / distanceToSpan);
                }

            }
            else
            {
                spanned = true;
            }

            yield return new WaitForEndOfFrame();
        }

        offsetToSpanned = transform.position - spanPosition;

        camState = CameraState.Spanned;
        StopCoroutine(camMovement);
        camMovement = null;

        yield return null;
    }

}
