using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This scripts assume crowd is created inside a stadium steating area. Crowd will be created inside seats
//Object scale is 1 1 1
//Seats are inside 0.2x 0.8x and 0.05z 0.55z in localPosition. y of object will be determined by raycasting

[RequireComponent(typeof (MeshCollider))]
public class CrowdCreator : MonoBehaviour
{
    [HideInInspector] public List<GameObject> crowd;
    [HideInInspector] public List<List<Transform>> seatAreas; 
    public bool didSeatAreaLifters;

    int seatAreaColumnCount = 3;
    [Range (0f,1f)]public float fullnessRate = 1f;
    public int SeatCountAtRow = 12;

    int WatcherCount;

    GameObject watcherPrefab;
    
    List<List<Transform>> seat_rowXcolumns;

    Mesh seatingMesh;
    Vector3 actualScale;

    LayerMask layerMask;

    void Start()
    {

        seatingMesh = GetComponent<MeshFilter>().mesh;
        actualScale = Vector3.Scale(seatingMesh.bounds.size, transform.localScale);
        //Debug.Log("actualScale : " + actualScale);
        float seatArea = (actualScale.x) * (actualScale.z);
        WatcherCount = (int)seatArea / 2;
        //Debug.Log(actualScale);

        watcherPrefab = (GameObject)Resources.Load("Prefabs/Watcher");
        crowd = new List<GameObject>(WatcherCount);
        seat_rowXcolumns = new List<List<Transform>>();
        seatAreas = new List<List<Transform>>(seatAreaColumnCount);
        for(int i = 0;i<seatAreaColumnCount;i++)
        {
            seatAreas.Add(new List<Transform>());
        }

        layerMask = layerMask | (1 << gameObject.layer);

        //CreateWatchers(WatcherCount);
        WatcherCount = CreateAndAssignSeatPositions();
        CreateWatchers(WatcherCount);
        PlaceWatchers();
        //PlaceWatchers();

    }

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
    /// Place watchers inside seats. Place randomly inside 0.2x 0.8x and 0.05z 0.55z positions of object
    /// </summary>
    void PlaceWatchers()
    {
        int row = 0;
        int counter = 0;
        foreach(List<Transform> seatRow in seat_rowXcolumns)
        {
            //Random.InitState(Random.Range(0, 100));

            foreach(Transform seat in seatRow)
            {
                float rand = Random.Range(0f, 100f);
                //Debug.Log(rand + "  rate   " + ((100f * fullnessRate) - ((1f - fullnessRate) * row * 10f)));
                bool didOccupied = (rand < ((100f * fullnessRate) - ((1f - fullnessRate) * row * 10f))) ? true : false;

                if (didOccupied && crowd[counter])
                {
                    crowd[counter].transform.parent = seat;
                    crowd[counter].transform.position = seat.position;
                    counter++;
                }
                else if(!crowd[counter])
                {
                    return;
                }
            }

            row++;

        }
    }

    float CalculateY(Vector3 currPos)
    {
        RaycastHit rayHit;

        Vector3 origin = new Vector3(currPos.x, currPos.y, currPos.z);
        //org = origin;
        if (Physics.Raycast(origin, Vector3.down, out rayHit, 10f, layerMask))
        {
            //Debug.Log(rayHit.point);
            return rayHit.point.y;
        }

        return -1; // error code
    }


