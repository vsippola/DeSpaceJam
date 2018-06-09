using UnityEngine;
using System.Collections.Generic;

public class LevelStartController : MonoBehaviour {

    void Start ()
    {       
        SetupCamera();
        SetupApplicationSettings();

    }

    private void InitializeSingletons()
    {
        LevelData instance = LevelData.Instance;                
    }

    private void SetupApplicationSettings()
    {
        Application.targetFrameRate = 60;
    }

	private void SetupCamera()
    {
        Camera.main.transform.LookAt(new Vector3(0f, 0f, 0f));
    }
}
