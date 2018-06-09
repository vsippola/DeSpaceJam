using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class WormHoleIntoMoveBehaviour : MoveBehaviour
{
    public override void Move(LevelMoment next, LevelMoment prev, Lemming lemming, GameBlock block)
    {
        lemming.position = block.pos;

        LevelData.Instance.WormHole(next);
        next.timeTraveler = lemming;
        next.lemmings.Remove(lemming);
        next.phasedLemmings.Remove(lemming);
        next.fail--;
    }
}

