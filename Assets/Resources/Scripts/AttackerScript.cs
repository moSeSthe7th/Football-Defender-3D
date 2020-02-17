using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerScript : MonoBehaviour
{

    GameObject ball;
    public Transform[] dribblePoints;
    int dribblePointNo;

    Vector3 dribbleTo;
    float ballY;

    bool isTackled;
    Rigidbody rb;

    UIManager uIManager;

    Animator attackerAnimator;

    void Start()
    {
        attackerAnimator = GetComponent<Animator>();
        uIManager = FindObjectOfType(typeof(UIManager)) as UIManager;    
        DataScript.totalAttackerCount++;
        ball = transform.GetChild(0).gameObject;
        dribblePointNo = 0;
        //CloseRagdollPhysics();
        ballY = ball.transform.position.y;
        isTackled = false;
        StartCoroutine(DribbleTheBall());
        rb = GetComponent<Rigidbody>();
        
    }

    IEnumerator DribbleTheBall()
    {
        while (dribblePointNo < dribblePoints.Length && !isTackled && !DataScript.isGameOver)
        {
            DribbleWithBall();
            yield return new WaitForEndOfFrame();
        }

        if (!isTackled)
        {
            uIManager.GameOver();
            attackerAnimator.SetBool("isAttackerWon", true);
        }
           
    }

    void DribbleWithBall()
    {
        dribbleTo = dribblePoints[dribblePointNo].position;
        dribbleTo.y = ballY;

        Quaternion targetRotation = Quaternion.LookRotation(ball.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 20f * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, ball.transform.position, 0.05f);

        ball.transform.Rotate(new Vector3(-20f, 0, 0));
        ball.transform.position = Vector3.MoveTowards(transform.position, dribbleTo, 0.2f);


        isReachedDribblePoint();
    }

    void isReachedDribblePoint()
    {
        if (Vector3.SqrMagnitude(ball.transform.position - dribbleTo) < 0.3f)
        {
            dribblePointNo++;
        }
        
    }

    public void Tackled(Vector3 tacklePos)
    {
        isTackled = true;
        DataScript.tackledAttackerCount++;

        if(DataScript.tackledAttackerCount >= DataScript.totalAttackerCount)
        {
            DataScript.isLevelPassed = true;
            uIManager.LevelPassed();
        }
        OpenRagdollPhysics(tacklePos);
        
    }
    
    void OpenRagdollPhysics(Vector3 tacklePos)
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

            //this is for this game only!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            rigidbody.AddExplosionForce(3000f, tacklePos, 300f, 300f);
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

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
