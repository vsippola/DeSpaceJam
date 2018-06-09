using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerLevelNext : MonoBehaviour
{
    public GameObject parent;

    private void OnEnable()
    {
        StartGameplayButtonMenu script = GetComponentInChildren<StartGameplayButtonMenu>();
        string path = SingletonJsonLoadable<ConfigurationLevelDataPair>.Instance.data.nextLevelPath;

        if (path == "")
        {
            parent.SetActive(false);
            return;
        }


        script.enabled = true;
        script.path = path;
    }

}

