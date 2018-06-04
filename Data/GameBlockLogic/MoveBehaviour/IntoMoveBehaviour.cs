using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


//Default is to just move into the blocks zone
class IntoMoveBehaviour : MoveBehaviour
{
    public override void Move(LevelMoment data, Lemming lemming, GameBlock block)
    {
        lemming.position = block.pos;
    }

     
}