    /// <summary>
    /// Create seat positions inside seating area. Current stadium have this positions in 0.2x 0.8x and 0.05z 0.55z positions of seating area object
    /// </summary>
    int CreateAndAssignSeatPositions()
    {
        //-------
        //Start from left bottom corner of seating area and increase z position of z at each ray iteration. That way we will calculate rows of seating area
        //at each row calculate seat column number and columns.
        //while assining rows if ray hits to a different height from previous that means there is a new seating row we will add that to rows
        //while assining columns if ray hit
        //-------

        float xPos = (actualScale.x / (actualScale.x / 10f));
        float yPos = 0.85f;
        float zPos = (actualScale.z / 2.05f);

        Vector3 currentPosition = new Vector3(xPos, yPos, zPos);
        Vector3 seatPos = Vector3.zero;
        bool calculatedRows = false;

        int no = 0;
        while (calculatedRows == false)
        {
            GameObject seat = new GameObject("seat-" + no.ToString());
            no++;

            seat.transform.parent = this.transform;
            seat.transform.forward = this.transform.forward;

            if (no == 1)
            {
                seat.transform.localPosition = currentPosition; //assign initial seat in local position first
            }
            else
            {
                seat.transform.position = seatPos;
                seat.transform.localPosition = new Vector3(seat.transform.localPosition.x, currentPosition.y, seat.transform.localPosition.z);
            }

            Vector3 seatPosition = seat.transform.position; //take world location
            //seatPosition = CalculateLastPointAtDirection(seatPosition, transform.forward * -1f);//carry seatPosition to next steps beginnig

            seatPosition.y = CalculateY(seatPosition);

            //seat area for rows ended break while end exit assining rows
            if (seatPosition.y == -1)
            {
                calculatedRows = true;
                //Debug.Log(no);

                Destroy(seat);
                break;
            }

            //set seat position if position is at seat area
            seat.transform.position = seatPosition;

            //add seat as a new row (each row will be a list in seat_rowXcolumns containing columns)
            List<Transform> seatRow = new List<Transform>();
            seatRow.Add(seat.transform);

            //after assining row start assining columns for that row
            //scan to right at objects local position. At each iteration go 0.5f in right direction and if ray hits assign a seat to there
            bool calculatedColumns = false;
            int calculatedSeatAreaColumnCount = 0;
            Vector3 currentColumnPosition = seatRow[seatRow.Count - 1].position;

            seatAreas[calculatedSeatAreaColumnCount].Add(seat.transform);

            //Vector3 distanceBtwColumnWatcher = currentColumnPosition - CalculateLastPointAtDirection(currentColumnPosition, transform.right * -1f);
            float distanceBtwColumnWatcher = Vector3.Distance(currentColumnPosition, CalculateLastPointAtDirection(currentColumnPosition, transform.right * -1f)) / (float)SeatCountAtRow;
           // Debug.Log(distanceBtwColumnWatcher);

            while (calculatedColumns == false)
            {
                RaycastHit rayHit;
                Vector3 origin = currentColumnPosition;
                origin -= transform.right * distanceBtwColumnWatcher;
                origin.y++;//increase y since ray will be shooted down
                //Debug.DrawRay(origin, Vector3.down * 10f, Color.red, 100f);

                if (Physics.Raycast(origin, Vector3.down, out rayHit, 10f, layerMask))
                {
                    //if surface is not aimed that can be a seat position
                    if (rayHit.normal == Vector3.up)
                    {
                        GameObject columnSeat = new GameObject("seat-" + no.ToString());
                        no++;

                        columnSeat.transform.parent = this.transform;
                        columnSeat.transform.forward = this.transform.forward;

                        columnSeat.transform.position = rayHit.point;
                        currentColumnPosition = rayHit.point;

                        seatRow.Add(columnSeat.transform);
                        seatAreas[calculatedSeatAreaColumnCount].Add(columnSeat.transform);

                    }
                    else if (calculatedSeatAreaColumnCount >= seatAreaColumnCount - 1)
                    {
                        calculatedColumns = true;
                        break;
                    }
                    else
                    {
                        currentColumnPosition = CalculateLastPointAtDirection(origin, transform.right * -1f);
                        //currentColumnPosition -= transform.right * 0.05f;
                        currentColumnPosition.y = seatPosition.y;

                        distanceBtwColumnWatcher = Vector3.Distance(currentColumnPosition, CalculateLastPointAtDirection(currentColumnPosition, transform.right * -1f)) / ((float)SeatCountAtRow + 1);
                       // Debug.Log(distanceBtwColumnWatcher);

                        calculatedSeatAreaColumnCount++;
                        continue;
                    }
                }
                else
                {
                    Debug.LogError("seatler icin column atarken ray carpmadi..");

                    calculatedColumns = true;
                    break;
                }
            }

            seat_rowXcolumns.Add(seatRow);

            seatPos = seat.transform.position;
            seatPos = CalculateLastPointAtDirection(seatPos, transform.forward * -1f);//carry seatPosition to next steps begining
        }

        //update watcher count according to seat count and fullness rate 
        WatcherCount = (int)(fullnessRate * no);
        //Debug.Log(WatcherCount);
        return WatcherCount;
    }

    public static bool FastApproximately(float a, float b, float threshold)
    {
        return ((a < b) ? (b - a) : (a - b)) <= threshold;
    }

    //calculate last point of a flat object on given direction. Object will assumed it finished when height changes
    Vector3 CalculateLastPointAtDirection(Vector3 startingPosition, Vector3 direction)
    {
        RaycastHit rayHit;

        Vector3 origin = new Vector3(startingPosition.x, startingPosition.y + 1f , startingPosition.z);
        //Debug.Log("starting origin : " + startingPosition);
        //Physics.Raycast(origin, Vector3.down, out rayHit, 10f, layerMask);
        float currentHeight = 0f;
        bool heightChanged = false;

        while(!heightChanged)
        {
            if (Physics.Raycast(origin, Vector3.down, out rayHit, 10f, layerMask))
            {

                if(currentHeight == 0f)
                {
                    currentHeight = rayHit.distance;
                }
          
                //Debug.Log("currentHeight : " + currentHeight + " . rayHit.point.y : " + rayHit.distance);

                if (!FastApproximately(currentHeight, rayHit.distance,0.1f)) //height changed last point was the objects last point
                {
                   // Debug.DrawRay(origin, Vector3.down * 10f, Color.red, 100f);
                    //Debug.Log("currentHeight : " + currentHeight + " . rayHit.point.y : " + rayHit.distance);
                    heightChanged = true;
                    break;
                }
                else
                {
                    //Debug.DrawRay(origin, Vector3.down * 1f, Color.green, 100f);
                }

                origin += direction * 0.05f;
            }
            else
            {
                //Debug.Log("seat yerlestirirken ray carpmadi... row atamasi bitti. (Hata da olabilir)");
                heightChanged = true;
                return Vector3.zero;
            }
        }

        origin.y -= 1f;//normalize y
       // Debug.Log("ending origin : " + origin);

        return origin;
    }
}
