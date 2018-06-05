using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AliveCountUpdater : MonoBehaviour
{

    public Text text;

    private void Awake()
    {
        text.text = "";
    }

    private void Start()
    {
        text.text += LevelData.Instance.startingMoment.goal;
    }

    public void OnUpdate(LevelMoment moment)
    {
        text.text = "";
        if (moment.goal > 0) text.text += moment.goal;
    }

}

