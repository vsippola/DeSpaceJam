using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class IntoVanishMoveBehaviour : IntoMoveBehaviour
{
    readonly char fade;

    public IntoVanishMoveBehaviour(char n)
    {
        fade = --n;
    }

    public override void Move(LevelMoment next, LevelMoment prev, Lemming lemming, GameBlock block)
    {

        
        Vector3Int i = block.pos;
        
        if (fade > '0')
        {
            next.level[i.x, i.y, i.z] = fade;
        }
        else
        {
            next.level[i.x, i.y, i.z] = GameBoardCubeDictionary.OPEN_CUBE;
        }

        lemming.Turn();
    }
}

