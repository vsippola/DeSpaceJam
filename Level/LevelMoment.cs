using System.Collections.Generic;
using System;
using UnityEngine;

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

    public int  x, y, z;
    public string levelAsString;

    public string[] spawners;
    public List<Lemming> lemmings = new List<Lemming>();
    public List<Lemming> savedLemmings = new List<Lemming>();
    public List<Lemming> deadLemmings = new List<Lemming>();
    public List<Lemming> phasedLemmings = new List<Lemming>();

    public Vector2Int lemmingCount = Vector2Int.zero;

    public char[,,] level;


    public void BuildLevel()
    {      
        LevelConsistencyCheck();

        LevelAsStringToArray();        

    }

    private void LevelAsStringToArray()
    {
        level = new char[x, y, z];

        int currentIndex = 0;
        char currentChar;

        for (int j = 0; j < y; j++)
        {
            for (int k = 0; k < z; k++)
            {
                for (int i = 0; i < x; i++)
                {
                    //load next char, load a blank square if the string is too short for the given bounds
                    if (currentIndex < levelAsString.Length) currentChar = levelAsString[currentIndex++];
                    else currentChar = DEFAULT_CHAR;                   

                    level[i, j, k] = currentChar;
                }
            }
        }
    }

    private void LevelConsistencyCheck()
    {
        int size = x * y * z;
        
        if (size != levelAsString.Length)
        {
            Debug.Log("Warning level size (" + size + ") does not match level string length ("+levelAsString.Length+")");

            if(size < levelAsString.Length) Debug.Log("Truncating level string");
            else Debug.Log("Padding level with "+ DEFAULT_CHAR +" = blank square");
        }
    }

    public LevelMoment DeepCopyLevelMoment()
    {
        LevelMoment copy = new LevelMoment
        {
            x = this.x,
            y = this.y,
            z = this.z,

            lemmingCount = this.lemmingCount,

            //Shallow copy, but of type char. So the same as a deep copy. Change if switched to pointers please.
            level = (char[,,])this.level.Clone()
        };

        copy.lemmings = new List<Lemming>();

        foreach(Lemming lem in lemmings)
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

        return copy;
    }

    public void LogLevel()
    {
        if(level == null)
        {
            Debug.Log("Level not loaded.");
            return;
        }

        string log = "";
        for (int k = 0; k < z; k++)
        {
            log += "Level " + k + "\n";

            string xySlice = "";
            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    xySlice += level[i, j, k];
                }

                xySlice += "\n";
            }

            log += xySlice;
        }

        Debug.Log(log);
    }

    //Spawners
    //List<Spawner> spawners;

   
}

