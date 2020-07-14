using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraScrıpt : MonoBehaviour
{
    Camera thisCam;
    public Transform followedObject;

    enum CameraState
    {
        OnHome,
        OnGame,
        OnCorountine,
        Spanned,
        Follow
    }

    CameraState camState;

    Vector3 StartingPosition; //0, 35, 0
    Quaternion StartingRotation; //90, 0, 0
    float StartingOffset; //60f

    Vector3 GamePlayPosition;
    Quaternion GamePlayRotation;
    float GamePlayFOV;
    
    public Vector3 offsetToFollowed;
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

        offsetToFollowed = new Vector3(0f, 1f, -3f);

        if(turnSpeed == 0f)
        {
            Debug.LogWarning("Set turn speed to camera. Setting default : 5 ");
            turnSpeed = 5f;
        }

    }
    float spannedTime;
    private void LateUpdate()
    {
        if(camState == CameraState.OnGame)
        {
            Quaternion targetRot = transform.rotation;
            //Debug.Log(targetRot);
            targetRot.y = /*(transform.rotation * Quaternion.AngleAxis(*/(/* transform.rotation.y - */followedObject.position.x) * Time.deltaTime;//, Vector3.up)).y;
            //Debug.Log(targetRot);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRot, turnSpeed * Time.deltaTime);

            Vector3 targetPos = transform.position;//followedObject.position + offsetToFollowed;
            targetPos.z = followedObject.position.z + offsetToFollowed.z;
            targetPos.z = (targetPos.z < GamePlayPosition.z) ? GamePlayPosition.z : targetPos.z;
            //transform.position = targetPos;
           
        }
        else if (camState == CameraState.Follow)
        {
            Follow(offsetToFollowed);
            //transform.LookAt(followedObject, Vector3.up);
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

    public void SpanTo(Transform character, bool doNotCut = false)
    {
        if (camMovement == null)
            camMovement = StartCoroutine(SpanToPosition(character,turnSpeed));
        else
        {
            if (doNotCut)
            {
                StartCoroutine(WaitUntillNull(camMovement, character,turnSpeed));
            }
            else
            {
                Debug.LogWarning("camMovement is on use stopping other corountine");
                StopCoroutine(camMovement);
                camMovement = StartCoroutine(SpanToPosition(character,turnSpeed));
            }
        }
    }
    
    public void SpanTo(Transform character,float speed, bool doNotCut = false)
    {
        if (camMovement == null)
            camMovement = StartCoroutine(SpanToPosition(character,speed));
        else
        {
            if (doNotCut)
            {
                StartCoroutine(WaitUntillNull(camMovement, character, speed));
            }
            else
            {
                Debug.LogWarning("camMovement is on use stopping other corountine");
                StopCoroutine(camMovement);
                camMovement = StartCoroutine(SpanToPosition(character,speed));
            }
        }
    }


    IEnumerator RotateOnStart()
    {
        camState = CameraState.OnCorountine;

        bool rotated = false;
        bool positioned = false;
        bool fovSetted = false;
        while(!rotated || !positioned || !fovSetted)
        {
            if (Vector3.Distance(transform.position, GamePlayPosition) > 2f) //!Mathf.Approximately(transform.position.sqrMagnitude, GamePlayPosition.sqrMagnitude))
            {
                transform.position = Vector3.Lerp(transform.position, GamePlayPosition, turnSpeed / 200f);
            }
            else
            {
                //transform.position = GamePlayPosition;
                positioned = true;
            }

            if (Vector3.Distance(transform.rotation.eulerAngles, GamePlayRotation.eulerAngles) > 5f)//!Mathf.Approximately(transform.rotation.eulerAngles.sqrMagnitude, GamePlayRotation.eulerAngles.sqrMagnitude))
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, GamePlayRotation, turnSpeed / 200f);
            }
            else
            {
                //transform.rotation = transform.rotation;
                rotated = true;
            }

            if(Mathf.Abs(thisCam.fieldOfView - GamePlayFOV) > 2f)
            {
                thisCam.fieldOfView = Mathf.Lerp(thisCam.fieldOfView, GamePlayFOV, turnSpeed / 200f);
            }
            else
            {
                //thisCam.fieldOfView = GamePlayFOV;
                fovSetted = true;
            }

            yield return new WaitForEndOfFrame();
        }

        offsetToFollowed = transform.position - followedObject.position;

        camState = CameraState.OnGame;
        StopCoroutine(camMovement);
        camMovement = null;

        yield return null;
    }
     
    IEnumerator SpanToPosition(Transform spannedObj,float speed, float downModifier = 0f)
    {
        camState = CameraState.OnCorountine;

        //yield return new WaitForSeconds(1f);

        bool spanned = false;

        Vector3 spanPos = spannedObj.position;
        
        Vector3 offset = new Vector3(0f, 2f, -3f);
        
        while(!spanned)
        {
            
            float distanceToSpan = Vector3.Distance(transform.position, spanPos + (Vector3.down * downModifier) + offset);
            
            if (distanceToSpan > 1f)
            {
                transform.position = Vector3.Lerp(transform.position, spanPos + (Vector3.down * downModifier) + offset, speed / 150f);

                //if(distanceToSpan < 5f)
                {
                    var targetRotation = Quaternion.LookRotation(spanPos - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1f / distanceToSpan);
                }

            }
            else
            {
                spanned = true;
            }

            yield return new WaitForEndOfFrame();
        }

        offsetToFollowed = transform.position - spanPos;
        spannedPosition = spanPos;
        camState = CameraState.Follow;
        StopCoroutine(camMovement);
        camMovement = null;

        yield return null;
    }

    IEnumerator WaitUntillNull(Coroutine coroutine, Transform character,float speed)
    {
        while (camMovement != null)
        {
            yield return new WaitForEndOfFrame();
        }

        SpanTo(character,speed);
    }

    private Quaternion targetRotation;
    private bool rotated = false;
    private float totalAngles = 0f;
    private float remainingAngles = 0f;
    private int rotatedTime = 0;
    
    private float rotationDegreesPerSecond = 90f;
    private float rotationDegreesAmount = 180f;
    private float totalRotation = 0;
    
    Quaternion LookAtTarget(Quaternion rot)
    {
        Quaternion targetRot = Quaternion.AngleAxis(0f,Vector3.up);

        var rotatedBackward = followedObject.transform.forward.z < -0.4f && rotated == false;
        var rotateForward = followedObject.transform.forward.z > 0.1f && rotated;
        
        
        //Debug.Log(followedObject.transform.forward);
        if (rotatedBackward || rotateForward)
        {
            
         if(Mathf.Abs(totalRotation) >= Mathf.Abs(rotationDegreesAmount))
         {             Debug.LogError($" donme bitti {transform.eulerAngles.y } and total rot is {totalRotation} and rotationDegreesAmount { rotationDegreesAmount}");

             rotated = (!rotated);
             totalRotation = 0f;
             return targetRot;
         }
         
         Debug.Log($" donuyorrr {transform.eulerAngles.y } and forward is {followedObject.transform.forward.z}");
           
         float currentAngle2 = transform.rotation.eulerAngles.y;
         targetRot = 
             Quaternion.AngleAxis(( 0.1f * rotationDegreesPerSecond), Vector3.up);
                totalRotation += 0.1f * rotationDegreesPerSecond;
            
        }
        

        var angle = Quaternion.LookRotation(followedObject.forward, Vector3.up);//FixEuler(Mathf.Abs(followedObject.transform.eulerAngles.y - transform.eulerAngles.y));
        //Debug.Log($"angle diff : {FixEuler(angle.eulerAngles.y) - FixEuler(transform.eulerAngles.y)} transform angle {transform.eulerAngles.y} angle angle {angle.eulerAngles.y}" );
        
        Quaternion followDirectly = Quaternion.AngleAxis(FixEuler(FixEuler(angle.eulerAngles.y) - FixEuler(transform.eulerAngles.y)) * Time.deltaTime, Vector3.up);
        
        float currentAngle = transform.eulerAngles.y;
        float desiredAngle = Quaternion.LookRotation(followedObject.forward, Vector3.up).eulerAngles.y;// followedObject.transform.eulerAngles.y; 
       
        currentAngle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * 1.5f );
        

        

        Quaternion rotation = Quaternion.AngleAxis(ReverseEuler(currentAngle),Vector3.up);// Quaternion.Euler(0, FixEuler(currentAngle), 0);

        
        
        return targetRot;


    }
    
    public void Follow(Vector3 offset)
    {
        targetRotation = LookAtTarget(targetRotation);
        
        offsetToFollowed = targetRotation * offsetToFollowed; // offsetToFollowed = targetRotation * offsetToFollowed;
        transform.position = followedObject.position + offsetToFollowed; 
        transform.LookAt(followedObject);
        
        //Debug.LogWarning($"aci su an : {transform.eulerAngles.y}");
    }

    float FixEuler(float angle) // For the angle in angleAxis, to make the error a scalar
    {
        if (angle > 180f)
            return angle - 360f;
        else
            return angle;
    }

    float ReverseEuler(float angle)
    {
        if (angle > 0f)
            return angle;
        else
            return angle + 360f;
    }
    
}
