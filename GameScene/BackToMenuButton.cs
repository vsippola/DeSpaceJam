using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BackToMenuButton : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(() => BackToMenu());
    }

    private void BackToMenu()
    {
        GameObject backCont = GameObject.FindGameObjectWithTag("BackToMenuController");
        backCont.GetComponentInChildren<ControllerBackToMenu>().enabled = true;
    }
}

