using System.Collections.Generic;
using UnityEngine;

public class MomentDisplayBuilder
{
    private GameObjectPooler pool;
    private Transform parent;
    private LevelMoment moment;

    private Dictionary<char, List<GameObject>> displayPool;
    private bool built;

    public MomentDisplayBuilder(GameObjectPooler pool, Transform parent)
    {
        this.pool = pool;
        this.parent = parent;

        built = false;
    }

    public void BuildDisplay()
    {
        if (built) return;

        if (moment == null)
        {
            built = true;
            return;
        }

        displayPool = new Dictionary<char, List<GameObject>>();

        int x = moment.x;
        int y = moment.y;
        int z = moment.z;        

        for (int k = 0; k < z; k++)
        {
            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    char key = moment.level[i, j, k];

                    if (key == GameBoardCubeDictionary.OPEN_SPACE) continue;
                    if (key == GameBoardCubeDictionary.OPEN_CUBE) continue;

                    GameObject newObject = GetObject(key);

                    if (newObject == null) continue;

                    newObject.SetActive(true);
                    newObject.transform.localPosition = new Vector3(i, j, k);
                    newObject.transform.localEulerAngles = Vector3.zero;
                }
            }
        }

        PlaceLemmings(GameBoardCubeDictionary.LEMMING, moment.lemmings);
        PlaceLemmings(GameBoardCubeDictionary.LEMMING_LIVE, moment.savedLemmings);
        PlaceLemmings(GameBoardCubeDictionary.LEMMING_DEAD, moment.deadLemmings);
        PlaceLemmings(GameBoardCubeDictionary.LEMMING_PHASED, moment.phasedLemmings);
        PlaceSpawners(GameBoardCubeDictionary.SPAWNER, moment.spawners);

        if (moment.HasTimeTraveler()) PlaceLemming(GameBoardCubeDictionary.LEMMING, moment.timeTraveler);

        built = true;
    }

    private void PlaceSpawners(char key, List<Spawner> spawners)
    {
        foreach(Spawner spawn in spawners)
        {
            GameObject newObject = GetObject(key);

            if (newObject == null) return;

            Vector3Int pos = spawn.position;
            newObject.SetActive(true);

            newObject.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);

            Vector3 angles = new Vector3(0f, 90f, 0f);
            float turn = 0f;
            if (spawn.direction.z == -1) turn = 1;
            else if (spawn.direction.x == -1) turn = 2;
            else if (spawn.direction.z == 1) turn = 3;

            newObject.transform.localEulerAngles = angles * turn;
        }
        
    }

    private void PlaceLemming(char key, Lemming lem)
    {
        GameObject newObject = GetObject(key);

        if (newObject == null) return;

        Vector3Int pos = lem.position;
        newObject.SetActive(true);

        newObject.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);

        Vector3 angles = new Vector3(0f, 90f, 0f);
        float turn = 0f;
        if (lem.direction.z == -1) turn = 1;
        else if (lem.direction.x == -1) turn = 2;
        else if (lem.direction.z == 1) turn = 3;

        newObject.transform.localEulerAngles = angles * turn;
    }

    private void PlaceLemmings(char key, List<Lemming> lemmings)
    {
        foreach (Lemming lem in lemmings)
        {
            PlaceLemming(key, lem);
        }
    }

    private GameObject GetObject(char key)
    {
        if(!displayPool.ContainsKey(key))
        {
            List<GameObject> list = new List<GameObject>();
            displayPool.Add(key, list);
        }

        //Get new object and attach it to the parent.
        GameObject newObject = pool.GetObject(key);

        if (newObject == null) return null;

        newObject.transform.parent = parent;

        //Add to list of objects so the object can be added back to the pool when level is unbuilt.    
        displayPool[key].Add(newObject);

        return newObject;
    }

    public void SetMoment(LevelMoment moment)
    {
        if (this.moment == moment) return;
        UnbuildDisplay();
        this.moment = moment;
    }

    public void UnbuildDisplay()
    {
        FreePool();
        built = false;
    }

    private void FreePool()
    {       
        if (displayPool == null) return;

        foreach(char key in displayPool.Keys)
        {
            List<GameObject> list = displayPool[key];
            foreach(GameObject obj in list)
            {
                pool.ReturnObject(key, obj);
            }
        }

        displayPool = null;
    }
}
