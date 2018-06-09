using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class StartGameplayButtonMenu : MonoBehaviour
{
    public string path;

    private Button button;

    private Animator foreground;
    private ControllerForeground contFore;
    private DataTransition dataTransition;

    private void Awake()
    {
        if(SingletonJsonLoadable<DataGameDataPairs>.Instance.data != null)
        path = SingletonJsonLoadable<DataGameDataPairs>.Instance.data.firstLevel;
    }

    private void Start()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(() => OnButtonPress());

        GameObject obj = GameObject.FindGameObjectWithTag("ForegroundUI");
        foreground = obj.GetComponentInChildren<Animator>();
        contFore = obj.GetComponentInChildren<ControllerForeground>();

        obj = GameObject.FindGameObjectWithTag("TransitionData");
        dataTransition = obj.GetComponentInChildren<DataTransition>();
    }

    private void OnButtonPress()
    {
        contFore.FadeInEvent += StartGameplay;
        foreground.SetTrigger("FadeOut");               
    }

    private void StartGameplay()
    {
        contFore.FadeInEvent -= StartGameplay;
        dataTransition.nextLevelPath = path;
        SceneManager.LoadSceneAsync("Gameplay");
    }

}
