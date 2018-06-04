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

    public void OnUpdate(LevelMoment moment)
    {
        text.text = "";
        if (moment.lemmingCount.x > 0) text.text += moment.lemmingCount.x;
    }

}

