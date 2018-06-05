using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class WormHoleIntoMoveBehaviour : MoveBehaviour
{
    public override void Move(LevelMoment data, Lemming lemming, GameBlock block)
    {
        lemming.position = block.pos;

        LevelData.Instance.WormHole(data);
        data.timeTraveler = lemming;
    }
}

