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
            case GameBoardCubeDictionary.MOVE_CUBE:            
            case GameBoardCubeDictionary.OPEN_SPACE:            
                {
                    return new BlockedIntoMoveBehaviour();
                }
            case GameBoardCubeDictionary.LEMMING:
            case GameBoardCubeDictionary.LEMMING_PHASED:
                {
                    return new MovingIntoMoveBehaviour();
                }
            case GameBoardCubeDictionary.WORMHOLE_IN:
                {
                    return new WormHoleIntoMoveBehaviour();
                }
            case GameBoardCubeDictionary.VANISH_CUBE1:
            case GameBoardCubeDictionary.VANISH_CUBE2:
            case GameBoardCubeDictionary.VANISH_CUBE3:
                {                   
                    return new IntoVanishMoveBehaviour(key);
                }
            case GameBoardCubeDictionary.OPEN_CUBE:
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
            case GameBoardCubeDictionary.VANISH_CUBE1:
            case GameBoardCubeDictionary.VANISH_CUBE2:
            case GameBoardCubeDictionary.VANISH_CUBE3:
                {
                    return new OverVanishMoveBehaviour(key);
                }
            case GameBoardCubeDictionary.OPEN_SPACE:
            case GameBoardCubeDictionary.DEATH_CUBE:
            case GameBoardCubeDictionary.OPEN_CUBE:
            case GameBoardCubeDictionary.NEGA_CUBE:
                {
                    return new IntoMoveBehaviour();
                }
            default:
                {
                    return new MoveBehaviour();
                }
        }
    }
}

