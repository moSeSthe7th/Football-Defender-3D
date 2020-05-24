using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherScript : MonoBehaviour
{
    public GameObject board;
    Transform hand;

    SkinnedMeshRenderer watcherRenderer;
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        watcherRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        foreach(Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.name.ToLower().Contains("hand"))
                hand = t;
        }

        if (!hand)
            Debug.LogError("Could not assign hand");

        transform.GetChild(0).gameObject.SetActive(false);

        if(anim)
        {
            anim.speed += Random.Range(-0.5f, 0.5f);

            int randAnim = Random.Range(0, 2);

            if(randAnim == 1)
            {
                anim.SetBool("Clapping", true);
            }
            else if(randAnim == 2)
            {
                 anim.SetBool("Standing", true);
            }

        }

      /*  if(renderer)
        {
            int randColor = Random.Range(0, 5);

            switch (randColor)
            {
                case 0:
                    renderer.material.color = Color.red;
                    break;
                case 1:
                    renderer.material.color = Color.green;
                    break;
                case 2:
                    renderer.material.color = Color.blue;
                    break;
                case 3:
                    renderer.material.color = Color.cyan;
                    break;
                case 4:
                    renderer.material.color = Color.white;
                    break;
                case 5:
                default:
                    break;
            }

        }*/
    }

    public void LiftBoard(Color32 boardColor)
    {
        if (board)
        {
            board.GetComponent<Renderer>().material.color = boardColor;
            StartCoroutine(Lifting(board));
        }
        else
            Debug.LogError("seating area is not setted as lifter for watcher " + this.name);
    }

    bool finishedLifting;
    IEnumerator Lifting(GameObject board)
    {
        finishedLifting = false;
        float reactionTime = Random.Range(0.000f, 0.250f);
        yield return new WaitForSeconds(reactionTime);

        anim.SetTrigger("Lift");

        float timePassed = 0f;
        while(!finishedLifting && timePassed < 2f)
        {
            Vector3 boardPos = hand.transform.position + (hand.up * 0.1f);
            board.transform.position = boardPos;

            Quaternion boardRot = board.transform.localRotation;
            //boardRot.
            board.transform.rotation = hand.transform.rotation;//Quaternion.RotateTowards(board.transform.rotation, Quaternion.Euler(Random.insideUnitSphere), 100f);

            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
        }

        yield return new WaitForEndOfFrame();

        board.transform.position = Vector3.down * -50f;

        //anim.ResetTrigger("Lift");

        StopCoroutine(Lifting(board));
    }

    public void OnFinishLift()
    {
        finishedLifting = true;
    }

}
