using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerBackToMenu : MonoBehaviour
{

    private Animator foreground;
    private ControllerForeground contFore;
    private DataTransition transData;

    private void OnEnable()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("ForegroundUI");
        foreground = obj.GetComponentInChildren<Animator>();
        contFore = obj.GetComponentInChildren<ControllerForeground>();

        contFore.FadeInEvent += BackToMenu;
        foreground.SetTrigger("FadeOut");

        obj = GameObject.FindGameObjectWithTag("TransitionData");
        transData = obj.GetComponentInChildren<DataTransition>();
    }

    private void BackToMenu()
    {
        contFore.FadeInEvent -= BackToMenu;

        UpdateData();
        SaveData();
        GotoMenu();
    }

    private void GotoMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateData()
    {
        SingletonJsonLoadable<DataGameDataPairs>.Instance.data.lastLevel = transData.nextLevelPath;
    }

    private void SaveData()
    {
        SingletonJsonLoadable<DataGameDataPairs>.Instance.Save();
        SingletonJsonLoadable<LevelsGameDataPairs>.Instance.Save();
        SingletonJsonLoadable<SettingsGameDataPairs>.Instance.Save();
    }
}
