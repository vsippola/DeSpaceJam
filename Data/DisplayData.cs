using UnityEngine;

public class DisplayData : MonoBehaviour
{
    public Transform translationTransform;
    public Transform rotationTransform;
    public Transform centeringTransform;

    public Vector3 targetTranslation;
    public float targetRotation;

    public Vector2Int coord;

    public bool translating;
    public bool rotating;

    public MomentDisplayBuilder builder;

    private void Awake()
    {       
        targetTranslation = Vector3.zero;

        targetRotation = 0f;

        translating = false;
        rotating = false;
    }

    public void SetTranslation(/*Vector3 target*/)
    {
        //targetTranslation = target;
        translating = true;
    }

    public void SetRotation(float degrees)
    {
        targetRotation = degrees;
        rotating = true;
    }

    public void ChangeRotation(float degrees)
    {
        targetRotation += degrees;
        rotating = true;
    }

    public void ChangePosition(Vector3 change)
    {
        targetTranslation += change;
        translating = true;
    }
}
