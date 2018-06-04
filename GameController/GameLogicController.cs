﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLogicController : MonoBehaviour
{
    public delegate void MomentBuilt(Vector2Int i);
    public event MomentBuilt MomentBuiltEvent;

    private LevelData levelData;

    public bool updating;

    [Range(0.01f, 10f)]
    public float timeBetweenUpdates;
    private float updateTimer;    

    private void Awake()
    {
        updateTimer = timeBetweenUpdates;
        levelData = LevelData.Instance;
        updating = true;
        CheckBuilding();
    }
	
	void Update ()
    {
        if (!updating) return;

        updateTimer -= Time.deltaTime;

        if (updateTimer > 0) return;

        updateTimer = timeBetweenUpdates;
        CheckBuilding();
	}

    private void CheckBuilding()
    {
        //if (!Input.GetKeyDown(KeyCode.N)) return;

        bool changed = false;

        //Find next map to update. (for not just go linearly)
        List<LevelMoment> timeline = levelData.moments.Last<List<LevelMoment>>();
        LevelMoment prevMoment = timeline.Last<LevelMoment>().DeepCopyLevelMoment();
        LevelMoment nextMoment = prevMoment.DeepCopyLevelMoment();

        //do wormhole logic later?

        List<Lemming> curLemming = new List<Lemming>();
        //Keep track of saved and dead lemming count
        Vector2Int lemCounts = new Vector2Int(nextMoment.savedLemmings.Count, nextMoment.deadLemmings.Count);
        //Go one past the last moment
        if ((lemCounts.x > 0) || (lemCounts.y > 0)) changed = true;
        nextMoment.lemmingCount += lemCounts;
        nextMoment.savedLemmings = new List<Lemming>();
        nextMoment.deadLemmings = new List<Lemming>();


        foreach (Lemming lem in nextMoment.lemmings)
        {
            Vector3Int pos = lem.position;

            Vector3Int forwardIndex = lem.position + lem.direction;
            Vector3Int belowIndex = new Vector3Int(pos.x, pos.y - 1, pos.z);            

            GameBlock current = null;
            GameBlock move = null;
            GameBlock below = null;
            
            current = GetBlock(nextMoment, pos);

            changed |= true;


            move = GetMoveBlock(nextMoment, forwardIndex, belowIndex);
            below = GetBlock(nextMoment, belowIndex);
            
            if(move != null) move.enterMoveBehaviour.Move(nextMoment, lem, move);
            if(below != null) below.overMoveBehaviour.Move(nextMoment, lem, below);
            current.exitMoveBehaviour.Move(nextMoment, lem, below);

            GameBlock onBlock = GetBlock(nextMoment, lem.position);

            if (onBlock.key == GameBoardCubeDictionary.END_ZONE)
            {
                nextMoment.savedLemmings.Add(lem);
            }
            else if( CheckDead(lem) )
            {
                nextMoment.deadLemmings.Add(lem);
            }
            else
            {
                curLemming.Add(lem);
            }

        }

        if(changed)
        {
            nextMoment.lemmings = curLemming;
        }

        //If a new moment was created, add it to the timeline, call event
        if (changed && (MomentBuiltEvent != null))
        {
            int i = timeline.Count;
            timeline.Add(nextMoment);
            MomentBuiltEvent(new Vector2Int(0,i));
        }

        updating = changed;
    }

    private bool CheckDead(Lemming lem)
    {
        //If lemming is falling off the map it's dead
        if (lem.position.y == 0) return true;

        //If lemming is 25 seconds old it dies
        if (lem.age-- == 0) return true;

        return false;
    }

    private bool InBounds(LevelMoment mom, Vector3Int pos)
    {
        if ((pos.x < 0) || (pos.y < 0) || (pos.z < 0)) return false;

        Vector3Int upper = new Vector3Int(mom.x, mom.y, mom.z);
        upper -= pos;
        if ((upper.x <= 0) || (upper.y <= 0) || (upper.z <= 0)) return false;

        return true;
    }

    private GameBlock GetMoveBlock(LevelMoment moment, Vector3Int dPos, Vector3Int fPos)
    {
        GameBlock result = null;

        //Check below for 
        result = GetBlock(moment, dPos);

        if (!(result.enterMoveBehaviour is BlockedIntoMoveBehaviour)) return result;

        result = GetBlock(moment, fPos);

        return result;
    }

    private GameBlock GetBlock(LevelMoment moment, Vector3Int pos)
    {

        GameBlockBuilder blockBuilder = new GameBlockBuilder();

        //Count oob as a normal block
        if (!InBounds(moment, pos))
        {
            return blockBuilder.Build(GameBoardCubeDictionary.NORMAL_CUBE, pos);
        }

        GameBlock result = blockBuilder.Build(moment.level[pos.x, pos.y, pos.z], pos);
        return result;
    }
}