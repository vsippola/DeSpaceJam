using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLogicController : MonoBehaviour
{
    public delegate void MomentBuilt(Vector2Int i);
    public event MomentBuilt MomentBuiltEvent;

    public delegate void OnGameWinEvent();
    public event OnGameWinEvent GameWinEvent;

    private LevelData levelData;

    public bool isWin = false;
    public Vector2Int winIndex;

    public bool updating;

    [Range(0f, 10f)]
    public float timeBetweenUpdates;
    private float updateTimer;

    [Range(1f, 10f)]
    public float winCheckDelay = 5f;

    private float startDelay = 2f;

    private void Awake()
    {
        updateTimer = timeBetweenUpdates;
        levelData = LevelData.Instance;
        updating = true;
        CheckBuilding();
    }
	
	void Update ()
    {
        if (isWin) return;
        winCheckDelay -= Time.deltaTime;
        CheckWin();

        if (!updating) return;

        startDelay -= Time.deltaTime;
        if (startDelay > 0) return;
        updateTimer -= Time.deltaTime;

        if (updateTimer > 0) return;

        updateTimer = timeBetweenUpdates;
        CheckBuilding();
	}

    private void CheckBuilding()
    {
        //if (!Input.GetKeyDown(KeyCode.N)) return;        

        if (isWin) return;
        bool changed = false;

        //Find next map to update. (for not just go linearly)        
        LevelMoment prevMoment = FindOldMoment();
        if (prevMoment == null)
        {
            updating = false;
            return;
        }

        
        LevelMoment nextMoment = prevMoment.DeepCopyLevelMoment();
        if(prevMoment.terminalBuffer == 0) prevMoment.terminalBuffer++;

        nextMoment.Age();

        //Keep track of saved and dead lemming count
        Vector2Int lemCounts = new Vector2Int(nextMoment.savedLemmings.Count, nextMoment.deadLemmings.Count);

        //Go one past the last moment
        if ((lemCounts.x > 0) || (lemCounts.y > 0))
        {
            changed = true;
        }
        
        nextMoment.goal -= lemCounts.x;
        nextMoment.fail -= lemCounts.y;

        List<Lemming> movingLemmings = new List<Lemming>();
        movingLemmings.AddRange(nextMoment.lemmings);
        movingLemmings.AddRange(nextMoment.phasedLemmings);

        nextMoment.lemmings = new List<Lemming>();
        nextMoment.savedLemmings = new List<Lemming>();
        nextMoment.deadLemmings = new List<Lemming>();
        nextMoment.phasedLemmings = new List<Lemming>();

        

        foreach (Lemming lem in movingLemmings)
        {
            Vector3Int pos = lem.position;

            Vector3Int forwardIndex = lem.position + lem.direction;
            Vector3Int belowIndex = new Vector3Int(pos.x, pos.y - 1, pos.z);            

            GameBlock current = null;
            GameBlock move = null;
            GameBlock below = null;
            
            current = GetBlock(prevMoment, pos);

            changed |= true;

            move = GetBlock(prevMoment, forwardIndex);
            below = GetBlock(prevMoment, belowIndex);

            Vector3Int prevPos = lem.position;
            if(below != null) below.overMoveBehaviour.Move(nextMoment, prevMoment, lem, below);
            if((move != null)&&(prevPos == lem.position)) move.enterMoveBehaviour.Move(nextMoment, prevMoment, lem, move);
            current.exitMoveBehaviour.Move(nextMoment, prevMoment, lem, below);

            Vector3Int lPos = lem.position; ;
            char key = nextMoment.level[lPos.x, lPos.y, lPos.z];           

            if (key == GameBoardCubeDictionary.END_ZONE)
            {
                nextMoment.savedLemmings.Add(lem);
            }
            else if( CheckDead(lem) )
            {
                if (nextMoment.LemmingInPhase(lem)) nextMoment.deadLemmings.Add(lem);
            }
            else
            {
                if (nextMoment.LemmingInPhase(lem)) nextMoment.lemmings.Add(lem);
                else nextMoment.phasedLemmings.Add(lem);
            }

        }

        if(changed)
        {
            if (nextMoment.HasTimeTraveler()) nextMoment.lemmings.Remove(nextMoment.timeTraveler);
           
        }

        ProcessSpawners(nextMoment);
        nextMoment.RecieveTimeTraveler();

        LevelData.Instance.CheckPast(nextMoment);

        //If a new moment was created, add it to the timeline, call event

        if ((MomentBuiltEvent != null))
        {
            List<LevelMoment> timeline = levelData.moments[nextMoment.phase];
            Vector2Int i = new Vector2Int(nextMoment.phase, nextMoment.time);
            timeline.Add(nextMoment);
            MomentBuiltEvent(i);
        }        
    }

    private void CheckWin()
    {

        if (winCheckDelay > 0) return;

        List<List<LevelMoment>> moments = LevelData.Instance.moments;
        for (int j = 0; j < moments.Count; j++)
        {
            int i = moments[j].Count - 1;
            bool check = moments[j][i].IsWin();

            if (check)
            {                
                while( (i > 1) && (moments[j][i-1].IsWin()))
                {
                    i--;
                }

                string newPath = SingletonJsonLoadable<ConfigurationLevelDataPair>.Instance.data.nextLevelPath;
                SingletonJsonLoadable<DataGameDataPairs>.Instance.data.lastLevel = newPath;
                SingletonJsonLoadable<DataGameDataPairs>.Instance.Save();

                GameObject obj = GameObject.FindGameObjectWithTag("TransitionData");
                obj.GetComponentInChildren<DataTransition>().nextLevelPath = newPath;

                isWin = true;
                winIndex = new Vector2Int(i, j);
                if (GameWinEvent != null) GameWinEvent();
                return;
            }
        }
    }

    private bool ProcessSpawners(LevelMoment mom)
    {
        bool result = false;

        List<Spawner> spawners = new List<Spawner>();
        spawners.AddRange(mom.spawners);

        foreach(Spawner spawn in spawners)
        {
            spawn.age--;

            if (spawn.age == 0) mom.spawners.Remove(spawn);
            if (spawn.age % 2 != 0) continue;

            Lemming lem = spawn.MakeLemming();
            if (mom.IsFree(lem.position)) mom.lemmings.Add(lem);
        }

        return result;
    }

    private LevelMoment FindOldMoment()
    {
        foreach(List<LevelMoment> timeline in levelData.moments)
        {
            LevelMoment terminalMoment = timeline[timeline.Count - 1];
            if (terminalMoment.CanChange())
            {
                if((terminalMoment.time == 0)&&(terminalMoment.phase > 0))
                {
                    terminalMoment.RecieveTimeTraveler();
                }
                return terminalMoment;
            }
        }

        return null;
    }

    private bool CheckDead(Lemming lem)
    {
        //If lemming is falling off the map it's dead
        if (lem.position.y == 0) return true;

        //If lemming is 25 seconds old it dies
        if (lem.age-- <= 0) return true;

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

        GameBlock result = blockBuilder.Build(moment.CharAt(pos), pos);
        return result;
    }
}
