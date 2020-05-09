using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerScript : MonoBehaviour
{
    Animator attackerAnimator;

    GameObject ball;
    float ballY;

    Transform[] dribblePoints;
    int dribblePointNo;
    Vector3 dribbleTo;

    bool isTackled;

    Transform net;

    public float attackerSpeed = 3f;
    void Start()
    {

        attackerAnimator = GetComponent<Animator>();
        DataScript.totalAttackerCount++;
        ball = transform.GetChild(0).gameObject;
        dribblePointNo = 0;
        
        ballY = ball.transform.position.y;
        isTackled = false;
        dribblePoints = transform.GetChild(1).GetComponent<DribblePoints>().GetDribblePoints();
        if (!net)
            net = GameObject.Find("Net_01").transform;

        OpenColliders();
        DataScript.onGameStarted += StartRunning;
    }

    void StartRunning()
    {
        StartCoroutine(DribbleTheBall());
    }

    void OnDestroy()
    {
        DataScript.onGameStarted -= StartRunning;
    }

    IEnumerator DribbleTheBall()
    {
        //Wait until ball than start dribble
        yield return new WaitUntil(() => ball.transform.parent == this.transform);

        attackerAnimator.SetBool("isGameStarted", true);
        while (dribblePointNo < dribblePoints.Length && !isTackled/* && !DataScript.isGameOver*/)
        {
            DribbleWithBall();
            yield return new WaitForEndOfFrame();
        }

        if (!isTackled)
        {
            //if attacker is stayed left on the net play attackerKickMirrored
            if(net.position.x - transform.position.x > 0f)
                attackerAnimator.SetBool("isAttackerWon", true);
            else
                attackerAnimator.SetBool("isAttackerWonMirror", true);

            yield return new WaitForSeconds(0.7f); //kick animation ends after 0.7 sec approx. Could set animation event will be much more accurate

            ball.transform.parent = null;
             
            SendBallToGoal();
            DataScript.goalCount++;
        }

        StopCoroutine(DribbleTheBall());
           
    }

    void SendBallToGoal()
    {
        ball.GetComponent<Rigidbody>().AddForce((net.position - ball.transform.position) * 3f, ForceMode.VelocityChange);
        ball.GetComponent<Rigidbody>().AddTorque((net.position - ball.transform.position) * 3f, ForceMode.VelocityChange);
    }

    void DribbleWithBall()
    {
        dribbleTo = dribblePoints[dribblePointNo].position;
        dribbleTo.y = ballY;

        ball.transform.Rotate(new Vector3(-40f, 0, 0));
        ball.transform.position = Vector3.MoveTowards(transform.position, dribbleTo, attackerSpeed * 2f * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(ball.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, ball.transform.position, attackerSpeed * Time.deltaTime);

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
        if(isTackled == false)
        {
            isTackled = true;
            ApplyTackleForce(tacklePos);
            //OpenRagdollPhysics();
        }        
    }
    
    void OpenColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.isTrigger = false;
        }

    }

    void ApplyTackleForce(Vector3 tacklePos)
    {
        attackerAnimator.enabled = false;
        transform.parent = null;

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.useGravity = true;

            //this is for this game only!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            rigidbody.AddExplosionForce(1250f, tacklePos, 100f, 20f);
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
    }

    void OpenRagdollPhysics()
    {
        attackerAnimator.enabled = false;
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
        }
        
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
            }
        }
    }

}
