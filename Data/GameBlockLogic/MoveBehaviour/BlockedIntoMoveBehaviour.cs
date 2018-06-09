using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class BlockedIntoMoveBehaviour : MoveBehaviour
{
    public override void Move(LevelMoment next, LevelMoment prev, Lemming lemming, GameBlock block)
    {
        lemming.Turn();
    }
}

