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

    public void ChangeFuture(LevelMoment moment)
    {
        Vector2Int i = new Vector2Int(moment.phase, moment.time);

        List<LevelMoment> timeline = new List<LevelMoment>();
        for(int j = 0; j <= i.y; j++)
        {
            timeline.Add(moments[i.x][j]);
        }

        moments[i.x] = timeline;
    }

    public void WormHole(LevelMoment moment)
    {
        Vector2Int i = moment.GetIndex();

        if (moments.Count < (i.x + 2) )
        {
            List<LevelMoment> timeline = new List<LevelMoment>();
            List<LevelMoment> prevTimeline = moments[i.x];

            LevelMoment prevStart = LevelData.Instance.startingMoment;
            LevelMoment newStart = prevStart.DeepCopyLevelMoment();

            newStart.phase++;
            newStart.RecieveTimeTraveler();
            timeline.Add(newStart);

            moments.Add(timeline);
        }
    }
}
