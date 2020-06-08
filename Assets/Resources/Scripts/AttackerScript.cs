using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerScript : MonoBehaviour
{
    Animator attackerAnimator;

    public Transform cubeReplica;

    GameObject ball;
    float ballY;

    Rigidbody[] rigidbodies;
    Transform[] dribblePoints;
    int dribblePointNo;
    Vector3 dribbleTo;

    bool isTackled;

    Transform net;

    public float attackerSpeed = 3f;

    Coroutine dribbleBall;

    SkinnedMeshRenderer attackerRenderer;

    void OnEnable()
    {

        attackerAnimator = GetComponent<Animator>();
        DataScript.totalAttackerCount++;
        ball = transform.GetChild(0).gameObject;
        dribblePointNo = 0;
        
        ballY = ball.transform.position.y;
        isTackled = false;
        dribblePoints = transform.GetChild(1).GetComponent<DribblePoints>().GetDribblePoints();

        rigidbodies = GetComponentsInChildren<Rigidbody>();
        attackerRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (!net)
            net = GameObject.Find("Net_01").transform;

        if (cubeReplica == null)
            foreach(Transform cubeRep in GetComponentsInChildren<Transform>())
            {
                if (cubeRep.name.ToLower().Contains("cubeman"))
                    cubeReplica = cubeRep;
            }
        cubeReplica.gameObject.SetActive(false);

        OpenColliders();
        DataScript.onGameStarted += StartRunning;
    }

    void OnDestroy()
    {
        DataScript.onGameStarted -= StartRunning;
    }

    void StartRunning()
    {
        if(dribbleBall == null)
        {
            dribbleBall = StartCoroutine(DribbleTheBall());
        }
        else
        {
            Debug.LogError("Attacker coruntine is not null");
        }
    }

    IEnumerator DribbleTheBall()
    {
        //Wait until ball than start dribble
        yield return new WaitUntil(() => ball.transform.parent == this.transform);

        attackerAnimator.SetBool(DataScript.animHash.attackerHash.gameStarted, true);
        while (dribblePointNo < dribblePoints.Length && !isTackled/* && !DataScript.isGameOver*/)
        {
            DribbleWithBall();
            yield return new WaitForEndOfFrame();
        }

        if (!isTackled)
        {
            //if attacker is stayed left on the net play attackerKickMirrored
            if(net.position.x - transform.position.x > 0f)
                attackerAnimator.SetBool(DataScript.animHash.attackerHash.win, true);
            else
                attackerAnimator.SetBool(DataScript.animHash.attackerHash.winMirror, true);

            //wait for kick
            yield return new WaitForSeconds(0.7f); //kick animation ends after 0.7 sec approx. Could set animation event will be much more accurate

            ball.transform.parent = null;
             
            SendBallToGoal();
            DataScript.goalCount++;
        }

        StopCoroutine(dribbleBall);
           
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

    IEnumerator DisposeCube()
    {
        yield return new WaitForEndOfFrame();
        //if position not changed continue
        /*while (didMoved(replicaPosition))
        {
            replicaPosition = cubeReplica.position;
            yield return new WaitForEndOfFrame();
        }*/

        cubeReplica.gameObject.SetActive(true);
        attackerRenderer.enabled = false;

    }

    bool didMoved(Vector3 oldPosition)
    {
        if (Vector3.Distance(oldPosition, cubeReplica.position) < 0.01f)
        {
            return false;
        }

        return true;
    }

    public void Tackled(Vector3 tacklePos)
    {
        if(isTackled == false)
        {
            isTackled = true;
            ApplyTackleForce(tacklePos);
            StopCoroutine(dribbleBall);
            cubeReplica.gameObject.SetActive(true);
            attackerRenderer.enabled = false;
        }        
    }
    
    void OpenColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider coll in colliders)
        {
            coll.isTrigger = false;
        }

    }

    void ApplyTackleForce(Vector3 tacklePos)
    {
        attackerAnimator.enabled = false;
        transform.parent = null;

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.useGravity = true;

            //this is for this game only!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            rigidbody.AddExplosionForce(750f, tacklePos, 50f, 100f);
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
    }
}
