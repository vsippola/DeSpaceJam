using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicManager : MonoBehaviour
{
    public GameLogicController logic;
    public MomentDisplay displayBuilder;

    private void Start()
    {
        logic.MomentBuiltEvent += displayBuilder.OnMomentBuildEvent;
    }
}
