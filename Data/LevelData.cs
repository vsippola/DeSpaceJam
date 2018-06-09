using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public delegate void TimeLineCreated(Vector2Int i);
    public event TimeLineCreated TimeLineCreatedEvent;

    private static LevelData instance = null;
    private static readonly object padlock = new object();

    public LevelMoment startingMoment;
    public List<List<LevelMoment>> moments;

    private MomentDisplay momentDisplay;
    private static DataTransition transData;

    private SingletonJsonLoadable<ConfigurationLevelDataPair> levelConfig;

    private LevelData()
    {
              
    }

    public void LoadData()
    {
        if (transData == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("TransitionData");
            transData = obj.GetComponentInChildren<DataTransition>();
        }

        if (levelConfig == null)
        {
            levelConfig = SingletonJsonLoadable<ConfigurationLevelDataPair>.Instance;
        }

        levelConfig.Configure("LevelData/" + transData.nextLevelPath, "LevelData/" + transData.nextLevelPath);
        levelConfig.Load();

        startingMoment = levelConfig.data.startingMoment;
        startingMoment.BuildLevel(levelConfig.data.levelAsString);

        moments = new List<List<LevelMoment>>();

        List<LevelMoment> startingTimeline = new List<LevelMoment>
        {
            startingMoment.DeepCopyLevelMoment()
        };
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

        if (present.Count <= past.Count) return;

        LevelMoment terminalMoment = past[past.Count - 1];

        int k = Mathf.Abs(present.Count - past.Count);
        for (int i = 0; i < k; i++)
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

            newStart.phase = i.x + 1;

            newStart.PhaseInLemmings();
    
            for(int l = 0; l < newStart.y; l++)
            {
                for(int k = 0; k < newStart.z; k++)
                {
                    for(int m = 0; m < newStart.x; m++)
                    {
                        char key = newStart.level[m, l, k];
                        if(key == GameBoardCubeDictionary.WORMHOLE_IN)
                        {
                            newStart.Remove(new Vector3Int(m, l, k), key);
                        }
                    }
                }
            }

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

