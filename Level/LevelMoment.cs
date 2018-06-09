using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class MoveTuple
{
    public string key;
    public int count;

    public MoveTuple(string key, int count)
    {
        this.key = key;
        this.count = count;
    }
}

[Serializable]
public class LevelMoment
{
    /**
     * Character -> Block Conversion
     * 
     * 0 = ground
     * 1-9 = groud crumbles after # steps
     * a = open space
     * 
     * z = ending
     */

    //Constants
    private const char DEFAULT_CHAR = 'a';

    //Variables

    public int x, y, z;
    public List<MoveTuple> moveObj = new List<MoveTuple>();

    public List<Spawner> spawners = new List<Spawner>();
    public List<Lemming> lemmings = new List<Lemming>();
    public List<Lemming> savedLemmings = new List<Lemming>();
    public List<Lemming> deadLemmings = new List<Lemming>();
    public List<Lemming> phasedLemmings = new List<Lemming>();

    private Dictionary<char, int> moveables = new Dictionary<char, int>();

    public char[,,] level;

    Vector3Int wormholeOutPos;

    public int goal;
    public int fail;
    public int phase, time;

    public Lemming timeTraveler = new Lemming();

    public int terminalBuffer = 3;

    public void BuildLevel(string levelAsString)
    {
        wormholeOutPos.x = -1;
        LevelConsistencyCheck();

        LevelAsStringToArray(levelAsString);
        BuildMoveables();
    }

    private void BuildMoveables()
    {
        foreach(MoveTuple tup in moveObj)
        {
            char key = tup.key[0];
            moveables.Add(key, tup.count);
        }
    }

    private void LevelAsStringToArray(string levelAsString)
    {
        y++;
        level = new char[x, y, z];

        for (int k = 0; k < z; k++)
        {
            for (int i = 0; i < x; i++)
            {
 
                level[i, 0, k] = GameBoardCubeDictionary.DEATH_CUBE;

            }
        }

        int currentIndex = 0;
        char currentChar;

        for (int j = 1; j < y; j++)
        {
            for (int k = 0; k < z; k++)
            {
                for (int i = 0; i < x; i++)
                {
                    //load next char, load a blank square if the string is too short for the given bounds
                    while ((currentIndex < levelAsString.Length) && (levelAsString[currentIndex] == ' ')) currentIndex++;

                    if (currentIndex < levelAsString.Length) currentChar = levelAsString[currentIndex++];
                    else currentChar = DEFAULT_CHAR;

                    if((j > 1))
                    {
                        if(currentChar == GameBoardCubeDictionary.OPEN_SPACE)
                        {
                            if (level[i, j - 1, k] != GameBoardCubeDictionary.OPEN_SPACE)
                            {
                                currentChar = GameBoardCubeDictionary.OPEN_CUBE;
                            }
                        }                        
                    }

                    level[i, j, k] = currentChar;

                    if (currentChar == GameBoardCubeDictionary.WORMHOLE_OUT)
                        wormholeOutPos = new Vector3Int(i, j, k);
                }
            }
        }
    }

    private void LevelConsistencyCheck()
    {

    }

    public LevelMoment DeepCopyLevelMoment()
    {
        LevelMoment copy = new LevelMoment
        {
            x = this.x,
            y = this.y,
            z = this.z,

            goal = this.goal,
            fail = this.fail,
            time = this.time,
            phase = this.phase,
            terminalBuffer = this.terminalBuffer,
            wormholeOutPos = this.wormholeOutPos,
            //Shallow copy, but of type char. So the same as a deep copy. Change if switched to pointers please.
            level = (char[,,])this.level.Clone()

        };

        foreach (char key in moveables.Keys)
        {
            copy.moveables.Add(key, this.moveables[key]);
        }
        copy.lemmings = new List<Lemming>();

        foreach (Lemming lem in lemmings)
        {
            copy.lemmings.Add(lem.DeepCopy());
        }

        foreach (Lemming lem in savedLemmings)
        {
            copy.savedLemmings.Add(lem.DeepCopy());
        }

        foreach (Lemming lem in deadLemmings)
        {
            copy.deadLemmings.Add(lem.DeepCopy());
        }

        foreach (Lemming lem in phasedLemmings)
        {
            copy.phasedLemmings.Add(lem.DeepCopy());
        }

        foreach (Spawner spawn in spawners)
        {
            copy.spawners.Add(spawn.DeepCopy());
        }

        return copy;
    }

