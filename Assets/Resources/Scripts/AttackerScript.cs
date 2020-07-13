using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerScript : MonoBehaviour
{
    Animator attackerAnimator;

    public Transform cubeReplica;

    BallScript myBall;

    Rigidbody[] rigidbodies;
    //Transform[] dribblePoints;
    //int dribblePointNo;
    //Vector3 dribbleTo;

    public bool isTackled;
    private bool isShooted;

    Transform net;
    private Transform shootPos;
    
    public float attackerSpeed;

    Coroutine dribbleBall;
    SkinnedMeshRenderer attackerRenderer;

    void OnEnable()
    {
        attackerSpeed = 1.5f;
        
        attackerAnimator = GetComponent<Animator>();
        DataScript.totalAttackerCount++;
        myBall = transform.GetChild(0).gameObject.GetComponent<BallScript>();
       // dribblePointNo = 0;
        
        isTackled = false;
        isShooted = false;
        //dribblePoints = transform.GetChild(1).GetComponent<DribblePoints>().GetDribblePoints();

        rigidbodies = GetComponentsInChildren<Rigidbody>();
        attackerRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (!net)
            net = GameObject.Find("Net_01").transform;

        if (!shootPos)
            shootPos = GameObject.Find("ShootLine").transform;

        if (cubeReplica == null)
            foreach(Transform childTransform in GetComponentsInChildren<Transform>())
            {
                if (childTransform.name.ToLower().Contains("cubeman"))
                    cubeReplica = childTransform;
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
        yield return new WaitUntil(() => myBall.placed == true);
        
        attackerAnimator.SetBool(DataScript.animHash.attackerHash.gameStarted, true);
        while (!isShooted && !isTackled)
        {
            DribbleWithBall();
            yield return new WaitForFixedUpdate();
        }

        if (!isTackled && isShooted)
        {
            //if attacker is stayed left on the net play attackerKickMirrored
            if(net.position.x - transform.position.x > 0f)
                attackerAnimator.SetBool(DataScript.animHash.attackerHash.win, true);
            else
                attackerAnimator.SetBool(DataScript.animHash.attackerHash.winMirror, true);

            //wait for kick
            yield return new WaitForSeconds(0.7f); //kick animation ends after 0.7 sec approx. Could set animation event will be much more accurate
            
            SendBallToGoal();
            
            DataScript.goalCount++;
        }

        StopCoroutine(dribbleBall);
    }

    void SendBallToGoal()
    {
        Vector3 force = (net.position - myBall.transform.position) * attackerSpeed * 2f;
        myBall.rb.AddForce(force, ForceMode.VelocityChange);
        myBall.rb.AddTorque(force, ForceMode.VelocityChange);
    }

    void KickBall()
    {
        Vector3 kickPos = transform.position;
        kickPos.z -= 1;
        kickPos.x += Random.Range(-0.5f, 0.5f);
        

        myBall.rb.AddForce((kickPos - transform.position) * 3f,ForceMode.VelocityChange);
        myBall.rb.AddTorque((kickPos - transform.position) * 3f, ForceMode.VelocityChange);
    }
    
    void DribbleWithBall()
    {

        if (Mathf.Abs(Vector3.Distance(transform.position, myBall.transform.position)) < 0.1f) //Mathf.Approximately(Vector3.Distance(transform.position, myBall.transform.position), 0f))
        {
            if (myBall.transform.position.z < shootPos.position.z)
            {
                //shoot goal
                isShooted = true;
                return;
            }
            else
            {
                //kick ball
                KickBall();
            }
        }

        Quaternion targetRotation = Quaternion.LookRotation(myBall.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, attackerSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, myBall.transform.position, attackerSpeed * Time.deltaTime);
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
