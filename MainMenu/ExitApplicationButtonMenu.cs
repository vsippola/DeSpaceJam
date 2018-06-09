using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitApplicationButtonMenu : MonoBehaviour
{
    private Button button;

	void Start ()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(() => ExitApplication());
	}
	
    private void ExitApplication()
    {
        GameObject exitCont = GameObject.FindGameObjectWithTag("GameExitController");
        exitCont.GetComponentInChildren<ControllerExitGame>().enabled = true;       
    }
}
