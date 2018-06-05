using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightColorController : MonoBehaviour
{
    private Light dLight;
    public MomentDisplayController displayController;

    private Color regColor;
    public Color winColor;
    public Color failColor;

    private void Start()
    {
        dLight = transform.GetComponent<Light>();
        regColor = dLight.color;

        displayController.MomentDisplayChangeEvent += OnUpdate;
    }

    public void OnUpdate(LevelMoment moment)
    {
        //If the player won
        if(moment.IsWin()) dLight.color = winColor;
        //If the player can't win in this timeline
        else if (moment.IsLose()) dLight.color = failColor;
        else dLight.color = regColor;
    }

}
