using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSwapButton : MonoBehaviour
{
    public GameObject main;
    public GameObject swap;

    private Button button;

    private Animator foreground;
    private ControllerForeground contFore;


    void Start ()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(() => OnClick());

        GameObject obj = GameObject.FindGameObjectWithTag("ForegroundUI");
        foreground = obj.GetComponentInChildren<Animator>();
        contFore = obj.GetComponentInChildren<ControllerForeground>();
    }
	
    private void OnClick()
    {
        contFore.FadeInEvent += LoadSettings;       
        foreground.SetTrigger("FadeOut");
    }

    private void LoadSettings()
    {
        contFore.FadeInEvent -= LoadSettings;
        if(main != null) main.SetActive(false);
        if(swap != null) swap.SetActive(true);
        foreground.SetTrigger("FadeIn");        
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
