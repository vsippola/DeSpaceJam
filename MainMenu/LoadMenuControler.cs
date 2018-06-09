using UnityEngine;
using UnityEngine.UI;

public class LoadMenuControler : MonoBehaviour
{
    public GameObject contine;
    public GameObject start;
    public GameObject load;

    private Animator foreground;

    void Start ()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("ForegroundUI");
        foreground = obj.GetComponentInChildren<Animator>();

        ActivateButtons();
        DoneLoading();
    }

    private void DoneLoading()
    {
        if (foreground != null) foreground.SetTrigger("FadeIn");
    }

    private void ActivateButtons()
    {
        start.SetActive(true);
        //load.SetActive(true);

        SingletonJsonLoadable<DataGameDataPairs> gameData = SingletonJsonLoadable<DataGameDataPairs>.Instance;

        string path = gameData.data.lastLevel;

        if(path != "")
        {
            contine.SetActive(true);
            contine.GetComponent<StartGameplayButtonMenu>().path = gameData.data.lastLevel;
        }
    
    }
}
