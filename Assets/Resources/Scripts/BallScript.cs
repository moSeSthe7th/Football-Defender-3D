using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    Rigidbody rb;
    Transform myAttacker;
    Vector3 initialPosition;

    public bool placed;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; //at start set as kinematic

        myAttacker = transform.parent;
        initialPosition = transform.position;

        GetComponent<MeshRenderer>().enabled = false;

        placed = false;

    }

    public void FallToAttacker(Vector3 startingBallPosition)
    {
        transform.parent = null; //detach ball
        transform.position = startingBallPosition;

        GetComponent<MeshRenderer>().enabled = true;

        StartCoroutine(Fall());

    }

    IEnumerator Fall()
    {
        rb.isKinematic = false;
        rb.AddExplosionForce(750f, transform.position, 200f);

        while (Vector3.Distance(transform.position, initialPosition) > 0.05f )
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, 5f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        transform.position = initialPosition;
        transform.parent = myAttacker;

        placed = true;

        yield return null;
    }

}
