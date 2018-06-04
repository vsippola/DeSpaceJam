using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudController : MonoBehaviour
{
    public MomentDisplayController momentDisplayController;
    public AliveCountUpdater aliveLemCount;
    public DeadCountUpdater deadLemCount;


    private void Start()
    {
        momentDisplayController.MomentDisplayChangeEvent += aliveLemCount.OnUpdate;
        momentDisplayController.MomentDisplayChangeEvent += deadLemCount.OnUpdate;
    }


}
