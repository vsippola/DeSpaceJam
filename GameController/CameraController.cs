using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Range(1f, 5f)]
    public float zoomMin = 3f;
    [Range(5f, 150f)]
    public float zoomMax = 100f;
    [Range(1f, 10f)]
    public float zoomStep = 1f;
    [Range(1f, 25f)]
    public float smoothing = 25f;
    [Range(0f, 1f)]
    public float zoomDeadzone = 1f;


    private float targetZoom;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        targetZoom = _camera.orthographicSize;
    }

    // Update is called once per frame
    void Update ()
    {
        CheckInput();
        MoveCamera();
	}

    //Lerp Zoom to target
    private void MoveCamera()
    {
        float currentZoom = _camera.orthographicSize;

        if (Mathf.Abs(currentZoom - targetZoom) < zoomDeadzone) return;

        _camera.orthographicSize = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * smoothing);
    }

    private void CheckInput()
    {
        //Check if the mouse wheel has been scrolled
        float zoomChange = Input.GetAxisRaw("Mouse ScrollWheel");
        if (zoomChange == 0) return;

        //If the wheel is scrolled move the target
        targetZoom -= zoomChange * zoomStep;
        targetZoom = Mathf.Clamp(targetZoom, zoomMin, zoomMax);
    }
}