    public void LogLevel()
    {
        if (level == null)
        {
            Debug.Log("Level not loaded.");
            return;
        }

        string log = "";
        for (int k = 0; k < y; k++)
        {
            log += "Level " + k + "\n";

            string xySlice = "";
            for (int j = 0; j < z; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    xySlice += level[i, k, j];
                }

                xySlice += "\n";
            }

            log += xySlice;
        }

        Debug.Log(log);
    }

    public bool IsWin()
    {
        return (goal <= 0);
    }

    public bool IsLose()
    {
        return (fail <= 0);
    }

    public char CharAt(Vector3Int i)
    {
        foreach (Lemming lem in lemmings)
        {
            if (i == lem.position)
            {
                return GameBoardCubeDictionary.LEMMING;
            }
        }

        foreach (Lemming lem in phasedLemmings)
        {
            if (i == lem.position)
            {
                return GameBoardCubeDictionary.LEMMING_PHASED;
            }
        }
        
        return level[i.x, i.y, i.z];
    }

    public bool IsFree(Vector3Int i)
    {
        return (CharAt(i) == GameBoardCubeDictionary.OPEN_CUBE);
    }

    public bool Change(Vector3Int i, char key)
    {
        if (!moveables.ContainsKey(key)) return false;
        if (moveables[key] <= 0) return false;
        if (!IsFree(i)) return false;

        level[i.x, i.y, i.z] = key;
        moveables[key]--;

        return true;  
    }

    public bool Remove(Vector3Int i, char key)
    {
        if (!moveables.ContainsKey(key))
        {
            moveables.Add(key, 0);
        }
        if (level[i.x, i.y, i.z] != key) return false;

        level[i.x, i.y, i.z] = GameBoardCubeDictionary.OPEN_CUBE;
        moveables[key]++;

        return true;
    }

    public int MoveableCount(char key)
    {
        return (moveables.ContainsKey(key))?moveables[key]:0;
    }

    public Vector2Int GetIndex()
    {
        return new Vector2Int(this.phase, this.time);
    }

    public void Age()
    {
        time++;
    }

    private void WormHole(Lemming lem)
    {
        
        if (wormholeOutPos.x < 0) return;

        //IF something is blocking the exit the lemming dies.

        if (CharAt(wormholeOutPos) != GameBoardCubeDictionary.WORMHOLE_OUT) return;

        Lemming newLemming = lem.DeepCopy();

        //Flip directions
        newLemming.direction *= -1;
        newLemming.position = wormholeOutPos;

        phasedLemmings.Add(newLemming);
    }

    public bool LemmingInPhase(Lemming lem)
    {
        return (lem.phase == this.phase);
    }

    public void PhaseInLemmings()
    {
        foreach(Lemming lem in lemmings)
        {
            lem.phase = this.phase;
        }

        foreach(Spawner spawn in spawners)
        {
            spawn.phase = this.phase;
        }
    }

    public void RecieveTimeTraveler()
    {        
        if (phase == 0) return;

        List<LevelMoment> prevTimeline = LevelData.Instance.moments[phase - 1];

        //Check if there can be a time traveler.
        if ((prevTimeline.Count - time) < 3) return;

        LevelMoment wormHole = prevTimeline[time + 2];
        
        if (!wormHole.HasTimeTraveler()) return;

        WormHole(wormHole.timeTraveler);
    }

    public bool CanChange()
    {
        if ((lemmings.Count > 0) || (phasedLemmings.Count > 0)) return true;
        if (spawners.Count > 0) return true;

        if(phase > 0)
        {
            List<LevelMoment> prevTimeline = LevelData.Instance.moments[phase - 1];
            if (time < (prevTimeline.Count - 1)) return true;
        }

        if (terminalBuffer > 0)
        {
            terminalBuffer--;
            return true;
        }

        return false;
    }

    public bool HasTimeTraveler()
    {
        return (timeTraveler.position.x >= 0);
    }
}

