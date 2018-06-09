using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerWinMenu : MonoBehaviour
{
    public GameLogicController logicCont;
    public GameObject winMenu;
	    
	void Start ()
    {
        logicCont.GameWinEvent += OnWin;
	}

    private void OnWin()
    {
        logicCont.GameWinEvent -= OnWin;
        winMenu.SetActive(true);
    }
}
