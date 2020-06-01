using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManDispose : MonoBehaviour
{
    Coroutine dispose;
    Coroutine disposeToDefender;
    GameObject defender;
    PointBar bar;
    MeshRenderer[] cubesMeshes;
    Transform[] cubes;

    Vector3 targetPosition;
    FlowToDirection cubeFlow;
    public Light pointLight;

    void OnEnable()
    {
        bar = FindObjectOfType(typeof(PointBar)) as PointBar;
        defender = GameObject.FindWithTag("Defender");

        cubesMeshes = GetComponentsInChildren<MeshRenderer>();
        cubes = new Transform[cubesMeshes.Length];

        int count = 0;
        foreach (MeshRenderer cubeMesh in cubesMeshes)
        {
            cubes[count] = cubeMesh.transform;
            count++;
        }

        if (dispose == null)
        {
            dispose = StartCoroutine(Dispose());
        }
    }

    IEnumerator Dispose()
    {
        foreach (Transform cube in cubes)
        {
            float targetXPos = cube.position.x + Random.Range(-10, 10);
            float targetZPos = cube.position.z + Random.Range(-10, 10);
            float targetYPos = cube.position.y + Random.Range(10, 20);
            targetPosition = new Vector3(targetXPos,targetYPos,targetZPos);
            cubeFlow = cube.gameObject.AddComponent<FlowToDirection>();
            
            cubeFlow.flowPoint = targetPosition;
            StartCoroutine(cubeFlow.Flow());
           // yield return new WaitForSeconds(0.001f);
        }
        yield return new WaitForEndOfFrame();

      /*  if (DataScript.GetState() == DataScript.GameState.PassedLevel && !DataScript.isLevelAnimPlayed)
        {
            DataScript.isLevelAnimPlayed = true;
            StartCoroutine(managerScript.LevelPassedAnimations());
        }*/
        StopCoroutine(dispose);
    }

    
}
