using UnityEngine;

public class ControllerExitGame : MonoBehaviour
{    
    private Animator foreground;
    private ControllerForeground contFore;

    private void OnEnable()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("ForegroundUI");
        foreground = obj.GetComponentInChildren<Animator>();
        contFore = obj.GetComponentInChildren<ControllerForeground>();

        contFore.FadeInEvent += EndApplication;
        foreground.SetTrigger("FadeOut");
    }

    private void EndApplication()
    {
        contFore.FadeInEvent -= EndApplication;

        SaveData();        

        Application.Quit();
    }

    private void SaveData()
    {
        SingletonJsonLoadable<DataGameDataPairs>.Instance.Save();
        SingletonJsonLoadable<LevelsGameDataPairs>.Instance.Save();
        SingletonJsonLoadable<SettingsGameDataPairs>.Instance.Save();
    }
}
