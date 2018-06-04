using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Lemming
{
    public Vector3Int direction = new Vector3Int(1, 0, 0);
    public Vector3Int position = new Vector3Int(0, 0, 0);
    public int phase = 0;
    public int age = 3;  

    //Turns to the left
    public void Turn()
    {
        if(direction.x != 0)
        {
            direction = new Vector3Int(0, 0, direction.x);
        }
        else
        {
            direction = new Vector3Int(-direction.z, 0, 0);
        }
    }

    public Lemming DeepCopy()
    {
        Lemming result = new Lemming
        {
            age = this.age,
            phase = this.phase,
            direction = this.direction,
            position = this.position
        };

        return result;
    }
}
