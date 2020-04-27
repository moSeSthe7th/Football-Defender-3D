using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherScript : MonoBehaviour
{
    SkinnedMeshRenderer renderer;
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();

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
}
