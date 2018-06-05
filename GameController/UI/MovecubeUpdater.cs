using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovecubeUpdater : MonoBehaviour
{

    public Text text;
    public char key;

    GameObject button;

    private void Awake()
    {
        text.text = "";
        button = transform.GetChild(0).gameObject;       
    }

    private void Start()
    {
        LevelMoment moment = LevelData.Instance.moments[0][0];
        text.text += moment.MoveableCount(key);
    }

    public void OnUpdate(LevelMoment moment)
    {
        text.text = "";

        if (moment.goal > 0) text.text += moment.MoveableCount(key);

        button.SetActive(moment.MoveableCount(key) > 0);
    }
}
