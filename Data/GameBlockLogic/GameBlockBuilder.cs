using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GameBlockBuilder
{
    public GameBlock Build(char key, Vector3Int position)     
    {
        GameBlock result = new GameBlock
        {
            pos = position,
            key = key,
            enterMoveBehaviour = EnterBehaviourSelector(key),
            exitMoveBehaviour = ExitBehaviourSelector(key),
            overMoveBehaviour = OverBehaviourSelector(key)
        };

        return result;
    }

    private MoveBehaviour EnterBehaviourSelector(char key)
    {
        switch (key)
        {
            case GameBoardCubeDictionary.NORMAL_CUBE:
                {
                    return new BlockedIntoMoveBehaviour();
                }
            default:
                {
                    return new IntoMoveBehaviour();
                }
        }
    }

    private MoveBehaviour ExitBehaviourSelector(char key)
    {
        switch(key)
        {
            default:
                {
                    return new MoveBehaviour();
                }
        }        
    }

    private MoveBehaviour OverBehaviourSelector(char key)
    {
        switch (key)
        {
            default:
                {
                    return new MoveBehaviour();
                }
        }
    }
}

