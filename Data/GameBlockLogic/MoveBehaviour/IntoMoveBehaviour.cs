using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


//Default is to just move into the blocks zone
class IntoMoveBehaviour : MoveBehaviour
{
    public override void Move(LevelMoment next, LevelMoment prev, Lemming lemming, GameBlock block)
    {
        Lemming check = null;

        foreach (Lemming lem in next.lemmings)
        {
            if (check != null) break;
            if (lem.position == block.pos)
            {
                check = lem;
            }
        }

        foreach (Lemming lem in next.phasedLemmings)
        {
            if (check != null) break;
            if (lem.position == block.pos)
            {
                check = lem;
            }
        }

        if (check != null)
        {
            lemming.Turn();
            return;
        }

        lemming.position = block.pos;
    }

     
}

