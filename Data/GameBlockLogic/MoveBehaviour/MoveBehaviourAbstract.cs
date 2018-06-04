using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class MoveBehaviourAbstract
{
    public abstract void Move(LevelMoment data, Lemming lemming, GameBlock block);
}