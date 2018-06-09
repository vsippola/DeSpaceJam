using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerLevelLoad : MonoBehaviour
{
    private readonly string levelPath = "LevelData/";

    private DataTransition transitionData;

    private GameObject globalMusic;
    private GameObject momentDisplayCont;

    private Animator foreground;
    private ControllerForeground contFore;

    public GameObject[] startObjs;

    private bool firstUpdate = false;

    private void AssignValues()
    {
        transitionData = GameObject.FindGameObjectWithTag("TransitionData")
                                        .GetComponentInChildren<DataTransition>();

        globalMusic = GameObject.FindGameObjectWithTag("GlobalMusic");
        momentDisplayCont = GameObject.FindGameObjectWithTag("MomentDisplayController");

        GameObject obj = GameObject.FindGameObjectWithTag("ForegroundUI");
        if(obj != null)
        {
            foreground = obj.GetComponentInChildren<Animator>();
            contFore = obj.GetComponentInChildren<ControllerForeground>();
        }        
    }

    private void OnEnable()
    {
        
        
        LoadLevelData();
        StartObjects();
        firstUpdate = true;

        
    }

    private void Update()
    {
        if (!firstUpdate) return;
        AssignValues();
        HookMusicController();

        if (foreground != null) foreground.SetTrigger("FadeIn");
        enabled = false;
    }

    private void StartObjects()
    {
       foreach(GameObject obj in startObjs)
        {
            obj.SetActive(true);
        }
    }

    private void HookMusicController()
    {
        if(globalMusic == null) AssignValues();
        MusicControllers musicCont = globalMusic.GetComponentInChildren<MusicControllers>();
        musicCont.displayController = momentDisplayCont.GetComponentInChildren<MomentDisplayController>();
        musicCont.enabled = true;
    }

    private void LoadLevelData()
    {
        LevelData.Instance.LoadData();
    }
}
