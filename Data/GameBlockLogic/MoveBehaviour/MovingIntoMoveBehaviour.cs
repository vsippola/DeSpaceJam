using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


//Default is to just move into the blocks zone
class MovingIntoMoveBehaviour : MoveBehaviour
{
    public override void Move(LevelMoment next, LevelMoment prev, Lemming lemming, GameBlock block)
    {
        Lemming check = null;

        foreach(Lemming lem in prev.lemmings)
        {
            if (check != null) break;
            if(lem.position == block.pos)
            {
                check = lem;
            }
        }

        foreach (Lemming lem in prev.phasedLemmings)
        {
            if (check != null) break;
            if (lem.position == block.pos)
            {
                check = lem;
            }
        }

        if(check != null)
        {
            Vector3Int test = check.direction * -1;
            if(lemming.direction == test)
            {               
                if(check.position.y > 0)
                {
                    char below = prev.level[check.position.x, check.position.y - 1, check.position.z];
                    if ((below != GameBoardCubeDictionary.OPEN_CUBE) && (below != GameBoardCubeDictionary.NEGA_CUBE))
                    {
                        lemming.Turn();
                        return;
                    }
                }
                              
            }
        }

        lemming.position = block.pos;
    }

     
}

