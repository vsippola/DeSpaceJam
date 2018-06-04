using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectQueue
{
    public GameObject _object;
    public Queue<GameObject> queue;

    public GameObjectQueue(GameObject obj)
    {
        this._object = obj;
        queue = new Queue<GameObject>();
    }
}