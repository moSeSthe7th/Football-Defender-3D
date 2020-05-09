using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This scripts assume crowd is created inside a stadium steating area. Crowd will be created inside seats
//Object scale is 1 1 1
//Seats are inside 0.2x 0.8x and 0.05z 0.55z in localPosition. y of object will be determined by raycasting

[RequireComponent(typeof (MeshCollider))]
public class CrowdCreator : MonoBehaviour
{
    public int WatcherCount = 100;

    GameObject watcherPrefab;
    List<GameObject> crowd;
    List<Transform> seats;

    Mesh seatingMesh;
    Vector3 actualScale;

    LayerMask layerMask;

    void Start()
    {

        seatingMesh = GetComponent<MeshFilter>().mesh;
        actualScale = Vector3.Scale(seatingMesh.bounds.size, transform.localScale);
        Debug.Log("actualScale : " + actualScale);
        float seatArea = (actualScale.x) * (actualScale.z);
        WatcherCount = (int)seatArea / 2;
        //Debug.Log(actualScale);

        watcherPrefab = (GameObject)Resources.Load("Prefabs/Watcher");
        crowd = new List<GameObject>(WatcherCount);
        seats = new List<Transform>();

        layerMask = layerMask | (1 << gameObject.layer);

        //CreateWatchers(WatcherCount);
        CreateAndAssignSeatPositions();
        //PlaceWatchers();

    }

  /*  Vector3 org;
    private void FixedUpdate()
    {
        //foreach()
        Debug.DrawRay(org, Vector3.down, Color.green);
    }*/

    void CreateWatchers(int count)
    {
        for(int i=0;i<count;i++)
        {
            GameObject watcher = Instantiate(watcherPrefab, this.transform);
            watcher.transform.localScale *= 0.5f;
            crowd.Add(watcher);
        }
    }

    /// <summary>
    /// Create seat positions inside seating area. Current stadium have this positions in 0.2x 0.8x and 0.05z 0.55z positions of seating area object
    /// </summary>
    void CreateAndAssignSeatPositions()
    {
        float rowCount = (actualScale.x / (actualScale.x / 10f)) + (actualScale.x / (actualScale.x / 10f));
        float columnCount = (actualScale.z / 2.1f) + (actualScale.z / 10f);
        Debug.Log("rowCount : " + rowCount + " . columnCount : " + columnCount);

        //Start from left bottom corner of seating area and increase z position of z at each ray iteration. That way we will calculate rows of seating area
        //if ray hits to a different height from previous that means there is a new seating row we will add that to rows
        bool calculated = false;

        float xPos = (actualScale.x / (actualScale.x / 10f));
        float yPos = 0.85f;
        float zPos = (actualScale.z / 2.1f);

        Vector3 currentPosition = new Vector3(xPos,yPos,zPos);

        GameObject seat = new GameObject("seat");
        seat.transform.parent = this.transform;
        seat.transform.localPosition = currentPosition;

        while(!calculated)
        {
            RaycastHit rayHit;
            Vector3 origin = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);
           
            //org = origin;
            if (Physics.Raycast(origin, Vector3.down, out rayHit, 10f,layerMask))
            {

            }
            else // means we exited seating area set as calculated
            {
                calculated = true;
            }
        }


    }

    /// <summary>
    /// Place watchers inside seats. Place randomly inside 0.2x 0.8x and 0.05z 0.55z positions of object
    /// </summary>
    void PlaceWatchers()
    {
        foreach(GameObject watcher in crowd)
        {
            float xPos = Random.Range((-actualScale.x / (actualScale.x / 10f)), (actualScale.x / (actualScale.x / 10f))); // x is in the middle of seating object
            float zPos = Random.Range((-actualScale.z / 10f), (actualScale.z / 2.1f)); // z is in the middle of seating object


            float yPos = 0.85f; // set first height a little higher than middle to put crowd correctly on upper seats
            
            watcher.transform.localPosition = new Vector3(xPos, yPos, zPos); // first set place without setting height

            yPos = CalculateY(watcher.transform);// +  (transform.localScale.y * 0.25f);

            watcher.transform.position = new Vector3(watcher.transform.position.x, yPos, watcher.transform.position.z); // Finally update height
        }
    }

    float CalculateY(Transform currPos)
    {
        RaycastHit rayHit;

        Vector3 origin = new Vector3(currPos.position.x, currPos.position.y, currPos.position.z);
        //org = origin;
        if(Physics.Raycast(origin, Vector3.down,out rayHit,10f))
        {
            //Debug.Log(rayHit.point);
            return rayHit.point.y;
        }

        return 15f; // if there is an error set y very high so we dont see that in game
    }
}
