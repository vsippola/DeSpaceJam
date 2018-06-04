using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This way of going about it seems wrong
public class GameObjectPooler : MonoBehaviour
{
    [Range(1, 10)]
    public int multiplier = 10;

    //Gameobjects
    public Transform parent;
    public GameObject normalCube;
    public GameObject ending;
    public GameObject lemming;
    public GameObject lemmingLive;
    public GameObject lemmingDead;

    private static Dictionary<char, GameObjectQueue> gameObjectMap;    

    private void Awake()
    {
        gameObjectMap = new Dictionary<char, GameObjectQueue>();
        
        Initialize(GameBoardCubeDictionary.NORMAL_CUBE, normalCube);
        Initialize(GameBoardCubeDictionary.END_ZONE, ending);
        Initialize(GameBoardCubeDictionary.LEMMING, lemming);
        Initialize(GameBoardCubeDictionary.LEMMING_LIVE, lemmingLive);
        Initialize(GameBoardCubeDictionary.LEMMING_DEAD, lemmingDead);
    }

    public GameObject GetObject(char key)
    {

        if(key == GameBoardCubeDictionary.OPEN_SPACE) return null;

        GameObjectQueue pair = gameObjectMap[key];

        if (pair == null) return null;

        //If queue is empty expand pool
        if (pair.queue.Count == 0) Expand(key);

        return pair.queue.Dequeue();
    }

    //This seems clunky
    public void ReturnObject(char key, GameObject obj)
    {
        obj.transform.position = new Vector3(0f, 0f, 0f);
        obj.transform.rotation = Quaternion.identity;
        obj.transform.parent = parent;
        obj.SetActive(false);

        gameObjectMap[key].queue.Enqueue(obj);
    }

    private void Initialize(char key, GameObject obj)
    {
        //Initialize object and queue
        GameObjectQueue newQueue = new GameObjectQueue(obj);
        gameObjectMap.Add(key, newQueue);

        //Set initial capacity.
        Expand(key);
    }

    //Expand capacity for the pool with the given key.
    private void Expand(char key)
    {
        GameObjectQueue pair = gameObjectMap[key];

        GameObject original = pair._object;
        Queue<GameObject> queue = pair.queue;


        int i = multiplier;
        while (0 < i--)
        {
            GameObject newObject = GameObject.Instantiate(original, parent);
            newObject.SetActive(false);
            queue.Enqueue(newObject);
        }
    }
}
