using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DribblePoints : MonoBehaviour
{

    Transform[] dribblePoints;


    public Transform[] GetDribblePoints()
    {
        transform.position = gameObject.transform.parent.position;
        transform.parent = null;
        dribblePoints = GetComponentsInChildren<Transform>();
        return dribblePoints;
    }

}
