using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdController : MonoBehaviour
{
    struct LifterCrowd 
    {
        public List<List<WatcherScript>> seatAreaWatchers;
    }

    List<LifterCrowd> LifterCrowds;
    List<WatcherScript> allWatchers;

    private void Start()
    {
        // SetAllWatchers and SetLifterCrowds could be one funtion but this way is more readable 
        //sets all watchers from crowdcreator scripts
        allWatchers = SetAllWatchers();
        //sets all crowds with lifter setted to true. lifter crowds will be full and lift pictures. 
        LifterCrowds = SetLifterCrowds();

       // SpriteToBoards countDownSprites = new SpriteToBoards("LifterSprites/CountDown");
       // StartLifting(countDownSprites.spriteMaps);

        //StartCoroutine(startCountDown());
    }

    public void StartLiftingArea(int AreaRow, List<SpriteToBoards.SpriteMap> parsedSprites)
    {
        for (int i = 0; i < LifterCrowds.Count; i++)
        {
            for (int watcher = 0; watcher < LifterCrowds[i].seatAreaWatchers[AreaRow].Count; watcher++)
            {
                LifterCrowds[i].seatAreaWatchers[AreaRow][watcher].LiftBoard(parsedSprites[AreaRow].boards[watcher].boardColor);
            }
        }
    }

    public void StartLifting(List<SpriteToBoards.SpriteMap> parsedSprites)
    {
        for(int i = 0; i < LifterCrowds.Count; i++)
        {
            for (int area = 0; area < LifterCrowds[i].seatAreaWatchers.Count;area++) 
            {
                for (int watcher = 0; watcher < LifterCrowds[i].seatAreaWatchers[area].Count; watcher++)
                {
                    LifterCrowds[i].seatAreaWatchers[area][watcher].LiftBoard(parsedSprites[area].boards[watcher].boardColor);
                }
            }
        }
    }

    List<WatcherScript> SetAllWatchers()
    {
        List<WatcherScript> watchers = new List<WatcherScript>();

        CrowdCreator[] createdCrowds = FindObjectsOfType<CrowdCreator>();

        foreach (CrowdCreator crowd in createdCrowds)
        {
            foreach(GameObject watcher in crowd.crowd)
            {
                watchers.Add(watcher.GetComponent<WatcherScript>());
            }
        }

        return watchers;
    }

    //if seat area setted as lifter add that seat area as liftercrowd. Lifter Crowds will be managed in this script
    List<LifterCrowd> SetLifterCrowds()
    {
        List<LifterCrowd> allCrowd = new List<LifterCrowd>();
        CrowdCreator[] createdCrowds = FindObjectsOfType<CrowdCreator>();

        //Set boards for lifting crowds watchers
        GameObject dummyBoard = (GameObject)Resources.Load("Prefabs/Board");

        foreach(CrowdCreator crowd in createdCrowds)
        {
            if (crowd.didSeatAreaLifters == false) //break if seat area is not setted as counter
                continue;

            LifterCrowd currentCrowd = new LifterCrowd();
            currentCrowd.seatAreaWatchers = new List<List<WatcherScript>>();

            int currentArea = 0;

            foreach(List<Transform> seatArea in crowd.seatAreas)
            {
                currentCrowd.seatAreaWatchers.Add(new List<WatcherScript>());

                foreach (Transform seat in seatArea)
                {
                    WatcherScript watcher = seat.GetComponentInChildren<WatcherScript>();

                    if (watcher)
                    {
                        GameObject watchersBoard = Instantiate(dummyBoard, Vector3.down * 10f, Quaternion.identity, watcher.transform);
                        watcher.board = watchersBoard;
                        currentCrowd.seatAreaWatchers[currentArea].Add(watcher);
                    }
                }

                currentArea++;

            }

            allCrowd.Add(currentCrowd);

        }

        return allCrowd;
    }
}
