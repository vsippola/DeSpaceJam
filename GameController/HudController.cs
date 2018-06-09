using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudController : MonoBehaviour
{
    public delegate void RedrawHudEvent(LevelMoment moment);
    event RedrawHudEvent OnRedrawHudEvent;

    public MomentDisplayController momentDisplayController;

    public AliveCountUpdater aliveLemCount;
    public DeadCountUpdater deadLemCount;
    public MovecubeUpdater[] moveCubeUpdaters;

    public GameLogicController logicCont;
    public GameObject HUD;

    private void Start()
    {
        momentDisplayController.MomentDisplayChangeEvent += aliveLemCount.OnUpdate;
        OnRedrawHudEvent += aliveLemCount.OnUpdate;

        momentDisplayController.MomentDisplayChangeEvent += deadLemCount.OnUpdate;
        OnRedrawHudEvent += deadLemCount.OnUpdate;
        foreach(MovecubeUpdater move in moveCubeUpdaters)
        {
            momentDisplayController.MomentDisplayChangeEvent += move.OnUpdate;
            OnRedrawHudEvent += move.OnUpdate;
        }

        OnRedrawHudEvent(LevelData.Instance.startingMoment);

        logicCont.GameWinEvent += OnWin;
    }

    public void RedrawHud(LevelMoment moment)
    {

        if (OnRedrawHudEvent != null) OnRedrawHudEvent(moment);
    }

    private void OnWin()
    {
        HUD.SetActive(false);
        enabled = false;
    }
}
