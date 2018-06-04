
using UnityEngine;

public class InputControllerManager : MonoBehaviour
{
    public CameraController cameraController;
    public MomentDisplayController momentDisplayController;

    private bool isRotating = false;

    private void Start()
    {
        momentDisplayController.SetInputCheck(isRotating);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) ToggleMouseRotate();
        else if (Input.GetMouseButtonUp(1)) ToggleMouseRotate();
    }

    private void ToggleMouseRotate()
    {
        isRotating = !isRotating;

        Cursor.visible = !isRotating;
        Cursor.lockState = isRotating ? CursorLockMode.Locked : CursorLockMode.None;

        momentDisplayController.SetInputCheck(isRotating);
    }


    public void Pause()
    {
        SetGamePause(true);
    }

    public void UnPause()
    {
        SetGamePause(false);
    }

    private void SetGamePause(bool isPaused)
    {
        cameraController.enabled = isPaused;
        momentDisplayController.SetInputCheck(isRotating);
    }
}
