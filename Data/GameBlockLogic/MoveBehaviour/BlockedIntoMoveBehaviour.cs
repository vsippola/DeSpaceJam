using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class BlockedIntoMoveBehaviour : MoveBehaviour
{
    public override void Move(LevelMoment data, Lemming lemming, GameBlock block)
    {
        lemming.Turn();
    }
}

