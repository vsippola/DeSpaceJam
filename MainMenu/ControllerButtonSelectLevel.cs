using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class ControllerButtonSelectLevel : MonoBehaviour
{
    public string levelName = "Test Level";
    public string levelpath = "testlevel.json"; 

    private Button button;
    private Text text;

    private void Start()
    {
        button = GetComponentInChildren<Button>();
        text = GetComponentInChildren<Text>();

        text.text = levelName;
    }

}

