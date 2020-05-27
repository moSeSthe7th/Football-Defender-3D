using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowToDirection : MonoBehaviour
{

    public Vector3 flowPoint = Vector3.zero;
    public GameObject explosionSystem;

    private void OnEnable()
    {
        transform.parent = null;
        explosionSystem = Resources.Load<GameObject>("Prefabs/ExplosionPS");
    }

    

    public IEnumerator Flow()
    {
        bool isExploded = false;
        float waitTime = Random.Range(0.000f, 0.500f);

        yield return new WaitForSeconds(waitTime);

        while(Vector3.Distance(flowPoint, transform.position) > 0.5f)
        {
            transform.position = Vector3.Slerp(transform.position, flowPoint, 0.05f);
            // transform.Rotate(Vector3.right * 10f, 10f, Space.World);

            if (DataScript.GetState() == DataScript.GameState.PassedLevel && (Mathf.Abs(gameObject.transform.position.x) <10f) ||
                (Mathf.Abs(gameObject.transform.position.z) < 15f))//gameObject.transform.position.y <= -2f && !isExploded)
            {
                if(gameObject.transform.position.y <= -2f && !isExploded)
                {
                    isExploded = true;
                    Instantiate(explosionSystem, transform.position, Quaternion.identity);
                }
            }
            else
            {
                if (gameObject.transform.position.y <= 0 && !isExploded)
                {
                    isExploded = true;
                    Instantiate(explosionSystem, transform.position, Quaternion.identity);
                }
            }

            yield return new WaitForEndOfFrame();
        }

       
        //Destroy(this.gameObject);
        StopCoroutine(Flow());
    }

  /*  private void Update()
    {
        if(flowPoint != Vector3.zero && Vector3.Distance(flowPoint,transform.position) > 0.5f)
        {
            transform.position = Vector3.Slerp(transform.position, flowPoint, 0.05f);
            transform.Rotate(Vector3.right * 10f, 10f,Space.World);
        }
    }*/

}
