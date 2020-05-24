using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowToDirection : MonoBehaviour
{

    public Vector3 flowPoint = Vector3.zero;

    private void OnEnable()
    {
        transform.parent = null;
        StartCoroutine(Flow());
    }

    IEnumerator Flow()
    {
        float waitTime = Random.Range(0.000f, 0.500f);

        yield return new WaitForSeconds(waitTime);

        while(Vector3.Distance(flowPoint, transform.position) > 0.5f)
        {
            transform.position = Vector3.Slerp(transform.position, flowPoint, 0.05f);
           // transform.Rotate(Vector3.right * 10f, 10f, Space.World);
            yield return new WaitForEndOfFrame();
        }

        Destroy(this.gameObject);
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
