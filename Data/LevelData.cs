using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    private static LevelData instance = null;
    private static readonly object padlock = new object();

    public string levelName = "level0";
    public string levelPath = "level0.json";

    public LevelMoment startingMoment;
    public List<List<LevelMoment>> moments;

    private LevelData()
    {

        startingMoment = LevelLoadFromFile.LoadFromFile(levelPath);
        moments = new List<List<LevelMoment>>();

        List<LevelMoment> startingTimeline = new List<LevelMoment>();
        startingTimeline.Add(startingMoment.DeepCopyLevelMoment());
        moments.Add(startingTimeline);

    }

    public static LevelData Instance
    {
        get
        {
            if (instance == null)
            {
                lock(padlock)
                {
                    if(instance == null)
                    {
                        instance = new LevelData();
                    }
                }
                
            }
            return instance;
        }
        
    }
}
