using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class MoveBehaviourAbstract
{
    public abstract void Move(LevelMoment next, LevelMoment prev, Lemming lemming, GameBlock block);
}