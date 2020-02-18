using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtmostInput;

public class DefenderScript : MonoBehaviour
{
    InputX inputX;

    Vector2 touchStartPos;
    Vector2 touchDelta;

    Animator animator;

   
    List<Vector3> defenderSlidePositions;
    UIManager uIManager;

    void Start()
    {
        uIManager = FindObjectOfType(typeof(UIManager)) as UIManager;
        defenderSlidePositions = new List<Vector3>();
        //playerInputPositions = new List<Vector2>();
        touchStartPos = new Vector2();
        touchDelta = new Vector2();
        animator = GetComponentInChildren<Animator>();
        //CloseRagdollPhysics();
        inputX = new InputX();
    }

  
    void Update()
    {
        if (inputX.GetInputs() && !DataScript.inputLock)
        {
            
            GeneralInput gInput = inputX.GetInput(0);

            if(gInput.phase == IPhase.Began)
            {
                touchStartPos = gInput.currentPosition;
            }
            else if(gInput.phase == IPhase.Ended)
            {
                //touchDelta = gInput.currentPosition - touchStartPos;
                //playerInputPositions.Add(touchDelta);
               
                StartCoroutine(SlidingTackle());
                DataScript.inputLock = true;
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(gInput.currentPosition);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit))
                {
                    if (raycastHit.transform.gameObject.tag == "Pitch")
                    {
                        defenderSlidePositions.Add(raycastHit.point);
                    }

                }

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Attacker")
        {
            other.GetComponent<AttackerScript>().Tackled(transform.position);
        }
      
    }

    IEnumerator SlidingTackle()
    {
       
        animator.SetBool("isTackling", true);

        float slideSpeed = 0.15f; //Vector3.SqrMagnitude(slideTo) / 500f;

        for (int i = 0;i < defenderSlidePositions.Count-1; i++)
        {
            while (Vector3.SqrMagnitude(defenderSlidePositions[i] - transform.position) > 0.2f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(defenderSlidePositions[i] - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 20f * Time.deltaTime);

                transform.position = Vector3.MoveTowards(transform.position, defenderSlidePositions[i], slideSpeed);
                yield return new WaitForEndOfFrame();
            }
        }

        animator.SetBool("isTackling", false);
        animator.SetBool("isTackleEnded", true);

        yield return new WaitForSecondsRealtime(0.5f);
        if (!DataScript.isLevelPassed)
        {
            DataScript.isGameOver = true;
            uIManager.GameOver();
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
