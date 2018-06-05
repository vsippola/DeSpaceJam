using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveItemController : MonoBehaviour
{       
    public GameObjectPooler pool;

    public GameLogicController logicController;
    public MomentDisplay momDisplay;
    public HudController hudControl;

    public Color highlight;

    char key;
    Ray ray;
    RaycastHit hit;

    public Color oldColor;
    Transform oldTrans;

    Transform overTrans;
    Vector3Int overI;

    float startUpDelay = 0f;

    void Start()
    {
        overTrans = null;
    }

    void Update()
    {
        startUpDelay -= Time.deltaTime;
        if (startUpDelay > 0) return;

        CheckDisplayUpdate();
        CheckPlacement();
    }

    private void OnEnable()
    {
        startUpDelay = .5f;
    }

    private void CheckPlacement()
    {
        //Check if the user is trying to place the object/cancel out.
        if (Input.GetButtonDown("Fire1"))
        {
            //If the object is active, place it
            if (oldTrans != null)
            {
                RemoveObject();
            }
        }
    }

    private void CheckDisplayUpdate()
    {
        //See if the mouse is over any point
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Vector2Int i = momDisplay.displayCenter;
            Transform centerDisplay = momDisplay.displays[i.x][i.y].transform;

            Transform t = hit.collider.transform;

            //Only place the new object in the centre display
            if (!t.IsChildOf(centerDisplay)) return;

            //Check that the square in the game space is free
            LevelMoment moment = momDisplay.GetFocusedMoment();

            Vector3Int j = new Vector3Int(Mathf.RoundToInt(t.localPosition.x),
                Mathf.RoundToInt(t.localPosition.y),
                Mathf.RoundToInt(t.localPosition.z));


            key = moment.level[j.x, j.y, j.z];

            if (IsRemoveable(key))
            {
                overI = j;
                overTrans = t;

                Highlight();
            }
            else
            {
                UnHighlight();
            }
        }
        else
        {
            UnHighlight();
        }
    }

    private void Highlight()
    {
        if (overTrans == null) return;
        if (overTrans == oldTrans) return;
        UnHighlight();

        Renderer rend = overTrans.GetComponentInChildren<Renderer>();

        if (rend == null) return;

        oldColor = rend.material.color;
        oldTrans = overTrans;

        rend.material.color = highlight;
    }

    private void UnHighlight()
    {
        if (oldTrans == null) return;

        Renderer rend = oldTrans.GetComponent<Renderer>();
        oldTrans = null;

        if (rend == null) return;

        rend.material.color = oldColor;

        overTrans = null;
    }

    private bool IsRemoveable(char key)
    {
        if (key == GameBoardCubeDictionary.MOVE_CUBE) return true;
        if (key == GameBoardCubeDictionary.WORMHOLE_IN) return true;
        return false;
    }


    private void DisableController()
    {
        overTrans = null;
        this.enabled = false;
    }

    private void RemoveObject()
    {
        if (oldTrans == null) return;

        LevelMoment moment = momDisplay.GetFocusedMoment();
        bool changed = moment.Remove(overI, key);

        if (!changed) return;

        LevelData.Instance.ChangeFuture(moment);
        logicController.updating = true;
        momDisplay.UpdateTimeline(moment);
        hudControl.RedrawHud(moment);
    }
}
