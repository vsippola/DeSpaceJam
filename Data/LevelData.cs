using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public delegate void TimeLineCreated(Vector2Int i);
    public event TimeLineCreated TimeLineCreatedEvent;

    private static LevelData instance = null;
    private static readonly object padlock = new object();

    public string levelName = "level0";
    public string levelPath = "level0.json";

    public LevelMoment startingMoment;
    public List<List<LevelMoment>> moments;

    private MomentDisplay momentDisplay;

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

        List<List<LevelMoment>> newMoments = new List<List<LevelMoment>>();

        for(int k = 0; k < i.x; k++)
        {
            newMoments.Add(moments[k]);
        }

        newMoments.Add(timeline);

        moments = newMoments;
    }

    public void CheckPast(LevelMoment moment)
    {
        if (moment.phase == 0) return;

        int p = moment.phase;

        List<LevelMoment> present = moments[p];
        List<LevelMoment> past = moments[p - 1];

        if (present.Count < past.Count) return;

        LevelMoment terminalMoment = past[past.Count - 1];

        for (int i = past.Count; i <= present.Count; i++)
        {          
            past.Add(terminalMoment);
        }

        TimeLineCreatedEvent(new Vector2Int(p - 1, past.Count));

        //Proceed recursively
        terminalMoment = past[past.Count - 1];
        CheckPast(terminalMoment);
    }

    public LevelMoment GetMoment(int phase, int time)
    {
        if( (phase < 0) || (moments.Count <= phase) 
            || (time < 0) || (moments[phase].Count <= time) ) return null;

        return moments[phase][time];
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

            newStart.PhaseInLemmings();
    
            newStart.RecieveTimeTraveler();
            timeline.Add(newStart);

            moments.Add(timeline);

            Vector2Int j = new Vector2Int( i.x + 1, 0);
            if (TimeLineCreatedEvent != null) TimeLineCreatedEvent(j);
            else AddToEvent(j);
        }
    }

    private void AddToEvent(Vector2Int j)
    {
        if(momentDisplay == null)
        {
            GameObject display = GameObject.FindGameObjectWithTag("MomentDisplay");
            momentDisplay = display.GetComponent<MomentDisplay>();
        }

        TimeLineCreatedEvent += momentDisplay.OnMomentBuildEvent;

        if (TimeLineCreatedEvent != null) TimeLineCreatedEvent(j);
    }
}

