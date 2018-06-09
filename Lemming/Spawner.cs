using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Spawner
{
    public Vector3Int direction;
    public Vector3Int position = new Vector3Int(-1, 0, 0);
    public int age;
    public int phase;

    public Lemming MakeLemming()
    {
        Lemming result = new Lemming();
        result.age = 25;
        result.direction = direction;
        result.position = position + Vector3Int.down;
        result.phase = phase;

        return result;
    }

    //Turns to the left
    public Spawner DeepCopy()
    {
        Spawner result = new Spawner
        {
            age = this.age,
            direction = this.direction,
            position = this.position,
            phase = this.phase
        };

        return result;
    }
}
