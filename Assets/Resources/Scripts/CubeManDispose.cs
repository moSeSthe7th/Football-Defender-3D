using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManDispose : MonoBehaviour
{
    Coroutine dispose;
    PointBar bar;

    void OnEnable()
    {
        bar = FindObjectOfType(typeof(PointBar)) as PointBar;

        if (dispose == null)
        {
            dispose = StartCoroutine(Dispose());
        }
    }

    IEnumerator Dispose()
    {
        MeshRenderer[] cubesMeshes = GetComponentsInChildren<MeshRenderer>();
        Transform[] cubes = new Transform[cubesMeshes.Length];

        int count = 0;
        foreach (MeshRenderer cubeMesh in cubesMeshes)
        {
            cubes[count] = cubeMesh.transform;
            count++;
        }

        Vector3 targetPosition = Vector3.up * 15f;


        foreach (Transform cube in cubes)
        {
            FlowToDirection cubeFlow = cube.gameObject.AddComponent<FlowToDirection>();
            cubeFlow.flowPoint = targetPosition;
            
           // yield return new WaitForSeconds(0.001f);
        }
        yield return new WaitForEndOfFrame();

        StopCoroutine(dispose);
    }

}
