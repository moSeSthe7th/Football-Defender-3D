using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowToDirection : MonoBehaviour
{

    public Vector3 flowPoint = Vector3.zero;
    public GameObject explosionSystem;
    public GameObject defender;
    Light pointLight;
    public bool isColorCloseToGreen;

    private void OnEnable()
    {
        Color cubeColor = RandomColorSelector();
        gameObject.GetComponent<Renderer>().material.color = cubeColor;
        pointLight = gameObject.AddComponent<Light>();
        pointLight.color = cubeColor;
        pointLight.range = 10f;

        if (isColorCloseToGreen)
        {
            pointLight.range = 3f;
        }

        pointLight.intensity = 0;
        transform.parent = null;
        explosionSystem = Resources.Load<GameObject>("Prefabs/ExplosionPS");
        defender = GameObject.FindWithTag("Defender");
    }

    Color RandomColorSelector()
    {
        isColorCloseToGreen = false;
        float randHue = Random.Range(0, 1.0f);
        if(randHue<0.6f && randHue > 0.05f)
        {
            isColorCloseToGreen = true;
        }
        return Color.HSVToRGB(randHue, 1, 1);
    }

    

    public IEnumerator Flow()
    {
       
        float waitTime = Random.Range(0.000f, 0.500f);

        yield return new WaitForSeconds(waitTime);

        while(Vector3.Distance(flowPoint, transform.position) > 0.5f)
        {
            transform.position = Vector3.Slerp(transform.position, flowPoint, 0.05f);
            // transform.Rotate(Vector3.right * 10f, 10f, Space.World);
            
            yield return new WaitForEndOfFrame();
        }

       
        //Destroy(this.gameObject);
        StopCoroutine(Flow());
    }

    public IEnumerator FlowToDefender(GameObject defender)
    {
        pointLight.intensity = 2f;

        //float waitTime = Random.Range(0.000f, 0.500f);

        //yield return new WaitForSeconds(waitTime);
        Vector3 defenderHeadPos = defender.transform.position;

        while (Vector3.Distance(defenderHeadPos, transform.position) > 0.1f)
        {
           
            defenderHeadPos.y = defender.transform.position.y + 0.4f;
            transform.position = Vector3.MoveTowards(transform.position, defenderHeadPos, 0.5f);
            // transform.Rotate(Vector3.right * 10f, 10f, Space.World);

            yield return new WaitForEndOfFrame();
        }


        gameObject.SetActive(false);
        StopCoroutine(FlowToDefender(defender));
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
